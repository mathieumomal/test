using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
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

        public IUltimateUnitOfWork _uoW { get; set; }

        public SpecVersionsManager()
        {
        }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = _uoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        public List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId)
        {
            List<SpecVersion> result = new List<SpecVersion>();
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            result = repo.GetVersionsForSpecRelease(specificationId, releaseId);
            return result;
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;

            ////New version
            //if (versionId == -1)
            //    return new KeyValuePair<SpecVersion, UserRightsContainer>(new SpecVersion { MajorVersion = -1, TechnicalVersion = -1, EditorialVersion = -1 }, null);

            SpecVersion version = repo.Find(versionId);
            if (version == null)
                return new KeyValuePair<SpecVersion, UserRightsContainer>(null, null);

            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            var personRights = rightManager.GetRights(personId);

            // Get information about the releases, in particular the status.
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = _uoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            var specificationManager = new SpecificationManager();
            specificationManager.UoW = _uoW;
            //Get calculated rights
            KeyValuePair<Specification_Release, UserRightsContainer> specRelease_Rights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases);

            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specRelease_Rights.Value);
        }

        public Report UploadOrAllocateVersion(SpecVersion version, bool isDraft, int personId)
        {
            //Initialization
            Report result = new Report();
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = _uoW;
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = _uoW;
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = _uoW;

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

            //Transposition
            var frozen = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Frozen).FirstOrDefault();

            //Three conditions :
            //Spec underChangeControl + (Release Frozen OR Specification IsTransposedForce)
            var UCC = spec.IsUnderChangeControl ?? false;
            var isFrozen = release.Fk_ReleaseStatus.Equals(frozen.Enum_ReleaseStatusId);
            var transpoForced = specRelease.isTranpositionForced ?? false;
            if (UCC && (isFrozen || transpoForced))
            {
                var transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
                transposeMgr.Transpose(spec, version);
            }
            //Transposition

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

            }

            return result;
        }

        public List<Report> AllocateVersionFromMassivePromote(List<Specification> specifications, Release release, int personId)
        {
            List<Report> reports = new List<Report>();
            Report r;
            foreach (Specification s in specifications)
            {
                r= UploadOrAllocateVersion(new SpecVersion()
                {
                    Fk_SpecificationId= s.Pk_SpecificationId,
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

        #region Offline Sync Methods

        
        public bool InsertEntity(SpecVersion entity)
        {
            bool isSuccess = true;

            try
            {
                IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                offlineRepo.UoW = _uoW;
                offlineRepo.InsertOfflineEntity(entity);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool UpdateEntity(SpecVersion entity)
        {
            bool isSuccess = true;

            try
            {
                //[1] Get the DB Version Entity
                ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = _uoW;
                SpecVersion dbEntity = specVersionRepo.Find(entity.Pk_VersionId);

                //[2] Compare & Update SpecVersion Properties
                UpdateModifications(dbEntity, entity);

                //[3] Update modified entity in Context
                IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                offlineRepo.UoW = _uoW;
                offlineRepo.UpdateOfflineEntity(dbEntity);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool DeleteEntity(int primaryKey)
        {
            bool isSuccess = true;

            try
            {
                //[1] Get the DB Version Entity
                ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = _uoW;
                SpecVersion dbEntity = specVersionRepo.Find(primaryKey);

                //[2] Update modified entity in Context
                IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                offlineRepo.UoW = _uoW;
                offlineRepo.DeleteOfflineEntity(dbEntity);
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
        #endregion
    }
}
