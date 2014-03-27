using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Controls;

namespace Etsi.Ultimate.Module.WorkItem
{
    public partial class WorkItemDetails : System.Web.UI.Page
    {
        #region fields
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_RELATED_TAB = "Related";
        private const string CONST_EMPTY_FIELD = " - ";
        private const string DATE_FORMAT_STRING = "yyyy-MM-dd";
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private const string CONST_BASE_URL = "/desktopmodules/WorkItem/WorkItemDetails.aspx?workitemId=";

        private int UserId;
        public Nullable<int> WorkItemId;
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();

                LoadWorkItemDetails();
            }
        }

        private void LoadWorkItemDetails()
        {
            if (WorkItemId != null)
            {
                IWorkItemService svc = ServicesFactory.Resolve<IWorkItemService>();
                KeyValuePair<DomainClasses.WorkItem, DomainClasses.UserRightsContainer> wiRightsObject = svc.GetWorkItemById(UserId, WorkItemId.Value);
                Domain.WorkItem workitem = wiRightsObject.Key;
                DomainClasses.UserRightsContainer userRights = wiRightsObject.Value;

                if (workitem == null)
                {
                    wiDetailsBody.Visible = false;
                    wiWarning.Visible = true;

                }
                else
                {
                    lblHeaderText.Text = "WI # " + workitem.Pk_WorkItemUid + ((String.IsNullOrEmpty(workitem.Acronym)) ? "" : " - " + workitem.Acronym);
                    BuildTabsDisplay();
                    FillGeneralTab(userRights, workitem);
                    FillRelatedTab(workitem);


                    //Set Remarks control
                    RemarksControl rmk = wiRemarks as RemarksControl;
                    rmk.IsEditMode = false;
                    rmk.UserRights = userRights;
                    rmk.DataSource = workitem.Remarks.ToList();
                }
            }
            else
            {
                wiDetailsBody.Visible = false;
                wiWarning.Visible = true;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        /// <summary>
        /// Set the tabs display
        /// </summary>
        /// <param name="userRights"></param>
        private void BuildTabsDisplay()
        {
            wiDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected = true

                });

            wiDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_RELATED_TAB,
                    Text = CONST_RELATED_TAB,
                    Selected = false
                });

        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights"></param>
        /// <param name="release"></param>
        private void FillGeneralTab(DomainClasses.UserRightsContainer userRights, Domain.WorkItem workitem)
        {
            lblName.Text = workitem.Name;
            lblAcronym.Text = workitem.Acronym;
            lblType.Text = "Work Item";

            //Bind WI status
            if (workitem.TsgApprovalMtgId == null)
            {
                lblStatus.Text = "Awaiting approval";
                lblStatus.CssClass = "AwaitingApproval";
            }
            else if (workitem.TsgStoppedMtgId != null)
            {
                lblStatus.Text = "Stopped";
                lblStatus.CssClass = "Stopped";
            }
            else if (workitem.Completion != null)
            {
                if (workitem.Completion.Value == 0)
                {
                    lblStatus.Text = "Not started (0%)";
                    lblStatus.CssClass = "NotStarted";
                }
                if (workitem.Completion.Value > 0 && workitem.Completion < 100)
                {
                    lblStatus.Text = String.Format("Active ({0}%)", workitem.Completion.Value.ToString());
                    lblStatus.CssClass = "Active";
                }
                if (workitem.Completion.Value >= 100)
                {
                    lblStatus.Text = "Completed (100%)";
                    lblStatus.CssClass = "Completed";
                }
            }

            switch (workitem.WiLevel)
            {
                case 1:
                    lblWiLevel.Text = "Feature (1st level)";
                    break;
                case 2:
                    lblWiLevel.Text = "Building Block (Up to 2nd level)";
                    break;
                case 3:
                    lblWiLevel.Text = "Working Task (Up to 3rd Level)";
                    break;
                case 4:
                    lblWiLevel.Text = "Up to 4th level";
                    break;
                case 5:
                    lblWiLevel.Text = "Up to 5th level";
                    break;
                default:
                    lblWiLevel.Text = CONST_EMPTY_FIELD;
                    break;
            }

            lblRelease.Text = (workitem.Release != null) ? workitem.Release.Code : CONST_EMPTY_FIELD;
            lblStartDate.Text = (workitem.StartDate != null) ? workitem.StartDate.Value.ToString(DATE_FORMAT_STRING) : CONST_EMPTY_FIELD;
            lblEndDate.Text = (workitem.EndDate != null) ? workitem.EndDate.Value.ToString(DATE_FORMAT_STRING) : CONST_EMPTY_FIELD;
        }

        private void FillRelatedTab(Domain.WorkItem workitem)
        {
            //Bind child work items
            ChildWiTable.ClientSettings.Scrolling.ScrollHeight = Unit.Pixel(100);
            ChildWiTable.DataSource = workitem.ChildWis;
            ChildWiTable.DataBind();


            //Bind lables
            lnkRapporteur.Text = workitem.RapporteurStr;
            lblRapporteur.Text = (String.IsNullOrEmpty(workitem.RapporteurCompany)) ? "" : String.Format("({0})", workitem.RapporteurCompany);

            lblResponsibleGroups.Text = workitem.ResponsibleGroups;
            if (workitem.ParentWi != null && workitem.WiLevel > 1)
            {
                lnkParentWi.Text = workitem.Pk_WorkItemUid.ToString();
                lnkParentWi.NavigateUrl = CONST_BASE_URL + workitem.ParentWi.Pk_WorkItemUid;
                lnkParentWi.Visible = true;
                lblParentWorkItem.Visible = false;
            }
            else
            {
                lnkParentWi.Visible = false;
                lblParentWorkItem.Visible = true;
                lblParentWorkItem.Text = "None";
            }
        }

        public void ChildWiTable_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //Get workitem row
                DomainClasses.WorkItem currentWi = (DomainClasses.WorkItem)e.Item.DataItem;

                // Manage WI UID and link to description               
                HyperLink lnkWiDescription = e.Item.FindControl("lnkWiDescription") as HyperLink;
                lnkWiDescription.Text = currentWi.Pk_WorkItemUid.ToString();
                lnkWiDescription.NavigateUrl = CONST_BASE_URL + currentWi.Pk_WorkItemUid;
                lnkWiDescription.Visible = true;
            }
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            WorkItemId = (Request.QueryString["workitemId"] != null) ? (int.TryParse(Request.QueryString["workitemId"], out output) ? new Nullable<int>(output) : null) : null;
        }

        /// <summary>
        /// Retrieve person If
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
                if (Int32.TryParse(UserInfo.Profile.GetPropertyValue(DsId_Key), out personID))
                    return personID;
            }
            return 0;
        }

        protected void CloseWorkItemDetails_Click(object sender, EventArgs e)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
        }
    }
}