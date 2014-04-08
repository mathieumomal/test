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
        private const String CREATION_MODE = "Creation";
        private const String EDIT_MODE = "Edit";

        private static String CONST_GENERAL_TAB = "General";
        private static String CONST_ADMIN_TAB = "Administration";
        private static String CONST_HISTORY_TAB = "History";
        private const string CONST_EMPTY_FIELD = "";
        public static readonly string DsId_Key = "ETSI_DS_ID";
        public string serverSideJSScript = "";
        private int UserId;        
        private Nullable<int> ReleaseId;
        private string action;
        private int selectedTab;
        protected RemarksControl releaseRemarks;

        protected MeetingControl FreezeStage1Meeting;
        protected MeetingControl FreezeStage2Meeting;
        protected MeetingControl FreezeStage3Meeting;
        protected MeetingControl ReleaseEndMeeting;
        protected MeetingControl ReleaseClosureMeeting;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Getting request parameters
                GetRequestParameters();
                //build view 
                LoadReleaseDetails();
            }
            releaseRemarks.AddRemarkHandler += releaseRemarks_AddRemarkHandler;
        }


        /// <summary>
        /// Builds Release Edit/Creation view depending on request parameters
        /// </summary>
        private void LoadReleaseDetails()
        {
            // Populate fileds of the view with basic validation rules
            dataValidationSetUp();

            if (action.Equals(EDIT_MODE))
            {
                if (ReleaseId != null)
                {
                    IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                    KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId, ReleaseId.Value);
                    Domain.Release release = releaseRightsObject.Key;
                    DomainClasses.UserRightsContainer userRights = releaseRightsObject.Value;

                    if (userRights.HasRight(Domain.Enum_UserRights.Release_Edit))
                    {
                        if (release == null)
                        {
                            releaseDetailsBody.Visible = false;
                            releaseWarning.Visible = true;
                        }
                        else
                        {
                            BuildTabsDisplay(action);
                            FillGeneralTab(release, action);
                            FillAdminTab(release, svc.GetAllReleasesCodes(ReleaseId.Value), svc.GetPreviousReleaseCode(release.Pk_ReleaseId).Key);
                            
                            //Set Remarks control
                            releaseRemarks.UserRights = userRights;
                            releaseRemarks.DataSource = release.Remarks.ToList();

                            //Set History control
                            HistoryControl htr = releaseHistory as HistoryControl;
                            htr.DataSource = release.Histories.ToList();
                            htr.ScrollHeight = (int)ReleaseEditRadMultiPage.Height.Value - 50;

                            SaveBtnDisabled.Style.Add("display", "none");
                        }
                    }
                    else
                    {
                        releaseDetailsBody.Visible = false;
                        releaseError.Visible = true;
                        ErrorMsg.Text = "Sorry but you do not have the right to edit a release.";
                    }
                }
                else
                {
                    releaseDetailsBody.Visible = false;
                    releaseWarning.Visible = true;
                }
            }
            else if (action.Equals(CREATION_MODE))
            {
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                DomainClasses.UserRightsContainer userRights = svc.GetAllReleases(UserId).Value;

                BuildTabsDisplay(action);
                FillGeneralTab(null, action);

                releaseRemarks.UserRights = userRights;
                releaseRemarks.DataSource = null;

                var allReleases = svc.GetAllReleasesCodes(default(int));
                FillAdminTab(null, allReleases, allReleases.First().Key);

                SaveBtn.Style.Add("display", "none");
                SaveBtnDisabled.Style.Remove("display");
            }
            else
            {
                releaseDetailsBody.Visible = false;
                releaseError.Visible = true;

            }


        }

        /// <summary>
        /// Add New Remark
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void releaseRemarks_AddRemarkHandler(object sender, EventArgs e)
        {            
            List<Domain.Remark> datasource = releaseRemarks.DataSource;
            //Get display name
            IPersonService svc = ServicesFactory.Resolve<IPersonService>();
            string personDisplayName = svc.GetPersonDisplayName(GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo()));
            bool isUserMCCMember = svc.IsUserMCCMember(UserId);
            datasource.Add(new Domain.Remark()
            {
                Fk_PersonId = UserId,
                Fk_ReleaseId = ReleaseId,
                IsPublic = isUserMCCMember ? false : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = releaseRemarks.RemarkText,
                PersonName = personDisplayName
            });

            releaseRemarks.DataSource = datasource;
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
                    Selected = true
                });

            ReleaseDetailRadTabStrip.Tabs.Add(
                new RadTab()
                {
                    PageViewID = "RadPage" + CONST_ADMIN_TAB,
                    Text = CONST_ADMIN_TAB,
                    Selected = false
                });
            RadPageAdministration.Visible = true;


            if (action.Equals(EDIT_MODE))
            {
                ReleaseDetailRadTabStrip.Tabs.Add(
                    new RadTab()
                    {
                        PageViewID = "RadPage" + CONST_HISTORY_TAB,
                        Text = CONST_HISTORY_TAB,
                        Selected = false
                    });
            }

            ReleaseDetailRadTabStrip.SelectedIndex = selectedTab;
            ReleaseEditRadMultiPage.SelectedIndex = selectedTab;
        }

        /// <summary>
        /// Fill General Tab with retrieved data
        /// </summary>
        /// <param name="userRights"></param>
        /// <param name="release"></param>
        private void FillGeneralTab(Domain.Release release, string action)
        {
            //Populate fields with data
            if (action.Equals(EDIT_MODE))
            {
                releaseCodeVal.Text = release.Code;
                pageTitle.Value = releaseCodeVal.Text;
                ReleaseStatusVal.Text = release.Enum_ReleaseStatus.Description;
                ReleaseNameVal.Text = release.Name;
                ReleaseDescVal.Text = release.Description;
                ReleaseShortNameVal.Text = release.ShortName;
                if (release.StartDate != null)
                    ReleaseStartDateVal.SelectedDate = release.StartDate.Value;
                else
                    ReleaseStartDateVal.SelectedDate = null;
                FreezeStage1Meeting.SelectedMeetingId = (release.Stage1FreezeMtgId != null) ? release.Stage1FreezeMtgId.Value : default(int);
                FreezeStage2Meeting.SelectedMeetingId = (release.Stage2FreezeMtgId != null) ? release.Stage2FreezeMtgId.Value : default(int);
                FreezeStage3Meeting.SelectedMeetingId = (release.Stage3FreezeMtgId != null) ? release.Stage3FreezeMtgId.Value : default(int);
                ReleaseClosureMeeting.SelectedMeetingId = (release.ClosureMtgId != null) ? release.ClosureMtgId.Value : default(int);
                ReleaseEndMeeting.SelectedMeetingId = (release.EndMtgId != null) ? release.EndMtgId.Value : default(int);
            }

            if (action.Equals(CREATION_MODE))
            {
                ReleaseStatusVal.CssClass = "status " + ReleaseStatusVal.Text;
                ReleaseStartDateVal.SelectedDate = DateTime.Now;
            }

            //Set events' functions
            releaseCodeVal.Attributes.Add("onblur", string.Format("checkIfCodeUsed(\"{0}\"); return false;",
                                                        releaseCodeVal.UniqueID));
            ReleaseStatusVal.CssClass = "status " + ReleaseStatusVal.Text;

            ReleaseDescVal.Attributes.Add("onblur", string.Format("validateURL(\"{0}\"); return false;",
                                                        ReleaseDescVal.UniqueID));

            //Configure Meeting control to display date's label
            FreezeStage1Meeting.DisplayLabel = true;
            FreezeStage1Meeting.CssClass = "meetingControl";
            FreezeStage2Meeting.DisplayLabel = true;
            FreezeStage2Meeting.CssClass = "meetingControl";
            FreezeStage3Meeting.DisplayLabel = true;
            FreezeStage3Meeting.CssClass = "meetingControl";
            ReleaseEndMeeting.DisplayLabel = true;
            ReleaseEndMeeting.CssClass = "meetingControl";
            ReleaseClosureMeeting.DisplayLabel = true;
            ReleaseClosureMeeting.CssClass = "meetingControl";
        }

        /// <summary>
        /// Fill Administration Tab with retrieved data
        /// </summary>
        /// <param name="release"></param>
        /// <param name="previousCode"></param>
        private void FillAdminTab(Domain.Release release, Dictionary<int, string> allReleasesCodes, int previousReleaseID)
        {
            previousReleaseVal.DataTextField = "Value";
            previousReleaseVal.DataValueField = "Key";
            previousReleaseVal.DataSource = allReleasesCodes;
            previousReleaseVal.DataBind();

            // Setting the call backs for 2G and 3G
            Release2GDecimalVal.Attributes.Add("onchange", string.Format("convertTo36(\"{0}\"); return false;",
                                                    Release2GDecimalVal.UniqueID));
            Release3GDecimalVal.Attributes.Add("onchange", string.Format("convertTo36(\"{0}\"); return false;",
                                                    Release3GDecimalVal.UniqueID));

            //Populate view with Release data
            if (action.Equals(EDIT_MODE))
            {
                previousReleaseVal.SelectedValue = previousReleaseID.ToString();

                ITURCodeVal.Text = (release.IturCode == null) ? CONST_EMPTY_FIELD : release.IturCode;

                Release2GDecimalVal.Text = (release.Version2g == null) ? CONST_EMPTY_FIELD : release.Version2g.ToString();
                Release3GDecimalVal.Text = (release.Version3g == null) ? CONST_EMPTY_FIELD : release.Version3g.ToString();

                Release2GVal.Text = (release.Version2gBase36 == null) ? CONST_EMPTY_FIELD : release.Version2gBase36;
                Release3GVal.Text = (release.Version3gBase36 == null) ? CONST_EMPTY_FIELD : release.Version3gBase36;

                WPMCodes2GVal.Text = (release.WpmCode2g == null) ? CONST_EMPTY_FIELD : release.WpmCode2g.ToString();
                WPMCodes3GVal.Text = (release.WpmCode3g == null) ? CONST_EMPTY_FIELD : release.WpmCode3g.ToString();
            }
        }

        /// <summary>
        /// Populate fileds of the view with basic validation rules
        /// </summary>
        private void dataValidationSetUp()
        {
            releaseCodeVal.MaxLength = 10;
            releaseCodeVal.Attributes.Add("data-required", "true");

            ReleaseNameVal.Attributes.Add("data-required", "true");
            ReleaseNameVal.MaxLength = 50;

            ReleaseShortNameVal.Attributes.Add("data-required", "true");
            ReleaseShortNameVal.MaxLength = 20;

            ReleaseDescVal.MaxLength = 200;
            ReleaseDescVal.Columns = 60;

            previousReleaseVal.Attributes.Add("data-required", "true");

            Release2GDecimalVal.Attributes.Add("data-pattern", "^[0-9]+$");
            Release2GDecimalVal.MaxLength = 3;

            Release3GDecimalVal.Attributes.Add("data-pattern", "^[0-9]+$");
            Release3GDecimalVal.MaxLength = 3;

            ITURCodeVal.MaxLength = 20;

            WPMCodes2GVal.MaxLength = 50;
            WPMCodes3GVal.MaxLength = 50;
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());            
            ReleaseId = (Request.QueryString["releaseId"] != null) ? (int.TryParse(Request.QueryString["releaseId"], out output) ? new Nullable<int>(output) : null) : null;
            action = (Request.QueryString["action"] != null) ? Request.QueryString["action"] : string.Empty;
            selectedTab = (Request.QueryString["selectedTab"] != null) ? Convert.ToInt32(Request.QueryString["selectedTab"]) : 0;
        }

        /// <summary>
        /// Retrieve person Id
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

        /// <summary>
        /// Close event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseReleaseDetails_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["releaseId"] != null)
                Response.Redirect("/DesktopModules/Release/ReleaseDetails.aspx?releaseId=" + Request.QueryString["releaseId"]);
            else
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
        }

        /// <summary>
        /// Save event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveEditedRelease_Click(object sender, EventArgs e)
        {
            GetRequestParameters();

            IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
            Domain.Release editedRelease = new Domain.Release();

            // Validation of dates
            string errorList = string.Empty;
            if ((FreezeStage3Meeting.SelectedMeeting != null && ReleaseEndMeeting.SelectedMeeting != null
                && (DateTime.Compare(FreezeStage3Meeting.SelectedMeeting.END_DATE.Value, ReleaseEndMeeting.SelectedMeeting.END_DATE.Value) > 0)))
            {
                errorList += "End date must be greater than Freeze stage 3 date.";
                RadWindowManager1.RadAlert(errorList, 400, 150, "Error", "", "images/error.png");
            }
            else
            {
                if (ReleaseId != null)
                {
                    if (action.Equals(EDIT_MODE))
                    {
                        setReleaseEditionValues(editedRelease);
                        editedRelease.Pk_ReleaseId = ReleaseId.Value;
                        svc.EditRelease(editedRelease, int.Parse(previousReleaseVal.SelectedValue), UserId);
                    }
                }
                else if (action.Equals(CREATION_MODE))
                {
                    setReleaseEditionValues(editedRelease);
                    editedRelease.Pk_ReleaseId = svc.CreateRelease(editedRelease, int.Parse(previousReleaseVal.SelectedValue), UserId);
                }
                else
                {
                    //Bad request
                    releaseDetailsBody.Visible = false;
                    releaseError.Visible = true;
                }
                
                Response.Redirect("ReleaseDetails.aspx?releaseId=" + editedRelease.Pk_ReleaseId + "&fromEdit=true");
            }
        }


        /// <summary>
        /// Populates a Release object with the inserted data 
        /// </summary>
        /// <param name="editedRelease"></param>
        private void setReleaseEditionValues(Domain.Release editedRelease)
        {
            editedRelease.Code = releaseCodeVal.Text;
            editedRelease.Name = ReleaseNameVal.Text;
            editedRelease.Description = (ReleaseDescVal.Text.Equals(string.Empty) || ReleaseDescVal.Text.StartsWith("http://") || ReleaseDescVal.Text.StartsWith("https://")) ? ReleaseDescVal.Text : "http://" + ReleaseDescVal.Text;
            editedRelease.ShortName = ReleaseShortNameVal.Text;
            if (ReleaseStartDateVal.SelectedDate.HasValue)
                editedRelease.StartDate = ReleaseStartDateVal.SelectedDate;
            else
                editedRelease.StartDate = null;
            editedRelease.Stage1FreezeMtgId = FreezeStage1Meeting.SelectedMeetingId;
            editedRelease.Stage2FreezeMtgId = FreezeStage2Meeting.SelectedMeetingId;
            editedRelease.Stage3FreezeMtgId = FreezeStage3Meeting.SelectedMeetingId;
            editedRelease.EndMtgId = ReleaseEndMeeting.SelectedMeetingId;
            editedRelease.ClosureMtgId = ReleaseClosureMeeting.SelectedMeetingId;
            editedRelease.IturCode = ITURCodeVal.Text;
            int intParserBuffer = 0;
            editedRelease.Version2g = (int.TryParse(Release2GDecimalVal.Text, out intParserBuffer)) ? new Nullable<int>(int.Parse(Release2GDecimalVal.Text)) : null;
            editedRelease.Version3g = (int.TryParse(Release3GDecimalVal.Text, out intParserBuffer)) ? new Nullable<int>(int.Parse(Release3GDecimalVal.Text)) : null;
            editedRelease.WpmCode2g = WPMCodes2GVal.Text;
            editedRelease.WpmCode3g = WPMCodes3GVal.Text;
            editedRelease.Remarks = releaseRemarks.DataSource;
        }
    }
}