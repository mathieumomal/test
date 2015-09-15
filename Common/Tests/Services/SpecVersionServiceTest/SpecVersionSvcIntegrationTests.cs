using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
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

        #endregion

        #region data

        private void ProvideRightsToUpdateVersion()
        {
            var mockUserRight = MockRepository.GenerateMock<IRightsManager>();
            var userRightsContainer = new UserRightsContainer();
            userRightsContainer.AddRight(Enum_UserRights.Versions_Edit);
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
        #endregion
    }
}
