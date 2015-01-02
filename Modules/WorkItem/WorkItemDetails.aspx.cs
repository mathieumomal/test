using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Module.WorkItem
{
    public partial class WorkItemDetails : Page
    {
        #region Fields
        private const string ConstGeneralTab = "General";
        private const string ConstRelatedTab = "Related";
        private const string ConstEmptyField = " - ";
        private const string DateFormatString = "yyyy-MM-dd";
        private const string ConstBaseUrl = "/desktopmodules/WorkItem/WorkItemDetails.aspx?workitemId=";
        private const string DsIdKey = "ETSI_DS_ID";

        private int _userId;
        private int? _workItemId;
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();

                LoadWorkItemDetails();
            }
        }

        protected void ChildWiTable_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //Get workitem row
                var currentWi = (DomainClasses.WorkItem)e.Item.DataItem;

                // Manage WI UID and link to description               
                var lnkWiDescription = e.Item.FindControl("lnkWiDescription") as HyperLink;
                if (lnkWiDescription != null)
                {
                    lnkWiDescription.Text = currentWi.Pk_WorkItemUid.ToString(CultureInfo.InvariantCulture);
                    lnkWiDescription.NavigateUrl = ConstBaseUrl + currentWi.Pk_WorkItemUid;
                    lnkWiDescription.Visible = true;
                }
            }
        }        

        #endregion

        #region Overridden methods

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        #endregion

        #region Helper methods

        private void LoadWorkItemDetails()
        {
            if (_workItemId != null)
            {
                var svc = ServicesFactory.Resolve<IWorkItemService>();
                var wiRightsObject = svc.GetWorkItemByIdExtend(_userId, _workItemId.Value);
                var workitem = wiRightsObject.Key;
                var userRights = wiRightsObject.Value;

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
                    FillRelatedTab(userRights, workitem);


                    //Set Remarks control
                    var rmk = wiRemarks as RemarksControl;
                    if (rmk != null)
                    {
                        rmk.IsEditMode = false;
                        rmk.UserRights = userRights;
                        rmk.DataSource = workitem.Remarks.ToList();
                    }
                }
            }
            else
            {
                wiDetailsBody.Visible = false;
                wiWarning.Visible = true;
            }
        }

        /// <summary>
        /// Set the tabs display
        /// </summary>
        private void BuildTabsDisplay()
        {
            wiDetailRadTabStrip.Tabs.Add(
                new RadTab
                {
                    PageViewID = "RadPage" + ConstGeneralTab,
                    Text = ConstGeneralTab,
                    Selected = true

                });

            wiDetailRadTabStrip.Tabs.Add(
                new RadTab
                {
                    PageViewID = "RadPage" + ConstRelatedTab,
                    Text = ConstRelatedTab,
                    Selected = false
                });

        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights"></param>
        /// <param name="workitem"></param>
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
                    lblWiLevel.Text = "Building Block (2nd level)";
                    break;
                case 3:
                    lblWiLevel.Text = "Working Task (3rd Level)";
                    break;
                case 4:
                    lblWiLevel.Text = "4th level";
                    break;
                case 5:
                    lblWiLevel.Text = "5th level";
                    break;
                default:
                    lblWiLevel.Text = ConstEmptyField;
                    break;
            }

            lblRelease.Text = (workitem.Release != null) ? workitem.Release.Code : ConstEmptyField;
            lblStartDate.Text = (workitem.StartDate != null) ? workitem.StartDate.Value.ToString(DateFormatString) : ConstEmptyField;
            lblEndDate.Text = (workitem.EndDate != null) ? workitem.EndDate.Value.ToString(DateFormatString) : ConstEmptyField;
        }

        /// <summary>
        /// Fill Related Tab with retrieved data
        /// </summary>
        /// <param name="userRights"></param>
        /// <param name="workitem"></param>
        private void FillRelatedTab(DomainClasses.UserRightsContainer userRights, Domain.WorkItem workitem)
        {
            //Bind child work items
            ChildWiTable.ClientSettings.Scrolling.ScrollHeight = Unit.Pixel(100);
            ChildWiTable.DataSource = workitem.ChildWis;
            ChildWiTable.DataBind();

            // Rapporteur:
            // - if rapporteur is found in DB, put a link
            // - else, put the rapporteurStr, but only if user has right to View Personal data.
            if (workitem.RapporteurId.GetValueOrDefault() != default(int))
            {
                lnkRapporteur.Text = workitem.RapporteurName;
                lnkRapporteur.NavigateUrl = ConfigVariables.RapporteurDetailsAddress + workitem.RapporteurId.ToString();
            }
            else
            {
                lnkRapporteur.Visible = false;
                if (userRights.HasRight(Domain.Enum_UserRights.General_ViewPersonalData))
                {
                    lblRapporteur.Text = workitem.RapporteurStr;
                }
            }
            if (!string.IsNullOrEmpty(workitem.RapporteurCompany))
            {
                if (string.IsNullOrEmpty(lblRapporteur.Text) && string.IsNullOrEmpty(lnkRapporteur.Text))
                {
                    lblRapporteur.Text = workitem.RapporteurCompany;
                }
                else
                {
                    lblRapporteur.Text += " (" + workitem.RapporteurCompany + ")";
                }
            }
           
            // Responsible groups
            if (!string.IsNullOrEmpty(workitem.ResponsibleGroups))
                lblResponsibleGroups.Text = workitem.ResponsibleGroups;

            SetMeetingLink(lnkTsgMtg, workitem.TsgApprovalMtgRef, workitem.TsgApprovalMtgId);
            SetMeetingLink(lnkPcgMtg, workitem.PcgApprovalMtgRef, workitem.PcgApprovalMtgId);
            SetMeetingLink(lnkTsgStpMtg, workitem.TsgStoppedMtgRef, workitem.TsgStoppedMtgId);
            SetMeetingLink(lnkPcgStpMtg, workitem.PcgStoppedMtgRef, workitem.PcgStoppedMtgId);

            if (workitem.ParentWi != null && workitem.WiLevel > 1)
            {
                lnkParentWi.Text = workitem.ParentWi.Pk_WorkItemUid.ToString(CultureInfo.InvariantCulture);
                lnkParentWi.NavigateUrl = ConstBaseUrl + workitem.ParentWi.Pk_WorkItemUid;
                lnkParentWi.Visible = true;
                lblParentWorkItem.Text = String.Format(" - {0}", workitem.ParentWi.Name);
            }
            else
            {
                lnkParentWi.Visible = false;
                lblParentWorkItem.Text = "None";
            }

            // Link to specifications
            lnkSpecifications.Target = "_blank";
            lnkSpecifications.NavigateUrl = string.Format(ConfigVariables.RelativeUrlWiRelatedSpecs, workitem.Pk_WorkItemUid);

            //Link to related CRs
            lnkRelatedChanges.Target = "_blank";
            lnkRelatedChanges.NavigateUrl = string.Format(ConfigVariables.RelativeUrlWiRelatedCrs, workitem.Pk_WorkItemUid, 0, string.Empty);

            // Link to TDoc            
            if(!String.IsNullOrEmpty(workitem.Wid))
            {
                lnkWiVersion.Text = workitem.Wid;
                lnkWiVersion.NavigateUrl = "javascript:openTdoc('"+String.Format(ConfigVariables.TdocDetailsUrl, workitem.Wid)+"','"+workitem.Wid+"')";
            }
        }

        /// <summary>
        /// Set meeting link text
        /// </summary>
        /// <param name="link"></param>
        /// <param name="meetingRef"></param>
        /// <param name="meetingId"></param>
        private void SetMeetingLink(HyperLink link, String meetingRef, int? meetingId)
        {
            if (!String.IsNullOrEmpty(meetingRef) || meetingId != null)
            {
                link.Text = meetingRef;
                if (meetingId != null)
                    link.NavigateUrl = ConfigVariables.MeetingDetailsAddress + meetingId;
            }
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            _userId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            _workItemId = (Request.QueryString["workitemId"] != null) ? (int.TryParse(Request.QueryString["workitemId"], out output) ? new int?(output) : null) : null;
        }

        /// <summary>
        /// Retrieve person If
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private int GetUserPersonId(DotNetNuke.Entities.Users.UserInfo userInfo)
        {
            if (userInfo.UserID < 0)
                return 0;
            int personId;
            if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                return personId;
            return 0;
        }

        #endregion
    }
}