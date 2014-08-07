using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.Utils.ModelMails;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionUploadAction
    {

        private const string CACHE_KEY = "VERSION_UPLOAD";

        private const string CONST_VALID_FILENAME = "{0}-{1}{2}{3}";
        private const string CONST_FTP_ARCHIVE_PATH = "{0}\\Specs\\archive\\{1}_series\\{2}\\";
        private const string CONST_FTP_LATEST_PATH = "{0}\\Specs\\latest\\{1}\\{2}_series\\";
        private const string CONST_FTP_LATEST_DRAFTS_PATH = "{0}\\Specs\\latest-drafts\\";
        private const string CONST_FTP_VERSIONS_PATH = "{0}\\Specs\\{1}\\{2}\\{3}_series\\";

        private const string CONST_QUALITY_CHECK_REVISIONMARK = "Some revision marks left unaccepted";
        private const string CONST_QUALITY_CHECK_VERSION_HISTORY = "Invalid/missing version in history";
        private const string CONST_QUALITY_CHECK_VERSION_COVERPAGE = "Invalid/missing version in cover page";
        private const string CONST_QUALITY_CHECK_DATE_COVERPAGE = "Invalid/missing date in cover page";
        private const string CONST_QUALITY_CHECK_YEAR_COPYRIGHT = "Year not valid/missing in copyright statement";
        private const string CONST_QUALITY_CHECK_TITLE_COVERPAGE = "Incorrect/missing title in cover page";
        private const string CONST_QUALITY_CHECK_RELEASE_STYLE = "Release style should be 'ZGSM' in cover page";
        private const string CONST_QUALITY_CHECK_AUTO_NUMBERING = "Automatic numbering (of clauses, figures, tables, notes, examples etc…) should be disabled in the document";
        private const string CONST_QUALITY_CHECK_FIRST_TWO_LINES_TITLE = "The first two lines of the title must be correct, according to the TSG responsible for the specification";
        private const string CONST_QUALITY_CHECK_ANNEXURE_STYLE = "Annexes should be correctly styled as Heading 8(TS) or Heading 9(TR). In case of TS, (normative) or (informative) should appear immediately after annexure heading";
        private const string CONST_QUALITY_CHECK_RELEASE = "Invalid/missing release in cover page";

        public IUltimateUnitOfWork UoW;


        /// <summary>
        /// Entry point for checking the upload of a version. Will check that the version can be parsed, 
        /// and in this case launch the quality checks of the test.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path)
        {
            var svcResponse = new ServiceResponse<string>();

            GetRelatedSpecAndRelease(personId, version);

            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                try
                {
                    CheckPersonRightToUploadVersion(version, personId);
                    CheckUploadAllowed(version, path);

                    Report validationReport = svcResponse.Report;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    var fileExtension = String.Empty;
                    string fileToAnalyzePath = String.Empty;
                    

                    //[1] Check the file name
                    string validFileName = GetValidFileName(version);
                    if (!fileNameWithoutExtension.Equals(validFileName, StringComparison.InvariantCultureIgnoreCase))
                        validationReport.LogWarning(String.Format("Invalid file name. System will change this to '{0}'", validFileName));

                    bool allowToRunQualityChecks = false;
                    //[2] If file is in .zip format, check that .zip file and internal word file must have same name.
                    if (Path.GetExtension(path).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var zipContent = Zip.Extract(path, true);
                        List<string> zipContentShortPath = new List<string>();
                        zipContent.ForEach(x => zipContentShortPath.Add(x.Split('\\').Last()));


                        //1 - File match found
                        if (zipContentShortPath.Find(x => x.Equals(fileNameWithoutExtension + ".doc", StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".doc";
                            fileToAnalyzePath = Path.GetDirectoryName(zipContent.First()) + "\\" + fileNameWithoutExtension + ".doc";
                        }
                        else if (zipContentShortPath.Find(x => x.Equals(fileNameWithoutExtension + ".docx", StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".docx";
                            fileToAnalyzePath = Path.GetDirectoryName(zipContent.First()) + "\\" + fileNameWithoutExtension + ".docx";
                        }
                        else
                        {
                            if (zipContent.Count == 1 && (zipContent.First().ToLower().EndsWith(".doc")))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".doc";
                                fileToAnalyzePath = zipContent.First();
                                validationReport.LogWarning("Zip file and internal word file must have same name");
                            }
                            else if (zipContent.Count == 1 && zipContent.First().ToLower().EndsWith(".docx"))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".docx";
                                fileToAnalyzePath = zipContent.First();
                                validationReport.LogWarning("Zip file and internal word file must have same name");
                            }
                            else
                            {
                                validationReport.LogWarning("No matching files inside zip. Quality checks cannot be executed");
                            }
                        }
                    }
                    else //Copy to stream from .doc / .docx
                    {
                        if (!Path.GetExtension(path).Equals("doc", StringComparison.InvariantCultureIgnoreCase) &&
                            !Path.GetExtension(path).Equals("docx", StringComparison.InvariantCultureIgnoreCase))
                            throw new InvalidOperationException("Invalid file format provided");
                        allowToRunQualityChecks = true;
                    }

                    //If we have valid file & spec is under change control, run quality checks
                    if (allowToRunQualityChecks && version.Specification.IsUnderChangeControl.GetValueOrDefault())
                    {
                        string versionStr = String.Empty;
                        DateTime meetingDate = DateTime.MinValue;
                        string tsgTitle = String.Empty;

                        ICommunityManager communityMgr = ManagerFactory.Resolve<ICommunityManager>();
                        if (version.Specification.PrimeResponsibleGroup != null)
                        {
                            var communities = communityMgr.GetCommunities();
                            var community = communities.Where(x => x.TbId == version.Specification.PrimeResponsibleGroup.Fk_commityId).FirstOrDefault();
                            if (community != null)
                                tsgTitle = GetParentTSG(community, communities).TbTitle.Replace("Technical Specification Group -", "Technical Specification Group");
                        }
                        versionStr = String.Format("{0}.{1}.{2}", version.MajorVersion.Value, version.TechnicalVersion.Value, version.EditorialVersion.Value);

                        if (version.Source.HasValue)
                        {
                            MeetingManager meetingMgr = new MeetingManager();
                            meetingMgr.UoW = UoW;

                            meetingDate = meetingMgr.GetMeetingById(version.Source.Value).END_DATE.Value;
                        }
                        using (Stream fileStream = new FileStream(fileToAnalyzePath, FileMode.Open))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                fileStream.CopyTo(memoryStream);

                                var businessValidationReport = ValidateVersionDocument(fileExtension,
                                                                                           memoryStream,
                                                                                           Path.GetDirectoryName(path),
                                                                                           versionStr,
                                                                                           version.Specification.Title,
                                                                                           version.Release.Name,
                                                                                           meetingDate, tsgTitle,
                                                                                           version.Specification.IsTS.GetValueOrDefault());

                                validationReport.ErrorList.AddRange(businessValidationReport.ErrorList);
                                validationReport.WarningList.AddRange(businessValidationReport.WarningList);

                                fileStream.Close();
                            }
                        }
                    }
                    svcResponse.Result = Guid.NewGuid().ToString();
                    CacheManager.InsertForLimitedTime(CACHE_KEY + svcResponse.Result, new CacheUploadStorage(version, path, validationReport), 10);
                }
                catch (Exception exc)
                {
                    svcResponse.Report.LogError("An error occured: " + exc.Message);
                }
            }
            return svcResponse;
        }

        
        /// <summary>
        /// Does perform the upload of the version, given the token that was passed by CheckVersionForUpload function.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ServiceResponse<string> UploadVersion(int personId, SpecVersion version, string token)
        {
            var svcResponse = new ServiceResponse<string>();
            GetRelatedSpecAndRelease(personId, version);

            try
            {
                CheckPersonRightToUploadVersion(version, personId);
                if (svcResponse.Report.GetNumberOfErrors() == 0)
                {
                    var versionInfos = (CacheUploadStorage)CacheManager.Get(CACHE_KEY + token);
                    if (versionInfos == null)
                        throw new InvalidOperationException("An error occured during file retrieval. Please try again.");

                    TransferToFtp(versionInfos.Version, versionInfos.TmpUploadedFilePath);

                    UpdateDatabase(versionInfos.Version, versionInfos.ValidationReport, personId);
                }
            }
            catch (Exception e)
            {
                svcResponse.Report.LogError("An error occured while uploading: "+e.Message);
            }
            return svcResponse;
        }

        private void CheckPersonRightToUploadVersion(SpecVersion version, int personId)
        {
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            IRightsManager rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);
            var specReleaseRights = specMgr.GetRightsForSpecRelease(rights, personId, version.Specification, version.Fk_ReleaseId.Value, new List<Release>() { version.Release });

            if (!specReleaseRights.Value.HasRight(Enum_UserRights.Versions_Upload))
            {
                throw new InvalidOperationException(Utils.Localization.RightError);
            }
        }

        /// <summary>
        /// Checks that user has right to perform the upload, in so far as:
        /// - Version does not already exist
        /// - if version is a draft, it's major version is less or equal 2.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="path"></param>
        private void CheckUploadAllowed(SpecVersion version, string path)
        {
            // Version should not already exist
            var versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;

            var versionsForSpecRelease = versionMgr.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);
            if (versionsForSpecRelease.Where(v => v.MajorVersion == version.MajorVersion 
                && v.TechnicalVersion == version.TechnicalVersion 
                && v.EditorialVersion == version.EditorialVersion
                && !string.IsNullOrEmpty(v.Location)).Count() > 0)
            {
                var versionStr = version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion;
                throw new InvalidOperationException(String.Format(Utils.Localization.Upload_Version_Error_Version_Already_Exists, versionStr));
            }

            // Cannot upload a version lower than existing version, except if it's been allocated.
            var highestUploadedVersion = versionsForSpecRelease.Where(v => !string.IsNullOrEmpty(v.Location))
                .OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion)
                .FirstOrDefault();
            if (highestUploadedVersion != null && (version.MajorVersion < highestUploadedVersion.MajorVersion
                || version.MajorVersion == highestUploadedVersion.MajorVersion && version.TechnicalVersion < highestUploadedVersion.TechnicalVersion
                || version.MajorVersion == highestUploadedVersion.MajorVersion && version.TechnicalVersion == highestUploadedVersion.TechnicalVersion && version.EditorialVersion < highestUploadedVersion.EditorialVersion))
            {
                // Check allocation
                if (versionsForSpecRelease.Where(v => v.MajorVersion == version.MajorVersion && v.TechnicalVersion == version.TechnicalVersion && v.EditorialVersion == version.EditorialVersion).FirstOrDefault() == null)
                    throw new InvalidOperationException(Utils.Localization.Upload_Version_Error_Previous_Version);

            }


            // If version is a draft, then it's major number must not be higher than 2.
            if (!version.Specification.IsUnderChangeControl.GetValueOrDefault() && version.MajorVersion > 2)
            {
                throw new InvalidOperationException(String.Format(Utils.Localization.Upload_Version_Error_Draft_Major_Too_High));
            }
        }

        private string GetValidFileName(SpecVersion specVersion)
        {
            var specNumber = specVersion.Specification.Number;

            var utilsMgr = new UtilsManager();

            string majorVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.MajorVersion);
            string technicalVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.TechnicalVersion);
            string editorialVersionBase36 = utilsMgr.EncodeToBase36Digits2((long)specVersion.EditorialVersion);

            string validFileName = String.Format(CONST_VALID_FILENAME, specNumber.Replace(".",""), majorVersionBase36, technicalVersionBase36, editorialVersionBase36);
            return validFileName;
        }

        private void GetRelatedSpecAndRelease(int personId, SpecVersion specVersion)
        {
            //spec
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            if (!specVersion.Fk_SpecificationId.HasValue)
                throw new InvalidOperationException("Specification id is not provided");
            var spec = specMgr.GetSpecificationById(personId, specVersion.Fk_SpecificationId.Value).Key;

            if (spec == null)
                throw new InvalidOperationException("Specification is not found");

            specVersion.Specification = spec;

            //Release
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            if (!specVersion.Fk_ReleaseId.HasValue)
                throw new InvalidOperationException("Release id is not provided");
            var release = releaseMgr.GetReleaseById(0, specVersion.Fk_ReleaseId.Value).Key;

            if (release == null)
                throw new InvalidOperationException("Release is not found");

            specVersion.Release = release;
        }

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

        private Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string temporaryFolder, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
        {
            Report validationReport;

            if (fileExtension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
            {
                using (IQualityChecks qualityChecks = new DocXQualityChecks(memoryStream))
                {
                    validationReport = ValidateDocument(qualityChecks, version, title, release, meetingDate, tsgTitle, isTS);
                }
            }
            else
            {
                using (IQualityChecks qualityChecks = new DocQualityChecks(memoryStream, temporaryFolder))
                {
                    validationReport = ValidateDocument(qualityChecks, version, title, release, meetingDate, tsgTitle, isTS);
                }
            }

            return validationReport;
        }

        /// <summary>
        /// Validate Word Document
        /// </summary>
        /// <param name="qualityChecks">Quality Checks Interface</param>
        /// <param name="version">Version</param>
        /// <param name="title">Title</param>
        /// <param name="release">Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <param name="tsgTitle">Technical Specificaion Group Title</param>
        /// <param name="isTS">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>Validation Summary</returns>
        private Report ValidateDocument(IQualityChecks qualityChecks, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
        {
            Report validationReport = new Report();

            if (qualityChecks.HasTrackedRevisions())
                validationReport.LogWarning(CONST_QUALITY_CHECK_REVISIONMARK);

            if (!qualityChecks.IsHistoryVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_HISTORY);

            if (!qualityChecks.IsCoverPageVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_COVERPAGE);

            if (!qualityChecks.IsCoverPageDateCorrect(meetingDate))
                validationReport.LogWarning(CONST_QUALITY_CHECK_DATE_COVERPAGE);

            if (!qualityChecks.IsCopyRightYearCorrect())
                validationReport.LogWarning(CONST_QUALITY_CHECK_YEAR_COPYRIGHT);

            if (!qualityChecks.IsTitleCorrect(title))
                validationReport.LogWarning(CONST_QUALITY_CHECK_TITLE_COVERPAGE);

            if (!qualityChecks.IsReleaseCorrect(release))
                validationReport.LogWarning(CONST_QUALITY_CHECK_RELEASE);

            if (!qualityChecks.IsReleaseStyleCorrect(release))
                validationReport.LogWarning(CONST_QUALITY_CHECK_RELEASE_STYLE);

            if (qualityChecks.IsAutomaticNumberingPresent())
                validationReport.LogWarning(CONST_QUALITY_CHECK_AUTO_NUMBERING);

            if (!qualityChecks.IsFirstTwoLinesOfTitleCorrect(tsgTitle))
                validationReport.LogWarning(CONST_QUALITY_CHECK_FIRST_TWO_LINES_TITLE);

            if (!qualityChecks.IsAnnexureStylesCorrect(isTS))
                validationReport.LogWarning(CONST_QUALITY_CHECK_ANNEXURE_STYLE);

            return validationReport;
        }

        private void TransferToFtp(SpecVersion version, string path)
        {
            string ftpBasePath = ConfigVariables.FtpBasePhysicalPath;

            if (String.IsNullOrEmpty(ftpBasePath))
                throw new InvalidOperationException("FTP not yet configured");

            string specNumber = version.Specification.Number;
            string targetFolder = String.Format(CONST_FTP_ARCHIVE_PATH, ftpBasePath, specNumber.Split('.')[0], specNumber);
            string zipFilePath = path;

            string validFileName = GetValidFileName(version);
            string zipFileName = validFileName + ".zip";
            var versionPathToSave = Path.Combine(targetFolder, zipFileName);

            bool isTargetFolderExists = Directory.Exists(targetFolder);
            if (!isTargetFolderExists)
                Directory.CreateDirectory(targetFolder);

            //If it is not in zip format, compress & upload the same
            if (!Path.GetExtension(path).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                Zip.Compress(path, Path.GetDirectoryName(path));
                zipFilePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".zip";
            }

            File.Copy(zipFilePath, versionPathToSave);
            version.Location = versionPathToSave.Replace(ConfigVariables.FtpBasePhysicalPath, ConfigVariables.FtpBaseAddress).Replace("\\", "/");

            var spec = version.Specification;
            if (spec.IsActive && !spec.IsUnderChangeControl.GetValueOrDefault()) //Draft
            {
                string draftPath = String.Format(CONST_FTP_LATEST_DRAFTS_PATH, ftpBasePath);
                bool isDraftPathExists = Directory.Exists(draftPath);
                if (!isDraftPathExists)
                    Directory.CreateDirectory(draftPath);

                // We remove any existing draft.
                ClearLatestDraftFolder(ftpBasePath, version);


                string hardLinkPath = Path.Combine(draftPath, zipFileName);
                CreateHardLink(hardLinkPath, versionPathToSave, IntPtr.Zero);
            }
            else //Under Change Control
            {
                if (!version.Source.HasValue)
                    throw new InvalidOperationException("Meeting must be provided");

                MeetingManager meetingMgr = new MeetingManager();
                meetingMgr.UoW = UoW;

                var uploadMeeting = meetingMgr.GetMeetingById(version.Source.Value);
                if(uploadMeeting == null)
                    throw new InvalidOperationException("Meeting not found");

                if ((uploadMeeting.START_DATE != null))
                {
                    string latestFolder = ConfigVariables.VersionsLatestFTPFolder;
                    if (String.IsNullOrEmpty(latestFolder))
                        latestFolder = String.Format("{0:0000}-{1:00}", uploadMeeting.START_DATE.Value.Year, uploadMeeting.START_DATE.Value.Month);
                    string underChangeControlPath = String.Format(CONST_FTP_VERSIONS_PATH, ftpBasePath, latestFolder, version.Release.Code, spec.Number.Split('.')[0]);
                    bool isUCCPathExists = Directory.Exists(underChangeControlPath);
                    if (!isUCCPathExists)
                        Directory.CreateDirectory(underChangeControlPath);

                    string fileName = Path.GetFileName(versionPathToSave);
                    string hardLinkPath = Path.Combine(underChangeControlPath, fileName);

                    CreateHardLink(hardLinkPath, versionPathToSave, IntPtr.Zero);

                    //Delete all drafts for this version
                    ClearLatestDraftFolder(ftpBasePath, version);

                    //Create hard link in 'Latest' folder
                    string latestFolderPath = String.Format(CONST_FTP_LATEST_PATH, ftpBasePath, version.Release.Code, spec.Number.Split('.')[0]);
                    bool isLatestFolderPathExists = Directory.Exists(latestFolderPath);
                    if (!isLatestFolderPathExists)
                        Directory.CreateDirectory(latestFolderPath);
                    string hardLinkPathInLatestFolder = Path.Combine(latestFolderPath, fileName);
                    CreateHardLink(hardLinkPathInLatestFolder, hardLinkPath, IntPtr.Zero);
                }
            }
        }

        private void ClearLatestDraftFolder(string ftpBasePath, SpecVersion version)
        {
            var versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;
            var existingVersions = versionMgr.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);

            // Only clear the Latest-drafts folder if the version that has just been uploaded is greater that current one.
            if (existingVersions.Where(v => !string.IsNullOrEmpty(v.Location)
                && (v.MajorVersion > version.MajorVersion
                    || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion > version.TechnicalVersion)
                    || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion == version.TechnicalVersion && v.EditorialVersion > version.EditorialVersion))).Count() == 0)
            {
                string draftPath = String.Format(CONST_FTP_LATEST_DRAFTS_PATH, ftpBasePath);
                string collapsedSpecNumber = version.Specification.Number.Replace(".", String.Empty);

                if (Directory.Exists(draftPath))
                {
                    DirectoryInfo di = new DirectoryInfo(draftPath);
                    var draftFiles = di.GetFiles(collapsedSpecNumber + "-*").Select(x => x.FullName).ToList();
                    foreach (string draftFile in draftFiles)
                    {
                        if (File.Exists(draftFile))
                            File.Delete(draftFile);
                    }
                }
            }
        }

        /// <summary>
        /// Performs the update of the database.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="validationReport"></param>
        /// <param name="personId"></param>
        private void UpdateDatabase(SpecVersion version, Report validationReport, int personId)
        {
            //Initialization
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            ITranspositionManager transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
            transposeMgr.UoW = UoW;


            var specVersions = repo.GetVersionsForSpecRelease(version.Fk_SpecificationId ?? 0, version.Fk_ReleaseId ?? 0);
            var spec = version.Specification;
            var release = version.Release;
            var existingVersion = specVersions.Where(x => (x.MajorVersion == version.MajorVersion) &&
                                                          (x.TechnicalVersion == version.TechnicalVersion) &&
                                                          (x.EditorialVersion == version.EditorialVersion)).FirstOrDefault();


            if (validationReport != null && validationReport.WarningList.Count > 0)
            {
                String remarkText = "This version was uploaded with the following quality checks failures: " + string.Join(";", validationReport.WarningList);
                if (remarkText.Length > 250)
                    remarkText = remarkText.Substring(0, 247) + "...";

                var utcNow = DateTime.UtcNow;

                //Create a new remark for generated warnings during document validation
                Remark warningRemark = new Remark
                {
                    CreationDate = utcNow,
                    Fk_PersonId = personId,
                    RemarkText = remarkText,
                    IsPublic = false
                };

                var commentRemark = version.Remarks.FirstOrDefault();
                if (commentRemark != null)
                    commentRemark.CreationDate = utcNow.AddMilliseconds(5d);

                version.Remarks.Clear();

                // Only add the warning if version is UCC.
                if (version.Specification.IsUnderChangeControl.GetValueOrDefault())
                {
                    version.Remarks.Add(warningRemark);
                }
                if (commentRemark != null)
                    version.Remarks.Add(commentRemark);
            }

            version.DocumentUploaded = DateTime.UtcNow;
            version.ProvidedBy = personId;

            version.Specification = null;
            version.Release = null;
            if (existingVersion == null)
            {
                //Transposition of the existing version
                transposeMgr.Transpose(spec, version);
                repo.InsertOrUpdate(version);
            }
            else
            {
                existingVersion.Location = version.Location;
                existingVersion.Source = version.Source;
                existingVersion.ProvidedBy = version.ProvidedBy;
                existingVersion.DocumentUploaded = version.DocumentUploaded;
                version.Remarks.ToList().ForEach(r => existingVersion.Remarks.Add(r));
                //Transposition of the existing version
                transposeMgr.Transpose(spec, existingVersion);
            }

            if (validationReport != null && validationReport.GetNumberOfWarnings() > 0)
            {
                MailVersionAuthor(spec, version, validationReport, personId);
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

        private class CacheUploadStorage
        {
            public SpecVersion Version {get; set;}
            public string TmpUploadedFilePath {get; set;}
            public Report ValidationReport {get; set;}

            public CacheUploadStorage(SpecVersion version, string path, Report report)
            {
                this.Version = version;
                this.TmpUploadedFilePath = path;
                this.ValidationReport = report;
            }
        }

        /// <summary>
        /// When quality checks fail, and the user confirms anyway the upload of a version, an email must be sent to the user and to the spec manager
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="report"></param>
        /// <param name="personId"></param>
        public void MailVersionAuthor(Specification spec, SpecVersion version, Report report, int personId)
        {
            var to = new List<string>();

            //get connected user name
            var connectedUsername = String.Empty;
            var personManager = new PersonManager();
            personManager.UoW = UoW;

            var connectedUser = personManager.FindPerson(personId);
            if (connectedUser != null)
            {
                connectedUsername = new StringBuilder()
                    .Append((connectedUser.FIRSTNAME != null) ? connectedUser.FIRSTNAME : "")
                    .Append(" ")
                    .Append((connectedUser.LASTNAME != null) ? connectedUser.LASTNAME : "")
                    .ToString();

                to.Add(connectedUser.Email);
            }

            //Subject
            var subject = String.Format("Spec {0}, version {1} has been uploaded despite some quality checks failure", spec.Pk_SpecificationId, version.Version);

            //Body
            var body = new VersionUploadFailedQualityCheckMailTemplate(connectedUsername, spec.Number, version.Version.ToString(), report.WarningList);

            var roleManager = new RolesManager();
            roleManager.UoW = UoW;
            var cc = roleManager.GetSpecMgrEmail();

            // Send mail
            var mailManager = UtilsFactory.Resolve<IMailManager>();
            mailManager.SendEmail(null, to, cc, null, subject, body.TransformText());
        }
    }
}
