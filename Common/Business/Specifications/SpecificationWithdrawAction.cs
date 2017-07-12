using System;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    /// <summary>
    /// This class manages the withdrawal and the definitive withdrawal of a specification.
    /// </summary>
    public class SpecificationWithdrawAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Withdraws a specification from a release. 
        /// 
        /// Checks that:
        /// - user has right to withdraw the release
        /// - release is not already withdrawn, or spec is not already withdrawn
        /// 
        /// Then sets the flag of the spec-release record to withdrawn.
        /// </summary>
        /// <param name="personId">ID of the person requesting the withdrawal</param>
        /// <param name="releaseId">ID of release for which specification should be withdrawn</param>
        /// <param name="specId">ID of specification to withdraw</param>
        /// <param name="withdrawalMtgId">Id of the withdrawal meeting</param>
        /// <returns></returns>
        public bool WithdrawFromRelease(int personId, int releaseId, int specId, int withdrawalMtgId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;

            // Retrieve spec and change status
            var spec = specMgr.GetSpecificationById(personId, specId);
            if (spec.Key == null)
            {
                throw new InvalidOperationException("Spec " + specId + " cannot be found");
            }

            // Check rights
            var rights = specMgr.GetRightsForSpecReleases(personId, spec.Key);
            var rightsForRelease = rights.FirstOrDefault(r => r.Key.Fk_ReleaseId == releaseId);
            if (rightsForRelease.Value == null || !rightsForRelease.Value.HasRight(Enum_UserRights.Specification_WithdrawFromRelease))
            {
                throw new InvalidOperationException("User has no right to withdraw spec " + specId + " for release " + releaseId);
            }

            // Update spec release
            var specRel = spec.Key.Specification_Release.First(sr => sr.Fk_ReleaseId == releaseId);
            specRel.isWithdrawn = true;
            specRel.WithdrawMeetingId = withdrawalMtgId;

            //Update history
            var historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            string historyText = String.Format("Specification has been withdrawn for release '{0}'", (specRel.Release == null) ? String.Empty : specRel.Release.Name);
            var history = new History { Fk_SpecificationId = specId, Fk_PersonId = personId, CreationDate = DateTime.UtcNow, HistoryText = historyText };
            historyRepo.InsertOrUpdate(history);

            return true;
        }

        public bool UnWithdrawFromRelease(int personId, int releaseId, int specId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId);
            if (spec.Key == null)
            {
                throw new InvalidOperationException("Spec " + specId + " cannot be found");
            }

            // Check rights
            var rights = specMgr.GetRightsForSpecReleases(personId, spec.Key);
            var rightsForRelease = rights.FirstOrDefault(r => r.Key.Fk_ReleaseId == releaseId);
            if (rightsForRelease.Value == null || !rightsForRelease.Value.HasRight(Enum_UserRights.Specification_UnWithdrawFromRelease))
            {
                throw new InvalidOperationException("User has no right to unwithdraw spec " + specId + " for release " + releaseId);
            }

            // Update spec release
            var specRel = spec.Key.Specification_Release.First(sr => sr.Fk_ReleaseId == releaseId);
            specRel.isWithdrawn = false;
            specRel.WithdrawMeetingId = null;

            //Update history
            var historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            string historyText = String.Format("Specification has been unwithdrawn for release '{0}'", (specRel.Release == null) ? String.Empty : specRel.Release.Name);
            var history = new History { Fk_SpecificationId = specId, Fk_PersonId = personId, CreationDate = DateTime.UtcNow, HistoryText = historyText };
            historyRepo.InsertOrUpdate(history);
            return true;
        }
    }
}
