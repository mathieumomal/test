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

namespace Etsi.Ultimate.Module.Versions
{
    public partial class UploadVersion : System.Web.UI.Page
    {
        #region Variables / Properties

        // Custom controls
        protected MeetingControl UploadMeeting;

        //Static fields
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private static string specificationTitle = String.Empty;
        private static string releaseDescription = String.Empty;
        private static UploadedFile versionToSave;
        private static string versionPathToSave = String.Empty;
        private const string CONST_VALID_FILENAME = "{0}-{1}{2}{3}";
        private const string CONST_SPEC_VERSIONS_FTP_PATH = "FtpSpecVersions";
        private const string CONST_FTP_ARCHIVE_PATH = "{0}\\Specs\\archive\\{1}_series\\{2}\\";
        private const string CONST_FTP_LATEST_DRAFTS_PATH = "{0}\\Specs\\latest-drafts\\";
        private const string CONST_FTP_VERSIONS_PATH = "{0}\\Specs\\{1:0000}_{2:00}\\{3}\\{4}_series\\";

        //Properties
        private static int UserId;
        public static Nullable<int> releaseId;
        public static Nullable<int> specId;
        public static bool isDraft;
        public static string action;
        public static string versionUploadPath;
        public static string versionFTP_Path;
        private int errorNumber = 0;

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
                ResetPanelState();
                GetRequestParameters();
                ManageAllocationCase();
                LoadVersionUploadContent();
            }
        }

        /// <summary>
        /// Click event of Allocation button
        /// </summary>
        /// <param name="sender">Allocation button</param>
        /// <param name="e">event arguments</param>
        protected void AllocateVersion_Click(object sender, EventArgs e)
        {
            UploadOrAllocateVersion();
        }

        /// <summary>
        /// Click event of upload button
        /// </summary>
        /// <param name="sender">Upload button</param>
        /// <param name="e">event arguments</param>
        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            versionUploadScreen.Visible = false;
            confirmation.Visible = true;
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
        /// Upload the version
        /// </summary>
        /// <param name="sender">Confirmation upload button</param>
        /// <param name="e">event arguments</param>
        protected void Confirmation_Upload_OnClick(object sender, EventArgs e)
        {
            bool isFTPTransferSuccess = TransferToFTP();

            //If FTP transfer succeded, then insert/update version object
            if (isFTPTransferSuccess)
                UploadOrAllocateVersion();

            versionPathToSave = String.Empty; //Clear path
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
                    versionToSave = e.File;
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

                            if (!matchingFileNameFound)
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

                        //Validate document & get the summary report
                        ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                        var businessValidationReport = svc.ValidateVersionDocument(fileExtension, fileStream, version, specificationTitle, releaseDescription, meetingDate);
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Load page content
        /// </summary>
        private void LoadVersionUploadContent()
        {
            bool isActionUpload = action.Equals("upload");
            bool isActionAllocate = action.Equals("allocate");
            if ((releaseId != null) && (specId != null) && (isActionUpload || isActionAllocate))
            {
                //Specification Details
                ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                var spec = specSvc.GetSpecificationDetailsById(UserId, specId.Value).Key;
                isDraft = !(spec.IsUnderChangeControl.HasValue && spec.IsUnderChangeControl.Value);

                SpecNumberVal.Text = spec.Number;
                specificationTitle = spec.Title;

                //Get User Rights
                var allSpecReleasesWithPermissions = specSvc.GetRightsForSpecReleases(UserId, spec);
                var currentSpecReleaseWithPermission = allSpecReleasesWithPermissions.Where(x => x.Key.Fk_ReleaseId == releaseId).FirstOrDefault();
                var userRights = currentSpecReleaseWithPermission.Value ?? new UserRightsContainer();

                //Release Details
                IReleaseService releaseSvc = ServicesFactory.Resolve<IReleaseService>();
                var release = releaseSvc.GetReleaseById(UserId, releaseId.Value).Key;
                ReleaseVal.Text = release.Code;
                releaseDescription = release.Description;

                //SpecVersion Details
                ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                var specVersionsForAllReleases = specVersionSvc.GetVersionsBySpecId(specId.Value);
                var latestSpecVersionForAllReleases = specVersionsForAllReleases.OrderByDescending(x => x.MajorVersion ?? 0)
                                                                                .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                                                                .ThenByDescending(z => z.EditorialVersion ?? 0)
                                                                                .FirstOrDefault();

                var specVersionsForCurrentRelease = specVersionSvc.GetVersionsForSpecRelease(specId.Value, releaseId.Value);
                var latestSpecVersionForCurrentRelease = specVersionsForCurrentRelease.OrderByDescending(x => x.MajorVersion ?? 0)
                                                                                      .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                                                                      .ThenByDescending(z => z.EditorialVersion ?? 0)
                                                                                      .FirstOrDefault();

                var leastSpecVersionPendingUpload = specVersionsForCurrentRelease.Where(x => x.Location == null)
                                                                  .OrderBy(y => y.MajorVersion ?? 0)
                                                                  .ThenBy(z => z.TechnicalVersion ?? 0)
                                                                  .ThenBy(e => e.EditorialVersion ?? 0)
                                                                  .FirstOrDefault();

                string latestVersionNumber = String.Empty;
                if (latestSpecVersionForCurrentRelease != null)
                    latestVersionNumber = String.Format("{0}.{1}.{2}", latestSpecVersionForCurrentRelease.MajorVersion, latestSpecVersionForCurrentRelease.TechnicalVersion, latestSpecVersionForCurrentRelease.EditorialVersion);
                CurrentVersionVal.Text = String.Format("{0}-{1}", SpecNumberVal.Text, latestVersionNumber);

                //Set Default Values
                NewVersionMajorVal.Value = 0;
                NewVersionTechnicalVal.Value = 0;
                NewVersionEditorialVal.Value = 0;

                if ((leastSpecVersionPendingUpload != null) && isActionUpload)
                {
                    NewVersionMajorVal.Value = leastSpecVersionPendingUpload.MajorVersion ?? 0;
                    NewVersionTechnicalVal.Value = leastSpecVersionPendingUpload.TechnicalVersion ?? 0;
                    NewVersionEditorialVal.Value = leastSpecVersionPendingUpload.EditorialVersion ?? 0;
                    UploadMeeting.SelectedMeetingId = leastSpecVersionPendingUpload.Source ?? 0;
                }
                else
                {
                    
                    if (spec.IsUnderChangeControl.HasValue && spec.IsUnderChangeControl.Value)
                    {
                        NewVersionMajorVal.Value = release.Version2g ?? 0;
                        if ((latestSpecVersionForCurrentRelease != null) && (latestSpecVersionForCurrentRelease.TechnicalVersion.HasValue))
                            NewVersionTechnicalVal.Value = latestSpecVersionForCurrentRelease.TechnicalVersion.Value + 1;
                    }
                    else if ((latestSpecVersionForAllReleases != null) && (latestSpecVersionForAllReleases.TechnicalVersion.HasValue))
                        NewVersionTechnicalVal.Value = latestSpecVersionForAllReleases.TechnicalVersion.Value + 1;
                }

                //Set Major Version Status
                NewVersionMajorVal.Enabled = userRights.HasRight(Enum_UserRights.Versions_Modify_MajorVersion);
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
        /// Upload/Allocate Version
        /// </summary>
        private void UploadOrAllocateVersion()
        {

            KeyValuePair<bool, SpecVersion> buffer = fillSpecVersionObject();

            //bool operationSucceded = false;
            if (buffer.Key)
            {
                ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                Report result = svc.UploadOrAllocateVersion(buffer.Value, isDraft);

                if (result.ErrorList.Count > 0)
                {
                    versionUploadScreen.Visible = false;
                    confirmation.Visible = true;
                    
                    rptWarningsErrors.DataSource = result.ErrorList;
                    rptWarningsErrors.DataBind();

                }
                else
                {
                    lblSaveStatus.Text = String.Format("Version {0}.{1}.{2} {3} successfully", buffer.Value.MajorVersion, buffer.Value.TechnicalVersion, buffer.Value.EditorialVersion, action.Equals("upload") ? "uploaded" : "allocated");

                    versionUploadScreen.Visible = false;
                    state.Visible = true;
                }
            }

            ////End of process => redirection
            //if (operationSucceded)
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "show upload state", "ShowAllocationResult(\"success\");", true);
            //else
            //    Page.ClientScript.RegisterStartupScript(this.GetType(), "show upload state", "ShowAllocationResult(\"failure\");", true);
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
                btnConfirmUpload.Visible = false;
            }
        }

        /// <summary>
        /// Prepare the specVersion object to add in DB
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<bool, SpecVersion> fillSpecVersionObject()
        {
            //User's data
            int output;
            if (!int.TryParse(NewVersionMajorVal.Text, out output) || !int.TryParse(NewVersionTechnicalVal.Text, out output) || !int.TryParse(NewVersionEditorialVal.Text, out output))
            {
                return new KeyValuePair<bool, SpecVersion>(false, null);
            }
            else
            {
                SpecVersion version = new SpecVersion();
                version.Fk_SpecificationId = specId;
                version.Fk_ReleaseId = releaseId;
                version.MajorVersion = int.Parse(NewVersionMajorVal.Text);
                version.TechnicalVersion = int.Parse(NewVersionTechnicalVal.Text);
                version.EditorialVersion = int.Parse(NewVersionEditorialVal.Text);

                version.Remarks.Add(new Remark()
                {
                    RemarkText = CommentVal.Text,
                    CreationDate = new Nullable<System.DateTime>(DateTime.UtcNow),
                    Fk_PersonId = UserId
                });

                if (UploadMeeting.SelectedMeeting != null)
                {
                    version.Source = UploadMeeting.SelectedMeeting.MTG_ID;
                }
                if (action.Equals("upload"))
                {
                    version.Location = versionPathToSave;
                    version.DocumentUploaded = DateTime.UtcNow;
                    version.ProvidedBy = UserId;
                }
                return new KeyValuePair<bool, SpecVersion>(true, version);
            }
        }

        /// <summary>
        /// Retreive the URL parameters
        /// </summary>
        private void GetRequestParameters()
        {
            int output;
            UserId = GetUserPersonId(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo());

            releaseId = (Request.QueryString["releaseId"] != null) ? (int.TryParse(Request.QueryString["releaseId"], out output) ? new Nullable<int>(output) : null) : null;
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

            string validFileName = String.Format(CONST_VALID_FILENAME, specNumber, majorVersionBase36, technicalVersionBase36, editorialVersionBase36);
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

        /// <summary>
        /// Reset Panel Status
        /// </summary>
        private void ResetPanelState()
        {
            versionUploadScreen.Visible = true;
            analysis.Visible = false;
            confirmation.Visible = false;
            state.Visible = false;
            versionToSave = null;
            versionPathToSave = String.Empty;
        }

        /// <summary>
        /// Transfer files to FTP & create necessary hard links between files
        /// </summary>
        /// <returns>True/False</returns>
        private bool TransferToFTP()
        {
            bool isFTPTransferSuccess = false;
            StringBuilder errorList = new StringBuilder();
            string ftpBasePath = String.Empty;
            if (ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH] != null)
                ftpBasePath = ConfigurationManager.AppSettings[CONST_SPEC_VERSIONS_FTP_PATH].ToString();

            if (String.IsNullOrEmpty(ftpBasePath))
                errorList.AppendLine("FTP not yet configured");

            string targetFolder = String.Format(CONST_FTP_ARCHIVE_PATH, ftpBasePath, SpecNumberVal.Text.Split('.')[0], SpecNumberVal.Text);

            if (versionToSave != null)
            {
                try
                {
                    string validFileName = GetValidFileName();
                    string zipFileName = validFileName + ".zip";
                    versionPathToSave = Path.Combine(targetFolder, zipFileName);

                    bool isTargetFolderExists = Directory.Exists(targetFolder);
                    if (!isTargetFolderExists)
                        Directory.CreateDirectory(targetFolder);

                    FileToUploadVal.TargetFolder = targetFolder;

                    //If it is not in zip format, compress & upload the same
                    if (!versionToSave.GetExtension().Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var stream = File.Create(versionPathToSave);
                        var package = ZipPackage.Create(stream);
                        package.AddStream(versionToSave.InputStream, validFileName + versionToSave.GetExtension());
                        package.Close(true);
                    }
                    else
                        versionToSave.SaveAs(versionPathToSave);

                    isFTPTransferSuccess = true;
                }
                catch (Exception ex)
                {
                    errorList.AppendLine("Upload FTP Error: " + ex.Message);
                }
                finally
                {
                    versionToSave = null;
                }
            }

            //If FTP transfer succeded, then create / remove hard links
            if (isFTPTransferSuccess)
            {
                try
                {
                    ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    var spec = specSvc.GetSpecificationDetailsById(UserId, specId.Value).Key;
                    if (spec.IsActive && !(spec.IsUnderChangeControl ?? false)) //Draft
                    {
                        string draftPath = String.Format(CONST_FTP_LATEST_DRAFTS_PATH, ftpBasePath);
                        bool isDraftPathExists = Directory.Exists(draftPath);
                        if (!isDraftPathExists)
                            Directory.CreateDirectory(draftPath);

                        ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                        var allDraftVersions = specVersionSvc.GetVersionsBySpecId(specId.Value);
                        var latestUploadedDraft = allDraftVersions.Where(draft => draft.Location != null)
                                                                  .OrderByDescending(x => x.MajorVersion ?? 0)
                                                                  .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                                                  .ThenByDescending(z => z.EditorialVersion ?? 0)
                                                                  .FirstOrDefault();
                        bool createHardLink = true;

                        if (latestUploadedDraft != null)
                        {
                            string latestUploadedDraftVersion = String.Format("{0}{1}{2}", latestUploadedDraft.MajorVersion, latestUploadedDraft.TechnicalVersion, latestUploadedDraft.EditorialVersion);
                            string uploadingDraftVersion = String.Format("{0}{1}{2}", NewVersionMajorVal.Text, NewVersionTechnicalVal.Text, NewVersionEditorialVal.Text);

                            int uploadedVersionNumber;
                            int uploadingVersionNumber;
                            if (int.TryParse(latestUploadedDraftVersion, out uploadedVersionNumber) && int.TryParse(uploadingDraftVersion, out uploadingVersionNumber))
                            {
                                if (uploadedVersionNumber < uploadingVersionNumber)
                                {
                                    //Delete existing draft
                                    string fileName = Path.Combine(draftPath, Path.GetFileName(latestUploadedDraft.Location));
                                    if (File.Exists(fileName))
                                        File.Delete(fileName);
                                    createHardLink = true;
                                }
                            }
                            else
                                createHardLink = false;
                        }

                        if (createHardLink)
                        {
                            string fileName = Path.GetFileName(versionPathToSave);
                            string hardLinkPath = Path.Combine(draftPath, fileName);

                            CreateHardLink(hardLinkPath, versionPathToSave, IntPtr.Zero);
                        }
                    }
                    else //Under Change Control
                    {
                        if ((UploadMeeting.SelectedMeetingId > 0) && (UploadMeeting.SelectedMeeting.START_DATE != null))
                        {
                            string underChangeControlPath = String.Format(CONST_FTP_VERSIONS_PATH, ftpBasePath, UploadMeeting.SelectedMeeting.START_DATE.Value.Year, UploadMeeting.SelectedMeeting.START_DATE.Value.Month, ReleaseVal.Text, SpecNumberVal.Text.Split('.')[0]);
                            bool isUCPathExists = Directory.Exists(underChangeControlPath);
                            if (!isUCPathExists)
                                Directory.CreateDirectory(underChangeControlPath);

                            string fileName = Path.GetFileName(versionPathToSave);
                            string hardLinkPath = Path.Combine(underChangeControlPath, fileName);

                            CreateHardLink(hardLinkPath, versionPathToSave, IntPtr.Zero);

                            //Delete all drafts for this version
                            string draftPath = String.Format(CONST_FTP_LATEST_DRAFTS_PATH, ftpBasePath);
                            string specNumber = SpecNumberVal.Text.Replace(".", String.Empty);

                            if (Directory.Exists(draftPath))
                            {
                                DirectoryInfo di = new DirectoryInfo(draftPath);
                                var draftFiles = di.GetFiles(specNumber + "-*").Select(x => x.FullName).ToList();
                                foreach (string draftFile in draftFiles)
                                {
                                    if (File.Exists(draftFile))
                                        File.Delete(draftFile);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    isFTPTransferSuccess = false;
                }
            }

            return isFTPTransferSuccess;
        }

        /// <summary>
        /// Create Hard links between files
        /// </summary>
        /// <param name="lpFileName">New File Name(Hard Link Path)</param>
        /// <param name="lpExistingFileName">Original File Name</param>
        /// <param name="lpSecurityAttributes">Security Attributes</param>
        /// <returns>True/False</returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );

        #endregion
    }
}