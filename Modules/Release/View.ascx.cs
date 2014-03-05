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
using DotNetNuke.Common.Utilities;

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
    public partial class View : ReleaseModuleBase
    {
        private static String readonlyCssFreezeReach = "freezeReach";
        private static String readonlyCssClosedColor = "closed";

        public static readonly string DsId_Key = "ETSI_DS_ID";


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int personId = GetUserPersonId(UserInfo);

                //Example : mock to fake service layer -> ServicesFactory.Container.RegisterType<IReleaseService, ReleaseServiceMock>(new TransientLifetimeManager());
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();//Get the mock instead service classe

                //Example : mock to fake rights manager : ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

                //Calling the service
                KeyValuePair<List<DomainClasses.Release>,UserRightsContainer> releaseRightsObjects = svc.GetAllReleases(UserInfo.UserID);

                //Setting up the release list
                List<DomainClasses.Release> releasesList = releaseRightsObjects.Key.OrderByDescending(release => release.SortOrder).ToList();
                releasesTable.DataSource = releasesList;

                //Applying rights
                UserRightsContainer userRights = releaseRightsObjects.Value;

                //Show freezes if connected
                if (!userRights.HasRight(Enum_UserRights.Release_ViewCompleteDetails))
                {
                    releasesTable.MasterTableView.GetColumn("Stage1FreezeDate").Visible = false;
                    releasesTable.MasterTableView.GetColumn("Stage2FreezeDate").Visible = false;
                    releasesTable.MasterTableView.GetColumn("Stage3FreezeDate").Visible = false;
                }

                //New Button
                newRelease.Visible = userRights.HasRight(Enum_UserRights.Release_Create);
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


        /// <summary>
        /// Retrun javascript function to open a popup window
        /// </summary>
        /// <param name="currentPage">Current page</param>
        /// <param name="window">Window </param>
        /// <param name="htmlPage">Popup page</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        public static string OpenWindow(Page currentPage, String window, String htmlPage, Int32 width, Int32 height, Type winType)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("window.open('");
            sb.Append(htmlPage);
            sb.Append("','");
            sb.Append(window);
            sb.Append("','width=");
            sb.Append(width);
            sb.Append(",height=");
            sb.Append(height);
            sb.Append(",toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no");
            sb.Append("');");
            sb.Append("popWin.focus();");

            return sb.ToString();
        }

        

        /// <summary>
        /// Handler data cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void releasesTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            //Analyse on row
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;//Get row
                //Get table cell
                TableCell closureDate = dataItem["ClosureDate"];
                TableCell stage1FreezeDateCell = dataItem["Stage1FreezeDate"];
                TableCell stage2FreezeDateCell = dataItem["Stage2FreezeDate"];
                TableCell stage3FreezeDateCell = dataItem["Stage3FreezeDate"];

                //Get release row
                DomainClasses.Release currentRelease = (DomainClasses.Release) e.Item.DataItem;
                
                //Analyse column : Closure date
                if (currentRelease.ClosureDate != null && currentRelease.ClosureMtgRef != null)
                {
                    closureDate.Text = mixDateAndMtgRef((DateTime)currentRelease.ClosureDate, currentRelease.ClosureMtgRef);
                }
                closureDate.CssClass = readonlyCssClosedColor;

                //Analyse column : Freeze 1
                TreatFreezeDate(stage1FreezeDateCell, currentRelease.Stage1FreezeDate, currentRelease.Stage1FreezeMtgRef);
                //Analyse column : Freeze 2
                TreatFreezeDate(stage2FreezeDateCell, currentRelease.Stage2FreezeDate, currentRelease.Stage2FreezeMtgRef);
                //Analyse column : Freeze 3
                TreatFreezeDate(stage3FreezeDateCell, currentRelease.Stage3FreezeDate, currentRelease.Stage3FreezeMtgRef);

                //Set ReleaseId for details
                ImageButton details = dataItem["releaseDetails"].Controls[0] as ImageButton;
                //details.CommandArgument = currentRelease.Pk_ReleaseId.ToString();                
                
                System.Text.StringBuilder RedirectionURL = new System.Text.StringBuilder();
                RedirectionURL.Append("/desktopmodules/Release/ReleaseDetails.aspx");
                if (currentRelease != null)
                {
                    RedirectionURL.Append("?releaseId=");
                    RedirectionURL.Append(currentRelease.Pk_ReleaseId);
                }
                

                details.Attributes.Add("OnClick", OpenWindow(this.Page, "Release details window", RedirectionURL.ToString(), 850, 650, this.GetType()));
                
            }


        }

        /// <summary>
        /// Manage date with meeting reference
        /// </summary>
        /// <param name="freezeCell"></param>
        /// <param name="freezeDateObject"></param>
        /// <param name="freezeMtg"></param>
        private void TreatFreezeDate(TableCell freezeCell, DateTime? freezeDateObject, string freezeMtg)
        {
            var now = DateTime.Now;
            if (freezeDateObject != null)
            {
                var freezeDate = (DateTime)freezeDateObject;
                if (freezeMtg != null)
                {
                    freezeCell.Text = mixDateAndMtgRef(freezeDate, freezeMtg);
                }

                if (now > freezeDate.Date)
                    freezeCell.CssClass = readonlyCssFreezeReach;
            }
        }

        /// <summary>
        /// return string of date and meeting reference
        /// </summary>
        /// <param name="freezeDate"></param>
        /// <param name="freezeMtg"></param>
        /// <returns></returns>
        private string mixDateAndMtgRef(DateTime freezeDate, String freezeMtg)
        {
            return new StringBuilder().Append(String.Format("{0:yyyy-MM-dd}", freezeDate))
                        .Append(" (")
                        .Append(freezeMtg)
                        .Append(")")
                        .ToString();
        }
       
    }
}