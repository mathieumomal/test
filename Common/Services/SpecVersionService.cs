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

        public KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int VersionId, int personId)
        {            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetSpecVersionById(VersionId, personId);
            }
        }

        public bool AllocateVersion(SpecVersion version, int oldVersionId)
        {
            bool operationResult = true;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {                    
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.CheckIfVersionAlloawed(version, specVersionManager.GetSpecVersionById(oldVersionId, 0).Key, false))
                    {
                        specVersionManager.UploadOrAllocateVersion(version);
                        uoW.Save();
                    }
                    else
                    {
                        operationResult = false;
                    }
                    
                }
                catch (Exception ex)
                {
                    operationResult = false;
                }
            }
            return operationResult;
        }

        public bool UploadVersion(SpecVersion version, int oldVersionId)
        {
            bool operationResult = true;            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.CheckIfVersionAlloawed(version, specVersionManager.GetSpecVersionById(oldVersionId, 0).Key, true))
                    {
                        specVersionManager.UploadOrAllocateVersion(version);
                        uoW.Save();
                    }
                    else
                    {
                        operationResult = false;
                    }

                }
                catch (Exception ex)
                {
                    operationResult = false;
                }
            }
            return operationResult;
        }

    }
}

