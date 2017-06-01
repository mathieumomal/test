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
            var specReleaseRights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases, version);

            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specReleaseRights.Value);
        }

        /// <summary>
        /// Delete a specification version
        /// </summary>
        /// <param name="personId">UserId to check rights</param>
        /// <param name="versionId">Version Id</param>
        /// <returns>serviceResponse</returns>
        public ServiceResponse<bool> DeleteVersion(int personId, int versionId)
        {
            var result = new ServiceResponse<bool> { Result = false };
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            var version = repo.Find(versionId);
            if (version == null) // Version not found
            {
                result.Result = false;
                result.Report.ErrorList.Add(Localization.Version_Not_Found);
                return result;
            }

            if (version.MajorVersion > 2) // version has not draft status
            {
                result.Result = false;
                result.Report.ErrorList.Add(Localization.Version_not_Delete_ucc);
                return result;
            }
            
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            if (!personRights.HasRight(Enum_UserRights.Version_Draft_Delete)) // Check if user is spec manager
            {
                result.Result = false;
                result.Report.ErrorList.Add(Localization.Version_Delete_Not_Allowed);
                return result;
            }

            repo.Delete(version);
            result.Result = true;

            return result;
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
            ExtensionLogger.Info("ALLOCATE OR ASSOCIATE DRAFT VERSION: System is trying to allocate or associate draft version", new List<KeyValuePair<string, object>> { 
                new KeyValuePair<string, object>("personId", personId),
                new KeyValuePair<string, object>("specId", specId),
                new KeyValuePair<string, object>("releaseId", releaseId),
                new KeyValuePair<string, object>("meetingId", meetingId),
                new KeyValuePair<string, object>("majorVersion", majorVersion),
                new KeyValuePair<string, object>("technicalVersion", technicalVersion),
                new KeyValuePair<string, object>("editorialVersion", editorialVersion),
                new KeyValuePair<string, object>("relatedTdoc", relatedTdoc)
            });

            var svcResponse = new ServiceResponse<bool> { Result = true };

            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            // First remove any existing version with this related TDoc
            var previousVersions = repo.GetVersionsByRelatedTDoc(relatedTdoc);
            previousVersions.ForEach(v => v.RelatedTDoc = null);

            //Create SpecRelease if necessary
            var specReleaseMgr = ManagerFactory.Resolve<ISpecReleaseManager>();
            specReleaseMgr.UoW = UoW;
            specReleaseMgr.CreateSpecRelease(specId, releaseId);

            var version = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            if (version == null || version.Fk_ReleaseId != releaseId)
            {
                LogManager.Info("ALLOCATE OR ASSOCIATE DRAFT VERSION:   Version will be allocated.");
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
                LogManager.Info("ALLOCATE OR ASSOCIATE DRAFT VERSION:   Version will be associated.");
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
            ExtensionLogger.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: System is trying to check if current draft version can be allocated or associated", new List<KeyValuePair<string, object>> { 
                new KeyValuePair<string, object>("personId", personId),
                new KeyValuePair<string, object>("specId", specId),
                new KeyValuePair<string, object>("releaseId", releaseId),
                new KeyValuePair<string, object>("majorVersion", majorVersion),
                new KeyValuePair<string, object>("technicalVersion", technicalVersion),
                new KeyValuePair<string, object>("editorialVersion", editorialVersion)
            });

            var svcResponse = new ServiceResponse<bool> { Result = true };
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;
            var userRights = rightsMgr.GetRights(personId);

            //[0] Version Major number should be draft
            if (majorVersion > 2)
            {
                svcResponse.Report.LogError(Localization.Error_Version_Major_Number_Should_Be_Draft);
                svcResponse.Result = false;
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Error_Version_Major_Number_Should_Be_Draft);
                return svcResponse;
            }

            //[1] Spec should exist
            var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            var spec = specRepo.Find(specId);
            if (spec == null)
            {
                svcResponse.Report.LogError(Localization.Error_Spec_Does_Not_Exist);
                svcResponse.Result = false;
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Error_Spec_Does_Not_Exist);
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
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Error_Release_Does_Not_Exist);
                return svcResponse;
            }

            //[3] Spec should be in draft mode (Active & Not UCC => Draft)
            if (!(spec.IsActive && !spec.IsUnderChangeControl.GetValueOrDefault()))
            {
                svcResponse.Report.LogError(Localization.Error_Spec_Draft_Status);
                svcResponse.Result = false;
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Error_Spec_Draft_Status);
                return svcResponse;
            }

            //If user is not MCC
            if (!userRights.HasRight(Enum_UserRights.Contribution_DraftTsTrEnabledReleaseField))
            {
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: User is not MCC (doesn't have right: Contribution_DraftTsTrEnabledReleaseField)");
                //[4] Spec-Release should exist
                var specRelease = specRepo.GetSpecificationReleaseByReleaseIdAndSpecId(specId, releaseId, false);
                if (specRelease == null)
                {
                    svcResponse.Report.LogError(Localization.Allocate_Error_SpecRelease_Does_Not_Exist);
                    svcResponse.Result = false;
                    LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Allocate_Error_SpecRelease_Does_Not_Exist);
                    return svcResponse;
                }

                //[5] If version is already allocated or uploaded, system should stop here
                var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                var versions = specVersionRepo.GetVersionsBySpecId(specId);
                var dbversion = versions.FirstOrDefault(x => x.MajorVersion == majorVersion
                                                             && x.TechnicalVersion == technicalVersion
                                                             && x.EditorialVersion == editorialVersion);
                if (dbversion != null)
                {
                    if (dbversion.Fk_ReleaseId == releaseId)
                    {
                        svcResponse.Result = true;
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: Version already allocated/uploaded. OK.");
                    }
                    else
                    {
                        /* throw error when version exists but release doesn't correspond */
                        svcResponse.Report.LogError(Localization.Wrong_Release_Version);
                        svcResponse.Result = false;
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Wrong_Release_Version);
                    }

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
                         ((latestVersion.MajorVersion == majorVersion) &&
                          (latestVersion.TechnicalVersion > technicalVersion)) ||
                         ((latestVersion.MajorVersion == majorVersion) &&
                          (latestVersion.TechnicalVersion == technicalVersion) &&
                          (latestVersion.EditorialVersion >= editorialVersion))))
                    {
                        svcResponse.Report.LogError(String.Format(Localization.Error_Lower_Version,
                            latestVersion.MajorVersion, latestVersion.TechnicalVersion, latestVersion.EditorialVersion));
                        svcResponse.Result = false;
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + String.Format(Localization.Error_Lower_Version,
                            latestVersion.MajorVersion, latestVersion.TechnicalVersion, latestVersion.EditorialVersion));
                        return svcResponse;
                    }
                }

                //[7] Version is not allocated and we have necessary right to performed action (simulation)
                // 1) Spec should be active
                // 2) Spec-Release should not be withdrawn
                // 3) Release should not be closed
                // 4) User should have the right to allocate ---OR--- be prime rapporteur of this spec
                if (!(spec.IsActive && (!specRelease.isWithdrawn.GetValueOrDefault())
                      && (release.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed)
                      && (userRights.HasRight(Enum_UserRights.Versions_Allocate)
                          || spec.SpecificationRapporteurs.Any(x => x.Fk_RapporteurId == personId && x.IsPrime))))
                {
                    if (specRelease.isWithdrawn.GetValueOrDefault() || 
                        !spec.IsActive || 
                        release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
                    {
                        svcResponse.Report.LogError(Localization.Specification_reserve_withdrawn_error);
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Specification_reserve_withdrawn_error);
                    }
                    else
                    {
                        svcResponse.Report.LogError(Localization.Drafts_Rights_Error_Not_Reporteur);
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Drafts_Rights_Error_Not_Reporteur);
                    }

                    svcResponse.Result = false;
                    return svcResponse;
                }
            }
            else////If user is MCC
            {
                LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: User is MCC (has right: Contribution_DraftTsTrEnabledReleaseField)");
                //Release should not be closed
                if (release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
                {
                    svcResponse.Report.LogError(Localization.Specification_reserve_withdrawn_error);
                    svcResponse.Result = false;
                    LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Specification_reserve_withdrawn_error);
                    return svcResponse;
                }

                //Check if spec-release exists
                var specRelease = specRepo.GetSpecificationReleaseByReleaseIdAndSpecId(specId, releaseId, false);
                if (specRelease != null)
                {
                    //[4] Check specRelease not withdrawn and check release non closed
                    if (specRelease.isWithdrawn.GetValueOrDefault())
                    {
                        svcResponse.Report.LogError(Localization.Specification_reserve_withdrawn_error);
                        svcResponse.Result = false;
                        LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: " + Localization.Specification_reserve_withdrawn_error);
                        return svcResponse;
                    }

                    //Remark: version could exist or not, no matter => Creation or Association.
                    //Moreover, no matter about the fact that the version is the latest of the spec release. 
                }
                //Else: -> spec-release and version will be created. No matter if version number already exists for another spec-release
            }

            LogManager.Info("CHECK FOR ALLOCATE OR ASSOCIATE DRAFT VERSION: End.");
            return svcResponse;
        }

        /// <summary>
        /// Unlink tdoc from related version
        /// </summary>
        /// <param name="uid">Tdoc uid</param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        public ServiceResponse<bool> UnlinkTdocFromVersion(string uid, int personId)
        {
            ExtensionLogger.Info("UNLINK TDOC FROM VERSION(S): System is trying to unlink current tdoc from all the concerned versions...", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("uid", uid),
                new KeyValuePair<string, object>("personId", personId)
            });

            var svcResponse = new ServiceResponse<bool> { Result = true };

            //Check user rights
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;
            var userRights = rightsMgr.GetRights(personId);
                //-> Should at least have these rights (basic security control, no need complex rules here just to remove relatedTdoc data,
                // moreover this method should be use only after more deep rights checks on contribution side)
            if (!userRights.HasRight(Enum_UserRights.Contribution_Change_Type) &&
                !userRights.HasRight(Enum_UserRights.Contribution_Change_Type_Limited))
            {
                svcResponse.Result = false;
                svcResponse.Report.LogError(Localization.RightError);
                LogManager.Info("UNLINK TDOC FROM VERSION(S): User doesn't have rights Contribution_Change_Type or Contribution_Change_Type_Limited");
                return svcResponse;
            }

            //Unlink tdoc from version
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;
            var previousVersions = repo.GetVersionsByRelatedTDoc(uid);
            previousVersions.ForEach(v => v.RelatedTDoc = null);
            LogManager.Info("UNLINK TDOC FROM VERSION(S): Unlink done successfully for versions: " + string.Join(", ", previousVersions.Select(x => x.Pk_VersionId).ToList()));

            return svcResponse;
        }

        /// <summary>
        /// Create version for pCR tdoc if necessary (if doesn't already exist)
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <returns></returns>
        public ServiceResponse<bool> CreatepCrDraftVersionIfNecessary(int personId, int specId, int releaseId,
            int meetingId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = new ServiceResponse<bool> { Result = true };

            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = UoW;

            var versionExists = repo.CheckIfVersionExists(specId, releaseId, majorVersion, technicalVersion, editorialVersion);
            if (!versionExists)
            {
                var specVersionAllocateAction = ManagerFactory.Resolve<ISpecVersionAllocateAction>();
                specVersionAllocateAction.UoW = UoW;
                var version = new SpecVersion
                {
                    Fk_ReleaseId = releaseId,
                    Fk_SpecificationId = specId,
                    MajorVersion = majorVersion,
                    TechnicalVersion = technicalVersion,
                    EditorialVersion = editorialVersion,
                    Source = meetingId,
                    ProvidedBy = personId
                };
                var allocateVersionSvcResponse = specVersionAllocateAction.AllocateVersion(personId, version);
                svcResponse.Report.ErrorList.AddRange(allocateVersionSvcResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(allocateVersionSvcResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(allocateVersionSvcResponse.Report.InfoList);
            }

            if (svcResponse.Report.GetNumberOfErrors() > 0)
                svcResponse.Result = false;
            return svcResponse;
        }

        /// <summary>
        /// Check if user is allowed to edit version numbers
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        public ServiceResponse<bool> CheckVersionNumbersEditAllowed(SpecVersion version, int personId)
        {
            var response = new ServiceResponse<bool> { Result = true };

            //[1] User should have version edit major number right
            var rightsManager = ManagerFactory.Resolve<IRightsManager>();
            rightsManager.UoW = UoW;
            var userRights = rightsManager.GetRights(personId);
            if (!userRights.HasRight(Enum_UserRights.Versions_Edit))
            {
                response.Result = false;
                response.Report.LogError(Localization.RightError);
                return response;
            }

            if (!version.Fk_SpecificationId.HasValue)
            {
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
                return response;
            }

            var warnings = new List<string>();
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;
            var viewContributionWithAddData =
                RepositoryFactory.Resolve<IViewContributionsWithAditionnalDataRepository>();
            viewContributionWithAddData.UoW = UoW;


            //[2] Do not allowed deletion if version already uploaded
            if (!string.IsNullOrEmpty(version.Location) || version.DocumentUploaded != null)
            {
                response.Result = false;
                warnings.Add(Localization.Version_Already_Uploaded);
            }

            //[3] Do not allowed editing if version is linked to CR(s)
            var versionWithCrs = specVersionRepo.FindCrsLinkedToAVersion(version.Pk_VersionId);
            if (versionWithCrs.CurrentChangeRequests != null && (versionWithCrs.CurrentChangeRequests.Count > 0 || versionWithCrs.FoundationsChangeRequests.Count > 0))
            {
                response.Result = false;
                warnings.Add(string.Format(Localization.Version_edit_linked_CR, versionWithCrs.CurrentChangeRequests.Count + versionWithCrs.FoundationsChangeRequests.Count));
            }

            //[4] Do not allowed editing if version is linked to Tdoc(s)
            var tdocsRelatedToThisVersion = viewContributionWithAddData.FindContributionsRelatedToASpecAndVersionNumber(version.Fk_SpecificationId ?? 0, version.MajorVersion ?? 0, version.TechnicalVersion ?? 0, version.EditorialVersion ?? 0);
            if (tdocsRelatedToThisVersion.Count > 0)
            {
                warnings.Add(string.Format(Localization.Version_edit_linked_TDoc, tdocsRelatedToThisVersion.Count));
            }


            //Check for warnings : if at least one warning detected -> do not allowed deletion and precise reason why
            if (warnings.Count > 0)
            {
                response.Result = false;
                response.Report.WarningList.Add(Localization.Version_edit_Warnings);
                warnings.ForEach(x => response.Report.WarningList.Add(x));
            }

            return response;
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
        /// <param name="specVersion"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<SpecVersion> UpdateVersion(SpecVersion specVersion, int personId)
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
                /* Source is mandatory only for no draft version (draft version = (majorVersion < 3)) */
                if ((specVersion.MajorVersion == null || specVersion.TechnicalVersion == null || specVersion.EditorialVersion == null)
                    || (specVersion.MajorVersion != null && specVersion.MajorVersion > 2 && specVersion.Source == null))
                {
                    LogManager.Error("UpdateVersion - Invalid version object : "
                        + specVersion.Pk_VersionId + ", version numbers and source are mandatory");
                    response.Report.ErrorList.Add(Localization.GenericError);
                    return response;
                }

                //Get the DB Version Entity
                var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                var dbVersion = specVersionRepo.Find(specVersion.Pk_VersionId);

                if (dbVersion == null)
                {
                    LogManager.Error("UpdateVersion - Version not found : " + specVersion.Pk_VersionId);
                    response.Report.ErrorList.Add(Localization.Version_Not_Found);
                    return response;
                }

                //Apply modification on dbEntity
                UpdateVersionComparator(dbVersion, specVersion, personId, response.Report);

                //Check version number
                var specVersionNumberValidator = ManagerFactory.Resolve<ISpecVersionNumberValidator>();
                specVersionNumberValidator.UoW = UoW;
                var numberValidationResponse = specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, specVersion,
                    SpecNumberValidatorMode.Edit, personId);
                if (!numberValidationResponse.Result || numberValidationResponse.Report.ErrorList.Any())
                {
                    response.Report.ErrorList.AddRange(numberValidationResponse.Report.ErrorList);
                    return response;
                }

                if (!response.Report.ErrorList.Any())
                {
                    //Change spec to UCC when a version is edited with a major version number greater than 2
                    if (dbVersion.MajorVersion > 2
                        && (!dbVersion.Specification.IsUnderChangeControl.HasValue || !dbVersion.Specification.IsUnderChangeControl.Value))
                    {
                        var specChangeToUccAction = new SpecificationChangeToUnderChangeControlAction { UoW = UoW };
                        var responseUcc = specChangeToUccAction.ChangeSpecificationsStatusToUnderChangeControl(personId, new List<int> { dbVersion.Fk_SpecificationId ?? 0});

                        if (!responseUcc.Result && responseUcc.Report.ErrorList.Count > 0)
                        {
                            throw new Exception(responseUcc.Report.ErrorList.First());
                        }
                    }

                    specVersionRepo.UpdateVersion(dbVersion);
                    response.Result = dbVersion;
                }
                else { return response; }
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
        private void UpdateVersionComparator(SpecVersion dbVersion, SpecVersion versionUpdated, int personId, Report report)
        {
            dbVersion.SupressFromSDO_Pub = versionUpdated.SupressFromSDO_Pub;
            dbVersion.SupressFromMissing_List = versionUpdated.SupressFromMissing_List;
            dbVersion.Source = versionUpdated.Source;

            //Release
            if (versionUpdated.Fk_ReleaseId == null)//Raised an error if version not linked to a release
            {
                LogManager.Error("UpdateVersion - trying to linked a version to no release (not possible) - Pk_VersionId=" + versionUpdated.Pk_VersionId);
                report.ErrorList.Add(Localization.GenericError);
            }
            else if (dbVersion.Fk_ReleaseId != versionUpdated.Fk_ReleaseId)//Release change
            {
                if (versionUpdated.MajorVersion > 2)//Raised an error if version change of release and is not draft
                {
                    LogManager.Error("UpdateVersion - release change is allowed only for draft version - Pk_VersionId=" + versionUpdated.Pk_VersionId);
                    report.ErrorList.Add(Localization.GenericError);
                }
                else
                {
                    var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
                    releaseMgr.UoW = UoW;
                    var releases = releaseMgr.GetReleasesLinkedToASpec(dbVersion.Fk_SpecificationId ?? 0);

                    //Could just change release by a release already linked to the spec
                    if (releases.All(x => x.Pk_ReleaseId != versionUpdated.Fk_ReleaseId.Value))
                    {
                        LogManager.Error(string.Format("UpdateVersion - trying to change release of a version by a release not linked to a spec - Pk_VersionId={0}, New release id={1}", versionUpdated.Pk_VersionId, versionUpdated.Fk_ReleaseId.Value));
                        report.ErrorList.Add(Localization.GenericError);
                    }
                        
                    dbVersion.Fk_ReleaseId = versionUpdated.Fk_ReleaseId;
                }
            }

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
