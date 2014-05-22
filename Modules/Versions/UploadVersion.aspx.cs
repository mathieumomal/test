using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using Domain = Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Module.Versions
{
    public partial class UploadVersion : System.Web.UI.Page
    {
        // Custom controls
        protected MeetingControl UploadMeeting;

        //Static fields
        public static readonly string DsId_Key = "ETSI_DS_ID";

        //Properties
        private int UserId;
        public Nullable<int> versionId;
        public string action;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                GetRequestParameters();
                LoadVersionUploadContent();
            }
        }

        /// <summary>
        /// Load page content
        /// </summary>
        private void LoadVersionUploadContent()
        {
            if (versionId != null)
            {
                // Retrieve data
                ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                KeyValuePair<DomainClasses.SpecVersion, DomainClasses.UserRightsContainer> specVersionRightsObject = svc.GetVersionsById(versionId.Value, UserId);
                Domain.SpecVersion specVerion = specVersionRightsObject.Key;
                DomainClasses.UserRightsContainer userRights = specVersionRightsObject.Value;
                if (specVerion == null)
                {
                    versionUploadBody.Visible = false;
                    versionUploadMessages.Visible = true;
                    versionUploadMessages.CssClass = "Warning";
                    specificationMessagesTxt.CssClass = "WarningTxt";
                    specificationMessagesTxt.Text = "No avaible data for the requested query";
                }
                else
                {
                    // User does not have rights to perform the action
                    if (((action.Equals("upload")) && (userRights.HasRight(Domain.Enum_UserRights.Versions_Upload))) || ((action.Equals("allocate")) && (userRights.HasRight(Domain.Enum_UserRights.Versions_Allocate))))
                    {
                        LoadVersionDetails(specVerion);
                        ManageNewVersion(specVerion);

                    }
                    else
                    {
                        versionUploadBody.Visible = false;
                        versionUploadMessages.Visible = true;
                        versionUploadMessages.CssClass = "Error";
                        specificationMessagesTxt.CssClass = "ErrorTxt";
                        specificationMessagesTxt.Text = "You dont have the right to perform this action";
                    }
                }
            }
            else
            {
                versionUploadBody.Visible = false;
                versionUploadMessages.Visible = true;
                versionUploadMessages.CssClass = "Warning";
                specificationMessagesTxt.CssClass = "WarningTxt";
                specificationMessagesTxt.Text = "No avaible data for the requested query";
            }
        }
        /// <summary>
        /// Loads the Version's details
        /// </summary>
        private void LoadVersionDetails(Domain.SpecVersion version)
        {
            SpecNumberVal.Text = version.Specification.Number;
            ReleaseVal.Text = version.Release.Code;
            CurrentVersionVal.Text = SpecNumberVal.Text + "-" + version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion;
        }

        /// <summary>
        /// Manage the new version to upload
        /// </summary>
        private void ManageNewVersion(Domain.SpecVersion version)
        {
            // Version was not uploaded => force user to upload it
            if (version.DocumentUploaded == null)
            {
                NewVersionMajorVal.Text = version.MajorVersion.ToString();
                NewVersionMajorVal.Enabled = false;
                NewVersionTechnicalVal.Text = version.TechnicalVersion.ToString();
                NewVersionTechnicalVal.Enabled = false;
                NewVersionEditorialVal.Text = version.EditorialVersion.ToString();
                NewVersionEditorialVal.Enabled = false;
            }
            // Propose the version number
            else
            {
                NewVersionMajorVal.Text = version.MajorVersion.ToString();
                NewVersionTechnicalVal.Text = (version.TechnicalVersion+1).ToString();
                NewVersionEditorialVal.Text = "0";
            }            
        }

        

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());

            versionId = (Request.QueryString["versionId"] != null) ? (int.TryParse(Request.QueryString["versionId"], out output) ? new Nullable<int>(output) : null) : null;
            action = (Request.QueryString["action"] != null) ? Request.QueryString["action"] : string.Empty;
        }

        /// <summary>
        /// Retrieve person If exists
        /// </summary>
        /// <param name="UserInfo">Current user information</param>
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
    }
}