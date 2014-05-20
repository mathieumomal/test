using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public class SpecificationForceUnforceTranspositionAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Forces the transposition for the release and Id. This method:
        /// - Checks that user has the right to perform the action
        /// - Checks that release is not already frozen
        /// - Checks that transposition is not already forced.
        /// 
        /// It then:
        /// - Set the force transposition flag to true in the spec release record
        /// - Force transposition is a version is ready to go to transposition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="relId"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        public bool ForceTranspositionForRelease(int personId, int relId, int specId)
        {
            // First check that user has right to transpose.
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            var rights = rightsMgr.GetRights(personId);
            if (!rights.HasRight(DomainClasses.Enum_UserRights.Specification_ForceUnforceTransposition))
            {
                throw new InvalidOperationException("User has no right to force transposition");
            }

            // Then check that release is opened for this specification, and transposition is not already set to true
            var specRelRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRelRepo.UoW = UoW;
            var specRelease = specRelRepo.GetSpecificationRelease(specId, relId, true);
            if (specRelease == null || specRelease.isWithdrawn.GetValueOrDefault() || specRelease.Release.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Open)
            {
                throw new InvalidOperationException("Release is not opened, cannot force transposition");
            }
            if (specRelease.isTranpositionForced.GetValueOrDefault())
            {
                throw new InvalidOperationException("Transposition is already forced for this release.");
            }

            // We set the flag of specRel to true
            specRelease.isTranpositionForced = true;

            // Then, we check if there is something to send to transposition.
            var versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepo.UoW = UoW;
            var latestVersion = versionRepo.GetVersionsForSpecRelease(specId,relId)
                .OrderByDescending(s => s.MajorVersion).ThenByDescending(s => s.TechnicalVersion)
                .ThenByDescending(s => s.EditorialVersion).FirstOrDefault();

            if (latestVersion != null && latestVersion.DocumentUploaded != null)
            {
                var transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
                transposeMgr.Transpose(specRelRepo.Find(specId), latestVersion);
            }

            return true;
        }


    }
}
