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
            if (IsPostBack)
            {
                int Release2GDecimalValue, Release3GDecimalValue;

                if (int.TryParse(Release2GDecimalVal.Text, out Release2GDecimalValue))
                {
                    Release2GVal.Text = Domain.Release.Encode(Release2GDecimalValue);
                    Release2GDecimalVal.Attributes.Remove("class");
                    checkForValidation();
                }
                else
                {
                    Release2GDecimalVal.Attributes.Add("class", "error");
                    SaveBtn.Attributes.Add("disabled", "disabled");
                    SaveBtn.Attributes.Remove("class");
                    SaveBtn.Attributes.Add("class", "disabledLink");    
                }

                if (int.TryParse(Release3GDecimalVal.Text, out Release3GDecimalValue))
                {
                    Release3GVal.Text = Domain.Release.Encode(Release3GDecimalValue);
                    Release3GDecimalVal.Attributes.Remove("class");
                    checkForValidation();
                }
                else
                {
                    Release3GDecimalVal.Attributes.Add("class", "error");
                    SaveBtn.Attributes.Add("disabled", "disabled");
                    SaveBtn.Attributes.Remove("class");
                    SaveBtn.Attributes.Add("class", "disabledLink");                    
                }
                
                
            }
            releaseRemarks.AddRemarkHandler += releaseRemarks_AddRemarkHandler;
        }

        private void checkForValidation()
        {
            if (!ReleaseNameVal.CssClass.Contains("error") && !ReleaseShortNameVal.CssClass.Contains("error")
                && !previousReleaseVal.CssClass.Contains("error") && !releaseCodeVal.CssClass.Contains("error")
                && !Release2GDecimalVal.CssClass.Contains("error") && !Release3GDecimalVal.CssClass.Contains("error"))
            {
                SaveBtn.Attributes.Remove("disabled");
                SaveBtn.Attributes.Remove("class");
                SaveBtn.Attributes.Add("class", "LinkButton");   
            }            
        }

        private void LoadReleaseDetails()
        {

            dataValidationSetUp();

            if (action.Equals("Edit"))
            {
                if (ReleaseId != null)
                {

                    IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                    KeyValuePair<DomainClasses.Release, DomainClasses.UserRightsContainer> releaseRightsObject = svc.GetReleaseById(UserId, ReleaseId.Value);
                    Domain.Release release = releaseRightsObject.Key;
                    DomainClasses.UserRightsContainer userRights = releaseRightsObject.Value;

                    if (!userRights.HasRight(Domain.Enum_UserRights.Release_Edit))
                    {
                        releaseDetailsBody.Visible = false;
                        releaseError.Visible = true;
                        ErrorMsg.Text = "Sorry but you dont have the right to edit a release.";
                    }
                    else{

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
                            FillAdminTab(release, svc.GetAllReleasesCodes(UserId, ReleaseId.Value), svc.GetPreviousReleaseCode(UserId, release.Pk_ReleaseId).Key);


                            //Set Remarks control
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
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();                
                DomainClasses.UserRightsContainer userRights = svc.GetAllReleases(UserId).Value;

                BuildTabsDisplay(action);
                ReleaseDetailRadMultiPage.Height = new System.Web.UI.WebControls.Unit(750, UnitType.Pixel);
                ReleaseStatusVal.CssClass = "status " + ReleaseStatusVal.Text;
                ReleaseStartDateVal.SelectedDate = DateTime.Now;                

                releaseRemarks.UserRights = userRights;
                releaseRemarks.DataSource = null;

                previousReleaseVal.DataTextField = "Value";
                previousReleaseVal.DataValueField = "Key";
                previousReleaseVal.DataSource = svc.GetAllReleasesCodes(UserId, default(int));
                previousReleaseVal.DataBind();

                SaveBtn.Attributes.Add("disabled", "disabled");
                SaveBtn.CssClass = "disabledLink";
            }
            else {
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
            datasource.Add(new Domain.Remark()
            {
                Fk_PersonId = UserId,
                Fk_ReleaseId = ReleaseId,
                IsPublic = true,
                CreationDate = DateTime.UtcNow,
                RemarkText = releaseRemarks.RemarkText
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
            
            

            MeetingControl stage1Meeting = FreezeStage1Meeting as MeetingControl;
            stage1Meeting.SelectedMeetingId = (release.EndMtgId != null) ? release.Stage1FreezeMtgId.Value : default(int);
            stage1Meeting.DisplayLabel = true;
            stage1Meeting.CssClass = "meetingControl";

            MeetingControl stage2Meeting = FreezeStage2Meeting as MeetingControl;
            stage2Meeting.SelectedMeetingId = (release.EndMtgId != null) ? release.Stage2FreezeMtgId.Value : default(int);
            stage2Meeting.DisplayLabel = true;
            stage2Meeting.CssClass = "meetingControl";

            MeetingControl stage3Meeting = FreezeStage3Meeting as MeetingControl;
            stage3Meeting.SelectedMeetingId = (release.EndMtgId != null) ? release.Stage3FreezeMtgId.Value : default(int);
            stage3Meeting.DisplayLabel = true;
            stage3Meeting.CssClass = "meetingControl";
                                                   
            MeetingControl endMeeting = ReleaseEndMeeting as MeetingControl;
            endMeeting.SelectedMeetingId = (release.EndMtgId != null) ? release.EndMtgId.Value : default(int);
            endMeeting.DisplayLabel = true;
            endMeeting.CssClass = "meetingControl";

            
            MeetingControl closureMeeting = ReleaseClosureMeeting as MeetingControl;
            closureMeeting.SelectedMeetingId = (release.ClosureMtgId != null) ? release.ClosureMtgId.Value : default(int);
            closureMeeting.DisplayLabel = true;
            closureMeeting.CssClass = "meetingControl";

            
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

            if (action.Equals("Edit"))
            {
                previousReleaseVal.SelectedValue = previousReleaseID.ToString();

                ITURCodeVal.Text = (release.IturCode == null) ? CONST_EMPTY_FIELD : release.IturCode;

                Release2GDecimalVal.Text = (release.Version2g == null) ? CONST_EMPTY_FIELD : release.Version2g.ToString();
                Release2GDecimalVal.Attributes.Add("onchange", string.Format("realPostBack(\"{0}\", \"\"); return false;",
                                                        Release2GDecimalVal.UniqueID));
                Release3GDecimalVal.Text = (release.Version3g == null) ? CONST_EMPTY_FIELD : release.Version3g.ToString();
                Release3GDecimalVal.Attributes.Add("onchange", string.Format("realPostBack(\"{0}\", \"\"); return false;",
                                                        Release3GDecimalVal.UniqueID));

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
                Domain.Release editedRelease = new Domain.Release();

                if(action.Equals("Edit")){
                    editedRelease  = svc.GetReleaseById(UserId, ReleaseId.Value).Key;
                    setReleaseEditionValues(ref editedRelease);
                    svc.EditRelease(editedRelease,int.Parse(previousReleaseVal.SelectedValue), UserId);
                }
                else if(action.Equals("Creation")){
                    setReleaseEditionValues(ref editedRelease);
                    svc.CreateRelease(editedRelease,int.Parse(previousReleaseVal.SelectedValue), UserId);
                }
                else{
                    //Bad request
                    releaseDetailsBody.Visible = false;
                    releaseError.Visible = true;
                }                                
                Response.Redirect("ReleaseDetails.aspx?releaseId=" + ReleaseId.Value);
            }
        }



        private void setReleaseEditionValues(ref Domain.Release editedRelease)
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

            //editedRelease.Remarks = releaseRemarks.DataSource;
        }
    }
}