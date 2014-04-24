using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecificationTechnologiesManager
    {
        public IUltimateUnitOfWork UoW{get; set;}

        public SpecificationTechnologiesManager(){
        }

        public List<Enum_Technology> GetAllSpecificationTechnologies()
        {
            IEnumTechnologiesRepository repo = RepositoryFactory.Resolve<IEnumTechnologiesRepository>();
            repo.UoW = UoW;
            return repo.All.ToList();
        }
    }
}
