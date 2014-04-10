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
using DotNetNuke.Services.Localization;
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
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using System.Net;

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

        protected Etsi.Ultimate.Controls.FullView ultFullView;
        protected Etsi.Ultimate.Controls.ShareUrlControl ultShareUrl;
        protected Etsi.Ultimate.Controls.ReleaseSearchControl releaseSearchControl;

        private static string PathExportWorkPlan;
        private static string PathUploadWorkPlan;

        private const string CONST_ACRONYMS_DATASOURCE = "AcronymDataSource";
        private const string CONST_DSID_KEY = "ETSI_DS_ID";

        private int errorNumber = 0;
        private string tokenWorkPlanAnalysed = "";
        private bool fromShortUrl;

        #endregion

        #region Properties

        private List<string> Acronyms
        {
            get
            {
                if (ViewState[CONST_ACRONYMS_DATASOURCE] == null)
                    ViewState[CONST_ACRONYMS_DATASOURCE] = new List<string>();

                return (List<string>)ViewState[CONST_ACRONYMS_DATASOURCE];
            }
            set
            {
                ViewState[CONST_ACRONYMS_DATASOURCE] = value;
            }
        }

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

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GetRequestParameters();

                var wiService = ServicesFactory.Resolve<IWorkItemService>();
                if (!IsPostBack)
                {
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
                        lblLatestUpdated.Text = "Latest Updated " + workPlanFile.CreationDate.ToString("yyyy-MM-dd");

                        lnkFtpDownload.Visible = true;

                        if (IsFileOnFtp(ConfigVariables.FtpExportAddress + workPlanFile.WorkPlanFilePath))
                        {
                            lnkFtpDownload.NavigateUrl = ConfigVariables.FtpExportAddress + workPlanFile.WorkPlanFilePath;
                        }
                        else
                        {
                            lnkFtpDownload.NavigateUrl = ConfigVariables.FtpExportAddress;
                        }
                    }

                    Acronyms = wiService.GetAllAcronyms();
                    releaseSearchControl.Load += releaseSearchControl_Load;
                }

                racAcronym.DataSource = Acronyms;
                racAcronym.DataBind();

                // Display or not import WI
                List<int> releaseIDs = releaseSearchControl.SelectedReleaseIds;
                var personService = ServicesFactory.Resolve<IPersonService>();

                var userRights = personService.GetRights(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
                if (userRights.HasRight(Domain.Enum_UserRights.WorkItem_ImportWorkplan))
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

        private bool IsFileOnFtp(string url)
        {
            try
            {
            // create the request
            HttpWebRequest request = WebRequest.Create(url.Replace("ftp://","http://")) as HttpWebRequest;

            // instruct the server to return headers only
            request.Method = "HEAD";

            // make the connection
            
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                // get the status code
                HttpStatusCode status = response.StatusCode;
                return status == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                return false;
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
            workplanUploaded.SaveAs(new StringBuilder()
                .Append(PathUploadWorkPlan)
                .Append(workplanUploaded.FileName)
                .ToString());

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
                lblExportPath.Text = new StringBuilder()
                    .Append("The new work plan will be exported to: ")
                    .Append(PathExportWorkPlan)
                    .ToString();
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
                lblSaveStatus.Text = "Work plan was successfully imported.<br/>Word and Excel version of the work plan are available at:";
                lblExportedPath.Text = new StringBuilder()
                    .Append(PathExportWorkPlan)
                    .ToString();
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
            //Hide RadWindow
            RadWindow_workItemAnalysis.Visible = false;
            RadWindow_workItemConfirmation.Visible = false;
            RadWindow_workItemImport.Visible = false;
            RadWindow_workItemState.Visible = false;

            var wiService = ServicesFactory.Resolve<IWorkItemService>();

            if (wiService.GetWorkItemsCountBySearchCriteria(releaseSearchControl.SelectedReleaseIds, Convert.ToInt32(rddGranularity.SelectedValue), chkHideCompletedItems.Checked, racAcronym.Text.Trim().TrimEnd(';'), txtName.Text) > 500)
            {
                string script = "function f(){$find(\"" + RadWindow_workItemCount.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "customConfirmOpener", script, true);
            }
            else
            {
                fromShortUrl = false;
                loadWorkItemData();
            }
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
            releaseSearchControl.Reset();
            rddGranularity.SelectedValue = "1";
            chkHideCompletedItems.Checked = false;
            racAcronym.Entries.Clear();
            txtName.Text = String.Empty;

            loadWorkItemData();
        }

        /// <summary>
        /// Need data source event for WorkItems Tree List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void rtlWorkItems_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            var wiData = wiService.GetWorkItemsBySearchCriteria(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()), releaseSearchControl.SelectedReleaseIds, Convert.ToInt32(rddGranularity.SelectedValue), chkHideCompletedItems.Checked, racAcronym.Text.Trim().TrimEnd(';'), txtName.Text);
            rtlWorkItems.DataSource = wiData.Key;
        }

        /// <summary>
        /// Used to loadWorkItemData only after the releaseSearchControl control is rendered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void releaseSearchControl_Load(object sender, EventArgs e)
        {
            if (fromShortUrl)
            {
                if (!String.IsNullOrEmpty(Request.QueryString["releaseId"]))
                    releaseSearchControl.SelectedReleaseIds = Request.QueryString["releaseId"].Split(',').Select(n => int.Parse(n)).ToList();
                if (!String.IsNullOrEmpty(Request.QueryString["granularity"]))
                    rddGranularity.SelectedValue = Request.QueryString["granularity"].ToString();
                if (!String.IsNullOrEmpty(Request.QueryString["hideCompleted"]))
                    chkHideCompletedItems.Checked = Convert.ToBoolean(Request.QueryString["hideCompleted"]);
                if (!String.IsNullOrEmpty(Request.QueryString["name"]))
                    txtName.Text = Request.QueryString["name"].ToString();

                racAcronym.Entries.Clear();
                if (!String.IsNullOrEmpty(Request.QueryString["acronym"]))
                    racAcronym.Entries.Add(new AutoCompleteBoxEntry(Request.QueryString["acronym"].ToString(), String.Empty));
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

        #endregion

        #region Private methods

        /// <summary>
        /// Load work item data
        /// </summary>
        private void loadWorkItemData()
        {
            //Set Search Label
            string releaseIds = String.Join(",", releaseSearchControl.SelectedReleaseIds);
            int granularity = Convert.ToInt32(rddGranularity.SelectedValue);
            bool hidePercentComplete = chkHideCompletedItems.Checked;
            string wiAcronym = racAcronym.Text.Trim().TrimEnd(';');
            string wiName = txtName.Text;

            StringBuilder searchString = new StringBuilder();
            searchString.Append(String.IsNullOrEmpty(releaseSearchControl.SearchString) ? "Open Releases" : releaseSearchControl.SearchString);
            searchString.Append(", " + rddGranularity.SelectedText);
            if (!String.IsNullOrEmpty(wiAcronym))
                searchString.Append(", " + wiAcronym);
            if (!String.IsNullOrEmpty(wiName))
                searchString.Append(", " + wiName);
            if (hidePercentComplete)
                searchString.Append(", hidden completed items");

            lblSearchHeader.Text = String.Format("Search form ({0})", searchString.ToString());
            searchPanel.Expanded = false;

            //Set Short URL
            ManageShareUrl(releaseIds);

            rtlWorkItems.Rebind();
            rtlWorkItems.CollapseAllItems();
            rtlWorkItems.ExpandToLevel(granularity);
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

            var nameValueCollection = HttpContext.Current.Request.QueryString;
            var urlParams = new Dictionary<string, string>();

            if (!fromShortUrl)
            {
                urlParams.Add("shortUrl", "True");
                if (!string.IsNullOrEmpty(selectedReleases))
                    urlParams.Add("releaseId", selectedReleases);
                urlParams.Add("granularity", rddGranularity.SelectedValue);
                urlParams.Add("hideCompleted", chkHideCompletedItems.Checked.ToString());
                if (!String.IsNullOrEmpty(racAcronym.Text.Trim().TrimEnd(';')))
                    urlParams.Add("acronym", racAcronym.Text.Trim().TrimEnd(';'));
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

            ultShareUrl.UrlParams = urlParams;
        }

        /// <summary>
        /// Populate fields with available query strings
        /// </summary>
        private void GetRequestParameters()
        {
            fromShortUrl = (Request.QueryString["shortUrl"] != null) ? Convert.ToBoolean(Request.QueryString["shortUrl"]) : false;
        }

        #endregion
    }
}