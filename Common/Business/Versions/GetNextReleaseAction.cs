using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Versions
{
    public class GetNextReleaseAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get the new and current version for a spec
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="forUpload"></param>
        /// <returns>Return a Response service object with a report, a rights container and a result (the old version number with the version)</returns>
        public ServiceResponse<SpecVersionCurrentAndNew> GetNextVersionForSpec(int personId, int specId, int releaseId, bool forUpload)
        {
            var response = new ServiceResponse<SpecVersionCurrentAndNew>();
            var resultVersionsCurrentAndNew = new SpecVersionCurrentAndNew();

            try
            {
                // We fetch the release
                var release = FetchRelease(personId, releaseId);
                resultVersionsCurrentAndNew.NewSpecVersion.Fk_ReleaseId = releaseId;
                resultVersionsCurrentAndNew.NewSpecVersion.Release = release;

                // We fetch the specification info
                var spec = FetchSpecification(personId, specId);
                resultVersionsCurrentAndNew.NewSpecVersion.Fk_SpecificationId = specId;
                resultVersionsCurrentAndNew.NewSpecVersion.Specification = spec;
                var isSpecUcc = spec.IsUnderChangeControl.GetValueOrDefault();

                // We compute the version against a complex algorithm.
                ComputeVersion(specId, releaseId, forUpload, resultVersionsCurrentAndNew, release, isSpecUcc);

                // We determine the user rights
                var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
                rightsMgr.UoW = UoW;
                var userRights = rightsMgr.GetRights(personId);

                var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
                specMgr.UoW = UoW;

                response.Rights = specMgr.GetRightsForSpecRelease(userRights, personId, spec, releaseId, new List<Release> { release }).Value;

                response.Result = resultVersionsCurrentAndNew;
            }
            catch (Exception ex)
            {
                response.Report.LogError(ex.Message);
            }
            return response;

        }

        private void ComputeVersion(int specId, int releaseId, bool forUpload, SpecVersionCurrentAndNew resultVersions, Release release, bool isSpecUcc)
        {
            // Fetch the versions. In draft, we might need all versions, as a draft can be carried from
            // one release to another.
            var versionMgr = new SpecVersionsManager {UoW = UoW};
            var existingVersions = versionMgr.GetVersionsBySpecId(specId);
            if (isSpecUcc)
            {
                existingVersions = existingVersions.Where(v => v.Fk_ReleaseId == releaseId).ToList();
            }

            resultVersions.NewSpecVersion.EditorialVersion = 0;

            // If there is no version
            // - UCC, version is <Rel#>.0.0
            // - In draft, 0.0.0
            if (existingVersions.Count == 0)
            {
                resultVersions.NewSpecVersion.TechnicalVersion = 0;
                resultVersions.NewSpecVersion.MajorVersion = 0;
                if (isSpecUcc)
                {
                    resultVersions.NewSpecVersion.MajorVersion = release.Version3g;
                }

                return;
            }

            // In this case, there already are versions, so we start by ordering them.
            var orderedVersions = existingVersions.OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion);
            resultVersions.CurrentSpecVersion = orderedVersions.FirstOrDefault();
            var eligibleVersion = FindEligibleVersion(releaseId, forUpload, orderedVersions);

            // if an eligible version has been found, we use it.
            // else, we compute the next version from the current one.
            if (eligibleVersion != null)
            {
                resultVersions.NewSpecVersion.MajorVersion = eligibleVersion.MajorVersion;
                resultVersions.NewSpecVersion.TechnicalVersion = eligibleVersion.TechnicalVersion;
                resultVersions.NewSpecVersion.EditorialVersion = eligibleVersion.EditorialVersion;
                resultVersions.NewSpecVersion.Source = eligibleVersion.Source;
            }
            else
            {
                resultVersions.NewSpecVersion.MajorVersion = isSpecUcc ? release.Version3g : orderedVersions.First().MajorVersion;
                var technicalVersion = orderedVersions.First().TechnicalVersion;
                if (technicalVersion != null)
                    resultVersions.NewSpecVersion.TechnicalVersion = technicalVersion.Value + 1;
            }
        }

        /// <summary>
        /// Computes the eligible version as the earliest version that has not been uploaded, if it exists
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="forUpload"></param>
        /// <param name="orderedVersions"></param>
        /// <returns></returns>
        private SpecVersion FindEligibleVersion(int releaseId, bool forUpload, IEnumerable<SpecVersion> orderedVersions)
        {
            SpecVersion eligibleVersion = null;
            if (forUpload)
            {
                foreach (var v in orderedVersions.Where(v => v.Fk_ReleaseId == releaseId))
                {
                    // Version has not been allocated => propose this version.
                    if (string.IsNullOrEmpty(v.Location))
                        eligibleVersion = v;
                    else
                        break;
                }
            }
            return eligibleVersion;
        }

        private Specification FetchSpecification(int personId, int specId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId).Key;
            if (spec == null)
            {
                throw new InvalidOperationException(Utils.Localization.Error_Spec_Does_Not_Exist);
            }
            return spec;
        }

        private Release FetchRelease(int personId, int releaseId)
        {
            var relMgr = ManagerFactory.Resolve<IReleaseManager>();
            relMgr.UoW = UoW;
            var release = relMgr.GetReleaseById(personId, releaseId).Key;
            if (release == null)
            {
                throw new InvalidOperationException(Utils.Localization.Error_Release_Does_Not_Exist);
            }
            return release;
        }
    }
}
