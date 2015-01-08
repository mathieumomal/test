using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionsManager : ISpecVersionManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public SpecVersionsManager(){ }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        public List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId)
        {
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            var result = repo.GetVersionsForSpecRelease(specificationId, releaseId);
            return result;
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId)
        {
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            var version = repo.Find(versionId);
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

            var specificationManager = new SpecificationManager {UoW = UoW};
            //Get calculated rights
            var specReleaseRights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases);

            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specReleaseRights.Value);
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public ServiceResponse<VersionForCrListFacade> GetVersionNumberWithSpecNumberByVersionId(int personId, int versionId)
        {
            var response = new ServiceResponse<VersionForCrListFacade>();
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            var version = repo.AllIncluding(x => x.Specification).FirstOrDefault(x => x.Pk_VersionId == versionId);
            if (version == null)
                response.Report.LogError(Localization.Version_Not_Found);
            else
                response.Result = new VersionForCrListFacade { Version = version.Version, SpecNumber = version.Specification.Number };
            return response;
        }

        public List<Report> AllocateVersionFromMassivePromote(List<Specification> specifications, Release release, int personId)
        {
            var specVersionAllocateAction = new SpecVersionAllocateAction {UoW = UoW};

            var reports = new List<Report>();
            Report r;
            foreach (var s in specifications)
            {
                r = specVersionAllocateAction.AllocateVersion(personId,new SpecVersion()
                {
                    Fk_SpecificationId = s.Pk_SpecificationId,
                    Fk_ReleaseId = release.Pk_ReleaseId,
                    EditorialVersion = 0,
                    TechnicalVersion = 0,
                    MajorVersion = release.Version3g

                }).Report;

                reports.Add(r);
            }
            return reports;
        }

        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            // Retrieve the release 3G dec field to filter
            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            releaseManager.UoW = UoW;
            var releaseVersion = releaseManager.GetAllReleases(0).Key.Find(r => r.Pk_ReleaseId == releaseId).Version3g;
            if (!releaseVersion.HasValue)
                return 0;

            var specVersionMgr = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionMgr.UoW = UoW;
            return specVersionMgr.CountVersionsPendingUploadByReleaseId(releaseVersion.GetValueOrDefault());
        }

        /// <summary>
        /// Link TDoc to Version
        /// </summary>
        /// <param name="specId">The specification identifier</param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <param name="relatedTdoc">Related Tdoc</param>
        /// <returns>Success/Failure status</returns>
        public ServiceResponse<bool> UpdateVersionRelatedTdoc(int specId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc)
        {
            var svcResponse = new ServiceResponse<bool>() {Result = true};

            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            
            // First remove any existing version with this related TDoc
            var previousVersions = repo.GetVersionsByRelatedTDoc(relatedTdoc);
            previousVersions.ForEach(v => v.RelatedTDoc = null);

            var version = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            if (version == null)
                svcResponse.Report.LogInfo(Localization.Version_Tdoc_Link_Version_Not_Found);
            else
                version.RelatedTDoc = relatedTdoc;

            return svcResponse;
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
                    var syncInfo = new SyncInfo {TerminalName = terminalName, Offline_PK_Id = entity.Pk_VersionId};
                    entity.SyncInfoes.Add(syncInfo);

                    var offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
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
                    var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                    specVersionRepo.UoW = UoW;
                    var dbEntity = specVersionRepo.Find(entity.Pk_VersionId);

                    //Record may be deleted in serverside, while changes happen at offline
                    //So, priority is serverside, hence no more changes will update
                    if (dbEntity != null)
                    {
                        //[2] Compare & Update SpecVersion Properties
                        UpdateModifications(dbEntity, entity);

                        //[3] Update modified entity in Context
                        var offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
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
            var isSuccess = true;

            try
            {
                //[1] Get the DB Version Entity
                var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                var dbEntity = specVersionRepo.Find(primaryKey);

                //Record may be deleted in serverside, while changes happen at offline
                //So, priority is serverside, hence no more changes will update
                if (dbEntity != null)
                {
                    //[2] Update modified entity in Context
                    var offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
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
        #endregion
    }
}
