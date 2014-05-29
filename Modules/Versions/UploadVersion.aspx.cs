using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Etsi.Ultimate.Controls;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Telerik.Web.UI;
using System.Text;
using Etsi.Ultimate.DomainClasses;


namespace Etsi.Ultimate.Module.Versions
{
    public partial class UploadVersion : System.Web.UI.Page
    {
        // Custom controls
        protected MeetingControl UploadMeeting;

        //Static fields
        public static readonly string DsId_Key = "ETSI_DS_ID";

        //Properties
        private static int UserId;
        public static Nullable<int> versionId;
        public static string action;
        public static string versionUploadPath;
        public static string versionFTP_Path;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                GetRequestParameters();
                ManageAllocationCase();
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
                KeyValuePair<SpecVersion, UserRightsContainer> specVersionRightsObject = svc.GetVersionsById(versionId.Value, UserId);
                SpecVersion specVerion = specVersionRightsObject.Key;
                UserRightsContainer userRights = specVersionRightsObject.Value;

                //To be removed
                userRights.AddRight(Enum_UserRights.Versions_Upload);
                userRights.AddRight(Enum_UserRights.Versions_Allocate);

                if (specVerion == null || (!action.Equals("upload") && !action.Equals("allocate")))
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
                    if (((action.Equals("upload")) && (userRights.HasRight(Enum_UserRights.Versions_Upload))) || ((action.Equals("allocate")) && (userRights.HasRight(Enum_UserRights.Versions_Allocate))))
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
        private void LoadVersionDetails(SpecVersion version)
        {
            SpecNumberVal.Text = version.Specification.Number;
            ReleaseVal.Text = version.Release.Code;
            CurrentVersionVal.Text = SpecNumberVal.Text + "-" + version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion;
        }

        /// <summary>
        /// Manage the new version to upload
        /// </summary>
        private void ManageNewVersion(SpecVersion version)
        {
            // Version was not uploaded => force user to upload it
            if (version.DocumentUploaded == null)
            {
                NewVersionMajorVal.Text = version.MajorVersion.ToString();                
                NewVersionTechnicalVal.Text = version.TechnicalVersion.ToString();                
                NewVersionEditorialVal.Text = version.EditorialVersion.ToString();                
            }
            // Propose the version number
            else
            {
                NewVersionMajorVal.Text = version.MajorVersion.ToString();
                NewVersionTechnicalVal.Text = (version.TechnicalVersion+1).ToString();
                NewVersionEditorialVal.Text = "0";
            }
            //If not MCC representive => NewVersionMajorVal.Disabled = true;
        }

        /// <summary>
        /// Display or not file upload input
        /// Display or not allocation btn
        /// </summary>
        private void ManageAllocationCase()
        {
            if(action.Equals("allocate"))
            {
                FileToUploadLbl.Visible = false;
                FileToUploadVal.Visible = false;
                UploadBtnDisabled.Visible = false;
                AllocateBtn.Visible = true;
            }
        }

        /// <summary>
        /// Prepare the specVersion object to add in DB
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<bool ,SpecVersion> fillSpecVersionObject()
        {            

            //User's data
            int[] verionsOutputs = new int[] { 0, 0, 0 };
            if(!int.TryParse(NewVersionMajorVal.Text, out verionsOutputs[0]) || !int.TryParse(NewVersionTechnicalVal.Text, out verionsOutputs[1]) || !int.TryParse(NewVersionEditorialVal.Text, out verionsOutputs[2]))
            {
                return new KeyValuePair<bool ,SpecVersion>(false, null);
            }
            else
            {
                ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                KeyValuePair<SpecVersion, UserRightsContainer> specVersionRightsObject = svc.GetVersionsById(versionId.Value, UserId);
                SpecVersion specVerion = specVersionRightsObject.Key;
                if (specVerion == null)
                {
                    return new KeyValuePair<bool, SpecVersion>(false, null);
                }
                SpecVersion version = new SpecVersion();
                //Inherited data
                version.Fk_ReleaseId = specVerion.Fk_ReleaseId;
                version.Fk_SpecificationId = specVerion.Fk_SpecificationId;

                version.MajorVersion = int.Parse(NewVersionMajorVal.Text);
                version.TechnicalVersion = int.Parse(NewVersionTechnicalVal.Text);
                version.EditorialVersion = int.Parse(NewVersionEditorialVal.Text);
                
                version.Remarks.Add(new Remark()
                {
                    RemarkText = CommentVal.Text,
                    CreationDate = new Nullable<System.DateTime>(DateTime.Now),
                    Fk_PersonId = UserId
                });

                if (UploadMeeting.SelectedMeeting != null){
                    version.Source = UploadMeeting.SelectedMeeting.MTG_ID;
                }
                if (action.Equals("upload"))
                {
                    version.DocumentUploaded = DateTime.Now;
                    version.ProvidedBy = UserId;
                }

                return new KeyValuePair<bool ,SpecVersion>(true, version);
                
            }                        
        }

        /// <summary>
        /// Return true if the query's version identifier is a draft's one
        /// </summary>
        /// <returns></returns>
        private void IsDraft(SpecVersion version)
        {
            if (!version.Specification.IsUnderChangeControl.GetValueOrDefault())
                isDraft.Value = "1";
            else
                isDraft.Value = "0";
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

        /// <summary>
        /// Enable the upload action
        /// </summary>
        /// <param name="enableUpload"></param>
        private void EnableUploadButton(bool enableUpload)
        {
            if(enableUpload)
            {
                UploadBtn.Visible = true;
                UploadBtnDisabled.Visible = false;
            }
        }
        /// <summary>
        /// Method call by ajax when workplan is uploaded => WorkPlan Analyse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AsyncUpload_VersionUpload(object sender, FileUploadedEventArgs e)
        {
            //Get version
            UploadedFile versionUploaded = e.File;
            try
            {
                //Save the file in TMP folder
                /*versionUploaded.SaveAs(new StringBuilder()
                    .Append(versionUploadPath)
                    .Append(versionUploaded.FileName)
                    .ToString());  */

                // Call checker
                lblCountWarningErrors.Text = "0";
                btnConfirmUpload.Enabled = true;

            }
            catch (Exception exc)
            {
                LogManager.Error("Could not save the version file: " + exc.Message);
            }            
        }
       

        
        protected void AllocateVersion_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            KeyValuePair<bool ,SpecVersion> buffer = fillSpecVersionObject();
            bool operationSucceded = false;
            if (buffer.Key)
            {
                ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                operationSucceded = svc.AllocateVersion(buffer.Value, versionId.GetValueOrDefault());                                        
            }

            //End of process => redirection
            if (operationSucceded)
            {                
                Page.ClientScript.RegisterStartupScript(this.GetType(), "show uplod state", "ShowAllocationResult(\"success\");", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "show uplod state", "ShowAllocationResult(\"failure\");", true);
            }
        }

        /// <summary>
        /// Upload the version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmation_Upload_OnClick(object sender, EventArgs e)
        {
            ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
            //Tranfer FTP
            //If succeded
            KeyValuePair<bool, SpecVersion> buffer = fillSpecVersionObject();
            bool operationSucceded = false;
            if (buffer.Key)
            {
                operationSucceded = svc.UploadVersion(buffer.Value, versionId.GetValueOrDefault());
            }            
            
            
            //End of process => redirection
            if (operationSucceded)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "show uplod state", "ShowAllocationResult(\"success\");", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "show uplod state", "ShowAllocationResult(\"failure\");", true);
            }
        }

    }
}