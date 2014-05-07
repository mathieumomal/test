using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Tests.Services
{
    class RightsServiceTest: BaseTest
    {

        [Test]
        public void GetRights_RetrievesRightsForAnonymous()
        {
            var userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Release_Create);
            var mock = MockRepository.GenerateMock<IRightsManager>();
            mock.Stub(m => m.GetRights(0)).Return(userRights);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(mock);

            var rightsSvc = new RightsService();

            var rights = rightsSvc.GetGenericRightsForUser(0);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_Create));
        }


    }
}
