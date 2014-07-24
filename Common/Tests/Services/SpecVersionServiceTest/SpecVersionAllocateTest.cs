using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Business.Security;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Business;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecVersionAllocateTest : BaseEffortTest
    {
        const int USER_HAS_NO_RIGHT = 1;
        const int USER_HAS_RIGHT = 2;

        const int OPEN_RELEASE_ID = 2883;
        const int OPEN_SPEC_ID = 136080;

        [Test]
        public void Allocate_Fails_If_User_Has_No_Right()
        {
            var versionSvc = new SpecVersionService();

            SetupMocks();

            var myVersion = CreateVersion();

            var result = versionSvc.AllocateVersion(USER_HAS_NO_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
        }

        [Test]
        public void Allocate_NominalCase()
        {
            var versionSvc = new SpecVersionService();

            var myVersion = CreateVersion();
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(0, result.GetNumberOfErrors());
            Assert.AreEqual(0, result.GetNumberOfWarnings());
        }

        private SpecVersion CreateVersion()
        {
            return new SpecVersion()
            {
                MajorVersion = 13,
                TechnicalVersion = 1,
                EditorialVersion = 0,
                Fk_ReleaseId = OPEN_RELEASE_ID,
                Fk_SpecificationId = OPEN_SPEC_ID

            };
        }

        private void SetupMocks()
        {
            var noRights = new UserRightsContainer();
            var allocateRights = new UserRightsContainer();
            allocateRights.AddRight(Enum_UserRights.Versions_Allocate);

            var rightsManager = MockRepository.GenerateMock<IRightsManager>();
            rightsManager.Stub(x => x.GetRights(USER_HAS_NO_RIGHT)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(USER_HAS_RIGHT)).Return(allocateRights);

            ManagerFactory.Container.RegisterInstance<IRightsManager>(rightsManager);
        }


    }
}
