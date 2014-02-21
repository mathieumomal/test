using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Security
{
    public class Security_RightsManager
    {
        public Security_RightsManager() { }

        #region IRightsManager Membres

        public List<Enum_SecurityRights> GetRightsForUser(List<SecurityRoles> roles)
        {
            Security_IRightsRepository repo = RepositoryFactory.Resolve<Security_IRightsRepository>();
            return repo.GetRightsPerRoles(roles);
        }

        #endregion
    }
}
