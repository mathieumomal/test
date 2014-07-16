using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.ModelMails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecVersionsManager : ISpecVersionManager
    {
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

        public IUltimateUnitOfWork UoW { get; set; }

        public SpecVersionsManager(){ }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        public List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId)
        {
            List<SpecVersion> result = new List<SpecVersion>();
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            result = repo.GetVersionsForSpecRelease(specificationId, releaseId);
            return result;
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            ////New version
            //if (versionId == -1)
            //    return new KeyValuePair<SpecVersion, UserRightsContainer>(new SpecVersion { MajorVersion = -1, TechnicalVersion = -1, EditorialVersion = -1 }, null);

            SpecVersion version = repo.Find(versionId);
            if (version == null)
                return new KeyValuePair<SpecVersion, UserRightsContainer>(null, null);

            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            // Get information about the releases, in particular the status.
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            var specificationManager = new SpecificationManager();
            specificationManager.UoW = UoW;
            //Get calculated rights
            KeyValuePair<Specification_Release, UserRightsContainer> specRelease_Rights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases);

            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specRelease_Rights.Value);
        }

        public Report UploadOrAllocateVersion(SpecVersion version, bool isDraft, int personId, Report report = null)
        {
            //Initialization
            Report result = new Report();
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;
            ITranspositionManager transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
            transposeMgr.UoW = UoW;

            var specVersions = repo.GetVersionsForSpecRelease(version.Fk_SpecificationId ?? 0, version.Fk_ReleaseId ?? 0);
            var specRelease = specMgr.GetSpecReleaseBySpecIdAndReleaseId(version.Fk_SpecificationId ?? 0, version.Fk_ReleaseId ?? 0);
            var spec = specMgr.GetSpecificationById(personId, version.Fk_SpecificationId ?? 0).Key;
            var release = releaseMgr.GetReleaseById(personId, version.Fk_ReleaseId ?? 0).Key;
            var existingVersion = specVersions.Where(x => (x.MajorVersion == version.MajorVersion) &&
                                                          (x.TechnicalVersion == version.TechnicalVersion) &&
                                                          (x.EditorialVersion == version.EditorialVersion)).FirstOrDefault();

            //Check if spec and release exist
            if (spec == null)
                throw new InvalidDataException("Version's specId is not defined.");
            if (release == null)
                throw new InvalidDataException("Version's releaseId is not defined.");
            if (specRelease == null)
                throw new InvalidDataException("Relation between specification and release not defined.");

            if (report != null && report.WarningList.Count > 0)
            {
                var personManager = new PersonManager();
                personManager.UoW = UoW;
                string personDisplayName = personManager.GetPersonDisplayName(personId);

                String remrkText = "This version was uploaded with the following quality checks failures: " + string.Join(";", report.WarningList);
                if (remrkText.Length > 250)
                    remrkText = remrkText.Substring(0, 247) + "...";

                var utcNow = DateTime.UtcNow;

                //Create a new remark for generated warnings during document validation
                Remark warningRemark = new Remark
                    {
                        CreationDate = utcNow,
                        Fk_PersonId = personId,
                        PersonName = personDisplayName,
                        RemarkText = remrkText,
                        IsPublic = false
                    };

                var commentRemark = version.Remarks.FirstOrDefault();
                if (commentRemark != null)
                    commentRemark.CreationDate = utcNow.AddMilliseconds(5d);

                //Add above remark to appropriate version object 
                if (existingVersion != null)
                    existingVersion.Remarks.Add(warningRemark);
                else
                {
                    version.Remarks.Clear();
                    version.Remarks.Add(warningRemark);
                    if (commentRemark != null)
                        version.Remarks.Add(commentRemark);
                }
            }


            if (existingVersion != null) //Existing Version
            {
                if (existingVersion.Location == null && version.Location != null)
                {
                    if (existingVersion.Source != version.Source)
                        existingVersion.Source = version.Source;
                    if (existingVersion.DocumentUploaded != version.DocumentUploaded)
                        existingVersion.DocumentUploaded = version.DocumentUploaded;
                    if (existingVersion.ProvidedBy != version.ProvidedBy)
                        existingVersion.ProvidedBy = version.ProvidedBy;
                    if (existingVersion.Location != version.Location)
                        existingVersion.Location = version.Location;

                    var newRemark = version.Remarks.FirstOrDefault();
                    if (newRemark != null)
                        existingVersion.Remarks.Add(newRemark);

                    //Transposition of the existing version
                    transposeMgr.Transpose(spec, existingVersion);
                }
                else if (existingVersion.Location != null)
                {
                    result.LogError(String.Format("Document has already been uploaded to this version"));
                }
            }
            else
            {
                SpecVersion latestSpecVersion;
                if (isDraft)
                {
                    var specVersionsForAllReleases = repo.GetVersionsBySpecId(version.Fk_SpecificationId ?? 0);
                    latestSpecVersion = specVersionsForAllReleases.OrderByDescending(x => x.MajorVersion ?? 0)
                                                                                    .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                                                                    .ThenByDescending(z => z.EditorialVersion ?? 0)
                                                                                    .FirstOrDefault();
                }
                else
                {
                    var specVersionsForRelease = repo.GetVersionsForSpecRelease(version.Fk_SpecificationId ?? 0, version.Fk_ReleaseId ?? 0);
                    latestSpecVersion = specVersionsForRelease.OrderByDescending(x => x.MajorVersion ?? 0)
                                                                                .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                                                                .ThenByDescending(z => z.EditorialVersion ?? 0)
                                                                                .FirstOrDefault();
                }

                if (latestSpecVersion != null)
                {
                    int latestVersionNumber = int.Parse(latestSpecVersion.Version.Replace(".", ""));
                    int newVersionNumber = int.Parse(version.Version.Replace(".", ""));

                    if (isDraft)
                    {
                        if (newVersionNumber > latestVersionNumber && version.MajorVersion <= 2)
                            repo.InsertOrUpdate(version);
                        else
                            result.LogError(String.Format("Invalid draft version number!"));
                    }
                    else
                    {
                        if (newVersionNumber > latestVersionNumber)
                            repo.InsertOrUpdate(version); //New Version
                        else
                            result.LogError(String.Format("Invalid version number. Version number should be grater than {0}", latestSpecVersion.Version));
                    }
                }
                else
                {
                    if (version.MajorVersion > 2 && isDraft)
                        result.LogError(String.Format("Invalid draft version number!"));
                    else
                        repo.InsertOrUpdate(version);
                }

                //Transposition of the new version
                if (result.ErrorList.Count == 0)
                    transposeMgr.Transpose(spec, version);
            }

            if (report != null && report.WarningList.Count > 0 && result.ErrorList.Count == 0)
            {
                SpecVersion ver = existingVersion != null ? existingVersion : version;
                MailVersionAuthor(spec, ver, report, personId);
            }

            return result;
        }

        public List<Report> AllocateVersionFromMassivePromote(List<Specification> specifications, Release release, int personId)
        {
            List<Report> reports = new List<Report>();
            Report r;
            foreach (Specification s in specifications)
            {
                r = UploadOrAllocateVersion(new SpecVersion()
                {
                    Fk_SpecificationId = s.Pk_SpecificationId,
                    Fk_ReleaseId = release.Pk_ReleaseId,
                    EditorialVersion = 0,
                    TechnicalVersion = 0,
                    MajorVersion = release.Version3g

                }, ((s.IsActive) && !(s.IsUnderChangeControl.HasValue && s.IsUnderChangeControl.Value)), personId);

                reports.Add(r);
            }
            return reports;
        }

        public Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string temporaryFolder, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
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

        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            if (releaseId == null || releaseId == 0)
                return 0;

            var count = 0;

            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            ISpecVersionManager versionMgr = ManagerFactory.Resolve<ISpecVersionManager>();
            versionMgr.UoW = UoW;

            //"Versions pending upload =  Latest (and only latest) versions that are : ...
            // - that are in the Release of interest, 
            var relatedSpecs = specMgr.GetSpecsRelatedToARelease(releaseId);
            foreach (var spec in relatedSpecs)
            {
                // - and that are UCC.
                if (spec.IsUnderChangeControl ?? false)
                {
                    var versions = versionMgr.GetVersionsForASpecRelease(spec.Pk_SpecificationId, releaseId);
                    var latestVersion = versions.OrderByDescending(x => x.MajorVersion ?? 0)
                                        .ThenByDescending(y => y.TechnicalVersion ?? 0)
                                        .ThenByDescending(z => z.EditorialVersion ?? 0)
                                        .FirstOrDefault();
                    // - allocated and not yet uploaded of specs ".
                    if (latestVersion != null && String.IsNullOrEmpty(latestVersion.Location))
                        count++;
                }
            }
            return count;
        }


        #region Offline Sync Methods

        /// <summary>
        /// Insert SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Success/Failure</returns>
        public bool InsertEntity(SpecVersion entity, string terminalName)
        {
            bool isSuccess = true;

            try
            {
                if (entity != null)
                {
                    SyncInfo syncInfo = new SyncInfo();
                    syncInfo.TerminalName = terminalName;
                    syncInfo.Offline_PK_Id = entity.Pk_VersionId;
                    entity.SyncInfoes.Add(syncInfo);

                    IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                    offlineRepo.UoW = UoW;
                    offlineRepo.InsertOfflineEntity(entity);
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Update SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateEntity(SpecVersion entity)
        {
            bool isSuccess = true;

            try
            {
                if (entity != null)
                {
                    //[1] Get the DB Version Entity
                    ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                    specVersionRepo.UoW = UoW;
                    SpecVersion dbEntity = specVersionRepo.Find(entity.Pk_VersionId);

                    //Record may be deleted in serverside, while changes happen at offline
                    //So, priority is serverside, hence no more changes will update
                    if (dbEntity != null)
                    {
                        //[2] Compare & Update SpecVersion Properties
                        UpdateModifications(dbEntity, entity);

                        //[3] Update modified entity in Context
                        IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                        offlineRepo.UoW = UoW;
                        offlineRepo.UpdateOfflineEntity(dbEntity);
                    }
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Delete SpecVersion entity
        /// </summary>
        /// <param name="primaryKey">Primary Key</param>
        /// <returns>Success/Failure</returns>
        public bool DeleteEntity(int primaryKey)
        {
            bool isSuccess = true;

            try
            {
                //[1] Get the DB Version Entity
                ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                SpecVersion dbEntity = specVersionRepo.Find(primaryKey);

                //Record may be deleted in serverside, while changes happen at offline
                //So, priority is serverside, hence no more changes will update
                if (dbEntity != null)
                {
                    //[2] Update modified entity in Context
                    IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                    offlineRepo.UoW = UoW;
                    offlineRepo.DeleteOfflineEntity(dbEntity);
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update modified properties
        /// </summary>
        /// <param name="targetSpecVersion">Target SpecVersion</param>
        /// <param name="sourceSpecVersion">Source SpecVersion</param>
        private void UpdateModifications(SpecVersion targetSpecVersion, SpecVersion sourceSpecVersion)
        {
            if (targetSpecVersion.MajorVersion != sourceSpecVersion.MajorVersion)
                targetSpecVersion.MajorVersion = sourceSpecVersion.MajorVersion;
            if (targetSpecVersion.TechnicalVersion != sourceSpecVersion.TechnicalVersion)
                targetSpecVersion.TechnicalVersion = sourceSpecVersion.TechnicalVersion;
            if (targetSpecVersion.EditorialVersion != sourceSpecVersion.EditorialVersion)
                targetSpecVersion.EditorialVersion = sourceSpecVersion.EditorialVersion;
            if (targetSpecVersion.AchievedDate != sourceSpecVersion.AchievedDate)
                targetSpecVersion.AchievedDate = sourceSpecVersion.AchievedDate;
            if (targetSpecVersion.ExpertProvided != sourceSpecVersion.ExpertProvided)
                targetSpecVersion.ExpertProvided = sourceSpecVersion.ExpertProvided;
            if (targetSpecVersion.Location != sourceSpecVersion.Location)
                targetSpecVersion.Location = sourceSpecVersion.Location;
            if (targetSpecVersion.SupressFromSDO_Pub != sourceSpecVersion.SupressFromSDO_Pub)
                targetSpecVersion.SupressFromSDO_Pub = sourceSpecVersion.SupressFromSDO_Pub;
            if (targetSpecVersion.ForcePublication != sourceSpecVersion.ForcePublication)
                targetSpecVersion.ForcePublication = sourceSpecVersion.ForcePublication;
            if (targetSpecVersion.DocumentUploaded != sourceSpecVersion.DocumentUploaded)
                targetSpecVersion.DocumentUploaded = sourceSpecVersion.DocumentUploaded;
            if (targetSpecVersion.DocumentPassedToPub != sourceSpecVersion.DocumentPassedToPub)
                targetSpecVersion.DocumentPassedToPub = sourceSpecVersion.DocumentPassedToPub;
            if (targetSpecVersion.Multifile != sourceSpecVersion.Multifile)
                targetSpecVersion.Multifile = sourceSpecVersion.Multifile;
            if (targetSpecVersion.Source != sourceSpecVersion.Source)
                targetSpecVersion.Source = sourceSpecVersion.Source;
            if (targetSpecVersion.ETSI_WKI_ID != sourceSpecVersion.ETSI_WKI_ID)
                targetSpecVersion.ETSI_WKI_ID = sourceSpecVersion.ETSI_WKI_ID;
            if (targetSpecVersion.ProvidedBy != sourceSpecVersion.ProvidedBy)
                targetSpecVersion.ProvidedBy = sourceSpecVersion.ProvidedBy;
            if (targetSpecVersion.Fk_SpecificationId != sourceSpecVersion.Fk_SpecificationId)
                targetSpecVersion.Fk_SpecificationId = sourceSpecVersion.Fk_SpecificationId;
            if (targetSpecVersion.Fk_ReleaseId != sourceSpecVersion.Fk_ReleaseId)
                targetSpecVersion.Fk_ReleaseId = sourceSpecVersion.Fk_ReleaseId;
            if (targetSpecVersion.ETSI_WKI_Ref != sourceSpecVersion.ETSI_WKI_Ref)
                targetSpecVersion.ETSI_WKI_Ref = sourceSpecVersion.ETSI_WKI_Ref;
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
        #endregion
    }
}
