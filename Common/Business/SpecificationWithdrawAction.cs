using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
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
            var rightsForRelease = rights.Where(r => r.Key.Fk_ReleaseId == releaseId).FirstOrDefault();
            if (rightsForRelease.Value == null || !rightsForRelease.Value.HasRight(DomainClasses.Enum_UserRights.Specification_WithdrawFromRelease))
            {
                throw new InvalidOperationException("User has no right to withdraw spec " + specId + " for release " + releaseId);
            }

            // Update spec release
            var specRel = spec.Key.Specification_Release.Where(sr => sr.Fk_ReleaseId == releaseId).FirstOrDefault();
            specRel.isWithdrawn = true;
            specRel.WithdrawMeetingId = withdrawalMtgId;

            //Update history
            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            string historyText = String.Format("Specification has been withdrawn for release '{0}'", (specRel.Release == null) ? String.Empty : specRel.Release.Name);
            History history = new History() { Fk_SpecificationId = specId, Fk_PersonId = personId, CreationDate = DateTime.UtcNow, HistoryText = historyText };
            historyRepo.InsertOrUpdate(history);

            return true;
        }
    }
}
