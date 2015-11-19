using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Versions;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    public class SpecVersionSvcIntegrationTests: BaseEffortTest
    {
        #region GetVersionNumberWithSpecNumberByVersionId
        [TestCase(1, "", "", true)]
        [TestCase(400018, "22.110", "15.0.1", false)]
        [TestCase(428930, "22.101", "13.2.0", false)]
        public void GetVersionNumberWithSpecNumberByVersionId(int versionId, string expectedSpecNumber, string expectedVersionNumber, bool errorExpected)
        {
            var svc = ServicesFactory.Resolve<ISpecVersionService>();
            var result = svc.GetVersionNumberWithSpecNumberByVersionId(0, versionId);
            if (errorExpected)
            {
                Assert.AreEqual(1, result.Report.GetNumberOfErrors());
                Assert.AreEqual(Localization.Version_Not_Found, result.Report.ErrorList.FirstOrDefault());
                Assert.AreEqual(null, result.Result);
            }
            else
            {
                Assert.AreEqual(0, result.Report.GetNumberOfErrors());
                Assert.IsNotNull(result.Result);
                Assert.AreEqual(expectedVersionNumber, result.Result.Version);
                Assert.AreEqual(expectedSpecNumber, result.Result.SpecNumber);
            }
        }
        #endregion

        #region Update version
        [Test]
        public void UpdateVersion_SuccessCase()
        {
            //Given
            ProvideRightsToUpdateVersion();

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(GetBaseVersion(), 1);

            //Then
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.IsNotNull(result.Result);
            var newVersion = UoW.Context.SpecVersions.Include(x => x.Remarks).FirstOrDefault(x => x.Pk_VersionId == 428927);
            Assert.IsNotNull(newVersion);
            Assert.AreEqual(13, newVersion.MajorVersion);
            Assert.AreEqual(0, newVersion.TechnicalVersion);
            Assert.AreEqual(1, newVersion.EditorialVersion);
            Assert.IsTrue(newVersion.SupressFromSDO_Pub);
            Assert.IsTrue(newVersion.SupressFromMissing_List);
            Assert.AreEqual(22942, newVersion.Source);
            Assert.AreEqual(1, newVersion.Remarks.Count);
            Assert.IsTrue(newVersion.Remarks.First().IsPublic ?? false);
            Assert.AreEqual(1, newVersion.Remarks.First().Fk_PersonId);
            Assert.AreEqual("Remark", newVersion.Remarks.First().RemarkText);
        }

        [Test]
        public void UpdateDraftVersion_WithoutSource_SuccessCase()
        {
            //Given
            ProvideRightsToUpdateVersion();

            //When
            var svc = new SpecVersionService();
            var specVersion = svc.GetVersionsById(466666, 1).Key;
            specVersion.Remarks.Add(new Remark(){Fk_PersonId = 1 , RemarkText = "toto"});
            var result = svc.UpdateVersion(specVersion, 1);

            //Then
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.IsNotNull(result.Result);
            var newVersion =
                UoW.Context.SpecVersions.Include(x => x.Remarks).FirstOrDefault(x => x.Pk_VersionId == 466666);

            Assert.IsNotNull(newVersion);
            Assert.AreEqual(1, newVersion.MajorVersion);
            Assert.AreEqual(0, newVersion.TechnicalVersion);
            Assert.AreEqual(1, newVersion.EditorialVersion);
            Assert.IsNull(newVersion.Source);
            Assert.AreEqual(1, newVersion.Remarks.Count);
            Assert.AreEqual("toto", newVersion.Remarks.First().RemarkText);
        }

        [Test]
        public void UpdateVersion_RightError()
        {
            //Given
            var mockUserRight = MockRepository.GenerateMock<IRightsManager>();
            mockUserRight.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockUserRight);

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(GetBaseVersion(), 1);

            //Then
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.RightError, result.Report.ErrorList.First());
        }

        [Test]
        public void UpdateVersion_VersionUpdateValidation()
        {
            //Given
            ProvideRightsToUpdateVersion();
            var version = GetBaseVersion();
            version.MajorVersion = null;

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(version, 1);

            //Then
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.GenericError, result.Report.ErrorList.First());
        }

        [Test]
        public void UpdateVersion_VersionNotFound()
        {
            //Given
            ProvideRightsToUpdateVersion();
            var version = GetBaseVersion();
            version.Pk_VersionId = 9999;

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(version, 1);

            //Then
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Version_Not_Found, result.Report.ErrorList.First());
        }

        [Test]
        public void UpdateVersion_ReleaseChange_NominalCase()
        {
            //Mock version number validator
            var mockISpecVersionNumberValidator = MockRepository.GenerateMock<ISpecVersionNumberValidator>();
            mockISpecVersionNumberValidator.Stub(
                x =>
                    x.CheckSpecVersionNumber(Arg<SpecVersion>.Is.Anything, Arg<SpecVersion>.Is.Anything,
                        Arg<SpecNumberValidatorMode>.Is.Anything, Arg<int>.Is.Anything)).Return(new ServiceResponse<bool>{Result = true});
            ManagerFactory.Container.RegisterInstance(typeof (ISpecVersionNumberValidator),
                mockISpecVersionNumberValidator);

            //Given
            ProvideRightsToUpdateVersion();
            var version = GetBaseVersion();
            version.Fk_ReleaseId = 2884;
            version.MajorVersion = 1; // Draft version
            version.EditorialVersion = 99;

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(version, 1);

            //Then
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(2884, result.Result.Fk_ReleaseId);
        }

        [Test]
        public void UpdateVersion_ReleaseChange_FailIfVersionNotDraft()
        {
            //Given
            ProvideRightsToUpdateVersion();
            var version = GetBaseVersion();
            version.Fk_ReleaseId = 2884;
            version.MajorVersion = 13; // UCC version

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(version, 1);

            //Then
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.GenericError, result.Report.ErrorList.First());
        }

        [Test]
        public void UpdateVersion_ReleaseChange_FailIfReleaseNotProvided()
        {
            //Given
            ProvideRightsToUpdateVersion();
            var version = GetBaseVersion();
            version.Fk_ReleaseId = null;
            version.MajorVersion = 13; // UCC version

            //When
            var svc = new SpecVersionService();
            var result = svc.UpdateVersion(version, 1);

            //Then
            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.GenericError, result.Report.ErrorList.First());
        }

        [Test]
        public void UpdateVersion_NumbersVersionEdit_SuccessCase()
        {
            //Given
            ProvideRightsToUpdateVersion();

            //When
            var svc = new SpecVersionService();
            var draftVersion = GetDraftVersion();
            draftVersion.MajorVersion = 2;
            draftVersion.TechnicalVersion = 3;
            draftVersion.EditorialVersion = 4;

            var result = svc.UpdateVersion(draftVersion, 1);

            var specVersionService = ServicesFactory.Resolve<ISpecVersionService>();
            var dbversion = specVersionService.GetVersionsById(draftVersion.Pk_VersionId, 1).Key;

            //Then
            Assert.AreEqual(false, result.Report.ErrorList.Any());
            Assert.AreEqual(false, dbversion.Specification.IsUnderChangeControl);

            // New values
            Assert.AreEqual(dbversion.MajorVersion, 2);
            Assert.AreEqual(dbversion.TechnicalVersion, 3);
            Assert.AreEqual(dbversion.EditorialVersion, 4);
        }

        [Test]
        public void UpdateVersion_NumbersVersionEdit_UpgradeSpecUCC()
        {
            //Given
            ProvideRightsToUpdateVersion();

            //When
            var svc = new SpecVersionService();
            var draftVersion = GetDraftVersion();
            draftVersion.MajorVersion = 13;
            draftVersion.TechnicalVersion = 3;
            draftVersion.EditorialVersion = 4;

            var result = svc.UpdateVersion(draftVersion, 1);

            var specVersionService = ServicesFactory.Resolve<ISpecVersionService>();
            var dbversion = specVersionService.GetVersionsById(draftVersion.Pk_VersionId, 1).Key;

            // Old values
            Assert.AreEqual(dbversion.MajorVersion, 13);
            Assert.AreEqual(dbversion.TechnicalVersion, 3);
            Assert.AreEqual(dbversion.EditorialVersion, 4);

            Assert.AreEqual(false, result.Report.ErrorList.Any());
            Assert.AreEqual(true, dbversion.Specification.IsUnderChangeControl);
        }

        #endregion

        #region data

        private void ProvideRightsToUpdateVersion()
        {
            var mockUserRight = MockRepository.GenerateMock<IRightsManager>();
            var userRightsContainer = new UserRightsContainer();
            userRightsContainer.AddRight(Enum_UserRights.Versions_Edit);
            userRightsContainer.AddRight(Enum_UserRights.Specification_EditFull);
            mockUserRight.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRightsContainer);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockUserRight);
        }

        private void ProvideRightsToDeleteVersion(bool shouldProvideDeleteDraftVersionRight)
        {
            var mockUserRight = MockRepository.GenerateMock<IRightsManager>();
            var userRightsContainer = new UserRightsContainer();
            if (shouldProvideDeleteDraftVersionRight)
                userRightsContainer.AddRight(Enum_UserRights.Version_Draft_Delete);
            mockUserRight.Stub(x => x.GetRights(Arg<int>.Is.Anything)).Return(userRightsContainer);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockUserRight);
        }

        private SpecVersion GetBaseVersion()
        {
            return new SpecVersion
            {
                Pk_VersionId = 428927,
                MajorVersion = 13,
                TechnicalVersion = 0,
                EditorialVersion = 1,
                SupressFromSDO_Pub = true,
                SupressFromMissing_List = true,
                Source = 22942,
                Fk_ReleaseId = 2883,
                Remarks = new List<Remark>
                {
                    new Remark
                    {
                        CreationDate = DateTime.Now,
                        Fk_VersionId = 428927,
                        IsPublic = true,
                        PersonName = "MR",
                        RemarkText = "Remark"
                    }
                }
            };
        }

        private SpecVersion GetDraftVersion()
        {
            return new SpecVersion
            {
                Pk_VersionId = 428932,
                MajorVersion = 2,
                TechnicalVersion = 0,
                EditorialVersion = 0,
                SupressFromSDO_Pub = false,
                SupressFromMissing_List = false,
                Source = 29581,
                Fk_ReleaseId = 2883,
                Fk_SpecificationId = 136082,
                Remarks = new List<Remark>
                {
                    new Remark
                    {
                        CreationDate = DateTime.Now,
                        Fk_VersionId = 428932,
                        IsPublic = true,
                        PersonName = "MR",
                        RemarkText = "Remark"
                    }
                }
            };
        }
        #endregion

        #region Delete Draft Version
        [TestCase(466666, 1, true, true, Description = "Draft spec version and user has delete right")]
        [TestCase(428927, 1, false, true, Description = "UCC spec version and user has delete right")]
        [TestCase(466666, 1, true, false, Description = "Draft spec version and user has NOT delete right")]
        [TestCase(428927, 1, false, false, Description = "UCC spec version and user has NOT delete right")]
        public void DeleteVersion_VersionNotFound(int versionId, int personId, bool expectedDelete, bool hasDeleteRight)
        {
            //Given
            ProvideRightsToDeleteVersion(hasDeleteRight);
            var svc = new SpecVersionService();
            Assert.NotNull(svc.GetVersionsById(versionId, personId).Key);

            //Procceed
            var response = svc.DeleteVersion(personId, versionId);

            //Tests
            if (expectedDelete && hasDeleteRight)
            {
                Assert.AreEqual(0, response.Report.GetNumberOfErrors());
                Assert.AreEqual(true, response.Result);
            }
            else
            {
                Assert.AreEqual(1, response.Report.GetNumberOfErrors());
                Assert.AreEqual(false, response.Result);
            }
            
        }
        #endregion
    }
}
