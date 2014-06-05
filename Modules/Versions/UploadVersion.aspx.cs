using Etsi.Ultimate.Controls;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.Zip;

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
        public static Nullable<int> specId;
        public static string action;
        public static string versionUploadPath;
        public static string versionFTP_Path;
        private int errorNumber = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetPanelState();
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
                if (versionId != -1)
                {
                    // Retrieve data
                    ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                    KeyValuePair<SpecVersion, UserRightsContainer> specVersionRightsObject = svc.GetVersionsById(versionId.Value, UserId);
                    SpecVersion specVerion = specVersionRightsObject.Key;
                    UserRightsContainer userRights = specVersionRightsObject.Value;

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
                            ManageNewVersion(specVerion, userRights);
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
                    LoadVersionDetails(null);
                    NewVersionMajorVal.Text = NewVersionTechnicalVal.Text = NewVersionEditorialVal.Text = "0";
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
            if (version != null)
            {
                SpecNumberVal.Text = version.Specification.Number;
                ReleaseVal.Text = version.Release.Code;
                CurrentVersionVal.Text = SpecNumberVal.Text + "-" + version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion;
            }
            //If no previous versions are available
            else if (specId.HasValue)
            {
                ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                Specification_Release spec = specSvc.GetSpecificationDetailsById(UserId, specId.Value).Key.Specification_Release.FirstOrDefault();

                SpecNumberVal.Text = spec.Specification.Number;
                ReleaseVal.Text = spec.Release.Code;
                CurrentVersionVal.Text = SpecNumberVal.Text + "-";
            }

        }

        /// <summary>
        /// Manage the new version to upload
        /// </summary>
        private void ManageNewVersion(SpecVersion version, UserRightsContainer userRights)
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
                NewVersionMajorVal.MinValue = (version.MajorVersion.HasValue) ? version.MajorVersion.Value : default(int);

                NewVersionTechnicalVal.Text = (version.TechnicalVersion + 1).ToString();
                NewVersionTechnicalVal.MinValue = (version.MajorVersion.HasValue) ? version.MajorVersion.Value + 1 : default(int);

                NewVersionEditorialVal.Text = "0";
            }


            //If not MCC representive => NewVersionMajorVal.Disabled = true;
            NewVersionMajorVal.Enabled = userRights.HasRight(Enum_UserRights.Versions_Modify_MajorVersion);

        }

        /// <summary>
        /// Display or not file upload input
        /// Display or not allocation btn
        /// </summary>
        private void ManageAllocationCase()
        {
            if (action.Equals("allocate"))
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
        private KeyValuePair<bool, SpecVersion> fillSpecVersionObject()
        {

            //User's data
            int[] verionsOutputs = new int[] { 0, 0, 0 };
            if (!int.TryParse(NewVersionMajorVal.Text, out verionsOutputs[0]) || !int.TryParse(NewVersionTechnicalVal.Text, out verionsOutputs[1]) || !int.TryParse(NewVersionEditorialVal.Text, out verionsOutputs[2]))
            {
                return new KeyValuePair<bool, SpecVersion>(false, null);
            }
            else
            {

                SpecVersion version = new SpecVersion();

                if (versionId == -1 && specId.HasValue)
                {
                    ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    Specification_Release spec = specSvc.GetSpecificationDetailsById(UserId, specId.Value).Key.Specification_Release.FirstOrDefault();

                    version.Fk_ReleaseId = spec.Fk_ReleaseId;
                    version.Fk_SpecificationId = spec.Fk_SpecificationId;
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

                    version.Fk_ReleaseId = specVerion.Fk_ReleaseId;
                    version.Fk_SpecificationId = specVerion.Fk_SpecificationId;
                }

                version.MajorVersion = int.Parse(NewVersionMajorVal.Text);
                version.TechnicalVersion = int.Parse(NewVersionTechnicalVal.Text);
                version.EditorialVersion = int.Parse(NewVersionEditorialVal.Text);

                version.Remarks.Add(new Remark()
                {
                    RemarkText = CommentVal.Text,
                    CreationDate = new Nullable<System.DateTime>(DateTime.Now),
                    Fk_PersonId = UserId
                });

                if (UploadMeeting.SelectedMeeting != null)
                {
                    version.Source = UploadMeeting.SelectedMeeting.MTG_ID;
                }
                if (action.Equals("upload"))
                {
                    version.DocumentUploaded = DateTime.Now;
                    version.ProvidedBy = UserId;
                }

                return new KeyValuePair<bool, SpecVersion>(true, version);

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
            specId = (Request.QueryString["specId"] != null) ? (int.TryParse(Request.QueryString["specId"], out output) ? new Nullable<int>(output) : null) : null;
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
            if (enableUpload)
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
            using (MemoryStream fileStream = new MemoryStream())
            {
                try
                {
                    UploadedFile uploadedFile = e.File;
                    string fileExtension = String.Empty;
                    Report validationReport = new Report();

                    //[1] Check the file name
                    string validFileName = GetValidFileName();
                    if (!uploadedFile.GetNameWithoutExtension().Equals(validFileName, StringComparison.InvariantCultureIgnoreCase))
                        validationReport.LogWarning(String.Format("Invalid file name. System will change this to '{0}'", validFileName));

                    bool allowToRunQualityChecks = false;

                    //[2] If file is in .zip format, check that .zip file and internal word file must have same name.
                    if (uploadedFile.GetExtension().Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var zipFileName = uploadedFile.GetNameWithoutExtension();
                        using (ZipPackage package = ZipPackage.Open(uploadedFile.InputStream))
                        {
                            bool matchingFileNameFound = false;

                            foreach (var entry in package.ZipPackageEntries)
                            {
                                if (entry.Attributes != FileAttributes.Directory)
                                {
                                    string extension = Path.GetExtension(entry.FileNameInZip);
                                    if (extension.Equals(".doc", StringComparison.InvariantCultureIgnoreCase) ||
                                        extension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entry.FileNameInZip);

                                        if (zipFileName.Equals(fileNameWithoutExtension, StringComparison.InvariantCultureIgnoreCase))
                                            matchingFileNameFound = true;

                                        //Allow to run validation checks either filename matching with zip name or zip contains only one file
                                        if (matchingFileNameFound || package.ZipPackageEntries.Where(x => x.Attributes != FileAttributes.Directory).Count() == 1)
                                        {
                                            using (Stream stream = entry.OpenInputStream())
                                            {
                                                stream.CopyTo(fileStream);
                                            }
                                            fileExtension = extension;
                                            allowToRunQualityChecks = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if(!matchingFileNameFound)
                                validationReport.LogWarning("Zip file and internal word file must have same name");

                            if (!allowToRunQualityChecks)
                                validationReport.LogWarning("No matching files inside zip. Quality checks cannot be executed");
                        }
                    }
                    else //Copy to stream from .doc / .docx
                    {
                        using (Stream stream = uploadedFile.InputStream)
                        {
                            stream.CopyTo(fileStream);
                        }
                        fileExtension = uploadedFile.GetExtension();
                        allowToRunQualityChecks = true;
                    }

                    //If we have valid file, run quality checks
                    if (allowToRunQualityChecks)
                    {
                        string version = String.Empty;
                        DateTime meetingDate = DateTime.MinValue;
                        string title = String.Empty;
                        string release = String.Empty;

                        version = String.Format("{0}.{1}.{2}", NewVersionMajorVal.Text.Trim(), NewVersionTechnicalVal.Text.Trim(), NewVersionEditorialVal.Text.Trim());
                        if (UploadMeeting.SelectedMeeting != null)
                            meetingDate = UploadMeeting.SelectedMeeting.START_DATE ?? DateTime.MinValue;

                        ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                        if ((versionId.HasValue) && (versionId > 0))
                        {
                            var specVersionRightsObject = svc.GetVersionsById(versionId.Value, UserId);
                            var specVerion = specVersionRightsObject.Key;
                            title = specVerion.Specification.Title;
                            release = specVerion.Release.Description;
                        }

                        //Validate document & get the summary report
                        var businessValidationReport = svc.ValidateVersionDocument(fileExtension, fileStream, version, title, release, meetingDate);
                        validationReport.ErrorList.AddRange(businessValidationReport.ErrorList);
                        validationReport.WarningList.AddRange(businessValidationReport.WarningList);
                    }

                    // Update Errors / Warnings
                    lblCountWarningErrors.Text = new StringBuilder()
                        .Append("Found ")
                        .Append(validationReport.GetNumberOfErrors().ToString())
                        .Append(" error")
                        .Append(validationReport.GetNumberOfErrors() <= 1 ? "" : "s")
                        .Append(", ")
                        .Append(validationReport.GetNumberOfWarnings().ToString())
                        .Append(" warning")
                        .Append(validationReport.GetNumberOfWarnings() <= 1 ? "" : "s")
                        .Append(".")
                        .ToString();

                    if (validationReport.GetNumberOfErrors() > 0)
                    {
                        btnConfirmUpload.Enabled = false;
                        errorNumber = validationReport.GetNumberOfErrors();
                    }
                    else
                        btnConfirmUpload.Enabled = true;

                    List<string> datasource = new List<string>();
                    datasource.AddRange(validationReport.ErrorList);
                    datasource.AddRange(validationReport.WarningList);
                    rptWarningsErrors.DataSource = datasource;
                    rptWarningsErrors.DataBind();
                }
                catch (Exception exc)
                {
                    LogManager.Error("Could not save the version file: " + exc.Message);
                }
            }
        }

        protected void AllocateVersion_Click(object sender, EventArgs e)
        {
            GetRequestParameters();
            KeyValuePair<bool, SpecVersion> buffer = fillSpecVersionObject();
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

        #region Private Methods

        /// <summary>
        /// Provide the valid file name for Version upload
        /// </summary>
        /// <returns>Valid version file name</returns>
        private string GetValidFileName()
        {
            var specNumber = SpecNumberVal.Text.Replace(".", String.Empty);

            int majorVersion;
            string majorVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionMajorVal.Text, out majorVersion))
                majorVersionBase36 = EncodeToBase36(majorVersion);

            int technicalVersion;
            string technicalVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionTechnicalVal.Text, out technicalVersion))
                technicalVersionBase36 = EncodeToBase36(technicalVersion);

            int editorialVersion;
            string editorialVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionEditorialVal.Text, out editorialVersion))
                editorialVersionBase36 = EncodeToBase36(editorialVersion);

            string validFileName = String.Format("{0}-{1}{2}{3}", specNumber, majorVersionBase36, technicalVersionBase36, editorialVersionBase36);
            return validFileName;
        }

        /// <summary>
        /// Convert to Base36 string
        /// </summary>
        /// <param name="input">Base10 number</param>
        /// <returns>Base36 string</returns>
        private string EncodeToBase36(long input)
        {
            if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");
            var CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] clistarr = CharList.ToCharArray();
            var result = new Stack<char>();
            if (input == 0)
                result.Push('0');
            else
            {
                while (input != 0)
                {
                    result.Push(clistarr[input % 36]);
                    input /= 36;
                }
            }
            return new string(result.ToArray());
        }

        #endregion

        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            versionUploadScreen.Visible = false;
            confirmation.Visible = true;
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            ResetPanelState();
        }

        private void ResetPanelState()
        {
            versionUploadScreen.Visible = true;
            analysis.Visible = false;
            confirmation.Visible = false;
            state.Visible = false;
        }
        
        /// <summary>
        /// Item DataBound event for Error/Warning report
        /// </summary>
        /// <param name="Sender">Repeater control</param>
        /// <param name="e">Repeater item event arguments</param>
        protected void rptErrorsWarning_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
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
    }
}