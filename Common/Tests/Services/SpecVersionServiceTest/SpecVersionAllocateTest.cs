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
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Tests.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionAllocateTest : BaseEffortTest
    {
        const int USER_HAS_NO_RIGHT = 1;
        const int USER_HAS_RIGHT = 2;

       

        SpecVersionService versionSvc;
        SpecVersion myVersion;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            versionSvc = new SpecVersionService();
            myVersion = CreateVersion();
            SetupMocks();
        }

        [Test]
        public void Allocate_FailsIfUserHasNoRight()
        {
            var result = versionSvc.AllocateVersion(USER_HAS_NO_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.RightError, result.ErrorList.First());
        }

        [Test]
        public void Allocate_NominalCase()
        {
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(0, result.GetNumberOfErrors());
            Assert.AreEqual(0, result.GetNumberOfWarnings());
            
            // Now let's try to fetch the version, check it's there
            var newlyCreatedVersion = RepositoryFactory.Resolve<IUltimateUnitOfWork>().Context.SpecVersions
                .Where(v => v.Fk_SpecificationId == EffortConstants.SPECIFICATION_ACTIVE_ID 
                    && v.Fk_ReleaseId == EffortConstants.RELEASE_OPEN_ID && v.MajorVersion == myVersion.MajorVersion 
                    && v.TechnicalVersion == myVersion.TechnicalVersion && v.EditorialVersion == myVersion.EditorialVersion).FirstOrDefault();
            Assert.IsNotNull(newlyCreatedVersion);
        }

        [Test]
        public void Allocation_FailsIfNoReleaseAttached()
        {
            myVersion.Fk_ReleaseId = null;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfReleaseDoesNotExist()
        {
            myVersion.Fk_ReleaseId = 1;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Release_Does_Not_Exist, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfNoSpecAttached()
        {
            myVersion.Fk_SpecificationId = 0;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecDoesNotExist()
        {
            myVersion.Fk_SpecificationId = 1;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Spec_Does_Not_Exist, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecIsNotActive()
        {
            myVersion.Fk_SpecificationId = EffortConstants.SPECIFICATION_WITHDRAWN_ID;
            myVersion.Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.RightError, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecReleaseDoesNotExist()
        {
            myVersion.Fk_SpecificationId = EffortConstants.SPECIFICATION_ACTIVE_ID;
            myVersion.Fk_ReleaseId = EffortConstants.RELEASE_NEWLY_OPEN_ID;
            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_SpecRelease_Does_Not_Exist, result.ErrorList.First());
        }

        /**
         * In database, version 13.1.0 already exists, therefor it should not be able to allocate 13.0.0
         */
        [TestCase(1,4,3)]
        [TestCase(13,0,0)]
        [TestCase(13,1,0)]
        public void Allocation_FailsIfVersionIsSmallerThanExistingOneForTheRelease(int major, int technical, int editorial)
        {
            myVersion.Fk_SpecificationId = EffortConstants.SPECIFICATION_ACTIVE_ID;
            myVersion.Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID;
            myVersion.MajorVersion = major;
            myVersion.TechnicalVersion = technical;
            myVersion.EditorialVersion = editorial;

            var result = versionSvc.AllocateVersion(USER_HAS_RIGHT, myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_Version_Not_Allowed, result.ErrorList.First());
        }

        

        private SpecVersion CreateVersion()
        {
            return new SpecVersion()
            {
                MajorVersion = 13,
                TechnicalVersion = 3,
                EditorialVersion = 0,
                Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID,
                Fk_SpecificationId = EffortConstants.SPECIFICATION_ACTIVE_ID

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
