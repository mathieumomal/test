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
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId).Key;
            
            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.Where(r => r.Key.Fk_ReleaseId == relId).FirstOrDefault().Value;

            if (!rightsForRelease.HasRight(Enum_UserRights.Specification_ForceTransposition))
            {
                throw new InvalidOperationException("User " + personId + " does not have right to force transposition");
            }
           
            // Then update the spec release
            var specRelease = spec.Specification_Release.Where(sr => sr.Fk_ReleaseId == relId).FirstOrDefault();
            specRelease.isTranpositionForced = true;

            // Then, we check if there is something to send to transposition.
            var versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepo.UoW = UoW;
            var latestVersion = versionRepo.GetVersionsForSpecRelease(specId,relId)
                .OrderByDescending(s => s.MajorVersion).ThenByDescending(s => s.TechnicalVersion)
                .ThenByDescending(s => s.EditorialVersion).FirstOrDefault();

            if (latestVersion != null && latestVersion.Location != null && latestVersion.DocumentPassedToPub == null)
            {
                var transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
                transposeMgr._uoW = UoW;
                if (transposeMgr.Transpose(spec, latestVersion))
                {
                    latestVersion.DocumentPassedToPub = DateTime.Now;
                    latestVersion.ForcePublication = true;                    
                }
                else
                    return false; 
            }

            return true;
        }

        /// <summary>
        /// Unforces transposition for the given specification on the given release. System checks that: User has right to unforce transposition.
        /// It then sets flag to "true"
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="relId"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        public bool UnforceTranspositionForRelease(int personId, int relId, int specId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId).Key;

            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.Where(r => r.Key.Fk_ReleaseId == relId).FirstOrDefault().Value;
            if (!rightsForRelease.HasRight(Enum_UserRights.Specification_UnforceTransposition))
            {
                throw new InvalidOperationException("User " + personId + " does not have right to unforce transposition");
            }

            // Then update the spec release
            var specRelease = spec.Specification_Release.Where(sr => sr.Fk_ReleaseId == relId).FirstOrDefault();
            specRelease.isTranpositionForced = false;

            return true;

        }
    }

}
