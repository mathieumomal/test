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
        private const string CONST_EMPTY_FIELD = "";
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private int UserId;
        private Nullable<int> ReleaseId;
        private string action;
        protected RemarksControl releaseRemarks; 

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

            dataValidationSetUp();

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
                            FillAdminTab(release, svc.GetAllReleasesCodes(UserId, ReleaseId.Value));


                            //Set Remarks control
                            /*RemarksControl rmk = releaseRemarks as RemarksControl;*/
                            releaseRemarks.IsEditMode = true;
                            releaseRemarks.UserRights = userRights;
                            releaseRemarks.DataSource = release.Remarks.ToList();

                            //Set History control
                            HistoryControl htr = releaseHistory as HistoryControl;
                            htr.DataSource = release.Histories.ToList();
                            htr.ScrollHeight = (int)ReleaseDetailRadMultiPage.Height.Value - 50;
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
                ReleaseStartDateVal.SelectedDate = DateTime.Now;
                RemarksControl rmk = releaseRemarks as RemarksControl;
                rmk.IsEditMode = true;
                rmk.UserRights = userRights;
                rmk.DataSource = null;

                previousReleaseVal.DataTextField = "Value";
                previousReleaseVal.DataValueField = "Key";
                previousReleaseVal.DataSource = svc.GetAllReleasesCodes(UserId, ReleaseId.Value);
                previousReleaseVal.DataBind();

                /*SaveBtn.Attributes.Add("disabled", "disabled");
                SaveBtn.CssClass = "disabledLink";*/
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

            ReleaseDescVal.Text = release.Description;

            ReleaseShortNameVal.Text = release.ShortName;
            if (release.StartDate != null)
                ReleaseStartDateVal.SelectedDate = release.StartDate.Value;
            else
                ReleaseStartDateVal.SelectedDate = null;

            //FreezeStagesPanel
            
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
                
            
            

            /*if (release.EndDate != null)
                ReleaseEndDateVal.Text = Convert.ToDateTime(release.EndDate).ToString("yyyy-MM-dd");
            else
                ReleaseEndDateVal.Text = CONST_EMPTY_FIELD;

            ReleaseEndDateMeetingVal.Text = (release.EndMtgRef == null) ? CONST_EMPTY_FIELD : release.EndMtgRef;*/
            MeetingControl endMeeting = ReleaseEndMeeting as MeetingControl;
            endMeeting.SelectedMeetingId = (release.EndMtgId != null) ? release.EndMtgId.Value : default(int);
            endMeeting.DisplayLabel = true;

            /*if (release.ClosureDate != null)
                ReleaseClosureDateVal.Text = Convert.ToDateTime(release.ClosureDate).ToString("yyyy-MM-dd"); 
            else
                ReleaseClosureDateVal.Text = CONST_EMPTY_FIELD;

            ReleaseClosureDateMeetingVal.Text = (release.ClosureMtgRef == null) ? CONST_EMPTY_FIELD : release.ClosureMtgRef;*/
            MeetingControl closureMeeting = ReleaseClosureMeeting as MeetingControl;
            closureMeeting.SelectedMeetingId = (release.ClosureMtgId != null) ? release.ClosureMtgId.Value : default(int);
            closureMeeting.DisplayLabel = true;

            
        }

        /// <summary>
        /// Fill Administration Tab with retrieved data
        /// </summary>
        /// <param name="release"></param>
        /// <param name="previousCode"></param>
        private void FillAdminTab(Domain.Release release, Dictionary<int, string> allReleasesCodes)
        {
            //previousReleaseVal.Text = (previousCode == null) ? CONST_EMPTY_FIELD : previousCode;

            previousReleaseVal.DataTextField = "Value";
            previousReleaseVal.DataValueField = "Key";
            previousReleaseVal.DataSource = allReleasesCodes;  
            previousReleaseVal.DataBind();

            if (action.Equals("Edit"))
            {
                previousReleaseVal.SelectedValue = release.Pk_ReleaseId.ToString();
                previousReleaseVal.SelectedIndex = previousReleaseVal.SelectedIndex + 1; 

                ITURCodeVal.Text = (release.IturCode == null) ? CONST_EMPTY_FIELD : release.IturCode;

                Release2GDecimalVal.Text = (release.Version2g == null) ? CONST_EMPTY_FIELD : release.Version2g.ToString();
                Release3GDecimalVal.Text = (release.Version3g == null) ? CONST_EMPTY_FIELD : release.Version3g.ToString();

                Release2GVal.Text = (release.Version2gBase36 == null) ? CONST_EMPTY_FIELD : release.Version2gBase36;
                Release3GVal.Text = (release.Version3gBase36 == null) ? CONST_EMPTY_FIELD : release.Version3gBase36;

                WPMCodes2GVal.Text = (release.WpmCode2g == null) ? CONST_EMPTY_FIELD : release.WpmCode2g.ToString();
                WPMCodes3GVal.Text = (release.WpmCode3g == null) ? CONST_EMPTY_FIELD : release.WpmCode3g.ToString();
            }
        }

        private void dataValidationSetUp()
        {
            releaseCodeVal.Attributes.Add("data-required", "true");

            ReleaseNameVal.Attributes.Add("data-required", "true");
            Release2GDecimalVal.Attributes.Add("data-pattern", "^[0-9]+$");
            Release3GDecimalVal.Attributes.Add("data-pattern", "^[0-9]+$");
            ReleaseShortNameVal.Attributes.Add("data-required", "true");
            previousReleaseVal.Attributes.Add("data-required", "true");   
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

        protected void SaveEditedRelease_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            if (ReleaseId != null)
            {
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                Domain.Release editedRelease;
                
                

                if(action.Equals("Edit")){
                    editedRelease  = svc.GetReleaseById(UserId, ReleaseId.Value).Key;
                    editedRelease = setReleaseEditionValues(editedRelease);
                    svc.EditRelease(editedRelease,int.Parse(previousReleaseVal.SelectedValue));
                }
                else if(action.Equals("Creation")){
                    editedRelease = new Domain.Release();
                    editedRelease = setReleaseEditionValues(editedRelease);
                    svc.CreateRelease(editedRelease,int.Parse(previousReleaseVal.SelectedValue));
                }
                else{
                    //Bad request
                    releaseDetailsBody.Visible = false;
                    releaseError.Visible = true;
                }                                
                //Response.Redirect("ReleaseDetails.aspx?releaseId=" + ReleaseId.Value);
            }
        }

        private Domain.Release setReleaseEditionValues(Domain.Release editedRelease)
        {
            editedRelease.Code = releaseCodeVal.Text;
            editedRelease.Name = ReleaseNameVal.Text;
            editedRelease.Description = ReleaseDescVal.Text;
            editedRelease.ShortName = ReleaseShortNameVal.Text;
            editedRelease.StartDate = ReleaseStartDateVal.SelectedDate;
            //Need to check date format
            //editedRelease.EndDate = Convert.ToDateTime(ReleaseEndDateVal.Text);
            //editedRelease.ClosureDate = Convert.ToDateTime(ReleaseClosureDateVal.Text);  
            editedRelease.IturCode = ITURCodeVal.Text;
            editedRelease.Version2g = new Nullable<int>(int.Parse(Release2GDecimalVal.Text));
            editedRelease.Version3g = new Nullable<int>(int.Parse(Release3GDecimalVal.Text));
            editedRelease.WpmCode2g = WPMCodes2GVal.Text;
            editedRelease.WpmCode3g = WPMCodes3GVal.Text;

            return editedRelease;
        }
    }
}