using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Business
{
    public class SpecVersionNumberValidatorTests : BaseEffortTest
    {
        #region init

        private ISpecVersionNumberValidator _specVersionNumberValidator;
        private const int UserWithRight = 1;
        private const int UserWithoutRight = 0;
        private const int ReleaseWhichExists = 1;
        private const int ReleaseWhichDoesntExist = 0;

        [SetUp]
        public void Init()
        {
            _specVersionNumberValidator = ManagerFactory.Resolve<ISpecVersionNumberValidator>();
            _specVersionNumberValidator.UoW = UoW;

            //Mock ISpecVersionsRepository : should return each time (for any specs) a version which already exist for number : 1.1.1
            var mockVersionRepo = MockRepository.GenerateMock<ISpecVersionsRepository>();
            mockVersionRepo.Stub(x => x.GetVersionsBySpecId(Arg<int>.Is.Anything))
                .Return(new List<SpecVersion>
                {
                    new SpecVersion {MajorVersion = 1, TechnicalVersion = 1, EditorialVersion = 1}
                });
            RepositoryFactory.Container.RegisterInstance(typeof (ISpecVersionsRepository), mockVersionRepo);

            //Mock ISpecVersionManager
            var mockSpecVersionManager = MockRepository.GenerateMock<ISpecVersionManager>();
            mockSpecVersionManager.Stub(
                x => x.CheckVersionNumbersEditAllowed(Arg<SpecVersion>.Is.Anything, Arg<int>.Is.Equal(UserWithRight)))
                .Return(new ServiceResponse<bool> {Result = true});
            mockSpecVersionManager.Stub(
                x => x.CheckVersionNumbersEditAllowed(Arg<SpecVersion>.Is.Anything, Arg<int>.Is.Equal(UserWithoutRight)))
                .Return(new ServiceResponse<bool> { Result = false, Report = new Report{ErrorList = new List<string>{Localization.RightError}, WarningList = new List<string>{Localization.GenericError}}});
            ManagerFactory.Container.RegisterInstance(typeof (ISpecVersionManager), mockSpecVersionManager);

            //Mock IReleaseRepository
            var mockReleaseRepository = MockRepository.GenerateMock<IReleaseRepository>();
            mockReleaseRepository.Stub(x => x.Find(Arg<int>.Is.Equal(ReleaseWhichExists))).Return(new Release{Version3g = 4});
            mockReleaseRepository.Stub(x => x.Find(Arg<int>.Is.Equal(ReleaseWhichDoesntExist))).Return(null);
            RepositoryFactory.Container.RegisterInstance(typeof (IReleaseRepository), mockReleaseRepository);
        }

        #endregion

        #region ALLOCATE tests

        [Test]
        public void CheckSpecVersionNumber_NominalAllocate_Draft()
        {
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, GetVersion(), SpecNumberValidatorMode.Allocate, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckSpecVersionNumber_ErrorAlreadyExist()
        {
            var version = GetVersion();
            version.MajorVersion = 1;
            version.TechnicalVersion = 1;
            version.EditorialVersion = 1;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, version, SpecNumberValidatorMode.Allocate, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Allocate_Error_Version_Not_Allowed, response.Report.ErrorList.First());
        }

        [Test]
        public void CheckSpecVersionNumber_NominalAllocate_Ucc()
        {
            var version = GetVersion();
            version.MajorVersion = 4;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, version, SpecNumberValidatorMode.Allocate, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckSpecVersionNumber_IncorrectMajor_Ucc()
        {
            var version = GetVersion();
            version.MajorVersion = 5;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, version, SpecNumberValidatorMode.Allocate, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Version_Major_Number_Allocate_Not_Valid, response.Report.ErrorList.First());
        }

        #endregion

        #region UPLOAD tests
        [Test]
        public void CheckSpecVersionNumber_NominalUpload_Draft()
        {
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, GetVersion(), SpecNumberValidatorMode.Upload, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckSpecVersionNumber_NominalUpload_Ucc()
        {
            var version = GetVersion();
            version.MajorVersion = 4;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, version, SpecNumberValidatorMode.Upload, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckSpecVersionNumber_ErrorInvalidMajorNumber_Ucc()
        {
            var version = GetVersion();
            version.MajorVersion = 5;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(null, version, SpecNumberValidatorMode.Upload, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Version_Major_Number_Allocate_Not_Valid, response.Report.ErrorList.First());
        }
        #endregion

        #region EDIT tests

        [Test(Description = "Nothing changed")]
        public void CheckSpecVersionNumber_NothingChanged()
        {
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(GetVersion(), GetVersion(), SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());

        }

        [Test(Description = "User doesn't have right to edit version number")]
        public void CheckSpecVersionNumber_DontHaveRight()
        {
            var version = GetVersion();
            version.MajorVersion = 2;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(GetVersion(), version, SpecNumberValidatorMode.Edit, UserWithoutRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.RightError + "\n" + Localization.GenericError, response.Report.ErrorList.First());
        }

        [Test(Description = "Version already exist")]
        public void CheckSpecVersionNumber_AlreadyExist()
        {
            var version = GetVersion();
            version.MajorVersion = 1;
            version.TechnicalVersion = 1;
            version.EditorialVersion = 1;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(GetVersion(), version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(string.Format(Localization.Version_Edit_Already_Exists, "1.1.1"), response.Report.ErrorList.First());
        }

        [Test(Description = "Release doesn't exist")]
        public void CheckSpecVersionNumber_UnknownRelease()
        {
            var version = GetVersion();
            version.MajorVersion = 2;
            version.Fk_ReleaseId = ReleaseWhichDoesntExist;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(GetVersion(), version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Error_Release_Does_Not_Exist, response.Report.ErrorList.First());
        }

        [Test(Description = "Draft to Draft : no error should occured")]
        public void CheckSpecVersionNumber_DraftToDraft()
        {
            var dbVersion = GetVersion();
            var version = GetVersion();
            version.MajorVersion = 2;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(2, dbVersion.MajorVersion);
            Assert.AreEqual(0, dbVersion.TechnicalVersion);
            Assert.AreEqual(0, dbVersion.EditorialVersion);
        }

        [Test(Description = "Draft to Ucc : no error should occured because major number is 4 and release have version3g number = 4")]
        public void CheckSpecVersionNumber_DraftToUcc()
        {
            var dbVersion = GetVersion();
            var version = GetVersion();
            version.MajorVersion = 4;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(4, dbVersion.MajorVersion);
            Assert.AreEqual(0, dbVersion.TechnicalVersion);
            Assert.AreEqual(0, dbVersion.EditorialVersion);
        }

        [Test(Description = "Draft to Ucc : an error should be raised because major number is 5 BUT release have version3g number = 4")]
        public void CheckSpecVersionNumber_DraftToUcc_Error()
        {
            var dbVersion = GetVersion();
            var version = GetVersion();
            version.MajorVersion = 5;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(string.Format(Localization.Version_Edit_Draft_Allowed_Major_Numbers, 4), response.Report.ErrorList.First());
        }

        [Test(Description = "Ucc to Ucc : an error should be raised because changed")]
        public void CheckSpecVersionNumber_UccToUcc_Error()
        {
            var dbVersion = GetVersion();
            dbVersion.MajorVersion = 6;
            var version = GetVersion();
            version.MajorVersion = 5;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Version_Edit_Ucc_Major_Cannot_Be_Update, response.Report.ErrorList.First());
        }

        [Test(Description = "Ucc to Ucc : no error should occured because major version number change for correct release")]
        public void CheckSpecVersionNumber_UccToUcc_FixReleaseAllowed()
        {
            var dbVersion = GetVersion();
            dbVersion.MajorVersion = 6;
            var version = GetVersion();
            version.MajorVersion = 4;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        [Test(Description = "Ucc to Ucc : no error should occured because major version number not changed (even if incorrect)")]
        public void CheckSpecVersionNumber_UccToUcc_MajorNotChanged()
        {
            var dbVersion = GetVersion();
            dbVersion.MajorVersion = 6;
            var version = GetVersion();
            version.MajorVersion = 6;
            var response = _specVersionNumberValidator.CheckSpecVersionNumber(dbVersion, version, SpecNumberValidatorMode.Edit, UserWithRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
        }

        #endregion

        #region data
        /// <summary>
        /// Get fake version
        /// </summary>
        /// <returns></returns>
        private SpecVersion GetVersion()
        {
            return new SpecVersion
            {
                Fk_SpecificationId = 1,
                Fk_ReleaseId = ReleaseWhichExists,
                Release = new Release { Version3g = 4 },
                MajorVersion = 1,
                TechnicalVersion = 0,
                EditorialVersion = 0
            };
        }
        #endregion
    }
}
