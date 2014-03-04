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
using System.IO;
using System.Collections.Generic;

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
        private static string readonlyPathImportWorkPlan = "c:\\TEST_IMPORT\\";
        private static string readOnlyExtensionCsv = "csv";
        private static string readOnlyExtensionZip = "zip";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
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
            //Remove client cache
            Context.Cache.Remove(Session.SessionID + "UploadedWorkPlan");
            //Get workplan
            UploadedFile workplanUploaded = e.File;
            //Add uploaded workplan in cache
            Context.Cache.Insert(Session.SessionID + "UploadedWorkPlan", workplanUploaded, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero);
            //Update View Label of RadWindow_workItemConfirmation Modal with the suppose file path
            path_export.Text = new StringBuilder().Append(readonlyPathImportWorkPlan).Append(workplanUploaded.GetName()).ToString();

            //File analyse and update list of warnings dataSource
            List<string> datasource = new List<string>();
            datasource.Add("WpId, UID WI Unique_Id: Acronym \"Acronym name\" is duplicated on other work items with same level.");
            datasource.Add("WpId, UID WI Unique_Id: Cannot identify release : Release name.");
            datasource.Add("WpId, UID WI Unique_Id:Rapporteur email \"Email\" could not match an existing user in database.");
            datasource.Add("WpId Unique_Acronym \"Acronym name\" is duplicated on other work items with same level.");
            datasource.Add("WpIdCannotI Unique_Identify release : Releasgfe name.");
            datasource.Add("WpId, UID WI Unique_Idteur email \"Email\" could not match an existing user in database.");
            datasource.Add("Wue_Id: Acronym \"Acronye\" is duplicated on other work items with same level.");
            datasource.Add("WpId, UI Unique_Id: Cannot identify release : Releasegf name.");
            datasource.Add("W WI Unique_Id:Rapporteur email \"Email\" could not match an existing user in database.");
            repeater.DataSource = datasource;
            repeater.DataBind();

            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Method call when client confirm the import => save workplan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmation_import_OnClick(object sender, EventArgs e)
        {
            //Get workplan uploaded in cache
            var workplanUploaded = (UploadedFile)Context.Cache.Get(Session.SessionID + "UploadedWorkPlan");
            //Save workplan on server
            workplanUploaded.SaveAs(new StringBuilder()
                .Append(readonlyPathImportWorkPlan)
                .Append(workplanUploaded.GetName())
                .ToString(), 
            true);
            //Update RadWindow_workItemState label with real path file 
            path_available.Text = new StringBuilder()
                .Append(readonlyPathImportWorkPlan)
                .Append(workplanUploaded.GetName())
                .ToString();
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