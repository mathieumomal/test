using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class RightsFakeRepository : IUserRightsRepository
    {
        #region IRightsRepository Members

        public List<Ultimate.DomainClasses.Enum_UserRights> GetRightsForRoles(List<Ultimate.DomainClasses.Enum_UserRoles> roles)
        {
            var rights = new List<Enum_UserRights>();

            if (roles.Contains(Enum_UserRoles.Anonymous))
                rights.Add(Enum_UserRights.Release_ViewLimitedDetails);
            if (roles.Contains(Enum_UserRoles.EolAccountOwner))
                rights.Add(Enum_UserRights.Release_ViewDetails);
            if (roles.Contains(Enum_UserRoles.SuperUser))
                rights.Add(Enum_UserRights.Release_ViewCompleteDetails);

            if (roles.Contains(Enum_UserRoles.CommitteeOfficial))
                rights.Add(Enum_UserRights.Release_Close);

            return rights;


        }

        #endregion
    }
}
