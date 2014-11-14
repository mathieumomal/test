using Etsi.Ultimate.Business.SpecVersionBusiness;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class FinalizeCrsAction
    {

        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Associate any TSG approved CR corresponding to one of the TDoc Uids provided to a new version.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="tdocUids"></param>
        /// <returns></returns>
        public bool FinalizeCrs(int personId, List<string> tdocUids)
        {
            // Find approved CR status
            var crStatusManager = new ChangeRequestStatusManager() { UoW = UoW};
            var crApprovedStatus = crStatusManager.GetAllChangeRequestStatuses().Where(cr => cr.Code == Enum_ChangeRequestStatuses.Approved.ToString()).SingleOrDefault();


            // Retrieve Crs by TdocUids
            var crRepo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepo.UoW = UoW;
            var candidateChangeRequests = crRepo.GetChangeRequestListByContributionUidList(tdocUids).ToList()
                .Where( cr => cr.Fk_TSGStatus.GetValueOrDefault() == crApprovedStatus.Pk_EnumChangeRequestStatus
                && !cr.Fk_NewVersion.HasValue);

            var specManager = new SpecificationManager();
            specManager.UoW = UoW;

            var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepository.UoW = UoW;

            var releasesManager = new ReleaseManager() { UoW = UoW };
            var releases = releasesManager.GetAllReleases(personId).Key;

            foreach (var changeRequest in candidateChangeRequests)
            {
                // If release if closed, ignore
                var release = releases.Find(r => r.Pk_ReleaseId == changeRequest.Fk_Release);
                if (release == null || release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Closed)
                    break;

                var specification = specManager.GetSpecificationById(personId, changeRequest.Fk_Specification.GetValueOrDefault()).Key;
                if (specification == null || !specification.IsActive || !specification.IsUnderChangeControl.GetValueOrDefault())
                    break;

                var specRelease = specification.Specification_Release.Where(sr => sr.Fk_ReleaseId == changeRequest.Fk_Release.GetValueOrDefault()).FirstOrDefault();
                if (specRelease == null)
                {
                    var promoteAction = new SpecificationPromoteAction(UoW);
                    promoteAction.PromoteSpecificationBypassingRightsChecks(changeRequest.Fk_Specification.Value, changeRequest.Fk_Release.Value);
                }

                var version = versionRepository.GetVersionsForSpecRelease(changeRequest.Fk_Specification.GetValueOrDefault(), changeRequest.Fk_Release.GetValueOrDefault())
                    .OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion).FirstOrDefault();

                if (version == null)
                {
                    changeRequest.NewVersion = AllocateNewVersion(personId,release.Version3g.GetValueOrDefault(), 0, changeRequest);
                }
                else if (!string.IsNullOrEmpty(version.Location))
                {
                    changeRequest.NewVersion = AllocateNewVersion(personId, version.MajorVersion.Value, version.TechnicalVersion.Value + 1, changeRequest);
                }
                else
                {
                    changeRequest.Fk_NewVersion = version.Pk_VersionId;
                }
            }
            return true;
        }

        private SpecVersion AllocateNewVersion(int personId, int majorVersion, int technicalVersion, ChangeRequest changeRequest)
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

            var allocateAction = new SpecVersionAllocateAction() { UoW = UoW };
            var response = allocateAction.AllocateVersion(personId, newVersion).Result;
            
            
            if (response == null)
                throw new InvalidOperationException("Error occured while allocating version. User may not have right to perform such operation");
            return response;

            

        }
    }
}
