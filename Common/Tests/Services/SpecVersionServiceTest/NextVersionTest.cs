using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version")]
    class NextVersionTest: BaseEffortTest
    {
        const int USER_HAS_RIGHT = 1;
        const int USER_HAS_NO_RIGHT = 2;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SetupMocks();
        }      

        [TestCase(EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_NEWLY_OPEN_ID, false, 14, 0, 0)]
        public void GetNextVersion_ComputesRightVersionNumber(int specId, int relId, bool forUpload, int awaitedMajor, int awaitedTechnical, int awaitedEditorial )
        {
            var versionSvc = new SpecVersionService();
            var response = versionSvc.GetNextVersionForSpec(1, specId, relId, forUpload);

            Assert.IsNotNull(response);
            var newVersion = response.Result;
            Assert.IsNotNull(newVersion);

            Assert.AreEqual(awaitedMajor, response.Result.MajorVersion);
            Assert.AreEqual(awaitedTechnical, response.Result.TechnicalVersion);
            Assert.AreEqual(awaitedEditorial, response.Result.EditorialVersion);
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
