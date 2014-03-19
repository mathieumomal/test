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
using System.Web.UI.HtmlControls;
using System.Web;
using DotNetNuke.Entities.Tabs;

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
        protected Ultimate.Controls.FullView ultFullView;
        
        private const String cssFreezeReach = "freezeReach";
        private const String cssClosedColor = "closed";

        public const string DsId_Key = "ETSI_DS_ID";
        //Decoration class for help tooltips
        public const string cssHelpTooltip = "helpTooltip";


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ManageFullView();

                int personId = GetUserPersonId(UserInfo);

                //Example : mock to fake service layer -> ServicesFactory.Container.RegisterType<IReleaseService, ReleaseServiceMock>(new TransientLifetimeManager());
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();//Get the mock instead service classe

                //Example : mock to fake rights manager : ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

                //Calling the service
                KeyValuePair<List<DomainClasses.Release>,UserRightsContainer> releaseRightsObjects = svc.GetAllReleases(GetUserPersonId(UserInfo));

                //Setting up the release list
                List<DomainClasses.Release> releasesList = releaseRightsObjects.Key.OrderByDescending(release => release.SortOrder).ToList();
                releasesTable.DataSource = releasesList;

                //Applying rights
                UserRightsContainer userRights = releaseRightsObjects.Value;

                //Show freezes if connected
                if (!userRights.HasRight(Enum_UserRights.Release_ViewCompleteList))
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

        private void ManageFullView()
        {
            ultFullView.ModuleId = ModuleId;
            ultFullView.TabId = TabController.CurrentPage.TabID;

            var address = Request.IsSecureConnection ? "https://" : "http://";
            address += Request["HTTP_HOST"] + "/";
            ultFullView.BaseAddress = address;



            var nameValueCollection = HttpContext.Current.Request.QueryString;
            var urlParams = new Dictionary<string, string>();
            foreach (var k in nameValueCollection.AllKeys)
            {
                if (k != null && nameValueCollection[k] != null)
                    urlParams.Add (k, nameValueCollection[k]);
            }
            ultFullView.UrlParams = urlParams;
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
        /// Handler data cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void releasesTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            //Tooltip for header (just add "HeaderTooltip="Release name"" to GridTemplateColumn)
            /**
             * 
             *  .helpTooltip{
	                border-bottom: 1px gray dashed;
	                cursor: help;
                }
             * */
            if (e.Item is GridHeaderItem)
            {
                GridHeaderItem header = (GridHeaderItem)e.Item;
                foreach (TableCell cell in header.Cells)
                {
                    if (cell.Controls.Count > 0)
                    {
                        if (cell.ToolTip != "")
                        {
                            HtmlGenericControl helpSpan = new HtmlGenericControl("span");
                            helpSpan.Attributes.Add("class", cssHelpTooltip);
                            helpSpan.InnerHtml = cell.Text;
                            cell.Text = "";
                            cell.Controls.Add(helpSpan);
                            rdTooltipHeader.TargetControls.Add(cell.ClientID, true);
                        }
                    }
                }

            }
            //Analyse on row
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;//Get row
                //Get table cell
                TableCell tblCellClosureDate = dataItem["ClosureDate"];
                TableCell tblCellEndDate = dataItem["EndDate"];
                TableCell tblCellStage1FreezeDate = dataItem["Stage1FreezeDate"];
                TableCell tblCellStage2FreezeDate = dataItem["Stage2FreezeDate"];
                TableCell tblCellStage3FreezeDate = dataItem["Stage3FreezeDate"];

                //Get release row
                DomainClasses.Release currentRelease = (DomainClasses.Release) e.Item.DataItem;
                
                // Manage release Name and link to description
                Label lblReleaseName = e.Item.FindControl("lblReleaseName") as Label;
                HyperLink lnkReleaseDescription = e.Item.FindControl("lnkReleaseDescription") as HyperLink;
                lblReleaseName.Visible = string.IsNullOrEmpty(currentRelease.Description);
                lnkReleaseDescription.Visible = ! lblReleaseName.Visible;
                lblReleaseName.Text = currentRelease.Name;
                lnkReleaseDescription.Text = currentRelease.Name;
                lnkReleaseDescription.NavigateUrl = currentRelease.Description;

                //Analyse column : Closure date
                if (currentRelease.ClosureDate != null && currentRelease.ClosureMtgRef != null)
                {
                    tblCellClosureDate.Text = mixDateAndMtgRef((DateTime)currentRelease.ClosureDate, currentRelease.ClosureMtgRef);
                }
                tblCellClosureDate.CssClass = cssClosedColor;
                //Analyse column : End date
                if (currentRelease.EndDate != null && currentRelease.EndMtgRef != null)
                {
                    tblCellEndDate.Text = mixDateAndMtgRef((DateTime)currentRelease.EndDate, currentRelease.EndMtgRef);
                }
                

                //Analyse column : Freeze 1
                TreatFreezeDate(tblCellStage1FreezeDate, currentRelease.Stage1FreezeDate, currentRelease.Stage1FreezeMtgRef);
                //Analyse column : Freeze 2
                TreatFreezeDate(tblCellStage2FreezeDate, currentRelease.Stage2FreezeDate, currentRelease.Stage2FreezeMtgRef);
                //Analyse column : Freeze 3
                TreatFreezeDate(tblCellStage3FreezeDate, currentRelease.Stage3FreezeDate, currentRelease.Stage3FreezeMtgRef);

                //Set ReleaseId for details
            }


        }

        /// <summary>
        /// Manage date with meeting reference
        /// </summary>
        /// <param name="tblCellFreezeCell"></param>
        /// <param name="freezeDateObject"></param>
        /// <param name="freezeMtg"></param>
        private void TreatFreezeDate(TableCell tblCellFreezeCell, DateTime? freezeDateObject, string freezeMtg)
        {
            var now = DateTime.Now;
            if (freezeDateObject != null)
            {
                var freezeDate = (DateTime)freezeDateObject;
                if (freezeMtg != null)
                {
                    tblCellFreezeCell.Text = mixDateAndMtgRef(freezeDate, freezeMtg);
                }

                if (now > freezeDate.Date)
                    tblCellFreezeCell.CssClass = cssFreezeReach;
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