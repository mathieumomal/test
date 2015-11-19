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
using Etsi.Ultimate.Business.Versions;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Tests.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionAllocateTest : BaseEffortTest
    {
        const int USER_HAS_NO_RIGHT = 1;
        const int USER_HAS_RIGHT = 2;
		const int USER_HAS_SPEC_EDIT_RIGHT = 4;
		const int PRIMERAPPORTEUR_HAS_NO_RIGHT = 3;
       

        SpecVersionService _versionSvc;
        SpecVersion _myVersion;
        SpecVersion _myDraftVersion;
        private ISpecVersionNumberValidator _specVersionNumberValidatorMock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _versionSvc = new SpecVersionService();
            _myVersion = CreateVersion();
            _myDraftVersion = CreateDraftVersion();
            SetupMocks();

            //Mock version number validator
            _specVersionNumberValidatorMock = MockRepository.GenerateMock<ISpecVersionNumberValidator>();
            _specVersionNumberValidatorMock.Stub(
                x =>
                    x.CheckSpecVersionNumber(Arg<SpecVersion>.Is.Anything, Arg<SpecVersion>.Is.Anything,
                        Arg<SpecNumberValidatorMode>.Is.Anything, Arg<int>.Is.Anything))
                .Return(new ServiceResponse<bool> {Result = true});
            ManagerFactory.Container.RegisterInstance(typeof (ISpecVersionNumberValidator),
                _specVersionNumberValidatorMock);
        }

        [Test]
        public void Allocate_FailsIfUserHasNoRight()
        {
            var result = _versionSvc.AllocateVersion(USER_HAS_NO_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.RightError, result.ErrorList.First());
        }

        [Test]
        public void Allocate_NominalCase()
        {
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(0, result.GetNumberOfErrors());
            Assert.AreEqual(0, result.GetNumberOfWarnings());
            
            // Now let's try to fetch the version, check it's there
            var newlyCreatedVersion = RepositoryFactory.Resolve<IUltimateUnitOfWork>().Context.SpecVersions
                .Where(v => v.Fk_SpecificationId == EffortConstants.SPECIFICATION_ACTIVE_ID 
                    && v.Fk_ReleaseId == EffortConstants.RELEASE_OPEN_ID && v.MajorVersion == _myVersion.MajorVersion 
                    && v.TechnicalVersion == _myVersion.TechnicalVersion && v.EditorialVersion == _myVersion.EditorialVersion).FirstOrDefault();
            Assert.IsNotNull(newlyCreatedVersion);

            _specVersionNumberValidatorMock.AssertWasCalled(x => x.CheckSpecVersionNumber(Arg<SpecVersion>.Is.Anything, Arg<SpecVersion>.Is.Anything,
                        Arg<SpecNumberValidatorMode>.Is.Anything, Arg<int>.Is.Anything));
        }

        [Test]
        public void Allocate_NominalCase_UserHasNoRight_ButIsPrimeRapporteurOfRelatedSpec()
        {
            var result = _versionSvc.AllocateVersion(PRIMERAPPORTEUR_HAS_NO_RIGHT, _myVersion);
            Assert.AreEqual(0, result.GetNumberOfErrors());
            Assert.AreEqual(0, result.GetNumberOfWarnings());

            // Now let's try to fetch the version, check it's there
            var newlyCreatedVersion = RepositoryFactory.Resolve<IUltimateUnitOfWork>().Context.SpecVersions
                .Where(v => v.Fk_SpecificationId == EffortConstants.SPECIFICATION_ACTIVE_ID
                    && v.Fk_ReleaseId == EffortConstants.RELEASE_OPEN_ID && v.MajorVersion == _myVersion.MajorVersion
                    && v.TechnicalVersion == _myVersion.TechnicalVersion && v.EditorialVersion == _myVersion.EditorialVersion).FirstOrDefault();
            Assert.IsNotNull(newlyCreatedVersion);
        }

        [Test]
        public void Allocation_FailsIfNoReleaseAttached()
        {
            _myVersion.Fk_ReleaseId = null;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfReleaseDoesNotExist()
        {
            _myVersion.Fk_ReleaseId = 1;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Release_Does_Not_Exist, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfNoSpecAttached()
        {
            _myVersion.Fk_SpecificationId = 0;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_Missing_Release_Or_Specification, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecDoesNotExist()
        {
            _myVersion.Fk_SpecificationId = 1;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Error_Spec_Does_Not_Exist, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecIsNotActive()
        {
            _myVersion.Fk_SpecificationId = EffortConstants.SPECIFICATION_WITHDRAWN_ID;
            _myVersion.Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.RightError, result.ErrorList.First());
        }

        [Test]
        public void Allocation_FailsIfSpecReleaseDoesNotExist()
        {
            _myVersion.Fk_SpecificationId = EffortConstants.SPECIFICATION_ACTIVE_ID;
            _myVersion.Fk_ReleaseId = EffortConstants.RELEASE_WITH_NO_SPEC_RELEASE_ID;
            var result = _versionSvc.AllocateVersion(USER_HAS_RIGHT, _myVersion);
            Assert.AreEqual(1, result.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.Allocate_Error_SpecRelease_Does_Not_Exist, result.ErrorList.First());
        }


        [TestCase(13, true, Description = "Allocate spec version with major version 13")]
        [TestCase(2, false, Description = "Allocate spec version with major version 2")]
        public void Allocate_UpgradeAutomaticallyToUCC(int majorVersion, bool expectResult)
        {
            _myDraftVersion.MajorVersion = majorVersion;
            var result = _versionSvc.AllocateVersion(USER_HAS_SPEC_EDIT_RIGHT, _myDraftVersion);
            Assert.AreEqual(0, result.GetNumberOfErrors());
            Assert.AreEqual(0, result.GetNumberOfWarnings());

            // Now let's try to fetch the version, check it's there
            var newlyCreatedVersion = UoW.Context.SpecVersions
                .Where(v => v.Fk_SpecificationId == EffortConstants.SPECIFICATION_DRAFT_WITH_NO_DRAFT_ID
                    && v.Fk_ReleaseId == EffortConstants.RELEASE_OPEN_ID && v.MajorVersion == _myDraftVersion.MajorVersion
                    && v.TechnicalVersion == _myDraftVersion.TechnicalVersion && v.EditorialVersion == _myDraftVersion.EditorialVersion).FirstOrDefault();
            
            Assert.IsNotNull(newlyCreatedVersion);
            Assert.AreEqual(expectResult, newlyCreatedVersion.Specification.IsUnderChangeControl.Value);
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

        private SpecVersion CreateDraftVersion()
        {
            return new SpecVersion()
            {
                MajorVersion = 0,
                TechnicalVersion = 3,
                EditorialVersion = 0,
                Fk_ReleaseId = EffortConstants.RELEASE_OPEN_ID,
                Fk_SpecificationId = EffortConstants.SPECIFICATION_DRAFT_WITH_NO_DRAFT_ID
            };
        }

        private void SetupMocks()
        {
            var noRights = new UserRightsContainer();

            var allocateRights = new UserRightsContainer();
            allocateRights.AddRight(Enum_UserRights.Versions_Allocate);

            var editSpecRights = new UserRightsContainer();
            editSpecRights.AddRight(Enum_UserRights.Versions_Allocate);
            editSpecRights.AddRight(Enum_UserRights.Specification_EditFull);

            var rightsManager = MockRepository.GenerateMock<IRightsManager>();
            rightsManager.Stub(x => x.GetRights(USER_HAS_NO_RIGHT)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(PRIMERAPPORTEUR_HAS_NO_RIGHT)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(USER_HAS_RIGHT)).Return(allocateRights);
            rightsManager.Stub(x => x.GetRights(USER_HAS_SPEC_EDIT_RIGHT)).Return(editSpecRights);   

            ManagerFactory.Container.RegisterInstance<IRightsManager>(rightsManager);
        }


    }
}
