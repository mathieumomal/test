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

namespace Etsi.Ultimate.Module.Release
{
    public partial class ReleaseEdition : System.Web.UI.Page
    {
        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_ADMIN_TAB = "Administration";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = " - ";
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private int UserId;
        private Nullable<int> ReleaseId;
        private string action;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();

                LoadReleaseDetails();
            }  
        }
    

    private void LoadReleaseDetails()
        {
            IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
            KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId, ReleaseId.Value);
            Domain.Release release = releaseRightsObject.Key;
            DomainClasses.UserRightsContainer userRights = releaseRightsObject.Value;

            releaseCodeVal.Attributes.Add("required", "true");
            ReleaseNameVal.Attributes.Add("required", "true");

            if (action.Equals("Edit"))
            {
                if (ReleaseId != null)
                {
                    
                    
                    /*if (!userRights.HasRight(Domain.Enum_UserRights.Release_Edit))
                    {
                        releaseDetailsBody.Visible = false;
                        releaseError.Visible = true;
                        ErrorMsg.Text = "Sorry but you dont have the right to edit a release.";
                    }
                    else*/{

                        if (release == null)
                        {
                            releaseDetailsBody.Visible = false;
                            releaseWarning.Visible = true;

                        }
                        else
                        {
                            ReleaseDetailRadMultiPage.Height = new System.Web.UI.WebControls.Unit(750, UnitType.Pixel);
                            BuildTabsDisplay(action);
                            FillGeneralTab(userRights, release);

                            if (userRights.HasRight(Domain.Enum_UserRights.Release_ViewCompleteDetails))
                                FillAdminTab(release, svc.GetPreviousReleaseCode(UserId, release.Pk_ReleaseId));


                            //Set Remarks control
                            RemarksControl rmk = releaseRemarks as RemarksControl;
                            rmk.IsEditMode = true;
                            rmk.UserRights = userRights;
                            rmk.DataSource = release.Remarks.ToList();

                            //Set History control
                            HistoryControl htr = releaseHistory as HistoryControl;
                            htr.DataSource = null;
                            htr.ScrollHeight = (int)ReleaseDetailRadMultiPage.Height.Value - 10;
                        }
                    }
                }
                else
                {
                    releaseDetailsBody.Visible = false;
                    releaseWarning.Visible = true;
                }
            }
            else if (action.Equals("Creation"))
            {
                BuildTabsDisplay(action);
                ReleaseDetailRadMultiPage.Height = new System.Web.UI.WebControls.Unit(750, UnitType.Pixel);
                ReleaseStatusVal.CssClass = "status " + ReleaseStatusVal.Text;
                RemarksControl rmk = releaseRemarks as RemarksControl;
                rmk.IsEditMode = true;
                rmk.UserRights = userRights;
                rmk.DataSource = null;
            }
            else {
                releaseDetailsBody.Visible = false;
                releaseError.Visible = true;
            }
            

        }

     /// <summary>
        /// Set the tabs display
        /// </summary>
        /// <param name="userRights"></param>
        private void BuildTabsDisplay(string action)
        {

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_GENERAL_TAB,
                    Text = CONST_GENERAL_TAB,
                    Selected= true

                });            
            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_ADMIN_TAB,
                    Text = CONST_ADMIN_TAB,
                    Selected = false                     
                });
            RadPageAdministration.Visible = true;

            if (action.Equals("Edit"))
            {
                ReleaseDetailRadTabStrip.Tabs.Add(
                    new RadTab()
                    {
                        PageViewID = "RadPage" + CONST_HISTORY_TAB,
                        Text = CONST_HISTORY_TAB,
                        Selected = false
                    });
            }
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

            if (release.Description != null)
                ReleaseDescVal.Attributes.Add("href", release.Description);
            else
            {
                ReleaseDescVal.Visible = false;
                MissigDesc.Visible = true;
                MissigDesc.Text = CONST_EMPTY_FIELD;
            }

            ReleaseShortNameVal.Text = release.ShortName;
            if (release.StartDate != null)
                ReleaseStartDateVal.SelectedDate = release.StartDate.Value;
            else
                ReleaseStartDateVal.SelectedDate = null;

            //FreezeStagesPanel
            
            ReleaseFreezeStage1Meeting.Text = ((release.Stage1FreezeMtgRef != null) ) ? release.Stage1FreezeMtgRef : CONST_EMPTY_FIELD;
            if (release.Stage1FreezeDate != null)
                ReleaseFreezeStage1Date.SelectedDate =release.Stage1FreezeDate;
            else
                ReleaseFreezeStage1Date.SelectedDate = null;

            ReleaseFreezeStage2Meeting.Text = ((release.Stage2FreezeMtgRef != null) ) ? release.Stage2FreezeMtgRef : CONST_EMPTY_FIELD;
            if (release.Stage2FreezeDate != null)
                ReleaseFreezeStage2Date.SelectedDate = release.Stage2FreezeDate;
            else
                ReleaseFreezeStage2Date.SelectedDate = null;

            ReleaseFreezeStage3Meeting.Text = ((release.Stage3FreezeMtgRef != null) ) ? release.Stage3FreezeMtgRef : CONST_EMPTY_FIELD;

            if (release.Stage3FreezeDate != null)
                ReleaseFreezeStage3Date.SelectedDate = release.Stage3FreezeDate;
            else
                ReleaseFreezeStage3Date.SelectedDate = null;
                
            
            

            if (release.EndDate != null)
                ReleaseEndDateVal.SelectedDate = release.EndDate;
            else
                ReleaseEndDateVal.SelectedDate = null;

            ReleaseEndDateMeetingVal.Text = (release.EndMtgRef == null) ? CONST_EMPTY_FIELD : release.EndMtgRef;

            if (release.ClosureDate != null)
                ReleaseClosureDateVal.SelectedDate = release.ClosureDate;
            else
                ReleaseClosureDateVal.SelectedDate = null;

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
            Release2GDecimalVal.Attributes.Add("type", "number");
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
            action = (Request.QueryString["action"] != null) ? Request.QueryString["action"]  : string.Empty;
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

        protected void CloseReleaseDetails_Click(object sender, EventArgs e)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
        }
    }
}