using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationTechnologiesManager : ISpecificationTechnologiesManager
    {
        public IUltimateUnitOfWork UoW{get; set;}

        public List<Enum_Technology> GetAllSpecificationTechnologies()
        {
            var repo = RepositoryFactory.Resolve<IEnumTechnologiesRepository>();
            repo.UoW = UoW;
            return repo.All.ToList();
        }

        public List<Enum_Technology> GetASpecificationTechnologiesBySpecId(int id)
        {
            var result = new List<Enum_Technology>();
            var repo = RepositoryFactory.Resolve<ISpecificationTechnologiesRepository>();
            repo.UoW = UoW;
            repo.All.Where(s => s.Fk_Specification == id).ToList().ForEach(e => result.Add(e.Enum_Technology));
            return result;
        }
    }
}
