using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Business.Security;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business
{
    public class SpecVersionManager : ISpecVersionManager
    {

        private ISpecVersionRepository specVersionRepo;

        public IUltimateUnitOfWork UoW { get; set; }

        public SpecVersionManager() { }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            specVersionRepo = RepositoryFactory.Resolve<ISpecVersionRepository>();
            specVersionRepo.UoW = UoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }
    }
}
