using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface ISpecVersionService
    {
        List<SpecVersion> GetVersionsBySpecId(int specificationId);
    }
}
