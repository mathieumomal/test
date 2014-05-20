using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public interface ISpecVersionManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        List<SpecVersion> GetVersionsBySpecId(int specificationId);
    }
}
