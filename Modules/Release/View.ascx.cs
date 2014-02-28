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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DotNetNuke.Services.Installer.Dependencies;
using Etsi.Ultimate.Module.Release.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Etsi.Ultimate.Services;
using Microsoft.Practices.ObjectBuilder2;
using Rhino.Mocks;
using Telerik.Web.UI;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Module.Release
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ReleaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : ReleaseModuleBase, IActionable
    {
        private static String FreezeReach_READONLY_CSS = "freezeReach";
        private static String closedColor_READONLY_CSS = "closed";
        
        public static readonly string DsId_Key = "ETSI_DS_ID";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int personId = GetUserPersonId(UserInfo);

                //Example : mock to fake service layer -> ServicesFactory.Container.RegisterType<IReleaseService, ReleaseServiceMock>(new TransientLifetimeManager());
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();//Get the mock instead service classe

                //Example : mock to fake rights manager : ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

                KeyValuePair<List<DomainClasses.Release>, UserRightsContainer> releaseRightsObjects = svc.GetAllReleases( personId );
                List<DomainClasses.Release> releaseObjectsList = releaseRightsObjects.Key.OrderByDescending(release => release.SortOrder).ToList();

                releasesTable.DataSource = releaseObjectsList;

                //Show freezes if connected
                if (!releaseRightsObjects.Value.HasRight(Enum_UserRights.Release_ViewCompleteList))
                {
                    releasesTable.MasterTableView.GetColumn("Stage1FreezeDate").Visible = false;
                    releasesTable.MasterTableView.GetColumn("Stage2FreezeDate").Visible = false;
                    releasesTable.MasterTableView.GetColumn("Stage3FreezeDate").Visible = false;
                }
                //Button new
                if(releaseRightsObjects.Value.HasRight(Enum_UserRights.Release_Create)){
                    newRelease.Visible = true;
                }else{
                    newRelease.Visible = false;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo UserInfo)
        {
            if (UserInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
             return 0;
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

        protected string FormatDate(object date)
        {
            if (date != null)
                return Convert.ToDateTime(date).ToString("yyyy-MM-dd");
            return null;
        }

        /**
         * Handler data cell
         * 
         * */
        public void releasesTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            //Analyse on row
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;//Get row
                DomainClasses.Release currentRelease = (DomainClasses.Release) e.Item.DataItem;//GET row release

                //Analyse column : Closure date
                TableCell closureDate = dataItem["ClosureDate"];
                if (currentRelease.ClosureDate != null && currentRelease.ClosureMtgRef != null)
                {
                    closureDate.Text =
                    new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.ClosureDate))
                        .Append(" (")
                        .Append(currentRelease.ClosureMtgRef)
                        .Append(")")
                        .ToString();
                }
                closureDate.CssClass = closedColor_READONLY_CSS;

                DateTime now = DateTime.Now;

                //Analyse column : Freeze 1
                if (currentRelease.Stage1FreezeDate != null)
                {
                    DateTime dateStage1FreezeDate = (DateTime)currentRelease.Stage1FreezeDate;
                    TableCell freeze1 = dataItem["Stage1FreezeDate"];
                    if (currentRelease.Stage1FreezeMtgRef != null)
                    {
                        freeze1.Text =
                        new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage1FreezeDate))
                            .Append(" (")
                            .Append(currentRelease.Stage1FreezeMtgRef)
                            .Append(")")
                            .ToString();
                    }
                    if (now > dateStage1FreezeDate.Date)
                        freeze1.CssClass = FreezeReach_READONLY_CSS;
                }
                

                //Analyse column : Freeze 2
                if (currentRelease.Stage2FreezeDate!=null)
                {
                    DateTime dateStage2FreezeDate = (DateTime)currentRelease.Stage2FreezeDate;
                    TableCell freeze2 = dataItem["Stage2FreezeDate"];
                    if (currentRelease.Stage2FreezeMtgRef != null)
                    {
                        freeze2.Text =
                        new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage2FreezeDate))
                            .Append(" (")
                            .Append(currentRelease.Stage2FreezeMtgRef)
                            .Append(")")
                            .ToString();
                    }
                    if (now > dateStage2FreezeDate.Date)
                        freeze2.CssClass = FreezeReach_READONLY_CSS;
                }
               

                //Analyse column : Freeze 3
                if (currentRelease.Stage3FreezeDate != null)
                {
                    DateTime dateStage3FreezeDate = (DateTime)currentRelease.Stage3FreezeDate;
                    TableCell freeze3 = dataItem["Stage3FreezeDate"];
                    if (currentRelease.Stage3FreezeMtgRef != null)
                    {
                        freeze3.Text =
                        new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", currentRelease.Stage3FreezeDate))
                            .Append(" (")
                            .Append(currentRelease.Stage3FreezeMtgRef)
                            .Append(")")
                            .ToString();
                    }
                    if (now > dateStage3FreezeDate.Date)
                        freeze3.CssClass = FreezeReach_READONLY_CSS;
                }
            }


        }
    }
}