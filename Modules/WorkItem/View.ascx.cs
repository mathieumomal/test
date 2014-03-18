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
                if(Settings.Contains(Enum_Settings.WorkItem_ExportPath.ToString()))
                    PathExportWorkPlan = Settings[Enum_Settings.WorkItem_ExportPath.ToString()].ToString();
                if (Settings.Contains(Enum_Settings.WorkItem_UploadPath.ToString()))
                    PathUploadWorkPlan = Settings[Enum_Settings.WorkItem_UploadPath.ToString()].ToString();
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
            tokenWorkPlanAnalysed = (string) ViewState["WiImportToken"];
            bool success = wiService.ImportWorkPlan(tokenWorkPlanAnalysed);

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