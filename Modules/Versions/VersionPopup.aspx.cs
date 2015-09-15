using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Module.Versions
{
    public partial class VersionPopup : System.Web.UI.Page
    {
        #region properties and constants

        private const string GetParamVersionId = "versionId";
        private const string GetParamIsEditMode = "isEditMode";
        private const string DsIdKey = "ETSI_DS_ID";
        private const string ViewStateVersion = "VIEWSTATE_VERSION_{0}";
        private const string SpecTitle = "{0} - {1}";

        protected RemarksControl remarksCtrl;
        protected MeetingControl meetingCtrl;

        private int _versionId { get; set; }
        public int VersionId
        {
            get
            {
                if (_versionId == 0)
                    GetRequestParameters();
                return _versionId;
            }
            set { _versionId = value; }
        }

        private bool? _isEditMode { get; set; }
        public bool IsEditMode
        {
            get
            {
                if(_isEditMode == null)
                    GetRequestParameters();
                return _isEditMode.GetValueOrDefault();
            }
            set { _isEditMode = value; }
        }

        /// <summary>
        /// Version
        /// </summary>
        private SpecVersion Version
        {
            get
            {
                if (ViewState[string.Format(ViewStateVersion, ClientID)] == null)
                    ViewState[string.Format(ViewStateVersion, ClientID)] = new SpecVersion();

                return (SpecVersion)ViewState[string.Format(ViewStateVersion, ClientID)];
            }
            set
            {
                ViewState[string.Format(ViewStateVersion, ClientID)] = value;
            }
        }
        #endregion

        #region events

        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Check necessary properties send as get parameters
            if (!GetRequestParameters())
            {
                ThrowError(Localization.GenericError);
                return;
            }

            if (!IsPostBack)
            {
                //Load data
                var rights = LoadData();
                if (rights == null) return;

                //Load remarks inside remarksCtrl
                remarksCtrl.UserRights = rights;
                remarksCtrl.DataSource = Version.Remarks.ToList();

                //Adapt UI according to the current mode and user's rights
                ConfigureUi(remarksCtrl.UserRights);
            }

            //Configure remarks control
            ConfigureRemarksControl(remarksCtrl.UserRights);
        }

        /// <summary>
        /// Save version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            //Apply modification on Version object
            GetEditedVersion();

            var versionSvc = new SpecVersionService();
            var response = versionSvc.UpdateVersion(Version, GetPersonId());
            if (response.Report.GetNumberOfErrors() > 0 || response.Result == null)
            {
                ThrowError(response.Report.ErrorList.First());
            }
            RedirectionBetweenModes(false);
        }

        /// <summary>
        /// Move to edit mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            RedirectionBetweenModes(true);
        }

        /// <summary>
        /// Move to view mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectionBetweenModes(false);
        }

        #endregion

        #region remarks
        /// <summary>
        ///  Configure remarks control
        /// </summary>
        /// <param name="rights"></param>
        private void ConfigureRemarksControl(UserRightsContainer rights)
        {
            remarksCtrl.IsEditMode = IsEditMode;
            remarksCtrl.ScrollHeight = 100;

            if (IsEditMode)
                remarksCtrl.AddRemarkHandler += remarksControl_AddRemarkHandler;
        }

        /// <summary>
        /// Add Remark event handler to add remarks to grid
        /// </summary>
        /// <param name="sender">Remarks Component</param>
        /// <param name="e">Event arguments</param>
        private void remarksControl_AddRemarkHandler(object sender, EventArgs e)
        {
            List<Remark> datasource = remarksCtrl.DataSource;

            var remark = GetNewRemark();
            if (remark != null)
                datasource.Add(remark);

            remarksCtrl.DataSource = datasource;
        }

        /// <summary>
        /// Gets the new remark.
        /// </summary>
        /// <returns>Remark entity</returns>
        private Remark GetNewRemark()
        {
            //Get person name
            var personId = GetPersonId();
            var svc = ServicesFactory.Resolve<IPersonService>();
            var personDisplayName = svc.GetPersonDisplayName(personId);

            //User Rights
            var userRights = remarksCtrl.UserRights;

            //New Remark entry
            var remark = new Remark
            {
                Fk_PersonId = personId,
                IsPublic = userRights != null ? !userRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault) : true,
                CreationDate = DateTime.UtcNow,
                RemarkText = remarksCtrl.RemarkText,
                PersonName = personDisplayName,
                Fk_VersionId = VersionId
            };

            return remark;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Redirection between screens : MODE VIEW OR EDIT
        /// </summary>
        /// <param name="toEditMode"></param>
        private void RedirectionBetweenModes(bool toEditMode)
        {
            var absoluteUrl = Request.Url.AbsolutePath;
            var urlParams = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            urlParams.Set(GetParamIsEditMode, toEditMode.ToString());
            Response.Redirect(string.Format("{0}?{1}", absoluteUrl, urlParams));
        }

        /// <summary>
        /// Get version with new values
        /// </summary>
        private void GetEditedVersion()
        {
            int output;
            if (!int.TryParse(NewVersionMajorVal.Text, out output) ||
                !int.TryParse(NewVersionTechnicalVal.Text, out output) ||
                !int.TryParse(NewVersionEditorialVal.Text, out output) || 
                meetingCtrl.SelectedMeetingId == 0)
            {
                ThrowError(Localization.GenericError);
                return;
            }

            Version.MajorVersion = int.Parse(NewVersionMajorVal.Text);
            Version.TechnicalVersion = int.Parse(NewVersionTechnicalVal.Text);
            Version.EditorialVersion = int.Parse(NewVersionEditorialVal.Text);
            Version.SupressFromSDO_Pub = chckboxSdo.Checked;
            Version.SupressFromMissing_List = chckboxMissing.Checked;
            Version.Source = meetingCtrl.SelectedMeetingId;

            //Remarks
            Version.Remarks = remarksCtrl.DataSource;
        }

        /// <summary>
        /// Load data and return user rights container returned by the query to the service
        /// </summary>
        /// <returns></returns>
        private UserRightsContainer LoadData()
        {
            var versionSvc = new SpecVersionService();
            var response = versionSvc.GetVersionsById(VersionId, GetPersonId());
            //Version should exist
            if (response.Key == null)
            {
                ThrowError(Localization.GenericError);
                return null;
            }
            Version = response.Key;

            //In edit mode user should have right to edit version
            if (IsEditMode && !response.Value.HasRight(Enum_UserRights.Versions_Edit))
            {
                ThrowError(Localization.RightError);
                return null;
            }
            
            //Title
            SpecAndTitle.Text = string.Format(SpecTitle, Version.Specification.Number, Version.Specification.Title);
            //Version
            NewVersionMajorVal.Value = Version.MajorVersion;
            NewVersionTechnicalVal.Value = Version.TechnicalVersion;
            NewVersionEditorialVal.Value = Version.EditorialVersion;
            lkVersion.Text = Version.Version;
            lkVersion.NavigateUrl = Version.Location;
            if (Version.Location == null)
                lkVersion.Enabled = false;
            //Checkboxes
            chckboxSdo.Checked = Version.SupressFromSDO_Pub;
            chckboxMissing.Checked = Version.SupressFromMissing_List;
            //Meeting
            meetingCtrl.SelectedMeetingId = Version.Source ?? 0;
            //Meeting reference
            var mtgSvc = new MeetingService();
            var mtg = mtgSvc.GetMeetingById(Version.Source ?? 0);
            if (mtg != null)
            {
                lblMeeting.Text = mtg.MtgShortRef;
            }

            return response.Value;
        }

        /// <summary>
        /// Gets the request parameters.
        /// By default view mode
        /// if Version id not provided return false
        /// </summary>
        private bool GetRequestParameters()
        {
            if (Request.QueryString[GetParamVersionId] != null)
            {
                int tempVersionId;
                if (int.TryParse(Request.QueryString[GetParamVersionId], out tempVersionId))
                    VersionId = tempVersionId;
                else
                    return false;
            }
            else
                return false;

            if (Request.QueryString[GetParamIsEditMode] != null)
            {
                bool tempIsEditMode;
                if (bool.TryParse(Request.QueryString[GetParamIsEditMode], out tempIsEditMode))
                    IsEditMode = tempIsEditMode;
                else
                    IsEditMode = false;
            }
            else
                IsEditMode = false;
            return true;
        }

        /// <summary>
        ///  Adapt UI according to the current mode and user's rights
        /// </summary>
        /// <param name="rights"></param>
        private void ConfigureUi(UserRightsContainer rights)
        {
            if (IsEditMode)//Edit mode
            {
                //Buttons
                btnSave.Visible = true;
                btnCancel.Visible = true;
                btnEdit.Visible = false;
                btnClose.Visible = false;
                //Version
                versionInEditMode.Visible = true;
                versionInViewMode.Visible = false;
                NewVersionMajorVal.Enabled = false;
                NewVersionTechnicalVal.Enabled = false;
                NewVersionEditorialVal.Enabled = false;
                //Checkboxes
                chckboxSdo.Enabled = true;
                chckboxMissing.Enabled = true;
                //Meeting
                meetingCtrl.Visible = true;
                lblMeeting.Visible = false;
            }
            else//View mode
            {
                //Buttons
                btnSave.Visible = false;
                btnCancel.Visible = false;
                btnEdit.Visible = rights.HasRight(Enum_UserRights.Versions_Edit);
                btnClose.Visible = true;
                //Version
                versionInEditMode.Visible = false;
                versionInViewMode.Visible = true;
                NewVersionMajorVal.Enabled = false;
                NewVersionTechnicalVal.Enabled = false;
                NewVersionEditorialVal.Enabled = false;
                //Checkboxes
                chckboxSdo.Enabled = false;
                chckboxMissing.Enabled = false;
                //Meeting
                meetingCtrl.Visible = false;
                lblMeeting.Visible = true;
            }
        }

        /// <summary>
        /// Gets the person identifier.
        /// </summary>
        /// <returns></returns>
        private int GetPersonId()
        {
            var userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
            if (userInfo.UserID < 0)
                return 0;

            int personId;
            if (int.TryParse(userInfo.Profile.GetPropertyValue(DsIdKey), out personId))
                return personId;
            return 0;
        }

        /// <summary>
        /// Throw error : hide the form and display the error message
        /// </summary>
        /// <param name="message">Error message</param>
        private void ThrowError(string message)
        {
            pnlMessage.Visible = true;
            divVersionPopup.Visible = false;
            pnlMessage.CssClass = "messageBox error";
            lblMessage.Text = message;
        }

        #endregion
    }
}