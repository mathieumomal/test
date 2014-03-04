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

namespace Etsi.Ultimate.Module.Release
{
    public partial class ReleaseDetails : System.Web.UI.Page
    {
        #region fields
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_ADMIN_TAB = "Administration";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";

        private Nullable<int> UserId;
        private Nullable<int> ReleaseId;
        #endregion

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                getRequestParameters();

                if (ReleaseId != null && UserId != null)
                {
                    IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                    KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId.Value, ReleaseId.Value);
                    Domain.Release release = releaseRightsObject.Key;
                    DomainClasses.UserRightsContainer userRights = releaseRightsObject.Value;


                    BuildTabsDisplay(userRights);


                    if (release == null)
                    {
                        releaseDetailsBody.Visible = false;
                        releaseWarning.Visible = true;

                    }
                    else
                    {
                        FillGeneralTab(userRights, release);

                        if (userRights.HasRight(Domain.Enum_UserRights.Release_ViewCompleteDetails))
                            FillAdminTab(release, svc.GetPreviousReleaseCode(UserId.Value, release.Pk_ReleaseId));

                        ManageButtonDisplay(userRights);

                        //Set Remarks control
                        RemarksControl rmk = releaseRemarks as RemarksControl;
                        rmk.DataSource = release.Remarks.ToList();
                        rmk.IsEditMode = false;
                        rmk.HidePrivateRemarks = userRights.HasRight(Domain.Enum_UserRights.Remarks_ViewPrivate);

                        //Set History control
                        HistoryControl htr = releaseHistory as HistoryControl;
                        htr.DataSource = release.Histories.ToList();
                    }
                }
                else
                {
                    releaseDetailsBody.Visible = false;
                    releaseWarning.Visible = true;
                }
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
                    Selected= true

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
                RadPageAdministration.Visible = false;
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
            ReleaseStatusVal.Text = release.Enum_ReleaseStatus.ReleaseStatus;
            ReleaseNameVal.Text = release.Name;
            ReleaseDescVal.Attributes.Add("href" , release.Description);
            ReleaseShortNameVal.Text = release.ShortName;
            if (release.StartDate != null)
                ReleaseStartDateVal.Text = Convert.ToDateTime(release.StartDate).ToString("yyyy-MM-dd");

            //FreezeStagesPanel
            if(!(userRights.HasRight(Domain.Enum_UserRights.Release_ViewLimitedDetails)))
            {
                ReleaseFreezeStage1Meeting.Text = ((release.Stage1FreezeMtgRef != null) ) ? release.Stage1FreezeMtgRef : CONST_EMPTY_FIELD;
                if (release.Stage1FreezeDate != null)
                    ReleaseFreezeStage1Date.Text = Convert.ToDateTime(release.Stage1FreezeDate).ToString("yyyy-MM-dd");
                else
                    ReleaseFreezeStage1Date.Text = CONST_EMPTY_FIELD;

                ReleaseFreezeStage2Meeting.Text = ((release.Stage2FreezeMtgRef != null) ) ? release.Stage2FreezeMtgRef : CONST_EMPTY_FIELD;
                if (release.Stage2FreezeDate != null)
                    ReleaseFreezeStage2Date.Text = Convert.ToDateTime(release.Stage2FreezeDate).ToString("yyyy-MM-dd");
                else
                    ReleaseFreezeStage2Date.Text = CONST_EMPTY_FIELD;

                ReleaseFreezeStage3Meeting.Text = ((release.Stage3FreezeMtgRef != null) ) ? release.Stage3FreezeMtgRef : CONST_EMPTY_FIELD;

                if (release.Stage3FreezeDate != null)
                    ReleaseFreezeStage3Date.Text = Convert.ToDateTime(release.Stage3FreezeDate).ToString("yyyy-MM-dd");
                else                   
                    ReleaseFreezeStage3Date.Text = CONST_EMPTY_FIELD;
                
            }
            else{
                FreezeStagesPanel.Visible= false;
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
            previousReleaseVal.Text = (previousCode == null) ? CONST_EMPTY_FIELD : previousCode;
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
        private void getRequestParameters()
        {
            UserId = (Request.QueryString["UserID"]!=null) ? new Nullable<int>(int.Parse(Request.QueryString["UserID"])) : null;
            ReleaseId = (Request.QueryString["releaseId"]!=null) ? new Nullable<int>(int.Parse(Request.QueryString["releaseId"])) : null;
        }

        /// <summary>
        /// Manage buttons' display relying on user rights
        /// </summary>
        /// <param name="release"></param>
        /// <param name="userRights"></param>
        private void ManageButtonDisplay(DomainClasses.UserRightsContainer userRights)
        {
            if(userRights.HasRight((Domain.Enum_UserRights.Release_Edit)))
                EditBtn.Visible = true;
            if(userRights.HasRight((Domain.Enum_UserRights.Release_Freeze)))
                FreezeReleaseBtn.Visible = true;
            if(userRights.HasRight((Domain.Enum_UserRights.Release_Close)))
                CloseReleaseBtn.Visible = true;
            
        }
       
    }
}