﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.SpecVersionBusiness
{
    public class SpecVersionAllocateAction
    {
        public IUltimateUnitOfWork UoW;

        public Report AllocateVersion(int personId, SpecVersion version)
        {
            var report = new Report();

            try{
                CheckVersion(personId, version);

                // If we did not fail so far, we are good to allocate the SpecVersion
                var newVersion = new SpecVersion()
                {
                    Fk_ReleaseId = version.Fk_ReleaseId,
                    Fk_SpecificationId = version.Fk_SpecificationId,

                    EditorialVersion = version.EditorialVersion,
                    MajorVersion = version.MajorVersion,
                    Remarks = version.Remarks,
                    Source = version.Source,
                    TechnicalVersion = version.TechnicalVersion,
                    ProvidedBy = personId
                };

                var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                versionRepository.UoW = UoW;
                versionRepository.InsertOrUpdate(newVersion);
            } 
            catch( Exception ex)
            {
                report.LogError(ex.Message);
            }

            return report;
        }

        private void CheckVersion(int personId, SpecVersion version)
        {
            if (version.Fk_ReleaseId.GetValueOrDefault() == 0|| version.Fk_SpecificationId.GetValueOrDefault() == 0)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification);
            }

            // Now check that release exists
            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            releaseManager.UoW = UoW;
            version.Release = releaseManager.GetReleaseById(personId, version.Fk_ReleaseId.Value).Key;
            if (version.Release == null)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_Release_Does_Not_Exist);
            }

            // Now fetch specification
            var specificationManager = ManagerFactory.Resolve<ISpecificationManager>();
            specificationManager.UoW = UoW;
            version.Specification = specificationManager.GetSpecificationById(personId, version.Fk_SpecificationId.Value).Key;
            if (version.Specification == null)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_Spec_Does_Not_Exist);
            }

            // The spec release must be defined as well
            var specRelease = specificationManager.GetSpecReleaseBySpecIdAndReleaseId(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);
            if (specRelease == null)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_SpecRelease_Does_Not_Exist);
            }


            // User should have the right to allocate for this release
            IRightsManager rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);
            var specReleaseRights = specificationManager.GetRightsForSpecRelease(rights, personId, version.Specification, version.Fk_ReleaseId.Value, new List<Release>() { version.Release });

            if (!specReleaseRights.Value.HasRight(Enum_UserRights.Versions_Allocate))
            {
                throw new InvalidOperationException(Utils.Localization.RightError);
            }

            // Fetch all versions for this release
            var versionManager = ManagerFactory.Resolve<ISpecVersionManager>();
            versionManager.UoW = UoW;
            var versions = versionManager.GetVersionsForASpecRelease(version.Fk_SpecificationId.Value, version.Fk_ReleaseId.Value);
            if (versions.Count > 0)
            {
                var biggestVersion = versions.OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion).FirstOrDefault();
                if (biggestVersion.MajorVersion > version.MajorVersion
                     || (biggestVersion.MajorVersion == version.MajorVersion && biggestVersion.TechnicalVersion > version.TechnicalVersion)
                     || (biggestVersion.MajorVersion == version.MajorVersion && biggestVersion.TechnicalVersion == version.TechnicalVersion && biggestVersion.EditorialVersion >= version.EditorialVersion))
                {
                    throw new InvalidOperationException(Utils.Localization.Allocate_Error_Version_Not_Allowed);
                }
            }
            

            
        }
    }
}
