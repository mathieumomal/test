using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business.Security
{
    public interface Security_ISecurityManager
    {
        List<Enum_SecurityRights> GetRights(Object user);
    }
}
