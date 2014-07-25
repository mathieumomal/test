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

        SpecVersionService versionSvc;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SetupMocks();

            versionSvc = new SpecVersionService();

        }      

        [TestCase(EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_NEWLY_OPEN_ID, false, 14, 0, 0)]     // First version of a UCC spec is <rel#>.0.0
        [TestCase(EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_OPEN_ID, false, 13, 3, 0)]           // Next version for allocation is always <rel#>.<biggest+1>.0    
        [TestCase(EffortConstants.SPECIFICATION_DRAFT_WITH_EXISTING_DRAFTS_ID, EffortConstants.RELEASE_NEWLY_OPEN_ID, false, 2, 2, 0)]           // In case of draft, next version for allocation is <biggest>.<biggest+1>.0    
        [TestCase(EffortConstants.SPECIFICATION_DRAFT_WITH_NO_DRAFT_ID, EffortConstants.RELEASE_NEWLY_OPEN_ID, false, 0, 0, 0)]           // In case of draft, and no draft already allocated, 0.0.0
        [TestCase(EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_OPEN_ID, true, 13, 1, 1)]           // In case of upload, fetch lowest allocated version after an upload.
        [TestCase(EffortConstants.SPECIFICATION_DRAFT_WITH_EXISTING_DRAFTS_ID, EffortConstants.RELEASE_NEWLY_OPEN_ID, true, 2, 2, 0)]           // Here, we can't allocate, because draft is registered for previous release.   
        public void GetNextVersion_ComputesRightVersionNumber(int specId, int relId, bool forUpload, int awaitedMajor, int awaitedTechnical, int awaitedEditorial )
        {
            var response = versionSvc.GetNextVersionForSpec(1, specId, relId, forUpload);

            Assert.IsNotNull(response);
            var newVersion = response.Result;
            Assert.IsNotNull(newVersion);

            Assert.AreEqual(awaitedMajor, response.Result.NewSpecVersion.MajorVersion);
            Assert.AreEqual(awaitedTechnical, response.Result.NewSpecVersion.TechnicalVersion);
            Assert.AreEqual(awaitedEditorial, response.Result.NewSpecVersion.EditorialVersion);
        }

        [TestCase(EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_OPEN_ID, false, 13, 2, 0)]
        public void GetNextVersion_CurrentVersionNumber(int specId, int relId, bool forUpload, int awaitedMajor, int awaitedTechnical, int awaitedEditorial)
        {
            var response = versionSvc.GetNextVersionForSpec(1, specId, relId, forUpload);

            Assert.IsNotNull(response);
            var newVersion = response.Result;
            Assert.IsNotNull(newVersion);

            Assert.AreEqual(awaitedMajor, response.Result.CurrentSpecVersion.MajorVersion);
            Assert.AreEqual(awaitedTechnical, response.Result.CurrentSpecVersion.TechnicalVersion);
            Assert.AreEqual(awaitedEditorial, response.Result.CurrentSpecVersion.EditorialVersion);
        }

        [Test]
        public void GetNextVersion_ReturnsReleaseAndSpecDetails()
        {
            var response = versionSvc.GetNextVersionForSpec(1, EffortConstants.SPECIFICATION_ACTIVE_ID, EffortConstants.RELEASE_OPEN_ID, false);

            Assert.AreEqual("Rel-13", response.Result.NewSpecVersion.Release.Code);
            Assert.AreEqual("22.101", response.Result.NewSpecVersion.Specification.Number);
        }

        [Test]
        public void GetNextVersion_ReportsErrorForInvalidRelease()
        {
            var response = versionSvc.GetNextVersionForSpec(1, EffortConstants.SPECIFICATION_ACTIVE_ID, 1, false);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Release_Does_Not_Exist, response.Report.ErrorList.First());
        }

        [Test]
        public void GetNextVersion_ReportsErrorForInvalidSpec()
        {
            var response = versionSvc.GetNextVersionForSpec(1, 1, EffortConstants.RELEASE_OPEN_ID, false);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Spec_Does_Not_Exist, response.Report.ErrorList.First());
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
