using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using System.Data.Entity;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Tests.Business
{
    class SpecVersionsManagerTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionForASpecReleaseTest(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager();
            mng.UoW = uow;
            List<SpecVersion> result = mng.GetVersionsForASpecRelease(1, 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result[0].Pk_VersionId);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsBySpecIdTest(SpecVersionFakeDBSet specVersionsData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager();
            mng.UoW = uow;
            List<SpecVersion> result = mng.GetVersionsBySpecId(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Pk_VersionId);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsByIdTest(SpecVersionFakeDBSet specVersionsData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Versions_Upload);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
            Assert.IsTrue(mockRightsManager.GetRights(0).HasRight(Enum_UserRights.Versions_Upload));

            var statusOpen = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)new ReleaseFakeDBSet { 
                new Release() { Pk_ReleaseId = 1, Enum_ReleaseStatus = statusOpen }, new Release() { Pk_ReleaseId = 1, Enum_ReleaseStatus = statusOpen } 
            }.AsQueryable());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager();
            mng.UoW = uow;
            SpecVersion result = mng.GetSpecVersionById(1, 0).Key;
            Assert.IsNotNull(result);
            Assert.AreEqual("Location1", result.Location);
        }

        [Test, TestCaseSource("SpecVersionsData")]
        public void GetVersionsByWrongIdTest(SpecVersionFakeDBSet specVersionsData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Versions_Upload);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
            Assert.IsTrue(mockRightsManager.GetRights(0).HasRight(Enum_UserRights.Versions_Upload));

            var statusOpen = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)specVersionsData);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)new ReleaseFakeDBSet { 
                new Release() { Pk_ReleaseId = 1, Enum_ReleaseStatus = statusOpen }, new Release() { Pk_ReleaseId = 1, Enum_ReleaseStatus = statusOpen } 
            }.AsQueryable());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            SpecVersionsManager mng = new SpecVersionsManager();
            mng.UoW = uow;
            SpecVersion result = mng.GetSpecVersionById(0, 0).Key;
            Assert.IsNull(result);
        }

        [Test]
        public void SpecVersionsManager_InsertEntity()
        {
            //Arrange
            SpecVersion specVersion = new SpecVersion() { Pk_VersionId = 1, MajorVersion=1, EditorialVersion=0, TechnicalVersion=0 };
            string terminalName = "T1";

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new SpecVersionsManager();
            manager.UoW = uow;
            
            //Assert
            bool isSuccess = manager.InsertEntity(null, terminalName);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<SpecVersion>.Is.Anything));

            isSuccess = manager.InsertEntity(specVersion, terminalName);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<SpecVersion>.Matches(y => 
                   ((y.Pk_VersionId == specVersion.Pk_VersionId)
                && (y.MajorVersion == specVersion.MajorVersion)
                && (y.EditorialVersion == specVersion.EditorialVersion)
                && (y.TechnicalVersion == specVersion.TechnicalVersion)
                && (y.SyncInfoes.Count == 1)
                && (y.SyncInfoes.FirstOrDefault().TerminalName == terminalName)
                && (y.SyncInfoes.FirstOrDefault().Offline_PK_Id == specVersion.Pk_VersionId)))));
        }

        [Test]
        public void SpecVersionsManager_UpdateEntity()
        {
            //Arrange
            SpecVersion specVersion1 = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 1, TechnicalVersion = 0 };
            SpecVersion specVersion2 = new SpecVersion() { Pk_VersionId = 2, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };
            SpecVersion specVersionDB = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return(((IDbSet<SpecVersion>)new SpecVersionFakeDBSet() { specVersionDB }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new SpecVersionsManager();
            manager.UoW = uow;

            //Assert
            bool isSuccess = manager.UpdateEntity(null);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<SpecVersion>.Is.Anything));

            isSuccess = manager.UpdateEntity(specVersion2);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<SpecVersion>.Is.Anything));

            isSuccess = manager.UpdateEntity(specVersion1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<SpecVersion>.Matches(y =>
                   ((y.Pk_VersionId == specVersion1.Pk_VersionId)
                && (y.MajorVersion == specVersion1.MajorVersion)
                && (y.EditorialVersion == specVersion1.EditorialVersion)
                && (y.TechnicalVersion == specVersion1.TechnicalVersion)))));
        }

        [Test]
        public void SpecVersionsManager_DeleteEntity()
        {
            //Arrange
            SpecVersion specVersion1 = new SpecVersion() { Pk_VersionId = 1, MajorVersion = 1, EditorialVersion = 1, TechnicalVersion = 0 };
            SpecVersion specVersion2 = new SpecVersion() { Pk_VersionId = 2, MajorVersion = 1, EditorialVersion = 0, TechnicalVersion = 0 };           

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return(((IDbSet<SpecVersion>)new SpecVersionFakeDBSet() { specVersion1, specVersion2 }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new SpecVersionsManager();
            manager.UoW = uow;

            //Assert
            bool isSuccess = manager.DeleteEntity(0);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetDeleted(Arg<SpecVersion>.Is.Anything));

            isSuccess = manager.DeleteEntity(1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetDeleted(Arg<SpecVersion>.Matches(y =>
                   ((y.Pk_VersionId == specVersion1.Pk_VersionId)
                && (y.MajorVersion == specVersion1.MajorVersion)
                && (y.EditorialVersion == specVersion1.EditorialVersion)
                && (y.TechnicalVersion == specVersion1.TechnicalVersion)))));
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

                var statusOpen = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" };
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
                    Release = new Release()
                    {
                        Pk_ReleaseId = 1,
                        Enum_ReleaseStatus = statusOpen
                    },
                    Specification = new Specification()
                    {
                        Pk_SpecificationId = 1,
                        SpecificationRapporteurs = new List<SpecificationRapporteur>() { 
                            new SpecificationRapporteur() 
                            {
                                Pk_SpecificationRapporteurId=1,
                                IsPrime =true,
                                Fk_RapporteurId = 0
                            }
                        }
                    }
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
                    Release = new Release()
                    {
                        Pk_ReleaseId = 2,
                        Enum_ReleaseStatus = statusOpen
                    },
                    Specification = new Specification()
                    {
                        Pk_SpecificationId = 1,
                        SpecificationRapporteurs = new List<SpecificationRapporteur>() { 
                            new SpecificationRapporteur() 
                            {
                                Pk_SpecificationRapporteurId=1,
                                IsPrime =true,
                                Fk_RapporteurId = 0
                            }
                        }
                    }
                });

                yield return specVersionFakeDBSet;
            }
        } 

        #endregion
    }
}
