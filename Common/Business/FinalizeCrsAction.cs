using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class FinalizeCrsAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        ///     Associate any TSG approved CR corresponding to one of the TDoc Uids provided to a new version.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="tdocUids"></param>
        /// <returns></returns>
        public ServiceResponse<bool> FinalizeCrs(int personId, List<string> tdocUids)
        {
            var response = new ServiceResponse<bool> { Result = true };
            // Find approved CR status
            var crStatusManager = new ChangeRequestStatusManager { UoW = UoW };
            Enum_ChangeRequestStatus crApprovedStatus =
                crStatusManager.GetAllChangeRequestStatuses()
                    .SingleOrDefault(cr => cr.Code == Enum_ChangeRequestStatuses.Approved.ToString());


            // Retrieve Crs by TdocUids
            var crRepo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepo.UoW = UoW;

            var crs = crRepo.GetChangeRequestListByContributionUidList(
                tdocUids).ToList();

            var candidateChangeRequests = crs
                .Where(cr => cr.ChangeRequestTsgDatas.Any(x => x.Fk_TsgStatus == crApprovedStatus.Pk_EnumChangeRequestStatus)
                             && !cr.Fk_NewVersion.HasValue).ToList();

            var specManager = new SpecificationManager { UoW = UoW };

            var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepository.UoW = UoW;

            var releasesManager = new ReleaseManager { UoW = UoW };
            List<Release> releases = releasesManager.GetAllReleases(personId).Key;

            var alreadyAllocatedVersion = new List<SpecVersion>();
            var alreadyCreatedSpecRelease = new List<Specification_Release>();
            foreach (ChangeRequest changeRequest in candidateChangeRequests)
            {
                var requestReleaseId = changeRequest.Fk_Release;
                // If release if closed, ignore
                Release release = releases.Find(r => r.Pk_ReleaseId == requestReleaseId);
                if (release == null || release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
                    continue;

                var specification =
                    specManager.GetSpecificationById(personId, changeRequest.Fk_Specification.GetValueOrDefault()).Key;
                if (specification == null)
                {
                    continue;
                }
                if (!specification.IsActive)
                {
                    response.Report.LogWarning(String.Format(Localization.FinalizeCrs_Warn_WithDrawnSpec,
                        specification.Number));
                    continue;
                }
                if (!specification.IsUnderChangeControl.GetValueOrDefault())
                {
                    response.Report.LogWarning(String.Format(Localization.FinalizeCrs_Warn_DraftSpec,
                        specification.Number));
                    continue;
                }
   
                var specRelease = specification.Specification_Release.FirstOrDefault(
                        sr => sr.Fk_ReleaseId == changeRequest.Fk_Release.GetValueOrDefault());
                if (specRelease == null)//Spec release does not exist. Create it only if inhibitpromote flag not set to true, else raised an error
                {
                    if (specification.promoteInhibited.GetValueOrDefault())
                    {
                        string releaseName = releases.Find(r => r.Pk_ReleaseId == requestReleaseId.GetValueOrDefault()).Code;
                        response.Report.LogError(
                            String.Format(Localization.FinalizeCrs_Warn_SpecReleaseNotExistingAndCannotBeCreated,
                                specification.Number, releaseName));
                        response.Result = false;
                        continue;
                    }

                    //Create SpecRelease only if not already created during finalize process
                    if (!alreadyCreatedSpecRelease.Any(
                            x => x.Fk_SpecificationId == changeRequest.Fk_Specification.GetValueOrDefault()
                                 && x.Fk_ReleaseId == changeRequest.Fk_Release.GetValueOrDefault()))
                    {
                        var specReleaseMgr = ManagerFactory.Resolve<ISpecReleaseManager>();
                        specReleaseMgr.UoW = UoW;
                        specReleaseMgr.CreateSpecRelease(changeRequest.Fk_Specification.GetValueOrDefault(),
                            changeRequest.Fk_Release.GetValueOrDefault());

                        //Save the information that this specRelease has already been created
                        alreadyCreatedSpecRelease.Add(new Specification_Release
                        {
                            Fk_SpecificationId = changeRequest.Fk_Specification.GetValueOrDefault(),
                            Fk_ReleaseId = changeRequest.Fk_Release.GetValueOrDefault()
                        });
                    }
                }
                else if (specRelease.isWithdrawn.GetValueOrDefault())
                {
                    string releaseName = releases.Find(r => r.Pk_ReleaseId == requestReleaseId.GetValueOrDefault()).Code;
                    response.Report.LogWarning(String.Format(Localization.FinalizeCrs_Warn_SpecReleaseWithdrawn, specification.Number, releaseName));
                    continue;
                }

                var version =
                    versionRepository.GetVersionsForSpecRelease(changeRequest.Fk_Specification.GetValueOrDefault(),
                        changeRequest.Fk_Release.GetValueOrDefault())
                        .OrderByDescending(v => v.MajorVersion)
                        .ThenByDescending(v => v.TechnicalVersion)
                        .ThenByDescending(v => v.EditorialVersion)
                        .FirstOrDefault();

                LogManager.Debug(string.Format("FinalizeCrs : CR: {0}", changeRequest.Pk_ChangeRequest));
                LogManager.Debug(string.Format("CR, SPEC: {0}, RELEASE: {1}", changeRequest.Fk_Specification.GetValueOrDefault(), changeRequest.Fk_Release.GetValueOrDefault()));
                LogManager.Debug(string.Format("Last related version found is {0}", (version == null ? "VERSION NOT FOUND" : version.Pk_VersionId.ToString())));
                LogManager.Debug(string.Format("Location is {0}", (version == null ? "VERSION NOT FOUND" : version.Location)));
                if (version == null || !string.IsNullOrEmpty(version.Location))
                {
                    LogManager.Debug("NEW VERSION WILL BE ALLOCATED");
                    int majorVersion = release.Version3g.GetValueOrDefault();
                    int technicalVersion = (version == null) ? 0 : version.TechnicalVersion.GetValueOrDefault() + 1;

                    // Because of EF 6 leakage, if we already allocated the object on a previous CR, it will not find
                    // the version, so let's try to get it still.
                    version =
                        alreadyAllocatedVersion.FirstOrDefault(
                            v => v.Fk_SpecificationId == specification.Pk_SpecificationId
                                 && v.Fk_ReleaseId == requestReleaseId);

                    LogManager.Debug(string.Format("Version already allocated: Spec: {0}, Release: {1}", (version == null ? "VERSION NOT FOUND" : version.Fk_SpecificationId.ToString()), (version == null ? "VERSION NOT FOUND" : version.Fk_ReleaseId.ToString())));
                    if (version == null)
                    {
                        version = AllocateNewVersion(personId, majorVersion, technicalVersion,
                            changeRequest);
                        LogManager.Debug(string.Format("NEW Version allocated: Spec: {0}, Release: {1}, major: {2}, minor: {3}", (version == null ? "VERSION NOT FOUND" : version.Fk_SpecificationId.ToString()), (version == null ? "VERSION NOT FOUND" : version.Fk_ReleaseId.ToString()), majorVersion, technicalVersion));
                        alreadyAllocatedVersion.Add(version);
                    }
                    changeRequest.NewVersion = version;
                }
                else
                {
                    LogManager.Debug(string.Format("ALREADY EXISTING VERSION WILL BE USED, ID: {0}", version.Pk_VersionId));
                    changeRequest.Fk_NewVersion = version.Pk_VersionId;
                }
            }
            return response;
        }

        private SpecVersion AllocateNewVersion(int personId, int majorVersion, int technicalVersion,
            ChangeRequest changeRequest)
        {
            var newVersion = new SpecVersion
            {
                Fk_ReleaseId = changeRequest.Fk_Release,
                Fk_SpecificationId = changeRequest.Fk_Specification,
                MajorVersion = majorVersion,
                TechnicalVersion = technicalVersion,
                EditorialVersion = 0,
                Source = changeRequest.ChangeRequestTsgDatas != null ? (changeRequest.ChangeRequestTsgDatas.FirstOrDefault() != null ? changeRequest.ChangeRequestTsgDatas.First().TSGMeeting : 0) : 0,
                ProvidedBy = personId
            };

            var allocateAction = new SpecVersionAllocateAction { UoW = UoW };
            var response = allocateAction.AllocateVersion(personId, newVersion).Result;


            if (response == null)
                throw new InvalidOperationException(
                    "Error occured while allocating version. User may not have right to perform such operation");
            return response;
        }
    }
}