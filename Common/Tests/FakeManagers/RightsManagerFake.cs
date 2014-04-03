using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeManagers
{
    public class RightsManagerFake : IRightsManager
    {

        #region IRightsManager Membres

        public IUltimateUnitOfWork UoW { get; set; }
        

        public UserRightsContainer GetRights(int personID)
        {
            UserRightsContainer userRightsContainer = new UserRightsContainer();
            userRightsContainer.AddRight(Enum_UserRights.Release_Close);
            userRightsContainer.AddRight(Enum_UserRights.Release_Freeze);
            userRightsContainer.AddRight(Enum_UserRights.Release_Create);
            userRightsContainer.AddRight(Enum_UserRights.Release_Edit);
            userRightsContainer.AddRight(Enum_UserRights.Release_ViewCompleteDetails);
            return userRightsContainer;
        }

        public bool IsUserMCCMember(int personID)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
