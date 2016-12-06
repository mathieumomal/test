/*
' Copyright (c) 2014  Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Utils.Core;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using System.Net;
using System.Reflection;

namespace Etsi.Ultimate.Module.WorkItem
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from WorkItemModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : PortalModuleBase, IActionable
    {
        #region Fields

        protected Controls.FullView ultFullView;
        protected Controls.ShareUrlControl ultShareUrl;
        protected Controls.ReleaseSearchControl releaseSearchControl;

        private static string PathExportWorkPlan;
        private static string PathUploadWorkPlan;

        private const string CONST_ACRONYMS_DATASOURCE = "AcronymDataSource";
        private const string CONST_DSID_KEY = "ETSI_DS_ID";

        private int errorNumber = 0;
        private string tokenWorkPlanAnalysed = "";
        private bool isUrlSearch;

        private const string LatestUpdateText = "Latest Update: ";

        private const string VS_TB_ID = "WI_TB_ID";
        private const string VS_SUBTB_ID = "WI_SUBTB_ID";
        private string tbId
        {
            get
            {
                if (ViewState[VS_TB_ID] == null)
                    ViewState[VS_TB_ID] = "";
                return (string)ViewState[VS_TB_ID];
            }
            set
            {
                ViewState[VS_TB_ID] = value;
            }
        }

        private string subTBId
        {
            get
            {
                if (ViewState[VS_SUBTB_ID] == null)
                    ViewState[VS_SUBTB_ID] = "";
                return (string)ViewState[VS_SUBTB_ID];
            }
            set
            {
                ViewState[VS_SUBTB_ID] = value;
            }
        }

        //Style
        private const string CSS_CLASS_STOPPED_WI = "trStoppedWI";

        /// <summary>
        /// Acronym field
        /// </summary>
        private string SelectedAcronym
        {
            get
            {
                return racAcronym.Text;
            }
            set
            {
                racAcronym.Text = value;
            }
        }

        #endregion

        #region Properties

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), DotNetNuke.Services.Localization.Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }

        /// <summary>
        /// First page load flag
        /// </summary>
        private bool FirstLoad { get; set; }

        private WorkItemSearch SearchObj { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetRequestParameters();

                // init the full view component.
                ManageFullView("");

                if (!IsPostBack || !moduleWI.Visible)
                {
                    FirstLoad = true;
                    releaseSearchControl.IsLoadingNeeded = !moduleWI.Visible;
                    moduleWI.Visible = true;

                    //Get settings
                    if (Settings.Contains(Enum_Settings.WorkItem_ExportPath.ToString()))
                        PathExportWorkPlan = Settings[Enum_Settings.WorkItem_ExportPath.ToString()].ToString();
                    if (Settings.Contains(Enum_Settings.WorkItem_UploadPath.ToString()))
                        PathUploadWorkPlan = Settings[Enum_Settings.WorkItem_UploadPath.ToString()].ToString();

                    var wpSvc = ServicesFactory.Resolve<IWorkPlanFileService>();
                    var workPlanFile = wpSvc.GetLastWorkPlanFile();
                    if (workPlanFile != null)
                    {
                        lblLatestUpdated.Visible = true;
                        lblLatestUpdated.Text = LatestUpdateText + workPlanFile.CreationDate.ToString("yyyy-MM-dd");

                        lnkFtpDownload.Visible = true;

                        if (IsFileOnFtp(ConfigVariables.FtpExportAddress + workPlanFile.WorkPlanFilePath))
                        {
                            lnkFtpDownload.NavigateUrl = ConfigVariables.FtpExportAddress +
                                                         workPlanFile.WorkPlanFilePath;
                        }
                        else
                        {
                            lnkFtpDownload.NavigateUrl = ConfigVariables.FtpExportAddress;
                        }
                    }

                    releaseSearchControl.Load += releaseSearchControl_Load;

                    tbId = Request.QueryString["tbid"];
                    subTBId = Request.QueryString["SubTB"];    
                }
                else
                {
                    FirstLoad = false;
                }

                if (FirstLoad)
                {
                    // must be place after racAcronym.DataBind();
                    var searchObjFromCookie = CookiesHelper.GetCookie<WorkItemSearch>(Page.Request, ConfigVariables.CookieNameWisList);
                    if (searchObjFromCookie != null && searchObjFromCookie.GetType() == typeof (WorkItemSearch))
                    {
                        SearchObj = searchObjFromCookie;
                        LoadControlsFromSearchObj();
                    }
                }

                // Display or not import WI
                //List<int> releaseIDs = releaseSearchControl.SelectedReleaseIds;
                var personService = ServicesFactory.Resolve<IPersonService>();

                var userRights = personService.GetRights(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
                if (userRights.HasRight(Enum_UserRights.WorkItem_ImportWorkplan))
                {
                    WorkPlanImport_Btn.CssClass = "btn3GPP-success";
                    WorkPlanImport_Btn.Visible = true;
                }
                else
                {
                    WorkPlanImport_Btn.Visible = false;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Method call by ajax when workplan is uploaded => WorkPlan Analyse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AsyncUpload_FileImport(object sender, FileUploadedEventArgs e)
        {
            //Get workplan
            UploadedFile workplanUploaded = e.File;

            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            //var filename = RdAsyncUpload.UploadedFiles[0].
            try
            {
                workplanUploaded.SaveAs(new StringBuilder()
                    .Append(PathUploadWorkPlan)
                    .Append(workplanUploaded.FileName)
                    .ToString());
            }catch (Exception exc)
            {
                LogManager.Error("Could not save work plan file: " + exc.Message);
            }
            //Analyse file type (ZIP/CSV)
            var wiReport = wiService.AnalyseWorkPlanForImport(new StringBuilder()
                .Append(PathUploadWorkPlan)
                .Append(workplanUploaded.GetName())
                .ToString());

            //Get work plan's token to use it when user will press "confirm import" button
            tokenWorkPlanAnalysed = wiReport.Key;
            ViewState["WiImportToken"] = tokenWorkPlanAnalysed;

            //Get analyse report
            var report = wiReport.Value;

            lblCountWarningErrors.Text = new StringBuilder()
                .Append("Found ")
                .Append(report.GetNumberOfErrors().ToString())
                .Append(" error")
                .Append(report.GetNumberOfErrors() <= 1 ? "" : "s")
                .Append(", ")
                .Append(report.GetNumberOfWarnings().ToString())
                .Append(" warning")
                .Append(report.GetNumberOfWarnings() <= 1 ? "" : "s")
                .Append(".")
                .ToString();

            if (report.GetNumberOfErrors() > 0)
            {
                btnConfirmImport.Enabled = false;
                lblExportPath.Text = "Errors were found. WorkPlan cannot be imported.";
                errorNumber = report.GetNumberOfErrors();
            }
            else
            {
                if (ConfigVariables.ActivateWorkPlanExportAfterImport)
                {
                    lblExportPath.Text = new StringBuilder()
                    .Append("The new work plan will be exported to: ")
                    .Append(PathExportWorkPlan)
                    .ToString();
                }
                else
                {
                    lblExportPath.Text = string.Empty;
                }
                
                btnConfirmImport.Enabled = true;
            }

            List<string> datasource = new List<string>();
            datasource.AddRange(report.ErrorList);
            datasource.AddRange(report.WarningList);
            rptWarningsErrors.DataSource = datasource;
            rptWarningsErrors.DataBind();
        }

        /// <summary>
        /// Method call when client confirm the import => save workplan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmation_import_OnClick(object sender, EventArgs e)
        {
            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            tokenWorkPlanAnalysed = (string)ViewState["WiImportToken"];
            bool success = wiService.ImportWorkPlan(tokenWorkPlanAnalysed, PathExportWorkPlan);

            if (success)
            {
                //Store Last exported version date and path
                var wpSvc = ServicesFactory.Resolve<IWorkPlanFileService>();
                WorkPlanFile wpf = new WorkPlanFile();
                wpf.CreationDate = DateTime.Now;
                wpf.WorkPlanFilePath = "Work_plan_3gpp_" + DateTime.Now.ToString("yyMMdd") + ".zip";
                wpSvc.AddWorkPlanFile(wpf);

                //Update RadWindow_workItemState label with real files path  
                lblSaveStatus.Text = "Work plan was successfully imported.<br/>";
                if (ConfigVariables.ActivateWorkPlanExportAfterImport)
                {
                    lblExportedPath.Text = new StringBuilder()
                    .Append("Word and Excel version of the work plan are available at:")
                    .Append(PathExportWorkPlan)
                    .ToString();
                }
                else
                {
                    lblExportedPath.Text = string.Empty;
                }
            }
            else
            {
                lblSaveStatus.Text = "Error occured while saving the work plan. Please contact helpdesk for further investigation";
                lblExportedPath.Text = "";
            }
        }

        /// <summary>
        /// Click Event of Button Search
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            List<int> tbList = new List<int>();
            int value;
            if (!String.IsNullOrEmpty(subTBId))
            {
                tbList = subTBId.Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                tbList.RemoveAll(x => x == -1);
            }

            if (wiService.GetWorkItemsCountBySearchCriteria(releaseSearchControl.SelectedReleasesIds, Convert.ToInt32(rddGranularity.SelectedValue), chkHideCompletedItems.Checked, SelectedAcronym, txtName.Text, tbList) > 200)
            {
                //
                if (releaseSearchControl.SelectedReleasesIds.Count > 3)
                {
                    RadWindowManager1.RadAlert("Query will return many records. Please download the Excel workplan / select 3 or less Releases!", 400, 80, "WorkItem Search", String.Empty);
                }
                else
                {
                    string script = "function f(){$find(\"" + RadWindow_workItemCount.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);autoConfirmSearch();";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "customConfirmOpener", script, true);
                }
            }
            else
            {
                isUrlSearch = false;
                ultShareUrl.IsShortUrlChecked = false;
                loadWorkItemData();
            }

            FillSearchObj();
        }

        /// <summary>
        /// Click Event of Rad Confirmation when query take long time to perform
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void rbWorkItemCountOk_Click(object sender, EventArgs e)
        {
            loadWorkItemData();
        }

        /// <summary>
        /// Click Event of Button Default
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void btnDefault_Click(object sender, EventArgs e)
        {
            SearchObj = new WorkItemSearch();
            rddGranularity.SelectedValue = "1";
            chkHideCompletedItems.Checked = false;
            SelectedAcronym = string.Empty;
            txtName.Text = String.Empty;
            releaseSearchControl.Reset();
            CookiesHelper.SetCookie(Page.Response, ConfigVariables.CookieNameWisList, SearchObj);
        }

        /// <summary>
        /// Need data source event for WorkItems Tree List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void rtlWorkItems_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            if (FirstLoad)
            {
                rtlWorkItems.DataSource = new List<Domain.WorkItem>();
            }
            else
            {
                var wiService = ServicesFactory.Resolve<IWorkItemService>();
                List<int> tbList = new List<int>();
                int value;
                if (!String.IsNullOrEmpty(subTBId))
                {
                    tbList = subTBId.Split(',').Select(x => int.TryParse(x, out value) ? value : -1).ToList();
                    tbList.RemoveAll(x => x == -1);
                }

                var wiData =
                    wiService.GetWorkItemsBySearchCriteria(
                        GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()),
                        releaseSearchControl.SelectedReleasesIds, Convert.ToInt32(rddGranularity.SelectedValue),
                        chkHideCompletedItems.Checked, SelectedAcronym, txtName.Text, tbList);
                rtlWorkItems.DataSource = wiData.Key;
            }
        }

        /// <summary>
        /// Used to loadWorkItemData only after the releaseSearchControl control is rendered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void releaseSearchControl_Load(object sender, EventArgs e)
        {
            if (isUrlSearch)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["releaseId"]))
                    releaseSearchControl.SelectedReleasesWithKeywords = Request.QueryString["releaseId"];
                if (!String.IsNullOrEmpty(Request.QueryString["granularity"]))
                    rddGranularity.SelectedValue = Request.QueryString["granularity"].ToString();
                if (!String.IsNullOrEmpty(Request.QueryString["hideCompleted"]))
                    chkHideCompletedItems.Checked = Convert.ToBoolean(Request.QueryString["hideCompleted"]);
                if (!String.IsNullOrEmpty(Request.QueryString["name"]))
                    txtName.Text = Request.QueryString["name"].ToString();
                SelectedAcronym = string.Empty;
                if (!String.IsNullOrEmpty(Request.QueryString["acronym"]))
                {
                    SelectedAcronym = Request.QueryString["acronym"];
                }
            }
            else
            {
                if (FirstLoad && SearchObj != null && SearchObj.SelectedReleaseIds != null && SearchObj.SelectedReleaseIds.Count > 0)
                {
                    releaseSearchControl.SelectedReleasesIds = SearchObj.SelectedReleaseIds;
                }
            }

            loadWorkItemData();
        }

        protected void rptErrorsWarning_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            string item = (String)e.Item.DataItem;
            if (item != null)
            {
                Label lbl = e.Item.FindControl("lblErrorOrWarning") as Label;
                lbl.Text = item;
                if (errorNumber > 0)
                {
                    lbl.CssClass = "ErrorItem";
                    errorNumber--;
                }
                else
                    lbl.CssClass = "WarningItem";

            }
        }

        /// <summary>
        /// Method used to gray stopped WIs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rtlWorkItems_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (e.Item is TreeListDataItem)
            {
                TreeListDataItem item = (TreeListDataItem)e.Item;
                if (!string.IsNullOrEmpty(item["TsgStoppedMtgRef"].Text) && item["TsgStoppedMtgRef"].Text != "&nbsp;")
                    item.CssClass = CSS_CLASS_STOPPED_WI;
            }
        }


        #endregion

        #region Private methods

        /// <summary>
        /// Load work item data
        /// </summary>
        private void loadWorkItemData()
        {
            //Set Search Label
            string releaseIds = releaseSearchControl.SelectedReleasesWithKeywords;
            int granularity = Convert.ToInt32(rddGranularity.SelectedValue);
            bool hidePercentComplete = chkHideCompletedItems.Checked;
            string wiName = txtName.Text;

            StringBuilder searchString = new StringBuilder();
            string releaseText = releaseSearchControl.SearchString;
            searchString.Append(String.IsNullOrEmpty(releaseText) ? "Open Releases" : releaseText);
            searchString.Append(", " + rddGranularity.SelectedText);
            if (!String.IsNullOrEmpty(SelectedAcronym))
                searchString.Append(", " + SelectedAcronym);
            if (!String.IsNullOrEmpty(wiName))
                searchString.Append(", " + wiName);
            if (hidePercentComplete)
                searchString.Append(", hidden completed items");

            lblSearchHeader.Text = String.Format("Search form ({0})", searchString);

            //Set Short URL
            ManageShareUrl(releaseIds);
            ManageFullView(releaseIds);

            rtlWorkItems.Rebind();
            rtlWorkItems.CollapseAllItems();
            rtlWorkItems.ExpandToLevel(granularity);
        }

        private void LoadControlsFromSearchObj()
        {
            SetSelectedValue(rddGranularity, SearchObj.GranularityId);
            chkHideCompletedItems.Checked = SearchObj.HideCompletedItems;
            txtName.Text = SearchObj.NameUID;
            if (!string.IsNullOrWhiteSpace(SearchObj.Acronym))
            {
                SelectedAcronym = SearchObj.Acronym;
            }
        }

        private void FillSearchObj()
        {
            // SearchObj Values
            SearchObj = new WorkItemSearch();
            SearchObj.NameUID = txtName.Text;
            SearchObj.HideCompletedItems = chkHideCompletedItems.Checked;
            SearchObj.GranularityId = rddGranularity.SelectedValue;
            SearchObj.SelectedReleaseIds = releaseSearchControl.SelectedReleasesIds;

            if (!string.IsNullOrWhiteSpace(SelectedAcronym))
            {
                SearchObj.Acronym = SelectedAcronym;
            }
            else
            {
                SearchObj.Acronym = string.Empty;
            }

            // Save SearchObj into cookie
            CookiesHelper.SetCookie(Page.Response, ConfigVariables.CookieNameWisList, SearchObj);
        }

        private void SetSelectedValue(RadDropDownList rddl, string value)
        {
            try
            {
                rddl.SelectedValue = value;
            }
            catch (Exception)
            {
                // Value doesn't exists
            }
        }

        /// <summary>
        /// Retrieve person Id
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(CONST_DSID_KEY), out personID))
                    return personID;
            }
            return 0;
        }

        /// <summary>
        /// Populate ShareUrl using search parameters
        /// </summary>
        /// <param name="selectedReleases"></param>
        private void ManageShareUrl(string selectedReleases)
        {
            ultShareUrl.ModuleId = ModuleId;
            ultShareUrl.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            ultShareUrl.BaseAddress = address;


            ultShareUrl.UrlParams = ManageUrlParams(selectedReleases);
        }

        private void ManageFullView(string selectedReleases)
        {
            ultFullView.ModuleId = ModuleId;
            ultFullView.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"];
            ultFullView.BaseAddress = address;

            ultFullView.UrlParams = ManageUrlParams(selectedReleases);
            ultFullView.Display();
        }

        private Dictionary<string, string> ManageUrlParams(string selectedReleases)
        {
            var nameValueCollection = HttpContext.Current.Request.QueryString;
            var urlParams = new Dictionary<string, string>();

            if (!isUrlSearch)
            {
                urlParams.Add("q", "1");
                if (!String.IsNullOrEmpty(tbId))
                    urlParams.Add("tbid", tbId);
                if (!String.IsNullOrEmpty(subTBId))
                    urlParams.Add("SubTB", subTBId);
                if (!string.IsNullOrEmpty(selectedReleases))
                    urlParams.Add("releaseId", selectedReleases);
                urlParams.Add("granularity", rddGranularity.SelectedValue);
                urlParams.Add("hideCompleted", chkHideCompletedItems.Checked.ToString());
                if (!String.IsNullOrEmpty(SelectedAcronym))
                    urlParams.Add("acronym", SelectedAcronym);
                if (!String.IsNullOrEmpty(txtName.Text.Trim()))
                    urlParams.Add("name", txtName.Text);
            }
            else
            {
                foreach (var k in nameValueCollection.AllKeys)
                {
                    if (k != null && nameValueCollection[k] != null)
                        urlParams.Add(k, nameValueCollection[k]);
                }
            }
            return urlParams;
        }

        /// <summary>
        /// Populate fields with available query strings
        /// </summary>
        private void GetRequestParameters()
        {
            isUrlSearch = (Request.QueryString["q"] != null);
        }

        private bool IsFileOnFtp(string url)
        {
            try
            {
                // create the request
                HttpWebRequest request = WebRequest.Create(url.Replace("ftp://", "http://")) as HttpWebRequest;

                // instruct the server to return headers only
                request.Method = "HEAD";

                // make the connection

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                // get the status code
                HttpStatusCode status = response.StatusCode;
                return status == HttpStatusCode.OK || status == HttpStatusCode.NotModified;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #region FIX : Cannot unregister UpdatePanel...
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.upWorkItemCount != null)
                this.upWorkItemCount.Unload += new EventHandler(UpdatePanel_Unload);
        }
        void UpdatePanel_Unload(object sender, EventArgs e)
        {
            this.RegisterUpdatePanel(sender as UpdatePanel);
        }
        public void RegisterUpdatePanel(UpdatePanel panel)
        {
            foreach (MethodInfo methodInfo in typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (methodInfo.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel"))
                {
                    methodInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { upWorkItemCount });
                }
            }
        }
        #endregion

        #endregion
    }
}