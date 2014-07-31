using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.Zip;
using Etsi.Ultimate.Module.Versions.App_LocalResources;

namespace Etsi.Ultimate.Module.Versions
{
    public partial class UploadVersion : System.Web.UI.Page
    {
        #region Variables / Properties
        protected MeetingControl UploadMeeting;

        private const string CONST_WARNING_REPORT = "CONST_WARNING_REPORT";
        private const string CONST_USER_ID_VIEWSTATE_LABEL = "CONST_USER_ID_VIEWSTATE_LABEL";
        private const string CONST_RELEASE_ID_VIEWSTATE_LABEL = "CONST_RELEASE_ID_VIEWSTATE_LABEL";
        private const string CONST_SPEC_ID_VIEWSTATE_LABEL = "CONST_SPEC_ID_VIEWSTATE_LABEL";
        private const string CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL = "CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL";
        private const string CONST_IS_DRAFT_VIEWSTATE_LABEL = "CONST_IS_DRAFT_VIEWSTATE_LABEL";
        private const string CONST_VERSION_PATH_VIEWSTATE_LABEL = "CONST_VERSION_PATH_VIEWSTATE_LABEL";

        private int errorNumber = 0;

        private Report report
        {
            get
            {
                return ViewState[ClientID + CONST_WARNING_REPORT] != null ? (Report)ViewState[ClientID + CONST_WARNING_REPORT] : null;
            }
            set
            {
                ViewState[ClientID + CONST_WARNING_REPORT] = value;
            }
        }
        public int ReleaseId
        {
            get
            {
                if (ViewState[ClientID + CONST_RELEASE_ID_VIEWSTATE_LABEL] == null)
                    ViewState[ClientID + CONST_RELEASE_ID_VIEWSTATE_LABEL] = 0;

                return (int)ViewState[ClientID + CONST_RELEASE_ID_VIEWSTATE_LABEL];
            }
            set
            {
                ViewState[ClientID + CONST_RELEASE_ID_VIEWSTATE_LABEL] = value;
            }
        }
        public string VersionFileToken
        {
            get
            {
                if (ViewState[ClientID + CONST_VERSION_PATH_VIEWSTATE_LABEL] == null)
                    ViewState[ClientID + CONST_VERSION_PATH_VIEWSTATE_LABEL] = "";

                return (string)ViewState[ClientID + CONST_VERSION_PATH_VIEWSTATE_LABEL];
            }
            set
            {
                ViewState[ClientID + CONST_VERSION_PATH_VIEWSTATE_LABEL] = value;
            }
        }
        public int SpecId
        {
            get
            {
                if (ViewState[ClientID + CONST_SPEC_ID_VIEWSTATE_LABEL] == null)
                    ViewState[ClientID + CONST_SPEC_ID_VIEWSTATE_LABEL] = 0;

                return (int)ViewState[ClientID + CONST_SPEC_ID_VIEWSTATE_LABEL];
            }
            set
            {
                ViewState[ClientID + CONST_SPEC_ID_VIEWSTATE_LABEL] = value;
            }
        }
        public bool IsUploadMode
        {
            get
            {
                if (ViewState[ClientID + CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL] == null)
                    ViewState[ClientID + CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL] = false;

                return (bool)ViewState[ClientID + CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL];
            }
            set
            {
                ViewState[ClientID + CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL] = value;
            }
        }
        public bool IsDraft
        {
            get
            {
                if (ViewState[ClientID + CONST_IS_DRAFT_VIEWSTATE_LABEL] == null)
                    ViewState[ClientID + CONST_IS_DRAFT_VIEWSTATE_LABEL] = null;

                return (bool)ViewState[ClientID + CONST_IS_DRAFT_VIEWSTATE_LABEL];
            }
            set
            {
                ViewState[ClientID + CONST_IS_DRAFT_VIEWSTATE_LABEL] = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Upload Version - Constructor
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRequestParameters();//Retreive the URL parameters
                ResetPanelState();//Reset version upload/allocate popup and its attributes
                LoadVersionUploadContent();
            }
        }
        /// <summary>
        /// Click event of cancel button
        /// </summary>
        /// <param name="sender">Cancel button</param>
        /// <param name="e">event arguments</param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            ResetPanelState();
        }
        /// <summary>
        /// Item DataBound event for Error/Warning report
        /// </summary>
        /// <param name="Sender">Repeater control</param>
        /// <param name="e">Repeater item event arguments</param>
        protected void rptWarningsErrors_ItemDataBound(Object Sender, RepeaterItemEventArgs e) 
        {
            string item = (String)e.Item.DataItem;
            if (item != null)
            {
                Label lbl = e.Item.FindControl("lblErrorOrWarning") as Label;
                lbl.Text = item;
                if (errorNumber > 0)
                {
                    lbl.CssClass = "ErrorItem";
                    errorNumber--;
                }
                else
                    lbl.CssClass = "WarningItem";
            }
        }

        //Allocate events
        /// <summary>
        /// Click event of Allocation button
        /// </summary>
        /// <param name="sender">Allocation button</param>
        /// <param name="e">event arguments</param>
        protected void AllocateVersionBtn_Click(object sender, EventArgs e)
        {
            var report = new Report();
            ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
            var version = GetEditedSpecVersionObject();
            if (version.Key)
                report = specVersionSvc.AllocateVersion(GetUserPersonId(), version.Value);

            if (report.GetNumberOfErrors() == 0 && report.GetNumberOfWarnings() == 0)
            {
                lblSaveStatus.Text = String.Format(UploadVersion_aspx.SuccessMessage, version.Value.MajorVersion, version.Value.TechnicalVersion, version.Value.EditorialVersion, "allocated.");
                preVersionUploadScreen.Visible = false;
                analysis.Visible = false;
                confirmation.Visible = false;
                state.Visible = true;
                state_confirmation.OnClientClicked = "closeRadWindow";
            }
            else
            {
                ThrowAnError(UploadVersion_aspx.GenericErrorAllocation);
            }
        }
        //Upload events
        /// <summary>
        /// Click event for upload button
        /// </summary>
        /// <param name="sender">Upload button</param>
        /// <param name="e">event arguments</param>
        protected void UploadVersionBtn_Click(object sender, EventArgs e)
        {
            var meetingId = UploadMeeting.SelectedMeetingId;
            if (meetingId > 0 || IsDraft)
            {
                preVersionUploadScreen.Visible = false;
                confirmation.Visible = true;
            }
        }
        /// <summary>
        /// Confirm upload of the version
        /// </summary>
        /// <param name="sender">Confirmation upload button</param>
        /// <param name="e">event arguments</param>
        protected void Confirmation_Upload_OnClick(object sender, EventArgs e)
        {
            var svcResponse = new ServiceResponse<string>();
            ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
            var version = GetEditedSpecVersionObject();
            if (version.Key)
                svcResponse = specVersionSvc.UploadVersion(GetUserPersonId(), version.Value, VersionFileToken);

            if (svcResponse.Report.GetNumberOfErrors() == 0 && svcResponse.Report.GetNumberOfWarnings() == 0)
            {
                lblSaveStatus.Text = String.Format(UploadVersion_aspx.SuccessMessage, version.Value.MajorVersion, version.Value.TechnicalVersion, version.Value.EditorialVersion, "uploaded.");
                preVersionUploadScreen.Visible = false;
                analysis.Visible = false;
                confirmation.Visible = false;
                state.Visible = true;
                state_confirmation.OnClientClicked = "closeRadWindow";
            }
            else
            {
                ThrowAnError(UploadVersion_aspx.GenericErrorUpload);
            }
        }
        /// <summary>
        /// Method call by ajax when workplan is uploaded => WorkPlan Analyse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AsyncUpload_VersionUpload(object sender, FileUploadedEventArgs e)
        {
            var svcResponse = new ServiceResponse<string>();
            ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
            var uploadVersionTemporaryPath = Utils.ConfigVariables.UploadVersionTemporaryPath;
            if (!Directory.Exists(uploadVersionTemporaryPath))
            {
                Directory.CreateDirectory(uploadVersionTemporaryPath);
            }

            //Get the version file
            UploadedFile versionUploaded = e.File;
            var path = new StringBuilder()
                    .Append(uploadVersionTemporaryPath)
                    .Append(versionUploaded.FileName)
                    .ToString();
            try
            {
                versionUploaded.SaveAs(path);
            }
            catch (Exception exc)
            {
                LogManager.Error("Could not save work plan file: " + exc.Message);
            }

            var version = GetEditedSpecVersionObject();
            if (version.Key)
                svcResponse = specVersionSvc.CheckVersionForUpload(GetUserPersonId(), version.Value, path);

            //Get version file token to use it when user will press "confirm upload" button
            VersionFileToken = svcResponse.Result;

            //Confirmation popup
            DisplayWarningAndErrorInConfirmationPopUp(svcResponse.Report);
            errorNumber = svcResponse.Report.GetNumberOfErrors();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Reset version upload/allocate popup and its attributes
        /// </summary>
        private void ResetPanelState()
        {
            //UI elements
            preVersionUploadScreen.Visible = true;
            analysis.Visible = false;
            confirmation.Visible = false;
            state.Visible = false;
            if (!IsUploadMode)
            {
                FileToUploadLbl.Visible = false;
                FileToUploadVal.Visible = false;
                UploadBtnDisabled.Visible = false;
                AllocateBtn.Visible = true;
                btnConfirmUpload.Visible = false;
            }
        }
        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            ReleaseId = (Request.QueryString["releaseId"] != null) ? Convert.ToInt32(Request.QueryString["releaseId"]) : 0;
            SpecId = (Request.QueryString["specId"] != null) ? Convert.ToInt32(Request.QueryString["specId"]) : 0;
            if (ReleaseId == 0 || SpecId == 0)
                ThrowAnError(UploadVersion_aspx.NoAvailableDatas);

            var action = (Request.QueryString["action"] != null) ? Request.QueryString["action"] : string.Empty;
            if (action.Equals("upload") || action.Equals("allocate"))
                IsUploadMode = action.Equals("upload");
            else
                ThrowAnError(UploadVersion_aspx.GenericError);
        }
        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="UserInfo">Current user information</param>
        /// <returns></returns>
        private int GetUserPersonId()
        {
            var userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            if (userInfo.UserID < 0)
                return 0;
            else
            {
                int personID;
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(UploadVersion_aspx.DsId_Key), out personID))
                    return personID;
            }
            return 0;
        }
        /// <summary>
        /// Method to get the version edited by the user through the popup
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<bool, SpecVersion> GetEditedSpecVersionObject()
        {
            //User's data
            int output;
            if (!int.TryParse(NewVersionMajorVal.Text, out output) || !int.TryParse(NewVersionTechnicalVal.Text, out output) || !int.TryParse(NewVersionEditorialVal.Text, out output))
                return new KeyValuePair<bool, SpecVersion>(false, null);
            else
            {
                SpecVersion version = new SpecVersion();
                version.Fk_SpecificationId = SpecId;
                version.Fk_ReleaseId = ReleaseId;
                version.MajorVersion = int.Parse(NewVersionMajorVal.Text);
                version.TechnicalVersion = int.Parse(NewVersionTechnicalVal.Text);
                version.EditorialVersion = int.Parse(NewVersionEditorialVal.Text);

                if (!String.IsNullOrEmpty(CommentVal.Text))
                {
                    version.Remarks.Add(new Remark()
                    {
                        RemarkText = CommentVal.Text,
                        CreationDate = new Nullable<System.DateTime>(DateTime.UtcNow),
                        Fk_PersonId = GetUserPersonId(),
                        IsPublic = true
                    });
                }

                if (UploadMeeting.SelectedMeeting != null)
                {
                    version.Source = UploadMeeting.SelectedMeeting.MTG_ID;
                }
                if (IsUploadMode)
                {
                    version.DocumentUploaded = DateTime.UtcNow;
                    version.ProvidedBy = GetUserPersonId();
                }
                return new KeyValuePair<bool, SpecVersion>(true, version);
            }
        }
        /// <summary>
        /// Display an error
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ThrowAnError(string errorMessage)
        {
            versionUploadBody.Visible = false;
            versionUploadMessages.Visible = true;
            versionUploadMessages.CssClass = "messageBox error";
            specificationMessagesTxt.Text = errorMessage;
        }
        /// <summary>
        /// Display warnings and errors in the confirmation popup
        /// </summary>
        /// <param name="report"></param>
        private void DisplayWarningAndErrorInConfirmationPopUp(Report report)
        {
            preVersionUploadScreen.Visible = false;
            analysis.Visible = false;
            confirmation.Visible = true;
            state.Visible = false;

            if (report.GetNumberOfErrors() > 0)
                btnConfirmUpload.Enabled = false;
            else
                btnConfirmUpload.Enabled = true;

            lblCountWarningErrors.Text = new StringBuilder()
                .Append("Found ")
                .Append(report.GetNumberOfErrors().ToString())
                .Append(" error")
                .Append(report.GetNumberOfErrors() <= 1 ? "" : "s")
                .Append(", ")
                .Append(report.GetNumberOfWarnings().ToString())
                .Append(" warning")
                .Append(report.GetNumberOfWarnings() <= 1 ? "" : "s")
                .Append(".")
                .ToString();

            List<string> datasource = new List<string>();
            datasource.AddRange(report.ErrorList);
            datasource.AddRange(report.WarningList);
            rptWarningsErrors.DataSource = datasource;
            rptWarningsErrors.DataBind();
        }
        /// <summary>
        /// Load page content
        /// </summary>
        private void LoadVersionUploadContent()
        {
            if ((ReleaseId != 0) && (SpecId != 0))
            {
                ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                var svcResponseSpecVersion = specVersionSvc.GetNextVersionForSpec(GetUserPersonId(), SpecId, ReleaseId, IsUploadMode);

                if (svcResponseSpecVersion.Report.GetNumberOfErrors() == 0)
                {
                    var version = svcResponseSpecVersion.Result;
                    SpecAndReleaseAttributesHandler(version.NewSpecVersion);

                    //Set Major Version Status
                    NewVersionMajorVal.Enabled = svcResponseSpecVersion.Rights.HasRight(Enum_UserRights.Versions_Modify_MajorVersion);

                    string currentVersionNumber = "-";
                    if (svcResponseSpecVersion.Result.CurrentSpecVersion != null && svcResponseSpecVersion.Result.CurrentSpecVersion.MajorVersion != null && svcResponseSpecVersion.Result.CurrentSpecVersion.TechnicalVersion != null && svcResponseSpecVersion.Result.CurrentSpecVersion.EditorialVersion != null)
                        currentVersionNumber = String.Format("{0}.{1}.{2}", svcResponseSpecVersion.Result.CurrentSpecVersion.MajorVersion, svcResponseSpecVersion.Result.CurrentSpecVersion.TechnicalVersion, svcResponseSpecVersion.Result.CurrentSpecVersion.EditorialVersion);
                    CurrentVersionVal.Text = currentVersionNumber;

                    //Set version number values
                    NewVersionMajorVal.Value = version.NewSpecVersion.MajorVersion;
                    NewVersionTechnicalVal.Value = version.NewSpecVersion.TechnicalVersion;
                    NewVersionEditorialVal.Value = version.NewSpecVersion.EditorialVersion;

                    if (version.NewSpecVersion.Source != null)
                        UploadMeeting.SelectedMeetingId = version.NewSpecVersion.Source.GetValueOrDefault();
                }
                else
                {
                    ThrowAnError(UploadVersion_aspx.GenericError);
                }
            }
            else
            {
                ThrowAnError(UploadVersion_aspx.NoAvailableDatas);
            }
        }
        /// <summary>
        /// Spec and release attributes handler
        /// </summary>
        private void SpecAndReleaseAttributesHandler(SpecVersion version)
        {
            var spec = version.Specification;
            IsDraft = !(spec.IsUnderChangeControl.HasValue && spec.IsUnderChangeControl.Value && spec.IsActive);
            hidIsRequired.Value = (!IsDraft && IsUploadMode) ? "True" : "False";
            MeetingLbl.Text = (!IsDraft && IsUploadMode) ? "Meeting(<span class='requiredField'>*</span>):" : "Meeting:";
            SpecNumberVal.Text = spec.Number;

            var release = version.Release;
            ReleaseVal.Text = release.Code;
        }
        #endregion
    }
}