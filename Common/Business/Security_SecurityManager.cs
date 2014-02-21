using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business.Security
{
    class Security_SecurityManager : Security_ISecurityManager
    {

        #region ISecurityManager Membres

        public List<DomainClasses.Enum_SecurityRights> GetRights(object user)
        {
            Security_RolesManager rolesManager = new Security_RolesManager();
            Security_RightsManager rightManager = new Security_RightsManager();
            return rightManager.GetRightsForUser(rolesManager.GetRolesForUser(user));
        }

        #endregion
    }
}
