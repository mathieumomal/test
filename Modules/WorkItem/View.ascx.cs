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

using System;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Module.WorkItem;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Common.Utilities;
using Telerik.Web.UI;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Etsi.Ultimate.Services;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using Etsi.Ultimate.Controls;
using DotNetNuke.Entities.Tabs;
using System.Web;
using System.Web.UI;
using System.Drawing;

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
    public partial class View : WorkItemModuleBase, IActionable
    {
        protected Etsi.Ultimate.Controls.FullView ultFullView;
        protected Etsi.Ultimate.Controls.ShareUrlControl ultShareUrl;

        private static string PathExportWorkPlan;
        private static string PathUploadWorkPlan;
        private string tokenWorkPlanAnalysed = "";
        private int errorNumber = 0;
        private const string CONST_WORKITEM_DATASOURCE = "WorkItemDataSource";
        private const string CONST_FILTERED_DATASOURCE = "WorkItemFilteredDataSource";
        private const string CONST_ACRONYMS_DATASOURCE = "AcronymDataSource";
        private int granularity;
        private bool percentComplete;
        private string wiAcronym;
        private string wiName;
        protected ReleaseSearchControl releaseSearchControl;
        private static string selectedReleases;

        private List<DomainClasses.WorkItem> DataSource
        {
            get
            {
                if (ViewState[CONST_WORKITEM_DATASOURCE] == null)
                    ViewState[CONST_WORKITEM_DATASOURCE] = new List<DomainClasses.WorkItem>();

                return (List<DomainClasses.WorkItem>)ViewState[CONST_WORKITEM_DATASOURCE];
            }
            set
            {
                ViewState[CONST_WORKITEM_DATASOURCE] = value;
            }
        }

        private List<DomainClasses.WorkItem> FilteredDataSource
        {
            get
            {
                if (ViewState[CONST_FILTERED_DATASOURCE] == null)
                    ViewState[CONST_FILTERED_DATASOURCE] = new List<DomainClasses.WorkItem>();

                return (List<DomainClasses.WorkItem>)ViewState[CONST_FILTERED_DATASOURCE];
            }
            set
            {
                ViewState[CONST_FILTERED_DATASOURCE] = value;
            }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ManageShareUrl();


                //ultFullView.ModuleId = 12;
                //ultFullView.TabId = 13;
                //var urlParams = Page.ClientQueryString.Split('&').Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);
                //urlParams.Remove("tabId");
                //ultFullView.UrlParams = urlParams;
                //ultFullView.BaseAddress = "";

                //Get settings
                if (Settings.Contains(Enum_Settings.WorkItem_ExportPath.ToString()))
                    PathExportWorkPlan = Settings[Enum_Settings.WorkItem_ExportPath.ToString()].ToString();
                if (Settings.Contains(Enum_Settings.WorkItem_UploadPath.ToString()))
                    PathUploadWorkPlan = Settings[Enum_Settings.WorkItem_UploadPath.ToString()].ToString();

                if (!IsPostBack)
                {
                    var wiService = ServicesFactory.Resolve<IWorkItemService>();
                    Acronyms = wiService.GetAllAcronyms();
                    selectedReleases = String.Empty;
                }

                racAcronym.DataSource = Acronyms;
                racAcronym.DataBind();

                rtlWorkItems.ItemDataBound += rtlWorkItems_ItemDataBound;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ManageShareUrl()
        {
            ultShareUrl.ModuleId = ModuleId;
            ultShareUrl.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"] + "/";
            ultShareUrl.BaseAddress = address;

            var nameValueCollection = HttpContext.Current.Request.QueryString;
            var urlParams = new Dictionary<string, string>();
            foreach (var k in nameValueCollection.AllKeys)
            {
                if (k != null && nameValueCollection[k] != null)
                    urlParams.Add(k, nameValueCollection[k]);
            }
            ultShareUrl.UrlParams = urlParams;
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
                //Update RadWindow_workItemState label with real files path  
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
        /// Click Event of Button Search
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            List<int> releaseIDs = releaseSearchControl.SelectedReleaseIds;
            var wiService = ServicesFactory.Resolve<IWorkItemService>();

            if (wiService.GetWorkItemsCountByRelease(releaseIDs) > 500)
            {
                string script = "function f(){$find(\"" + RadWindow_workItemCount.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "customConfirmOpener", script, true);
            }
            else
            {
                loadWorkItemData(releaseIDs, wiService);
            }
        }

        /// <summary>
        /// Click Event of Rad Confirmation when query take long time to perform
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void rbWorkItemCountOk_Click(object sender, EventArgs e)
        {
            List<int> releaseIDs = releaseSearchControl.SelectedReleaseIds;
            var wiService = ServicesFactory.Resolve<IWorkItemService>();
            loadWorkItemData(releaseIDs, wiService);
        }

        /// <summary>
        /// Load work item data
        /// </summary>
        /// <param name="releaseIDs">List of Release Ids</param>
        /// <param name="wiService">List of Service</param>
        private void loadWorkItemData(List<int> releaseIDs, IWorkItemService wiService)
        {
            if (String.Join(",", releaseIDs) != selectedReleases)
            {
                var wiData = wiService.GetWorkItemsByRelease(UserId, releaseIDs);
                DataSource = wiData.Key;
                selectedReleases = String.Join(",", releaseIDs);
            }

            granularity = Convert.ToInt32(rddGranularity.SelectedValue);
            percentComplete = chkHideCompletedItems.Checked;
            wiAcronym = racAcronym.Text.Trim();
            if (wiAcronym.Length > 1)
                wiAcronym = wiAcronym.Remove(wiAcronym.Length - 1);
            wiName = txtName.Text;

            StringBuilder searchString = new StringBuilder();
            searchString.Append(releaseSearchControl.SearchString);
            searchString.Append(", " + rddGranularity.SelectedText);
            if (!String.IsNullOrEmpty(wiAcronym))
                searchString.Append(", " + wiAcronym);
            if (!String.IsNullOrEmpty(wiName))
                searchString.Append(", " + wiName);
            if (percentComplete)
                searchString.Append(", hidden completed items");

            lblSearchHeader.Text = String.Format("Search form ({0})", searchString.ToString());
            searchPanel.Expanded = false;

            if (String.IsNullOrEmpty(wiName) && String.IsNullOrEmpty(wiAcronym))
            {
                rtlWorkItems.CollapseAllItems();
                rtlWorkItems.ExpandToLevel(1);
                rtlWorkItems.Rebind();
            }
            else
                rtlWorkItems.ExpandAllItems();


        }

        void rtlWorkItems_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {

            if (e.Item is TreeListDataItem)
            {
                TreeListDataItem item = e.Item as TreeListDataItem;

                var displayStatus = (DomainClasses.WorkItem.DisplayStatus)DataBinder.Eval(item.DataItem, "Display");
                if (displayStatus == DomainClasses.WorkItem.DisplayStatus.matched)
                {
                    item.BackColor = Color.LightYellow;
                }

                var wiLevel = (System.Int32)DataBinder.Eval(item.DataItem, "wiLevel");
                if (wiLevel == 0)
                {
                    item["ViewWorkItem"].Visible = false;
                }

            }
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
        }

        /// <summary>
        /// Need data source event for WorkItems Tree List
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Arguments</param>
        protected void rtlWorkItems_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
        {
            if (e.RebindReason != TreeListRebindReason.PostBackEvent && DataSource != null)
            {
                if (string.IsNullOrEmpty(wiName) && string.IsNullOrEmpty(wiAcronym))
                {
                    DataSource.ForEach(x => x.Display = DomainClasses.WorkItem.DisplayStatus.none);
                    rtlWorkItems.DataSource = FilteredDataSource = DataSource.Where(x => (x.WiLevel != null && x.WiLevel <= granularity)
                                                        && (percentComplete ? (((x.Completion == null) ? 0 : x.Completion) >= 100) : true)).ToList(); ;
                }
                else
                {
                    var list = DataSource;
                    list.ForEach(x => x.Display = DomainClasses.WorkItem.DisplayStatus.none);
                    var modlist = list.Where(x => x.Name.StartsWith(wiName.Trim()) && x.Acronym.StartsWith(wiAcronym.Trim())
                                                        && (x.WiLevel != null && x.WiLevel <= granularity)
                                                        && (percentComplete ? (((x.Completion == null) ? 0 : x.Completion) >= 100) : true)).ToList();
                    foreach (var item in modlist)
                    {
                        //list.Find(x => x.Pk_WorkItemUid == item.Pk_WorkItemUid).Display = DomainClasses.WorkItem.DisplayStatus.matched;
                        item.Display = DomainClasses.WorkItem.DisplayStatus.matched;
                        MarkParent(list, item);
                    }

                    rtlWorkItems.DataSource = FilteredDataSource = list.Where(x => x.Display == DomainClasses.WorkItem.DisplayStatus.include
                                                                                || x.Display == DomainClasses.WorkItem.DisplayStatus.matched).ToList();
                }
            }
            else
            {
                if (FilteredDataSource != null)
                    rtlWorkItems.DataSource = FilteredDataSource;
            }
        }


        private void MarkParent(List<DomainClasses.WorkItem> list, DomainClasses.WorkItem parent)
        {
            if (parent != null)
            {
                var parentObj = list.Find(x => x.Pk_WorkItemUid == parent.Fk_ParentWiId);
                if (parentObj != null)
                {
                    parentObj.Display = DomainClasses.WorkItem.DisplayStatus.include;
                    MarkParent(list, parentObj);
                }
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }



    }
}