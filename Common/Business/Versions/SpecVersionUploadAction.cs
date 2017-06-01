using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.Utils.ModelMails;
using Etsi.Ultimate.Business.Specifications;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Business.Versions
{
    public class SpecVersionUploadAction
    {

        private const string CacheKey = "VERSION_UPLOAD";

        private const string ConstQualityCheckRevisionmark = "Some revision marks left unaccepted";
        private const string ConstQualityCheckVersionHistory = "Invalid/missing version in history";
        private const string ConstQualityCheckChangeHistoryIsNotTheLastTable = "Change history table is not at the end of the document";
        private const string ConstQualityCheckVersionCoverpage = "Invalid/missing version in cover page";
        private const string ConstQualityCheckDateCoverpage = "Invalid/missing date in cover page";
        private const string ConstQualityCheckYearCopyright = "Year not valid/missing in copyright statement";
        private const string ConstQualityCheckTitleCoverpage = "Incorrect/missing title in cover page";
        private const string ConstQualityCheckReleaseStyle = "Release style should be 'ZGSM' in cover page";
        private const string ConstQualityCheckAutoNumbering = "Automatic numbering (of clauses, figures, tables, notes, examples etcâ€¦) should be disabled in the document";
        private const string ConstQualityCheckFirstTwoLinesTitle = "The first two lines of the title must be correct, according to the TSG responsible for the specification";
        private const string ConstQualityCheckAnnexureStyle = "Annexes should be correctly styled as Heading 8(TS) or Heading 9(TR). In case of TS, (normative) or (informative) should appear immediately after annexure heading";
        private const string ConstQualityCheckRelease = "Invalid/missing release in cover page";

        public IUltimateUnitOfWork UoW;


        /// <summary>
        /// Entry point for checking the upload of a version. Will check that the version can be parsed, 
        /// and in this case launch the quality checks of the test.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <param name="path"></param>
        /// <param name="shouldAvoidQualityChecks"></param>
        /// <returns></returns>
        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path, bool shouldAvoidQualityChecks)
        {
            var svcResponse = new ServiceResponse<string>();

            GetRelatedSpecAndRelease(personId, version);

            if (svcResponse.Report.GetNumberOfErrors() == 0)
            {
                try
                {
                    CheckPersonRightToUploadVersion(version, personId);
                    CheckUploadAllowed(version, path, personId);

                    var validationReport = svcResponse.Report;
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    var uploadedFileExtension = Path.GetExtension(path);
                    var fileExtension = String.Empty;
                    var fileToAnalyzePath = String.Empty;
                    var isUcc = version.Specification.IsUnderChangeControl.GetValueOrDefault();

                    var specVersionUploadMgr = ManagerFactory.Resolve<ISpecVersionUploadManager>();
                    var uniquePath = specVersionUploadMgr.GetUniquePath(path);

                    //[1] Check the file name
                    var versionFilenameMgr = ManagerFactory.Resolve<IVersionFilenameManager>();
                    var validFileName = versionFilenameMgr.GenerateValidFilename(version.Specification.Number,
                        version.MajorVersion, version.TechnicalVersion, version.EditorialVersion);

                    if (!fileNameWithoutExtension.Equals(validFileName, StringComparison.InvariantCultureIgnoreCase))
                        validationReport.LogWarning(String.Format(Localization.Version_Upload_InvalidFilename,
                            validFileName));

                    var allowToRunQualityChecks = false;
                    //[2] If file is in .zip format, check that .zip file and internal word file must have same name.
                    if (uploadedFileExtension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var zipContent = Zip.Extract(uniquePath, true);
                        var zipContentShortPath = new List<string>();
                        zipContent.ForEach(x => zipContentShortPath.Add(x.Split('\\').Last()));


                        //1 - File match found
                        if (
                            zipContentShortPath.Find(
                                x =>
                                    x.Equals(fileNameWithoutExtension + ".doc",
                                        StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".doc";
                            fileToAnalyzePath = Path.GetDirectoryName(zipContent.First()) + "\\" +
                                                fileNameWithoutExtension + ".doc";
                        }
                        else if (
                            zipContentShortPath.Find(
                                x =>
                                    x.Equals(fileNameWithoutExtension + ".docx",
                                        StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            allowToRunQualityChecks = true;
                            fileExtension = ".docx";
                            fileToAnalyzePath = Path.GetDirectoryName(zipContent.First()) + "\\" +
                                                fileNameWithoutExtension + ".docx";
                        }
                        else
                        {
                            if (zipContent.Count == 1 && (zipContent.First().ToLower().EndsWith(".doc")))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".doc";
                                fileToAnalyzePath = zipContent.First();
                                validationReport.LogWarning(Localization.Version_Upload_ZipAndWordShouldHaveSameName);
                            }
                            else if (zipContent.Count == 1 && zipContent.First().ToLower().EndsWith(".docx"))
                            {
                                allowToRunQualityChecks = true;
                                fileExtension = ".docx";
                                fileToAnalyzePath = zipContent.First();
                                validationReport.LogWarning(Localization.Version_Upload_ZipAndWordShouldHaveSameName);
                            }
                            else
                            {
                                if (isUcc)
                                    validationReport.LogWarning(Localization.Version_Upload_NoWordInsideZip_ForUcc);
                            }
                        }
                    }
                    else //Copy to stream from .doc / .docx
                    {
                        fileExtension = uploadedFileExtension;
                        if (fileExtension == null ||
                            (!fileExtension.Equals(".doc", StringComparison.InvariantCultureIgnoreCase) &&
                             !fileExtension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase)))
                            throw new InvalidOperationException("Invalid file format provided");
                        fileToAnalyzePath = uniquePath;
                        allowToRunQualityChecks = true;
                    }

                    //Finally system should allow quality checks if shouldAvoidQualityChecks is false
                    allowToRunQualityChecks = allowToRunQualityChecks && !shouldAvoidQualityChecks;

                    //If we have valid file & spec is under change control, run quality checks
                    if (allowToRunQualityChecks && isUcc)
                    {
                        var meetingDate = DateTime.MinValue;
                        var tsgTitle = String.Empty;

                        var communityMgr = ManagerFactory.Resolve<ICommunityManager>();
                        if (version.Specification.PrimeResponsibleGroup != null)
                        {
                            var communities = communityMgr.GetCommunities();
                            var community =
                                communities.FirstOrDefault(
                                    x => x.TbId == version.Specification.PrimeResponsibleGroup.Fk_commityId);
                            if (community != null)
                                tsgTitle =
                                    GetParentTSG(community, communities)
                                        .TbTitle.Replace("Technical Specification Group -",
                                            "Technical Specification Group");
                        }
                        var versionStr = String.Format("{0}.{1}.{2}", version.MajorVersion.Value,
                            version.TechnicalVersion.Value, version.EditorialVersion.Value);

                        if (version.Source.HasValue)
                        {
                            var meetingMgr = new MeetingManager {UoW = UoW};

                            meetingDate = meetingMgr.GetMeetingById(version.Source.Value).END_DATE.Value;
                        }

                        using (var docDocumentManager =
                            ManagerFactory.ResolveWithString<IDocDocumentManager>(fileToAnalyzePath))
                        {
                            var qualityChecks =
                                ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentManager);

                            var businessValidationReport = ValidateDocument(qualityChecks, versionStr,
                                version.Specification.Title, version.Release.Name, meetingDate, tsgTitle,
                                version.Specification.IsTS.GetValueOrDefault());
                            validationReport.ErrorList.AddRange(businessValidationReport.ErrorList);
                            validationReport.WarningList.AddRange(businessValidationReport.WarningList);
                        }
                    }

                    var archiveFilePath = specVersionUploadMgr.ArchiveUploadedVersion(path, uniquePath);

                    svcResponse.Result = Guid.NewGuid().ToString();
                    CacheManager.InsertForLimitedTime(CacheKey + svcResponse.Result,
                        new CacheUploadStorage(version, archiveFilePath, validationReport), 10);
                }
                catch (InvalidOperationException exc)
                {
                    LogManager.Error("An InvalidOperationException error occured when system trying to CheckVersionForUpload", exc);
                    svcResponse.Report.LogError("An error occured: " + exc.Message);
                }
                catch (Exception e)
                {
                    LogManager.Error("An unexpected error occured when system trying to CheckVersionForUpload", e);
                    svcResponse.Report.LogError(Localization.GenericError);
                }
            }
            return svcResponse;
        }

        /// <summary>
        /// Does perform the upload of the version, given the token that was passed by CheckVersionForUpload function.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ServiceResponse<string> UploadVersion(int personId, string token)
        {
            var svcResponse = new ServiceResponse<string>();
            try
            {
                var versionInfos = (CacheUploadStorage)CacheManager.Get(CacheKey + token);
                if (versionInfos == null)
                    throw new InvalidOperationException("An error occured during file retrieval. Please try again.");

                ExtensionLogger.Info("UPLOAD VERSION: System is trying to upload version...", new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("personId", personId),
                    new KeyValuePair<string, object>("token", token),
                    new KeyValuePair<string, object>("version", versionInfos.Version),
                    new KeyValuePair<string, object>("report", versionInfos.ValidationReport),
                    new KeyValuePair<string, object>("TempFilePath", versionInfos.TmpUploadedFilePath)
                });

                GetRelatedSpecAndRelease(personId, versionInfos.Version);
                CheckPersonRightToUploadVersion(versionInfos.Version, personId);

                TransferToFtp(versionInfos.Version, versionInfos.TmpUploadedFilePath);

                UpdateDatabase(versionInfos.Version, versionInfos.ValidationReport, personId);
                LogManager.Info("UPLOAD VERSION: System successfully upload version for user: " + personId);
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, token }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                svcResponse.Report.LogError("An error occured while uploading: " + e.Message);
            }
            return svcResponse;
        }

        private void CheckPersonRightToUploadVersion(SpecVersion version, int personId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);
            var specReleaseRights = specMgr.GetRightsForSpecRelease(rights, personId, version.Specification, version.Fk_ReleaseId.Value, new List<Release>() { version.Release });

            if (!specReleaseRights.Value.HasRight(Enum_UserRights.Versions_Upload))
            {
                throw new InvalidOperationException(Localization.RightError);
            }
        }

        /// <summary>
        /// Checks that user has right to perform the upload, in so far as:
        /// - Version is not already uploaded
        /// - if version is a draft, it's major version is less or equal 2.
        /// Notes : version could be lower or greater than existing ones, already allocated or not, but should not be already uploaded
        /// </summary>
        /// <param name="version"></param>
        /// <param name="path"></param>
        /// <param name="personId"></param>
        private void CheckUploadAllowed(SpecVersion version, string path, int personId)
        {
            // Version should not already exist
            var versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;

            if ((version.Specification.IsUnderChangeControl.GetValueOrDefault()) && (version.Source == null || version.Source <= 0))
                throw new InvalidOperationException(Localization.Upload_Version_Error_Meeting_Id_Not_Provided);

            //Check if version already uploaded
            var versionsForSpecRelease = versionMgr.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);
            if (versionsForSpecRelease.Any(v => v.MajorVersion == version.MajorVersion 
                                                && v.TechnicalVersion == version.TechnicalVersion 
                                                && v.EditorialVersion == version.EditorialVersion
                                                && !string.IsNullOrEmpty(v.Location)))
            {
                var versionStr = version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion;
                throw new InvalidOperationException(String.Format(Localization.Upload_Version_Error_Version_Already_Exists, versionStr));
            }

            //Validate version number
            var specVersionNumberValidator = ManagerFactory.Resolve<ISpecVersionNumberValidator>();
            specVersionNumberValidator.UoW = UoW;
            var numberValidationResponse = specVersionNumberValidator.CheckSpecVersionNumber(null, version,
                SpecNumberValidatorMode.Upload, personId);
            if (!numberValidationResponse.Result || numberValidationResponse.Report.ErrorList.Any())
                throw new InvalidOperationException(string.Join(", ", numberValidationResponse.Report.ErrorList));
        }

        private void GetRelatedSpecAndRelease(int personId, SpecVersion specVersion)
        {
            //spec
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            if (!specVersion.Fk_SpecificationId.HasValue)
                throw new InvalidOperationException("Specification id is not provided");
            var spec = specMgr.GetSpecificationById(personId, specVersion.Fk_SpecificationId.Value).Key;

            if (spec == null)
                throw new InvalidOperationException("Specification is not found");

            specVersion.Specification = spec;

            //Release
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
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
                var parentCommunity = communities.FirstOrDefault(x => x.TbId == community.ParentCommunityId);
                if (parentCommunity != null)
                    return GetParentTSG(parentCommunity, communities);
                else
                    return community;
            }
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
        /// <param name="isTs">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>Validation Summary</returns>
        private Report ValidateDocument(IQualityChecks qualityChecks, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTs)
        {
            ExtensionLogger.Info("QUALITY CHECKS: are running on provided document with following arguments: ", new List<KeyValuePair<string, object>> { 
                new KeyValuePair<string, object>("version" , version),
                new KeyValuePair<string, object>("title" , title),
                new KeyValuePair<string, object>("release" , release),
                new KeyValuePair<string, object>("meetingDate" , meetingDate),
                new KeyValuePair<string, object>("tsgTitle" , tsgTitle),
                new KeyValuePair<string, object>("isTs" , isTs)
            });

            var validationReport = new Report();

            if (qualityChecks.HasTrackedRevisions())
                validationReport.LogWarning(ConstQualityCheckRevisionmark);

            var changeHistoryIsNotTheLastOne = !qualityChecks.ChangeHistoryTableIsTheLastOne();
            if (changeHistoryIsNotTheLastOne)
            {
                validationReport.LogWarning(ConstQualityCheckChangeHistoryIsNotTheLastTable);
            }
            else
            {
                if (!qualityChecks.IsHistoryVersionCorrect(version))
                    validationReport.LogWarning(ConstQualityCheckVersionHistory);
            }
            
            if (!qualityChecks.IsCoverPageVersionCorrect(version))
                validationReport.LogWarning(ConstQualityCheckVersionCoverpage);

            if (!qualityChecks.IsCoverPageDateCorrect(meetingDate))
                validationReport.LogWarning(ConstQualityCheckDateCoverpage);

            if (!qualityChecks.IsCopyRightYearCorrect(DateTime.UtcNow))
                validationReport.LogWarning(ConstQualityCheckYearCopyright);

            if (!qualityChecks.IsTitleCorrect(title))
                validationReport.LogWarning(ConstQualityCheckTitleCoverpage);

            if (!qualityChecks.IsReleaseCorrect(release))
                validationReport.LogWarning(ConstQualityCheckRelease);

            if (!qualityChecks.IsReleaseStyleCorrect(release))
                validationReport.LogWarning(ConstQualityCheckReleaseStyle);

            if (qualityChecks.IsAutomaticNumberingPresent())
                validationReport.LogWarning(ConstQualityCheckAutoNumbering);

            if (!qualityChecks.IsFirstTwoLinesOfTitleCorrect(tsgTitle))
                validationReport.LogWarning(ConstQualityCheckFirstTwoLinesTitle);

            if (!qualityChecks.IsAnnexureStylesCorrect(isTs))
                validationReport.LogWarning(ConstQualityCheckAnnexureStyle);

            return validationReport;
        }

        /// <summary>
        /// Transfers to FTP.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="path">The path.</param>
        private void TransferToFtp(SpecVersion version, string path)
        {
            var specVersionPathMgr = UtilsFactory.Resolve<ISpecVersionPathHelper>();
            var ftpBasePath = ConfigVariables.FtpBasePhysicalPath;

            //Check validations
            if (String.IsNullOrEmpty(ftpBasePath))
                throw new InvalidOperationException("FTP not yet configured");

            var spec = version.Specification;
            Meeting uploadMeeting = null;
            if (spec.IsActive && version.MajorVersion >= 3)
            {
                if (!version.Source.HasValue)
                    throw new InvalidOperationException("Meeting must be provided");

                var meetingMgr = new MeetingManager {UoW = UoW};

                uploadMeeting = meetingMgr.GetMeetingById(version.Source.Value);
                if (uploadMeeting == null)
                    throw new InvalidOperationException("Meeting not found");            
            }

            var zipFilePath = path;
            //If it is not in zip format, compress & upload the same
            if (!Path.GetExtension(path).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                Zip.Compress(path, Path.GetDirectoryName(path));
                zipFilePath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".zip";
            }

            var versionFilenameMgr = ManagerFactory.Resolve<IVersionFilenameManager>();
            var validFileName = versionFilenameMgr.GenerateValidFilename(version.Specification.Number,
                        version.MajorVersion, version.TechnicalVersion, version.EditorialVersion);
            var zipFileName = validFileName + ".zip";

            var specNumber = version.Specification.Number;
            var targetFolder = String.Format(specVersionPathMgr.GetFtpArchivePath, ftpBasePath, specNumber.Split('.')[0], specNumber);
            var versionPathToSave = Path.Combine(targetFolder, zipFileName);
            version.Location = versionPathToSave.Replace(ConfigVariables.FtpBasePhysicalPath, ConfigVariables.FtpBaseAddress).Replace("\\", "/");

            //Transfer to Actual Ftp
            TransferToFtp(zipFilePath, ftpBasePath, zipFileName, uploadMeeting, version);

            //Transfer to Backup Ftp
            var ftpBackupBasePath = ConfigVariables.FtpBaseBackupPhysicalPath;
            if (!String.IsNullOrEmpty(ftpBackupBasePath) && Directory.Exists(ftpBackupBasePath))
                TransferToFtp(zipFilePath, ftpBackupBasePath, zipFileName, uploadMeeting, version);
        }

        /// <summary>
        /// Transfers to FTP.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="destinationBasePath">The destination base path.</param>
        /// <param name="destinationFileName">Name of the destination file.</param>
        /// <param name="uploadMeeting">The upload meeting.</param>
        /// <param name="version">The version.</param>
        private void TransferToFtp(string sourceFile, string destinationBasePath, string destinationFileName, Meeting uploadMeeting, SpecVersion version)
        {
            var specVersionPathMgr = UtilsFactory.Resolve<ISpecVersionPathHelper>();

            LogManager.Debug(string.Format("UPLOAD VERSION:   START - Transfer to FTP of version (Spec: {0}, Rel: {1}, Number: {2}.{3}.{4}) begin...", version.Fk_SpecificationId, version.Fk_ReleaseId, version.MajorVersion, version.TechnicalVersion, version.EditorialVersion));
            var specNumber = version.Specification.Number;
            var destinationFolder = String.Format(specVersionPathMgr.GetFtpArchivePath, destinationBasePath, specNumber.Split('.')[0], specNumber);
            var destinationFile = Path.Combine(destinationFolder, destinationFileName);

            var isTargetFolderExists = Directory.Exists(destinationFolder);
            if (!isTargetFolderExists)
                Directory.CreateDirectory(destinationFolder);

            File.Copy(sourceFile, destinationFile, true);
            LogManager.Debug("UPLOAD VERSION:   (DURATION)Version just copy/paste at: " + destinationFile);

            var spec = version.Specification;
            if (spec.IsActive && version.MajorVersion < 3) //Draft
            {
                LogManager.Debug("UPLOAD VERSION:   Version considered as DRAFT");
                var draftPath = String.Format(specVersionPathMgr.GetFtpLatestDraftsPath, destinationBasePath);
                var isDraftPathExists = Directory.Exists(draftPath);
                if (!isDraftPathExists)
                    Directory.CreateDirectory(draftPath);

                // Define if current version is the latest-draft one. If yes, system need to clean folder latest-drafts and add current version
                var currentVersionIsLatestDraftVersionUploaded = ClearLatestDraftFolder(destinationBasePath, version);
                if (currentVersionIsLatestDraftVersionUploaded)
                {
                    var hardLinkPath = Path.Combine(draftPath, destinationFileName);
                    CreateHardLink(hardLinkPath, destinationFile, IntPtr.Zero);
                    LogManager.Debug("UPLOAD VERSION:   Version just copy/paste at: " + hardLinkPath);
                }
                else
                {
                    LogManager.Debug("UPLOAD VERSION:   Version was not last draft version for this spec release. No copy/paste occured.");
                }
            }
            else //Under Change Control
            {
                LogManager.Debug("UPLOAD VERSION:   Version considered as UCC");
                if ((uploadMeeting.START_DATE != null))
                {
                    var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
                    ftpFoldersManager.UoW = UoW;
                    var latestFolder = ftpFoldersManager.GetFTPLatestFolderName();

                    if (String.IsNullOrEmpty(latestFolder))
                    {
                        latestFolder = String.Format("{0:0000}-{1:00}", uploadMeeting.START_DATE.Value.Year, uploadMeeting.START_DATE.Value.Month);
                        LogManager.Debug("UPLOAD VERSION:   Latest folder not found in db. New latest folder will be created: " + latestFolder);
                    }

                    var underChangeControlPath = String.Format(specVersionPathMgr.GetFtpCustomPath, destinationBasePath, latestFolder, version.Release.Code, spec.Number.Split('.')[0]);
                    var isUccPathExists = Directory.Exists(underChangeControlPath);
                    if (!isUccPathExists)
                        Directory.CreateDirectory(underChangeControlPath);

                    var fileName = Path.GetFileName(destinationFile);
                    var hardLinkPath = Path.Combine(underChangeControlPath, fileName);

                    CreateHardLink(hardLinkPath, destinationFile, IntPtr.Zero);
                    LogManager.Debug("UPLOAD VERSION:   Version just copy/paste at: " + hardLinkPath);

                    //Delete all drafts for this version if current version is the latest version uploaded (with higher version number)
                    ClearLatestDraftFolder(destinationBasePath, version);

                    // Define if current version is the latest one. If yes, system need to clean folder latest and add current version
                    var latestFolderPath = String.Format(specVersionPathMgr.GetFtpLatestPath, destinationBasePath, version.Release.Code, spec.Number.Split('.')[0]);
                    var currentVersionIsTheLatestVersionUploaded = true;
                    if (!Directory.Exists(latestFolderPath))
                        Directory.CreateDirectory(latestFolderPath);
                    else
                    {
                        currentVersionIsTheLatestVersionUploaded = ClearLatestFolder(latestFolderPath, version);
                    }

                    if (currentVersionIsTheLatestVersionUploaded)
                    {
                        var hardLinkPathInLatestFolder = Path.Combine(latestFolderPath, fileName);
                        CreateHardLink(hardLinkPathInLatestFolder, hardLinkPath, IntPtr.Zero);
                        LogManager.Debug("UPLOAD VERSION:   Version just copy/paste at: " + hardLinkPathInLatestFolder);
                    }
                    
                    //Some versions need to be move inside a (configured <=> web.config setting) folder in order to be transposed
                    var isInternalTr = !(spec.IsForPublication ?? false) && !(spec.IsTS ?? false);
                    var releaseStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
                    releaseStatusRepo.UoW = UoW;
                    var releaseStatus = releaseStatusRepo.Find(version.Release.Fk_ReleaseStatus ?? 0);
                    var isInsideStage3Frozen = (releaseStatus.Code == Enum_ReleaseStatus.Frozen) 
                        && version.Release.Stage3FreezeDate < DateTime.Now;

                    if (spec.IsActive 
                        && (spec.IsUnderChangeControl ?? false) //SPEC UCC
                        && !isInternalTr //SPEC IS NOT INTERNAL TR
                        && isInsideStage3Frozen) // Release belongs to Stage 3 frozen
                    {
                        var versionsToBeTransposedFolder = string.Format(ConfigVariables.RelativeFtpPathForVersionsToBeTransposed,
                            destinationBasePath);
                        if (!Directory.Exists(versionsToBeTransposedFolder))
                            Directory.CreateDirectory(versionsToBeTransposedFolder);

                        var versionToBeTransposedCopyPath = Path.Combine(versionsToBeTransposedFolder, fileName);
                        if (!File.Exists(versionToBeTransposedCopyPath))
                        {
                            File.Copy(hardLinkPath, versionToBeTransposedCopyPath);
                        }
                        LogManager.Debug("UPLOAD VERSION:   Version 'to be transposed' just copy/paste at: " + versionToBeTransposedCopyPath);
                    }
                }
            }
            LogManager.Debug(string.Format("UPLOAD VERSION:   Transfer to FTP of version (Spec: {0}, Rel: {1}, Number: {2}.{3}.{4}) is now done.", version.Fk_SpecificationId, version.Fk_ReleaseId, version.MajorVersion, version.TechnicalVersion, version.EditorialVersion));
        }

        /// <summary>
        /// Clears the latest draft folder & detect if current version is the latest one
        /// </summary>
        /// <param name="ftpBasePath">The FTP base path.</param>
        /// <param name="version">The version.</param>
        /// <returns>TRUE - Latest version is the current one/ FALSE - latest version already uploaded</returns>
        private bool ClearLatestDraftFolder(string ftpBasePath, SpecVersion version)
        {
            var currentVersionIsLatestDraftVersionUploaded = false;

            var specVersionPathMgr = UtilsFactory.Resolve<ISpecVersionPathHelper>();
            var versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;
            var existingVersions = versionMgr.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);

            // Only clear the Latest-drafts folder if the version that has just been uploaded is greater that current one.
            if (!existingVersions.Any(v => !string.IsNullOrEmpty(v.Location)
                    && (v.MajorVersion > version.MajorVersion
                        || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion > version.TechnicalVersion)
                        || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion == version.TechnicalVersion && v.EditorialVersion > version.EditorialVersion))))
            {
                string draftPath = String.Format(specVersionPathMgr.GetFtpLatestDraftsPath, ftpBasePath);
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
                currentVersionIsLatestDraftVersionUploaded = true;
            }

            LogManager.Debug(currentVersionIsLatestDraftVersionUploaded ? "Latest-draft folder cleaned because current version is latest version" : "Current version is not the latest uploaded");
            return currentVersionIsLatestDraftVersionUploaded;
        }

        /// <summary>
        /// Clear the latest folder & detect if current version is the latest one
        /// </summary>
        /// <param name="versionLatestFolderPath">Latest folder path for current version. Ex: ftp.com\\Specs\\latest\\Rel12\\22_series\\</param>
        /// <param name="version">The version.</param>
        /// <returns>TRUE - Latest version is the current one/ FALSE - latest version already uploaded</returns>
        private bool ClearLatestFolder(string versionLatestFolderPath, SpecVersion version)
        {
            var currentVersionIsLatestVersionUploaded = false;

            var versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;
            var existingVersions = versionMgr.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);

            // Only clear the Latest folder if the version that has just been uploaded is greater that current one.
            if (!existingVersions.Any(v => !string.IsNullOrEmpty(v.Location)
                    && (v.MajorVersion > version.MajorVersion
                        || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion > version.TechnicalVersion)
                        || (v.MajorVersion == version.MajorVersion && v.TechnicalVersion == version.TechnicalVersion && v.EditorialVersion > version.EditorialVersion))))
            {
                string collapsedSpecNumber = version.Specification.Number.Replace(".", String.Empty);

                if (Directory.Exists(versionLatestFolderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(versionLatestFolderPath);
                    var versionFiles = di.GetFiles(collapsedSpecNumber + "-*").Select(x => x.FullName).ToList();
                    foreach (string versionFile in versionFiles)
                    {
                        if (File.Exists(versionFile))
                            File.Delete(versionFile);
                    }
                }
                currentVersionIsLatestVersionUploaded = true;
            }

            LogManager.Debug(currentVersionIsLatestVersionUploaded ? "UPLOAD VERSION:   Latest folder cleaned because current version is latest version" : "UPLOAD VERSION:   Current version is not the latest uploaded");
            return currentVersionIsLatestVersionUploaded;
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
            try
            {
                var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                repo.UoW = UoW;
                var transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
                transposeMgr.UoW = UoW;

                var specVersions = repo.GetVersionsForSpecRelease(version.Fk_SpecificationId ?? 0, version.Fk_ReleaseId ?? 0);
                var spec = version.Specification;
                var existingVersion = specVersions.FirstOrDefault(x => (x.MajorVersion == version.MajorVersion) &&
                                                                       (x.TechnicalVersion == version.TechnicalVersion) &&
                                                                       (x.EditorialVersion == version.EditorialVersion));

                if (validationReport != null && validationReport.WarningList.Count > 0)
                {
                    var remarkText = "This version was uploaded with the following quality checks failures: " + string.Join(";", validationReport.WarningList);
                    if (remarkText.Length > 250)
                        remarkText = remarkText.Substring(0, 247) + "...";

                    var utcNow = DateTime.UtcNow;

                    //Create a new remark for generated warnings during document validation
                    var warningRemark = new Remark
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

                //Change spec to UCC when a version is uploaded with a major version number greater than 2
                if (version.MajorVersion > 2
                    && (!version.Specification.IsUnderChangeControl.HasValue || !version.Specification.IsUnderChangeControl.Value))
                {
                    LogManager.Debug("UPLOAD VERSION:   Related spec of version transfored as UCC because major version greater than 2");
                    var specChangeToUccAction = new SpecificationChangeToUnderChangeControlAction { UoW = UoW };
                    var responseUcc = specChangeToUccAction.ChangeSpecificationsStatusToUnderChangeControl(personId, new List<int> { version.Fk_SpecificationId ?? 0 });

                    if (!responseUcc.Result && responseUcc.Report.ErrorList.Count > 0)
                    {
                        validationReport.ErrorList.AddRange(responseUcc.Report.ErrorList.ToArray());
                        LogManager.Error("UPLOAD VERSION:   Error occured when system trying to ChangeSpecificationsStatusToUnderChangeControl: " + string.Join(", ", responseUcc.Report.ErrorList));
                    }
                }

                if (validationReport != null && validationReport.GetNumberOfWarnings() > 0)
                {
                    MailVersionAuthor(spec, version, validationReport, personId);
                }
            }
            catch (Exception e)
            {
                validationReport.ErrorList.Add("A error occured while updating data during version upload");
                LogManager.Error("UPLOAD VERSION:   An error occured while updating database during uploading version", e);
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
            public SpecVersion Version { get; set; }
            public string TmpUploadedFilePath { get; set; }
            public Report ValidationReport { get; set; }

            public CacheUploadStorage(SpecVersion version, string path, Report report)
            {
                Version = version;
                TmpUploadedFilePath = path;
                ValidationReport = report;
            }
        }

        /// <summary>
        /// When quality checks fail, and the user confirms anyway the upload of a version, an email must be sent to the user and to the spec manager
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="version"></param>
        /// <param name="report"></param>
        /// <param name="personId"></param>
        private void MailVersionAuthor(Specification spec, SpecVersion version, Report report, int personId)
        {
            var to = new List<string>();

            //get connected user name
            var connectedUsername = String.Empty;
            var personManager = new PersonManager {UoW = UoW};

            var connectedUser = personManager.FindPerson(personId);
            if (connectedUser != null)
            {
                connectedUsername = new StringBuilder()
                    .Append(connectedUser.FIRSTNAME ?? "")
                    .Append(" ")
                    .Append(connectedUser.LASTNAME ?? "")
                    .ToString();

                to.Add(connectedUser.Email);
            }

            //Subject
            var subject = String.Format(Localization.Version_Upload_Mail_Warnings, spec.Number, version.Version);

            //Body
            var body = new VersionUploadFailedQualityCheckMailTemplate(connectedUsername, spec.Number, version.Version, report.WarningList);

            var roleManager = new RolesManager {UoW = UoW};
            var cc = roleManager.GetSpecMgrEmail();

            // Send mail
            var mailManager = UtilsFactory.Resolve<IMailManager>();
            mailManager.SendEmail(null, to, cc, null, subject, body.TransformText());
        }
    }
}