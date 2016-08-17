using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business.Specifications;

namespace Etsi.Ultimate.Business.Versions
{
    public class SpecVersionAllocateAction : ISpecVersionAllocateAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public ServiceResponse<SpecVersion> AllocateVersion(int personId, SpecVersion version)
        {
            var response = new ServiceResponse<SpecVersion>();

            try
            {
                CheckVersion(personId, version);

                // If we did not fail so far, we are good to allocate the SpecVersion
                var newVersion = new SpecVersion
                {
                    Fk_ReleaseId = version.Fk_ReleaseId,
                    Fk_SpecificationId = version.Fk_SpecificationId,

                    EditorialVersion = version.EditorialVersion,
                    MajorVersion = version.MajorVersion,
                    Remarks = version.Remarks,
                    Source = version.Source,
                    TechnicalVersion = version.TechnicalVersion,
                    ProvidedBy = personId,
                    RelatedTDoc = version.RelatedTDoc
                };

                var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                versionRepository.UoW = UoW;
                versionRepository.InsertOrUpdate(newVersion);

                //Change spec to UCC when a version is allocated with a major version number greater than 2
                if (newVersion.MajorVersion > 2
                    && (!newVersion.Specification.IsUnderChangeControl.HasValue || !newVersion.Specification.IsUnderChangeControl.Value))
                {
                    var specChangeToUccAction = new SpecificationChangeToUnderChangeControlAction { UoW = UoW };
                    var responseUcc = specChangeToUccAction.ChangeSpecificationsStatusToUnderChangeControl(personId, new List<int> { newVersion.Fk_SpecificationId ?? 0 });

                    if (!responseUcc.Result && responseUcc.Report.ErrorList.Count > 0)
                    {
                        throw new Exception(responseUcc.Report.ErrorList.First());
                    }
                }

                response.Result = newVersion;
            }
            catch (Exception ex)
            {
                response.Report.LogError(ex.Message);
            }

            return response;
        }

        private void CheckVersion(int personId, SpecVersion version)
        {
            if (!version.Fk_ReleaseId.HasValue || !version.Fk_SpecificationId.HasValue
                || version.Fk_ReleaseId.Value == 0 || version.Fk_SpecificationId.Value == 0)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification);
            }

            // Now check that release exists
            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            releaseManager.UoW = UoW;
            version.Release = releaseManager.GetReleaseById(personId, version.Fk_ReleaseId.Value).Key;
            if (version.Release == null)
            {
                throw new InvalidOperationException(Utils.Localization.Error_Release_Does_Not_Exist);
            }

            // Now fetch specification
            var specificationManager = ManagerFactory.Resolve<ISpecificationManager>();
            specificationManager.UoW = UoW;
            version.Specification = specificationManager.GetSpecificationById(personId, version.Fk_SpecificationId.Value).Key;
            if (version.Specification == null)
            {
                throw new InvalidOperationException(Utils.Localization.Error_Spec_Does_Not_Exist);
            }

            // The spec release must be defined as well
            var specRelease = version.Specification.Specification_Release.FirstOrDefault(sr => sr.Fk_ReleaseId == version.Fk_ReleaseId.Value);
            if (specRelease == null)
            {
                throw new InvalidOperationException(Utils.Localization.Allocate_Error_SpecRelease_Does_Not_Exist);
            }


            // User should have the right to allocate for this release
            var rightMgr = ManagerFactory.Resolve<IRightsManager>();
            rightMgr.UoW = UoW;

            var rights = rightMgr.GetRights(personId);
            var specReleaseRights = specificationManager.GetRightsForSpecRelease(rights, personId, version.Specification, 
                version.Fk_ReleaseId.Value, new List<Release> { version.Release });

            if (!specReleaseRights.Value.HasRight(Enum_UserRights.Versions_Allocate))
            {
                throw new InvalidOperationException(Utils.Localization.RightError);
            }

            //Validate version number
            var specVersionNumberValidator = ManagerFactory.Resolve<ISpecVersionNumberValidator>();
            specVersionNumberValidator.UoW = UoW;
            var numberValidationResponse = specVersionNumberValidator.CheckSpecVersionNumber(null, version,
                SpecNumberValidatorMode.Allocate, personId);
            if(!numberValidationResponse.Result || numberValidationResponse.Report.ErrorList.Any())
                throw new InvalidOperationException(string.Join(", ", numberValidationResponse.Report.ErrorList));
        }
    }

    public interface ISpecVersionAllocateAction
    {
        IUltimateUnitOfWork UoW { get; set; }

        ServiceResponse<SpecVersion> AllocateVersion(int personId, SpecVersion version);
    }
}
