using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{

    /// <summary>
    /// List all the roles that could be handled.
    /// </summary>
    public enum Enum_UserRoles
    {
        Anonymous,              // Has not logged on the portal
        EolAccountOwner,        // Has logged on the portal = delegate.
        StaffMember,            // is member of the staff, hence can perform additional operations
        CommitteeOfficial,       // Has specific rights within a commitee (chairman, vice chairman, ...)

        WorkPlanManager,        
        SuperUser,              // Is a super user (= Specification Manager)
        Administrator           // Has all rights. Acts as support.
    }
}
