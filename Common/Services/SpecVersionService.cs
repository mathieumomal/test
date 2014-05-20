using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public class SpecVersionService : ISpecVersionService
    {
        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetVersionsBySpecId(specificationId);
            }
        }

        public List<SpecVersion> GetVersionsForSpecRelease(int specificationId, int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetVersionsForASpecRelease(specificationId, releaseId);
            }
        }

        public SpecVersion GetVersionsById(int VersionId)
        {            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetSpecVersionById(VersionId);
            }
        }
    }
}

