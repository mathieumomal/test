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

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionsManager : ISpecVersionManager
    {
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

        public List<Report> AllocateVersionFromMassivePromote(List<Specification> specifications, Release release, int personId)
        {
            SpecVersionAllocateAction specVersionAllocateAction = new SpecVersionAllocateAction();
            specVersionAllocateAction.UoW = UoW;

            List<Report> reports = new List<Report>();
            Report r;
            foreach (Specification s in specifications)
            {
                r = specVersionAllocateAction.AllocateVersion(personId,new SpecVersion()
                {
                    Fk_SpecificationId = s.Pk_SpecificationId,
                    Fk_ReleaseId = release.Pk_ReleaseId,
                    EditorialVersion = 0,
                    TechnicalVersion = 0,
                    MajorVersion = release.Version3g

                });

                reports.Add(r);
            }
            return reports;
        }

        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            if (releaseId == 0)
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
                if ((spec.IsUnderChangeControl ?? false) && spec.IsActive)
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
        #endregion
    }
}
