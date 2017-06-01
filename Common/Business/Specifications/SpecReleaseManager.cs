using System;
using System.Linq;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecReleaseManager : ISpecReleaseManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public bool CreateSpecRelease(int specId, int releaseId)
        {
            ExtensionLogger.Info("CREATE SPEC-RELEASE: System is checking if spec-release already exists...", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("specId", specId),
                new KeyValuePair<string, object>("releaseId", releaseId)
            });

            var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;

            //Check spec exists
            var spec = specRepo.Find(specId);
            if (spec == null)
                return false;

            //Check specRelease doesn't exist
            if (spec.Specification_Release.FirstOrDefault(x => x.Fk_ReleaseId == releaseId) != null)
            {
                LogManager.Info("CREATE SPEC-RELEASE: already exists!");
                return false;//SpecRelease already exists
            }
            
            return CreateSpecReleaseWithSpecEntityProvidedAndWithoutExistenceChecks(spec, releaseId);
        }

        public bool CreateSpecReleaseWithSpecEntityProvidedAndWithoutExistenceChecks(Specification spec, int releaseId)
        {
            LogManager.Info("CREATE SPEC-RELEASE: spec-release created! (for the moment, inside memory context. Until a commit will be done...)");
            spec.Specification_Release.Add(new Specification_Release
            {
                isWithdrawn = false,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Fk_ReleaseId = releaseId,
                Fk_SpecificationId = spec.Pk_SpecificationId
            });
            return true;
        }
    }

    public interface ISpecReleaseManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        bool CreateSpecRelease(int specId, int releaseId);

        bool CreateSpecReleaseWithSpecEntityProvidedAndWithoutExistenceChecks(Specification spec, int releaseId);
    }
}
