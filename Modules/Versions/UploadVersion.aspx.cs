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

        //To remove ------------------------------------------------------
        public static readonly string DsId_Key = "ETSI_DS_ID";
        private const string CONST_VALID_FILENAME = "{0}-{1}{2}{3}";
        private const string CONST_FTP_ARCHIVE_PATH = "{0}\\Specs\\archive\\{1}_series\\{2}\\";
        private const string CONST_FTP_LATEST_PATH = "{0}\\Specs\\latest\\{1}\\{2}_series\\";
        private const string CONST_FTP_LATEST_DRAFTS_PATH = "{0}\\Specs\\latest-drafts\\";
        private const string CONST_FTP_VERSIONS_PATH = "{0}\\Specs\\{1}\\{2}\\{3}_series\\";
        //To remove ------------------------------------------------------


        private static string releaseDescription = String.Empty;
        private static UploadedFile versionToSave;
        private static string versionPathToSave = String.Empty;
        public static string versionUploadPath;
        public static string versionFTP_Path;
        private int errorNumber = 0;


        //Attributes validated --------------------------------------------------------------------
        private const string CONST_WARNING_REPORT = "CONST_WARNING_REPORT";
        private const string CONST_USER_ID_VIEWSTATE_LABEL = "CONST_USER_ID_VIEWSTATE_LABEL";
        private const string CONST_RELEASE_ID_VIEWSTATE_LABEL = "CONST_RELEASE_ID_VIEWSTATE_LABEL";
        private const string CONST_SPEC_ID_VIEWSTATE_LABEL = "CONST_SPEC_ID_VIEWSTATE_LABEL";
        private const string CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL = "CONST_IS_UPLOAD_MODE_VIEWSTATE_LABEL";
        private const string CONST_IS_DRAFT_VIEWSTATE_LABEL = "CONST_IS_DRAFT_VIEWSTATE_LABEL";

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
        /// <summary>
        /// true : upload case, false : allocate case
        /// </summary>
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

        //To discusss their utility :
        private static int communityID;
        private static bool isTS;
        private static string specificationTitle = String.Empty;
        #endregion

        #region OLD Events OLD
        /// <summary>
        /// Click event of upload button
        /// </summary>
        /// <param name="sender">Upload button</param>
        /// <param name="e">event arguments</param>
        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            var meetingId = UploadMeeting.SelectedMeetingId;
            if (meetingId > 0 || IsDraft)
            {
                versionUploadScreen.Visible = false;
                confirmation.Visible = true;
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
        /// Upload the version
        /// </summary>
        /// <param name="sender">Confirmation upload button</param>
        /// <param name="e">event arguments</param>
        protected void Confirmation_Upload_OnClick(object sender, EventArgs e)
        {
            //UploadOrAllocateVersion(true);
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

                    //If we have valid file & spec is under change control, run quality checks
                    if (allowToRunQualityChecks && !IsDraft)
                    {
                        string version = String.Empty;
                        DateTime meetingDate = DateTime.MinValue;
                        string tsgTitle = String.Empty;

                        ICommunityService communityService = ServicesFactory.Resolve<ICommunityService>();
                        var communities = communityService.GetCommunities();
                        var community = communities.Where(x => x.TbId == communityID).FirstOrDefault();
                        if (community != null)
                            tsgTitle = GetParentTSG(community, communities).TbTitle.Replace("Technical Specification Group -", "Technical Specification Group");

                        version = String.Format("{0}.{1}.{2}", NewVersionMajorVal.Text.Trim(), NewVersionTechnicalVal.Text.Trim(), NewVersionEditorialVal.Text.Trim());
                        if (UploadMeeting.SelectedMeeting != null)
                            meetingDate = UploadMeeting.SelectedMeeting.START_DATE ?? DateTime.MinValue;

                        //Validate document & get the summary report
                        ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                        var businessValidationReport = svc.ValidateVersionDocument(fileExtension, fileStream, Server.MapPath(FileToUploadVal.TemporaryFolder), version, specificationTitle, releaseDescription, meetingDate, tsgTitle, isTS);
                        validationReport.ErrorList.AddRange(businessValidationReport.ErrorList);
                        validationReport.WarningList.AddRange(businessValidationReport.WarningList);
                    }

                    // Update Errors / Warnings
                    lblCountWarningErrors.Text = new StringBuilder()
                        .Append("Found ")
                        .Append(validationReport.GetNumberOfErrors().ToString())
                        .Append(" error(s)")
                        .Append(validationReport.GetNumberOfErrors() <= 1 ? "" : "s")
                        .Append(", ")
                        .Append(validationReport.GetNumberOfWarnings().ToString())
                        .Append(" warning(s)")
                        .Append(validationReport.GetNumberOfWarnings() <= 1 ? "" : "s")
                        .Append(".")
                        .ToString();

                    if (validationReport.GetNumberOfErrors() > 0)
                    {
                        btnConfirmUpload.Enabled = false;
                        errorNumber = validationReport.GetNumberOfErrors();
                    }
                    else
                    {
                        if (validationReport.GetNumberOfWarnings() > 0)
                            report = validationReport;
                        btnConfirmUpload.Enabled = true;
                    }

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

        #region OLD Private Methods OLD
        /// <summary>
        /// Upload/Allocate Version
        /// </summary>
        /*private void UploadOrAllocateVersion(bool isUpload)
        {
            KeyValuePair<bool, SpecVersion> buffer = GetEditedSpecVersionObject();

            if (buffer.Key)
            {
                Report ftpTransferReport = new Report();
                Report result = new Report();

                if (isUpload)
                {
                    ftpTransferReport = TransferToFTP();
                    string ftpPhysicalPath = ConfigVariables.FtpBasePhysicalPath;
                    string ftpBaseAddress = ConfigVariables.FtpBaseAddress;
                    buffer.Value.Location = versionPathToSave.Replace(ftpPhysicalPath, ftpBaseAddress).Replace("/\\", "/").Replace("\\", "/");
                }

                if (ftpTransferReport.ErrorList.Count == 0)
                {
                    ISpecVersionService svc = ServicesFactory.Resolve<ISpecVersionService>();
                    result = svc.UploadOrAllocateVersion(buffer.Value, IsDraft, UserId, report);
                }
                else
                    result.ErrorList.AddRange(ftpTransferReport.ErrorList);

                if (result.ErrorList.Count > 0)
                {
                    versionUploadScreen.Visible = false;
                    analysis.Visible = false;
                    //Prevent upload of already uploaded versions
                    confirmation.Visible =true;
                    btnConfirmUpload.Enabled = result.ErrorList.Contains(String.Format("Document has already been uploaded to this version")) ? false : true;
                    state.Visible = false;

                    rptWarningsErrors.DataSource = result.ErrorList;
                    rptWarningsErrors.DataBind();
                }
                else
                {
                    lblSaveStatus.Text = String.Format("Version {0}.{1}.{2} {3} successfully", buffer.Value.MajorVersion, buffer.Value.TechnicalVersion, buffer.Value.EditorialVersion, IsUploadMode ? "uploaded" : "allocated");
                    versionUploadScreen.Visible = false;
                    analysis.Visible = false;
                    confirmation.Visible = false;
                    state.Visible = true;

                    state_confirmation.OnClientClicked = "closeRadWindow";
                }
            }

            versionPathToSave = String.Empty; //Clear path
        }*/
        /// <summary>
        /// Provide the valid file name for Version upload
        /// </summary>
        /// <returns>Valid version file name</returns>
        private string GetValidFileName()
        {
            var specNumber = SpecNumberVal.Text.Replace(".", String.Empty);
            var utilsService = new UtilsService();

            int majorVersion;
            string majorVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionMajorVal.Text, out majorVersion))
                majorVersionBase36 = utilsService.EncodeToBase36Digits2(majorVersion);

            int technicalVersion;
            string technicalVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionTechnicalVal.Text, out technicalVersion))
                technicalVersionBase36 = utilsService.EncodeToBase36Digits2(technicalVersion);

            int editorialVersion;
            string editorialVersionBase36 = String.Empty;
            if (int.TryParse(NewVersionEditorialVal.Text, out editorialVersion))
                editorialVersionBase36 = utilsService.EncodeToBase36Digits2(editorialVersion);

            string validFileName = String.Format(CONST_VALID_FILENAME, specNumber, majorVersionBase36, technicalVersionBase36, editorialVersionBase36);
            return validFileName;
        }
        /// <summary>
        /// Transfer files to FTP & create necessary hard links between files
        /// </summary>
        /// <returns>Error Report</returns>
        private Report TransferToFTP()
        {
            Report errorReport = new Report();
            string ftpBasePath = ConfigVariables.FtpBasePhysicalPath;


            if (String.IsNullOrEmpty(ftpBasePath))
                errorReport.LogError("FTP not yet configured");

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
                }
                catch (Exception ex)
                {
                    errorReport.LogError("Upload FTP Error: " + ex.Message);
                }
                finally
                {
                    versionToSave = null;
                }
            }

            //If FTP transfer succeded, then create / remove hard links
            if (errorReport.ErrorList.Count == 0)
            {
                try
                {
                    ISpecificationService specSvc = ServicesFactory.Resolve<ISpecificationService>();
                    var spec = specSvc.GetSpecificationDetailsById(GetUserPersonId(), SpecId).Key;
                    if (spec.IsActive && !(spec.IsUnderChangeControl ?? false)) //Draft
                    {
                        string draftPath = String.Format(CONST_FTP_LATEST_DRAFTS_PATH, ftpBasePath);
                        bool isDraftPathExists = Directory.Exists(draftPath);
                        if (!isDraftPathExists)
                            Directory.CreateDirectory(draftPath);

                        ISpecVersionService specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                        var allDraftVersions = specVersionSvc.GetVersionsBySpecId(SpecId);
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
                            string latestFolder = ConfigVariables.VersionsLatestFTPFolder;
                            if (String.IsNullOrEmpty(latestFolder))
                                latestFolder = String.Format("{0:0000}-{1:00}", UploadMeeting.SelectedMeeting.START_DATE.Value.Year, UploadMeeting.SelectedMeeting.START_DATE.Value.Month);
                            string underChangeControlPath = String.Format(CONST_FTP_VERSIONS_PATH, ftpBasePath, latestFolder, ReleaseVal.Text, SpecNumberVal.Text.Split('.')[0]);
                            bool isUCCPathExists = Directory.Exists(underChangeControlPath);
                            if (!isUCCPathExists)
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

                            //Create hard link in 'Latest' folder
                            string latestFolderPath = String.Format(CONST_FTP_LATEST_PATH, ftpBasePath, ReleaseVal.Text, SpecNumberVal.Text.Split('.')[0]);
                            bool isLatestFolderPathExists = Directory.Exists(latestFolderPath);
                            if (!isLatestFolderPathExists)
                                Directory.CreateDirectory(latestFolderPath);
                            string hardLinkPathInLatestFolder = Path.Combine(latestFolderPath, fileName);
                            CreateHardLink(hardLinkPathInLatestFolder, hardLinkPath, IntPtr.Zero);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorReport.LogError("Hard Link create error:" + ex.Message);
                }
            }

            return errorReport;
        }
        /// <summary>
        /// Get Parent TSG
        /// </summary>
        /// <param name="community">Community</param>
        /// <param name="communities">List of Communities</param>
        /// <returns>Parent TSG community</returns>
        private Community GetParentTSG(Community community, List<Community> communities)
        {
            if (community.TbType == "TSG")
                return community;
            else
            {
                var parentCommunity = communities.Where(x => x.TbId == community.ParentCommunityId).FirstOrDefault();
                if (parentCommunity != null)
                    return GetParentTSG(parentCommunity, communities);
                else
                    return community;
            }
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


        #region NEW events NEW
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

        //BTN click events
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

            if (report.GetNumberOfErrors() == 0)
            {
                lblSaveStatus.Text = String.Format(UploadVersion_aspx.SuccessMessage, version.Value.MajorVersion, version.Value.TechnicalVersion, version.Value.EditorialVersion, "allocated");
                versionUploadScreen.Visible = false;
                analysis.Visible = false;
                confirmation.Visible = false;
                state.Visible = true;
                state_confirmation.OnClientClicked = "closeRadWindow";
            }
            else
            {
                versionUploadScreen.Visible = false;
                analysis.Visible = false;
                //Prevent upload of already uploaded versions
                confirmation.Visible = true;
                btnConfirmUpload.Enabled = report.ErrorList.Contains(String.Format(UploadVersion_aspx.DocumentAlreadyUploaded)) ? false : true;
                state.Visible = false;

                rptWarningsErrors.DataSource = report.ErrorList;
                rptWarningsErrors.DataBind();
            }
        }
        //BTN click events

        //Upload events

        //Upload events
        #endregion

        #region NEW Private Methods NEW
        /// <summary>
        /// Reset version upload/allocate popup and its attributes
        /// </summary>
        private void ResetPanelState()
        {
            //UI elements
            versionUploadScreen.Visible = true;
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
            //Code behind attributes
            versionToSave = null;
            versionPathToSave = String.Empty;
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
                if (Int32.TryParse(userInfo.Profile.GetPropertyValue(DsId_Key), out personID))
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
                    SpecAttributesHandler(version.NewSpecVersion.Specification);
                    ReleaseAttributesHandler(version.NewSpecVersion.Release);

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
        //TO REFRACTOR -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Spec attributes handler
        /// </summary>
        private void SpecAttributesHandler(Specification spec)//---------------
        {
            IsDraft = !(spec.IsUnderChangeControl.HasValue && spec.IsUnderChangeControl.Value && spec.IsActive);
            hidIsRequired.Value = (!IsDraft && IsUploadMode) ? "True" : "False";
            MeetingLbl.Text = (!IsDraft && IsUploadMode) ? "Meeting(<span class='requiredField'>*</span>):" : "Meeting:";
            SpecNumberVal.Text = spec.Number;

            //----------------------------------------------- UTILITY ?
            communityID = spec.PrimeResponsibleGroup.Fk_commityId;
            isTS = spec.IsTS ?? true;
            specificationTitle = spec.Title;
            //----------------------------------------------- UTILITY ?
        }
        /// <summary>
        /// Release attributes handler
        /// </summary>
        private void ReleaseAttributesHandler(Release release)//-------------------
        {
            ReleaseVal.Text = release.Code;
            //----------------------------------------------- UTILITY ?
            releaseDescription = release.Name;
            //----------------------------------------------- UTILITY ?
        }
        //TO REFRACTOR -------------------------------------------------------------------------------------------------------------
        #endregion
    }
}