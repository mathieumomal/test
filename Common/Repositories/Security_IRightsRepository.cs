using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public interface Security_IRightsRepository : IDisposable
    {
        List<Enum_SecurityRights> GetRightsPerRoles(List<SecurityRoles> roles);
    }
}
