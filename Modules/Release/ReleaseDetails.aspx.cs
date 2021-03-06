﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Module.Release
{
    public partial class ReleaseDetails : System.Web.UI.Page
    {
        #region fields
        private const string CONST_GENERAL_TAB = "General";
        private const string CONST_ADMIN_TAB = "Administration";
        private const string CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";
        public const string DsId_Key = "ETSI_DS_ID";

        private int UserId;
        private bool fromEdit;
        public Nullable<int> ReleaseId;
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();
                
                LoadReleaseDetails();

                //Load parent page to reflect changes
                if (fromEdit)
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Refresh", "window.opener.location.reload(true);", true);
            }
        }

        private void LoadReleaseDetails()
        {
            if (ReleaseId != null)
            {
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId, ReleaseId.Value);
                Domain.Release release = releaseRightsObject.Key;
                DomainClasses.UserRightsContainer userRights = releaseRightsObject.Value;

                if (release == null)
                {
                    releaseDetailsBody.Visible = false;
                    releaseWarning.Visible = true;

                }
                else
                {
                    BuildTabsDisplay(userRights);
                    FillGeneralTab(userRights, release);

                    if (userRights.HasRight(Domain.Enum_UserRights.Release_ViewCompleteDetails))
                        FillAdminTab(release, svc.GetPreviousReleaseCode(release.Pk_ReleaseId).Value);

                    

                    //Set Remarks control
                    RemarksControl rmk = releaseRemarks as RemarksControl;
                    rmk.IsEditMode = false;
                    rmk.UserRights = userRights;
                    rmk.DataSource = release.Remarks.ToList();

                    //Set History control
                    HistoryControl htr = releaseHistory as HistoryControl;
                    htr.DataSource = release.Histories.ToList();
                    htr.ScrollHeight = (int)ReleaseDetailRadMultiPage.Height.Value - 50;

                    //Set Meeting in Freeze window
                    if (release.EndMtgId != null)
                    {
                        MeetingControl mtgFreeze = mcFreeze as MeetingControl;
                        mtgFreeze.SelectedMeetingId = release.EndMtgId.Value;
                    }
                    //Set Meeting in Close window
                    if (release.ClosureMtgId != null)
                    {
                        MeetingControl mtgClose = mcClose as MeetingControl;
                        mtgClose.SelectedMeetingId = release.ClosureMtgId.Value;
                    }

                    ManageButtonDisplay(release, userRights);
                }
            }
            else
            {
                releaseDetailsBody.Visible = false;
                releaseWarning.Visible = true;
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
        private void BuildTabsDisplay(DomainClasses.UserRightsContainer userRights)
        {

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected = true

                });

            if (userRights.HasRight(Domain.Enum_UserRights.Release_ViewCompleteDetails))
            {
                ReleaseDetailRadTabStrip.Tabs.Add(
                    new RadTab()
                    {
                        PageViewID = "RadPage" + CONST_ADMIN_TAB,
                        Text = CONST_ADMIN_TAB,
                        Selected = false
                    });
                RadPageAdministration.Visible = true;
            }

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_HISTORY_TAB,
                    Text = CONST_HISTORY_TAB,
                    Selected = false
                });

        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights"></param>
        /// <param name="release"></param>
        private void FillGeneralTab(DomainClasses.UserRightsContainer userRights, Domain.Release release)
        {
            releaseCodeVal.Text = release.Code;
            ReleaseStatusVal.Text = release.Enum_ReleaseStatus.Description;
            ReleaseStatusVal.CssClass = "status " + ReleaseStatusVal.Text;
            ReleaseNameVal.Text = release.Name;
            if (!string.IsNullOrEmpty(release.Description))
                lnkReleaseDescription.NavigateUrl = release.Description;
            else
            {
                lnkReleaseDescription.Visible = false;
                MissigDesc.Visible = true;
                MissigDesc.Text = CONST_EMPTY_FIELD;
            }

            ReleaseShortNameVal.Text = release.ShortName;
            if (release.StartDate != null)
                ReleaseStartDateVal.Text = Convert.ToDateTime(release.StartDate).ToString("yyyy-MM-dd");
            else
                ReleaseStartDateVal.Text = CONST_EMPTY_FIELD;

            fixContainer.Height = new System.Web.UI.WebControls.Unit(580, UnitType.Pixel);
            ReleaseDetailRadMultiPage.Height = new System.Web.UI.WebControls.Unit(530, UnitType.Pixel);
            //FreezeStagesPanel
            if (userRights.HasRight(Domain.Enum_UserRights.Release_ViewDetails) || userRights.HasRight(Domain.Enum_UserRights.Release_ViewCompleteList))
            {


                ReleaseFreezeStage1Meeting.Text = ((release.Stage1FreezeMtgRef != null)) ? release.Stage1FreezeMtgRef : CONST_EMPTY_FIELD;
                if (release.Stage1FreezeDate != null)
                    ReleaseFreezeStage1Date.Text = Convert.ToDateTime(release.Stage1FreezeDate).ToString("yyyy-MM-dd");
                else
                    ReleaseFreezeStage1Date.Text = CONST_EMPTY_FIELD;

                ReleaseFreezeStage2Meeting.Text = ((release.Stage2FreezeMtgRef != null)) ? release.Stage2FreezeMtgRef : CONST_EMPTY_FIELD;
                if (release.Stage2FreezeDate != null)
                    ReleaseFreezeStage2Date.Text = Convert.ToDateTime(release.Stage2FreezeDate).ToString("yyyy-MM-dd");
                else
                    ReleaseFreezeStage2Date.Text = CONST_EMPTY_FIELD;

                ReleaseFreezeStage3Meeting.Text = ((release.Stage3FreezeMtgRef != null)) ? release.Stage3FreezeMtgRef : CONST_EMPTY_FIELD;

                if (release.Stage3FreezeDate != null)
                    ReleaseFreezeStage3Date.Text = Convert.ToDateTime(release.Stage3FreezeDate).ToString("yyyy-MM-dd");
                else
                    ReleaseFreezeStage3Date.Text = CONST_EMPTY_FIELD;

            }
            else
            {
                FreezeStagesPanel.Visible = false;
            }


            if (release.EndDate != null)
                ReleaseEndDateVal.Text = Convert.ToDateTime(release.EndDate).ToString("yyyy-MM-dd");
            else
                ReleaseEndDateVal.Text = CONST_EMPTY_FIELD;

            ReleaseEndDateMeetingVal.Text = (release.EndMtgRef == null) ? CONST_EMPTY_FIELD : release.EndMtgRef;

            if (release.ClosureDate != null)
                ReleaseClosureDateVal.Text = Convert.ToDateTime(release.ClosureDate).ToString("yyyy-MM-dd");
            else
                ReleaseClosureDateVal.Text = CONST_EMPTY_FIELD;

            ReleaseClosureDateMeetingVal.Text = (release.ClosureMtgRef == null) ? CONST_EMPTY_FIELD : release.ClosureMtgRef;


        }

        /// <summary>
        /// Fill Administration Tab with retrieved data
        /// </summary>
        /// <param name="release"></param>
        /// <param name="previousCode"></param>
        private void FillAdminTab(Domain.Release release, string previousCode)
        {
            previousReleaseVal.Text = (previousCode == string.Empty) ? CONST_EMPTY_FIELD : previousCode;
            ITURCodeVal.Text = (release.IturCode == null) ? CONST_EMPTY_FIELD : release.IturCode;

            Release2GDecimalVal.Text = (release.Version2g == null) ? CONST_EMPTY_FIELD : release.Version2g.ToString();
            Release3GDecimalVal.Text = (release.Version3g == null) ? CONST_EMPTY_FIELD : release.Version3g.ToString();

            Release2GVal.Text = (release.Version2gBase36 == null) ? CONST_EMPTY_FIELD : release.Version2gBase36;
            Release3GVal.Text = (release.Version3gBase36 == null) ? CONST_EMPTY_FIELD : release.Version3gBase36;

            WPMCodes2GVal.Text = (release.WpmCode2g == null) ? CONST_EMPTY_FIELD : release.WpmCode2g.ToString();
            WPMCodes3GVal.Text = (release.WpmCode3g == null) ? CONST_EMPTY_FIELD : release.WpmCode3g.ToString();
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());
            ReleaseId = (Request.QueryString["releaseId"] != null) ? (int.TryParse(Request.QueryString["releaseId"], out output) ? new Nullable<int>(output) : null) : null;
            fromEdit = (Request.QueryString["fromEdit"] != null) ? Convert.ToBoolean(Request.QueryString["fromEdit"]) : false;
        }

        /// <summary>
        /// Manage buttons' display relying on user rights
        /// </summary>
        /// <param name="release">Release</param>
        /// <param name="userRights">User Rights</param>
        private void ManageButtonDisplay(DomainClasses.Release release, DomainClasses.UserRightsContainer userRights)
        {
            if (userRights.HasRight(Domain.Enum_UserRights.Release_Edit))
            {
                EditBtn.Visible = true;
                EditBtn.Text = "Edit";
            }

            ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
            var versionsPendingUpload = svc.CountVersionsPendingUploadByReleaseId(ReleaseId ?? 0).ToString();

            if (userRights.HasRight(Domain.Enum_UserRights.Release_Freeze))
            {
                Freeze_VersionsPendingUpload.Text = versionsPendingUpload;
                FreezeReleaseBtn.Visible = true;
                FreezeReleaseBtn.OnClientClick = "freezeRelease(); return false;";
            }
            if (userRights.HasRight(Domain.Enum_UserRights.Release_Close))
            {
                Close_VersionsPendingUpload.Text = versionsPendingUpload;
                CloseReleaseBtn.Visible = true;
                CloseReleaseBtn.OnClientClick = "closeRelease(); return false;";
            }
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

        protected void EditReleaseDetails_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            Response.Redirect("/DesktopModules/Release/ReleaseEdition.aspx?releaseId=" + ReleaseId.Value + "&action=Edit&selectedTab=" + ReleaseDetailRadMultiPage.SelectedIndex);
        }


        protected void btnConfirmFreeze_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            if (ReleaseId != null)
            {
                MeetingControl mtgControl = mcFreeze as MeetingControl;
                var mtg = mtgControl.SelectedMeeting;

                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                
                if ((mtg == null) || ((svc.GetReleaseById(UserId, ReleaseId.Value).Key.Stage3FreezeDate ?? default(DateTime)) <= mtg.END_DATE))
                {
                    ServiceResponse<bool> response;
                    if (mtg == null)
                        response = svc.FreezeRelease(ReleaseId.Value, null, UserId, null, null);
                    else
                        response = svc.FreezeRelease(ReleaseId.Value, (mtg.END_DATE ?? default(DateTime)), UserId, mtg.MTG_ID, mtg.MtgShortRef);

                    if (response.Result)
                    {
                        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close(); window.opener.location.reload(true);", true);
                    }
                    else if (response.Report.GetNumberOfErrors() > 0)
                    {
                        RadWindowManager1.RadAlert("Error occurred: " + response.Report.ErrorList.First(), 400, 150, "Error", "window.radopen(null, 'RadWindow_FreezeConfirmation')", "images/error.png");
                    }
                }
                else
                    RadWindowManager1.RadAlert("End date must be greater than Freeze stage 3 date.", 400, 150, "Error", "window.radopen(null, 'RadWindow_FreezeConfirmation')", "images/error.png");
            }
        }

        /// <summary>
        /// Click event of Closure Confirmation
        /// </summary>
        /// <param name="sender">Source of Event</param>
        /// <param name="e">Event Args</param>
        protected void btnConfirmClosure_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            if (ReleaseId != null)
            {
                MeetingControl mtgControl = mcClose as MeetingControl;
                var mtg = mtgControl.SelectedMeeting;

                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();

                ServiceResponse<bool> response;
                if (mtg == null)
                    response = svc.CloseRelease(ReleaseId.Value, null, null, null, UserId);
                else
                    response = svc.CloseRelease(ReleaseId.Value, (mtg.END_DATE ?? default(DateTime)), mtg.MtgShortRef, mtg.MTG_ID, UserId);

                if (response.Result)
                {
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close(); window.opener.location.reload(true);", true);
                }
                else if (response.Report.GetNumberOfErrors() > 0)
                {
                    RadWindowManager1.RadAlert("Error occurred: " + response.Report.ErrorList.First(), 400, 150, "Error", "window.radopen(null, 'RadWindow_FreezeConfirmation')", "images/error.png");
                }
            }
        }
    }
}