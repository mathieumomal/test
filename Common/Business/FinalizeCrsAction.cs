using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.SpecVersionBusiness;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

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
            IEnumerable<ChangeRequest> candidateChangeRequests = crRepo.GetChangeRequestListByContributionUidList(
                tdocUids).ToList()
                .Where(cr => cr.Fk_TSGStatus.GetValueOrDefault() == crApprovedStatus.Pk_EnumChangeRequestStatus
                             && !cr.Fk_NewVersion.HasValue);

            var specManager = new SpecificationManager { UoW = UoW };

            var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepository.UoW = UoW;

            var releasesManager = new ReleaseManager { UoW = UoW };
            List<Release> releases = releasesManager.GetAllReleases(personId).Key;

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

                var specRelease =
                    specification.Specification_Release.FirstOrDefault(
                        sr => sr.Fk_ReleaseId == changeRequest.Fk_Release.GetValueOrDefault());
                if (specRelease == null)
                {
                    string releaseName = releases.Find(r => r.Pk_ReleaseId == requestReleaseId.GetValueOrDefault()).Code;
                    response.Report.LogWarning(String.Format(Localization.FinalizeCrs_Warn_SpecReleaseNotExisting,
                        specification.Number, releaseName));
                    continue;
                }

                var version =
                    versionRepository.GetVersionsForSpecRelease(changeRequest.Fk_Specification.GetValueOrDefault(),
                        changeRequest.Fk_Release.GetValueOrDefault())
                        .OrderByDescending(v => v.MajorVersion)
                        .ThenByDescending(v => v.TechnicalVersion)
                        .ThenByDescending(v => v.EditorialVersion)
                        .FirstOrDefault();

                if (version == null)
                {
                    changeRequest.NewVersion = AllocateNewVersion(personId, release.Version3g.GetValueOrDefault(), 0,
                        changeRequest);
                }
                else if (!string.IsNullOrEmpty(version.Location))
                {
                    changeRequest.NewVersion = AllocateNewVersion(personId, version.MajorVersion.GetValueOrDefault(),
                        version.TechnicalVersion.GetValueOrDefault() + 1, changeRequest);
                }
                else
                {
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
                Source = changeRequest.TSGMeeting,
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