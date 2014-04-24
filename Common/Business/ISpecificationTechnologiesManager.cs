using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public interface ISpecificationTechnologiesManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        List<Enum_Technology> GetAllSpecificationTechnologies();

        List<Enum_Technology> GetASpecificationTechnologiesBySpecId(int id);
    }
}
