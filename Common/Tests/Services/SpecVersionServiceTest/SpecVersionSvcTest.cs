using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecVersionSvcTest : BaseTest
    {
        #region Variables

        private const int USERID = 0; 

        #endregion

        #region Tests

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsBySpecId(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            List<SpecVersion> result = versionsSvc.GetVersionsBySpecId(1);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Remarks.Count);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsForSpecRelease(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            List<SpecVersion> result = versionsSvc.GetVersionsForSpecRelease(1, 2);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Location2", result[0].Location);
            Assert.AreEqual("Remark 2", result[0].Remarks.ToList()[0].RemarkText);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsById(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var versionsSvc = new SpecVersionService();
            SpecVersion result = versionsSvc.GetVersionsById(2, 0).Key;
            Assert.AreEqual("Location2", result.Location);
            Assert.AreEqual("Remark 2", result.Remarks.ToList()[0].RemarkText);
        }

        [Test]
        public void SpecVersionService_InsertEntity()
        {
            //Arrange
            int online_PrimaryKey = 0;
            string terminalName = "T1";
            SpecVersion specVersion1 = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };
            SpecVersion specVersion2 = new SpecVersion() { Pk_VersionId = 2, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 1, Fk_VersionId = 1 };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)new SyncInfoFakeDBSet() { syncInfo });
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new SpecVersionService();

            //Assert
            //[1] Fault call
            online_PrimaryKey = service.InsertEntity(null, terminalName);
            Assert.AreEqual(0, online_PrimaryKey);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<SpecVersion>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            //[2] Call for already processed record
            online_PrimaryKey = service.InsertEntity(specVersion1, terminalName);
            Assert.AreEqual(1, online_PrimaryKey);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<SpecVersion>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            //[3] Call for new insert
            online_PrimaryKey = service.InsertEntity(specVersion2, terminalName);
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<SpecVersion>.Matches(y =>
                   ((y.Pk_VersionId == specVersion2.Pk_VersionId)
                && (y.MajorVersion == specVersion2.MajorVersion)
                && (y.EditorialVersion == specVersion2.EditorialVersion)
                && (y.TechnicalVersion == specVersion2.TechnicalVersion)
                && (y.SyncInfoes.Count == 1)
                && (y.SyncInfoes.FirstOrDefault().TerminalName == terminalName)
                && (y.SyncInfoes.FirstOrDefault().Offline_PK_Id == specVersion2.Pk_VersionId)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void SpecVersionService_UpdateEntity()
        {
            //Arrange
            SpecVersion specVersion1 = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 1, TechnicalVersion = 0 };
            SpecVersion specVersion2 = new SpecVersion() { Pk_VersionId = 2, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };
            SpecVersion specVersionDB = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return(((IDbSet<SpecVersion>)new SpecVersionFakeDBSet() { specVersionDB }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new SpecVersionService();

            //Assert
            bool isSuccess = service.UpdateEntity(null);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<SpecVersion>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            isSuccess = service.UpdateEntity(specVersion2);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<SpecVersion>.Is.Anything));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());

            isSuccess = service.UpdateEntity(specVersion1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<SpecVersion>.Matches(y =>
                   ((y.Pk_VersionId == specVersion1.Pk_VersionId)
                && (y.MajorVersion == specVersion1.MajorVersion)
                && (y.EditorialVersion == specVersion1.EditorialVersion)
                && (y.TechnicalVersion == specVersion1.TechnicalVersion)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void SpecVersionService_DeleteEntity()
        {
            //Arrange
            SpecVersion specVersion1 = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 1, TechnicalVersion = 0 };
            SpecVersion specVersion2 = new SpecVersion() { Pk_VersionId = 2, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return(((IDbSet<SpecVersion>)new SpecVersionFakeDBSet() { specVersion1, specVersion2 }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new SpecVersionService();

            //Assert
            bool isSuccess = service.DeleteEntity(0);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetDeleted(Arg<SpecVersion>.Is.Anything));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());

            isSuccess = service.DeleteEntity(1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetDeleted(Arg<SpecVersion>.Matches(y =>
                   ((y.Pk_VersionId == specVersion1.Pk_VersionId)
                && (y.MajorVersion == specVersion1.MajorVersion)
                && (y.EditorialVersion == specVersion1.EditorialVersion)
                && (y.TechnicalVersion == specVersion1.TechnicalVersion)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void CountVersionsPendingUploadByReleaseId()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)GetSpecVersions());
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleasesForCountVersionPendingUpload());

            var releaseMock = MockRepository.GenerateMock<IReleaseManager>();
            releaseMock.Stub(r => r.GetAllReleases(0)).Return(new KeyValuePair<List<Release>, UserRightsContainer>(GetReleases().ToList(), new UserRightsContainer()));
            ManagerFactory.Container.RegisterInstance<IReleaseManager>(releaseMock);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var versionsSvc = new SpecVersionService();
            var result = versionsSvc.CountVersionsPendingUploadByReleaseId(1);
            Assert.AreEqual(1, result);
        }

        #endregion

        #region TestData

        /// <summary>
        /// Get SpecVersions DATA
        /// </summary>
        public IEnumerable<SpecVersionFakeDBSet> SpecVersionsData
        {
            get
            {
                var rmkDbSet = new RemarkFakeDbSet();
                rmkDbSet.Add(new Remark() { Pk_RemarkId = 1, RemarkText = "Remark 1", Fk_VersionId = 1 });
                rmkDbSet.Add(new Remark() { Pk_RemarkId = 2, RemarkText = "Remark 2", Fk_VersionId = 2 });

                SpecVersionFakeDBSet specVersionFakeDBSet = new SpecVersionFakeDBSet();

                specVersionFakeDBSet.Add(new SpecVersion()
                {
                    Pk_VersionId = 1,
                    Multifile = false,
                    ForcePublication = false,
                    Location = "Location1",
                    Fk_ReleaseId = 1,
                    Fk_SpecificationId = 1,
                    Remarks = new List<Remark>() { rmkDbSet.ToList()[0] },
                    Release = new Release(),
                    Specification = new Specification()
                });
                specVersionFakeDBSet.Add(new SpecVersion()
                {
                    Pk_VersionId = 2,
                    Multifile = false,
                    ForcePublication = false,
                    Location = "Location2",
                    Fk_ReleaseId = 2,
                    Fk_SpecificationId = 1,
                    Remarks = new List<Remark>() { rmkDbSet.ToList()[1] },
                    Release = new Release(),
                    Specification = new Specification(),
                });

                yield return specVersionFakeDBSet;
            }
        }

        /// <summary>
        /// Get Fake SpecVersion Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecVersionFakeDBSet GetSpecVersions()
        {
            var versionDbSet = new SpecVersionFakeDBSet();

            var version = new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "L1",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 1, Fk_VersionId = 1, RemarkText = "R1" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };
            var version2 = new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = null,
                MajorVersion = 10,
                TechnicalVersion = 3,
                EditorialVersion = 1,
                Source = 1,
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1,
            };
            var version3 = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = "aa",
                MajorVersion = 10,
                TechnicalVersion = 4,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1
            };
            versionDbSet.Add(version);
            versionDbSet.Add(version2);
            versionDbSet.Add(version3);

            return versionDbSet;
        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "1", IsUnderChangeControl = true, IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "2", IsUnderChangeControl = true, IsActive = true });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "2", IsUnderChangeControl = false, IsActive = true });
            return list;
        }

        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" }, Version3g=10 });
            list.Add(new Release() { Pk_ReleaseId = 2, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 3, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 4, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            return list;
        }

        private IDbSet<Enum_ReleaseStatus> GetReleaseStatus()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen", Description = "Frozen" });
            return list;
        }

        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 3, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 2, Fk_ReleaseId = 2, isTranpositionForced = true });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 4, Fk_SpecificationId = 2, Fk_ReleaseId = 1, isTranpositionForced = false, Specification = new Specification() { Pk_SpecificationId = 2, Number = "2", IsUnderChangeControl = true, IsActive = true } });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 5, Fk_SpecificationId = 2, Fk_ReleaseId = 4, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 6, Fk_SpecificationId = 3, Fk_ReleaseId = 1, isTranpositionForced = true, Specification = new Specification() { Pk_SpecificationId = 3, Number = "2", IsUnderChangeControl = false, IsActive = true } });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 7, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isTranpositionForced = false });
            return list;
        }

        private IDbSet<Specification_Release> GetSpecReleasesForCountVersionPendingUpload()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 7,
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1,
                isTranpositionForced = false,
                Specification = new Specification()
                {
                    Pk_SpecificationId = 1,
                    Number = "1",
                    IsUnderChangeControl = true,
                    IsActive = true,
                    Versions = new List<SpecVersion>() { new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "L1",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 1, Fk_VersionId = 1, RemarkText = "R1" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            }, 
            new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = null,
                MajorVersion = 20,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 10, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1,
                ETSI_WKI_ID = 1,
            }}
                }
            });
            return list;
        }


        #endregion
    }
}
