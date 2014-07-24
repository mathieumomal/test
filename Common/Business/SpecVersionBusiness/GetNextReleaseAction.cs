﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class GetNextReleaseAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public ServiceResponse<SpecVersion> GetNextVersionForSpec(int personId, int specId, int releaseId, bool forUpload)
        {
            var response = new ServiceResponse<SpecVersion>();
            var resultVersion = new SpecVersion();

            try
            {
                // We fetch the release
                var release = FetchRelease(personId, releaseId);
                resultVersion.Fk_ReleaseId = releaseId;
                resultVersion.Release = release;

                // We fetch the specification info
                var spec = FetchSpecification(personId, specId);
                resultVersion.Fk_SpecificationId = specId;
                resultVersion.Specification = spec;
                var isSpecUCC = spec.IsUnderChangeControl.GetValueOrDefault();

                // We compute the version against a complex algorithm.
                ComputeVersion(specId, releaseId, forUpload, resultVersion, release, isSpecUCC);

                response.Result = resultVersion;
            }
            catch (Exception ex)
            {
                response.Report.LogError(ex.Message);
            }
            return response;

        }

        private void ComputeVersion(int specId, int releaseId, bool forUpload, SpecVersion resultVersion, Release release, bool isSpecUCC)
        {
            // Fetch the versions. In draft, we might need all versions, as a draft can be carried from
            // one release to another.
            var versionMgr = new SpecVersionsManager();
            versionMgr.UoW = UoW;
            var existingVersions = versionMgr.GetVersionsBySpecId(specId);
            if (isSpecUCC)
            {
                existingVersions = existingVersions.Where(v => v.Fk_ReleaseId == releaseId).ToList();
            }

            resultVersion.EditorialVersion = 0;

            // If there is no version
            // - UCC, version is <Rel#>.0.0
            // - In draft, 0.0.0
            if (existingVersions.Count == 0)
            {
                resultVersion.TechnicalVersion = 0;
                resultVersion.MajorVersion = 0;
                if (isSpecUCC)
                {
                    resultVersion.MajorVersion = release.Version3g;
                }

                return;
            }


            // In this case, there already are versions, so we start by ordering them.
            var orderedVersions = existingVersions.OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion);
            SpecVersion eligibleVersion = FindEligibleVersion(releaseId, forUpload, orderedVersions);

            // if an eligible version has been found, we use it.
            // else, we compute the next version from the current one.
            if (eligibleVersion != null)
            {
                resultVersion.MajorVersion = eligibleVersion.MajorVersion;
                resultVersion.TechnicalVersion = eligibleVersion.TechnicalVersion;
                resultVersion.EditorialVersion = eligibleVersion.EditorialVersion;
                resultVersion.Source = eligibleVersion.Source;
            }
            else
            {
                if (isSpecUCC)
                {
                    resultVersion.MajorVersion = release.Version3g;
                }
                else
                {
                    resultVersion.MajorVersion = orderedVersions.First().MajorVersion;
                }
                resultVersion.TechnicalVersion = orderedVersions.First().TechnicalVersion.Value + 1;
            }
        }

        /// <summary>
        /// Computes the eligible version as the earliest version that has not been uploaded, if it exists
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="forUpload"></param>
        /// <param name="orderedVersions"></param>
        /// <returns></returns>
        private SpecVersion FindEligibleVersion(int releaseId, bool forUpload, IOrderedEnumerable<SpecVersion> orderedVersions)
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
