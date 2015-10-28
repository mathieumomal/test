using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Versions
{
    public class SpecVersionsManager : ISpecVersionManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        /// <summary>
        /// Get latest version of each relaease for the given spec ids
        /// </summary>
        /// <param name="specIds">The specification identifiders</param>
        /// <returns>List of Spec Versions</returns>
        public List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds)
        {
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            return specVersionRepo.GetLatestVersionsBySpecIds(specIds);
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

            var specificationManager = new SpecificationManager { UoW = UoW };
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
            var specVersionAllocateAction = new SpecVersionAllocateAction { UoW = UoW };

            var reports = new List<Report>();
            foreach (var s in specifications)
            {
                Report r = specVersionAllocateAction.AllocateVersion(personId, new SpecVersion
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
        /// <param name="personId"></param>
        /// <param name="specId">The specification identifier</param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <param name="relatedTdoc">Related Tdoc</param>
        /// <returns>Success/Failure status</returns>
        public ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc)
        {
            var svcResponse = new ServiceResponse<bool> { Result = true };

            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            // First remove any existing version with this related TDoc
            var previousVersions = repo.GetVersionsByRelatedTDoc(relatedTdoc);
            previousVersions.ForEach(v => v.RelatedTDoc = null);

            var version = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            if (version == null)
            {
                //Version don't exist -> ALLOCATION
                var specVersionAllocateAction = new SpecVersionAllocateAction { UoW = UoW };
                version = new SpecVersion
                {
                    Fk_ReleaseId = releaseId,
                    Fk_SpecificationId = specId,
                    MajorVersion = majorVersion,
                    TechnicalVersion = technicalVersion,
                    EditorialVersion = editorialVersion,
                    Source = meetingId,
                    ProvidedBy = personId,
                    RelatedTDoc = relatedTdoc
                };
                var allocateVersionSvcResponse = specVersionAllocateAction.AllocateVersion(personId, version);
                svcResponse.Report.ErrorList.AddRange(allocateVersionSvcResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(allocateVersionSvcResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(allocateVersionSvcResponse.Report.InfoList);
            }
            else
            {
                //Version exist -> ASSOCIATION
                version.RelatedTDoc = relatedTdoc;
            }

            if (svcResponse.Report.GetNumberOfErrors() > 0)
                svcResponse.Result = false;
            return svcResponse;
        }

        /// <summary>
        /// Checks the draft creation or association.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specId">The spec identifier.</param>
        /// <param name="releaseId">The release identifier.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <returns>Draft creation or association status along with validation failures</returns>
        public ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = new ServiceResponse<bool> { Result = true };

            //[1] Spec should exist
            var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            var spec = specRepo.Find(specId);
            if (spec == null)
            {
                svcResponse.Report.LogError(Localization.Error_Spec_Does_Not_Exist);
                svcResponse.Result = false;
                return svcResponse;
            }

            //[2] Release should exists
            var releaseRepo = RepositoryFactory.Resolve<IReleaseRepository>();
            releaseRepo.UoW = UoW;
            var release = releaseRepo.Find(releaseId);
            if (release == null)
            {
                svcResponse.Report.LogError(Localization.Error_Release_Does_Not_Exist);
                svcResponse.Result = false;
                return svcResponse;
            }

            //[3] Spec should be in draft mode (Active & Not UCC => Draft)
            if (!(spec.IsActive && !spec.IsUnderChangeControl.GetValueOrDefault()))
            {
                svcResponse.Report.LogError(Localization.Error_Spec_Draft_Status);
                svcResponse.Result = false;
                return svcResponse;
            }

            //[4] Spec-Release should exist
            var specRelease = specRepo.GetSpecificationReleaseByReleaseIdAndSpecId(specId, releaseId, false);
            if (specRelease == null)
            {
                svcResponse.Report.LogError(Localization.Allocate_Error_SpecRelease_Does_Not_Exist);
                svcResponse.Result = false;
                return svcResponse;
            }

            //[5] If version is already allocated or uploaded, system should stop here
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;
            var versions = specVersionRepo.GetVersionsBySpecId(specId);
            if (versions.Exists(x => x.MajorVersion == majorVersion && x.TechnicalVersion == technicalVersion && x.EditorialVersion == editorialVersion))
            {
                svcResponse.Result = true;
                return svcResponse;
            }

            //[6] Else, check version is latest to allocate
            if (versions.Count > 0)
            {
                var latestVersion = versions.OrderByDescending(x => x.MajorVersion)
                                            .ThenByDescending(y => y.TechnicalVersion)
                                            .ThenByDescending(z => z.EditorialVersion)
                                            .FirstOrDefault();
                if ((latestVersion != null) &&
                    ((latestVersion.MajorVersion > majorVersion) ||
                     ((latestVersion.MajorVersion == majorVersion) && (latestVersion.TechnicalVersion > technicalVersion)) ||
                     ((latestVersion.MajorVersion == majorVersion) && (latestVersion.TechnicalVersion == technicalVersion) && (latestVersion.EditorialVersion >= editorialVersion))))
                {
                    svcResponse.Report.LogError(String.Format(Localization.Error_Lower_Version, latestVersion.MajorVersion, latestVersion.TechnicalVersion, latestVersion.EditorialVersion));
                    svcResponse.Result = false;
                    return svcResponse;
                }
            }

            //[7] Version is not allocated and we have necessary right to performed action (simulation)
            // 1) Spec should be active
            // 2) Spec-Release should not be withdrawn
            // 3) Release should no be closed
            // 4) User should have the right to allocate ---OR--- be prime rapporteur of this spec
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;

            var userRights = rightsMgr.GetRights(personId);
            if (!(spec.IsActive && (!specRelease.isWithdrawn.GetValueOrDefault())
                  && (release.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed)
                  && (userRights.HasRight(Enum_UserRights.Versions_Allocate) 
                    || spec.SpecificationRapporteurs.Any(x => x.Fk_RapporteurId == personId && x.IsPrime))))
            {
                svcResponse.Report.LogError(Localization.RightError);
                svcResponse.Result = false;
                return svcResponse;
            }


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
                    var syncInfo = new SyncInfo { TerminalName = terminalName, Offline_PK_Id = entity.Pk_VersionId };
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

        /// <summary>
        /// Update version (allowed fields and remarks)
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<SpecVersion> UpdateVersion(SpecVersion version, int personId)
        {
            var response = new ServiceResponse<SpecVersion>();

            try
            {
                //Check user right
                var rightMgr = ManagerFactory.Resolve<IRightsManager>();
                var rights = rightMgr.GetRights(personId);
                if (!rights.HasRight(Enum_UserRights.Versions_Edit))
                {
                    LogManager.Error("UpdateVersion - Right error. PersonId : " + personId + ", " + Localization.RightError);
                    response.Report.ErrorList.Add(Localization.RightError);
                    return response;
                }

                //Version validation
                if (version.MajorVersion == null ||
                    version.TechnicalVersion == null ||
                    version.EditorialVersion == null ||
                    version.Source == null)
                {
                    LogManager.Error("UpdateVersion - Invalid version object : " + version.Pk_VersionId + ", version numbers and source are mandatory");
                    response.Report.ErrorList.Add(Localization.GenericError);
                    return response;
                }

                //Get the DB Version Entity
                var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                var dbVersion = specVersionRepo.Find(version.Pk_VersionId);

                if (dbVersion == null)
                {
                    LogManager.Error("UpdateVersion - Version not found : " + version.Pk_VersionId);
                    response.Report.ErrorList.Add(Localization.Version_Not_Found);
                    return response;
                }else{
                    //Apply modification on dbEntity to be able to save it with only allowed modifications
                    UpdateVersionComparator(dbVersion, version, personId);

                    specVersionRepo.UpdateVersion(dbVersion);
                    response.Result = dbVersion;
                }
            }
            catch (Exception e)
            {
                LogManager.Error("UpdateVersion - Error", e);
                response.Report.ErrorList.Add(Localization.GenericError);
            }
            return response;
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
            if (targetSpecVersion == null || sourceSpecVersion == null)
                return;

            targetSpecVersion.MajorVersion = sourceSpecVersion.MajorVersion;
            targetSpecVersion.TechnicalVersion = sourceSpecVersion.TechnicalVersion;
            targetSpecVersion.EditorialVersion = sourceSpecVersion.EditorialVersion;
            targetSpecVersion.AchievedDate = sourceSpecVersion.AchievedDate;
            targetSpecVersion.ExpertProvided = sourceSpecVersion.ExpertProvided;
            targetSpecVersion.Location = sourceSpecVersion.Location;
            targetSpecVersion.SupressFromSDO_Pub = sourceSpecVersion.SupressFromSDO_Pub;
            targetSpecVersion.ForcePublication = sourceSpecVersion.ForcePublication;
            targetSpecVersion.DocumentUploaded = sourceSpecVersion.DocumentUploaded;
            targetSpecVersion.DocumentPassedToPub = sourceSpecVersion.DocumentPassedToPub;
            targetSpecVersion.Multifile = sourceSpecVersion.Multifile;
            targetSpecVersion.Source = sourceSpecVersion.Source;
            targetSpecVersion.ETSI_WKI_ID = sourceSpecVersion.ETSI_WKI_ID;
            targetSpecVersion.ProvidedBy = sourceSpecVersion.ProvidedBy;
            targetSpecVersion.Fk_SpecificationId = sourceSpecVersion.Fk_SpecificationId;
            targetSpecVersion.Fk_ReleaseId = sourceSpecVersion.Fk_ReleaseId;
            targetSpecVersion.ETSI_WKI_Ref = sourceSpecVersion.ETSI_WKI_Ref;
        }

        /// <summary>
        /// Update version comparator (only necessary fields will be modified)
        /// </summary>
        private void UpdateVersionComparator(SpecVersion dbVersion, SpecVersion versionUpdated, int personId)
        {
            dbVersion.SupressFromSDO_Pub = versionUpdated.SupressFromSDO_Pub;
            dbVersion.SupressFromMissing_List = versionUpdated.SupressFromMissing_List;
            dbVersion.Source = versionUpdated.Source;

            //Insert remarks
            var remarksToInsert = versionUpdated.Remarks.ToList().Where(x => dbVersion.Remarks.ToList().All(y => y.Pk_RemarkId != x.Pk_RemarkId)).ToList();
            remarksToInsert.ToList().ForEach(x => x.Fk_PersonId = personId);//Apply personId to inserted remarks
            remarksToInsert.ToList().ForEach(x => dbVersion.Remarks.Add(x));//Add remarks
            //Update remarks
            var remarksToUpdate = versionUpdated.Remarks.ToList().Where(x => dbVersion.Remarks.ToList().Any(y => y.Pk_RemarkId == x.Pk_RemarkId && y.IsPublic != x.IsPublic));
            remarksToUpdate.ToList().ForEach(x => dbVersion.Remarks.ToList().Find(y => y.Pk_RemarkId == x.Pk_RemarkId).IsPublic = x.IsPublic);
        }
        #endregion
    }
}
