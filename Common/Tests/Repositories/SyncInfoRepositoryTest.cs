using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class SyncInfoRepositoryTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("SyncInfoData")]
        public void SyncInfoRepository_GetAll(SyncInfoFakeDBSet syncInfoData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)syncInfoData).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            var allSyncInfos = repo.All;

            //Assert
            Assert.AreEqual(5, allSyncInfos.ToList().Count);
            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("SyncInfoData")]
        public void SyncInfoRepository_GetAllIncluding(SyncInfoFakeDBSet syncInfoData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)syncInfoData).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            var allSyncInfos = repo.AllIncluding();

            //Assert
            Assert.AreEqual(5, allSyncInfos.ToList().Count);
            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("SyncInfoData")]
        public void SyncInfoRepository_Find(SyncInfoFakeDBSet syncInfoData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)syncInfoData).Repeat.Times(3);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            var syncInfo1 = repo.Find(1);
            var syncInfo3 = repo.Find(3);
            var syncInfo5 = repo.Find(5);

            //Assert
            Assert.AreEqual(1, syncInfo1.Pk_SyncId);
            Assert.AreEqual("T1", syncInfo1.TerminalName);
            Assert.AreEqual(386, syncInfo1.Offline_PK_Id);
            Assert.AreEqual(396, syncInfo1.Fk_VersionId);

            Assert.AreEqual(3, syncInfo3.Pk_SyncId);
            Assert.AreEqual("T1", syncInfo3.TerminalName);
            Assert.AreEqual(388, syncInfo3.Offline_PK_Id);
            Assert.IsNull(syncInfo3.Fk_VersionId);

            Assert.AreEqual(5, syncInfo5.Pk_SyncId);
            Assert.AreEqual("T2", syncInfo5.TerminalName);
            Assert.AreEqual(386, syncInfo5.Offline_PK_Id);
            Assert.AreEqual(398, syncInfo5.Fk_VersionId);
            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot add or update a SyncInfo entity")]
        public void SyncInfoRepository_InsertOrUpdate()
        {
            //Arrange
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 386, Fk_VersionId = 396 };
            
            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            repo.InsertOrUpdate(syncInfo);           
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot delete SyncInfo entity")]
        public void SyncInfoRepository_Delete()
        {
            //Arrange
            int syncInfoId = 1;

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            repo.Delete(syncInfoId);
        }

        [Test, TestCaseSource("SyncInfoData")]
        public void SyncInfoRepository_FindWithTerminalName(SyncInfoFakeDBSet syncInfoData)
        {
            //Arrange
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)syncInfoData).Repeat.Times(9);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new SyncInfoRepository();
            repo.UoW = uow;
            var syncInfo_T1_386 = repo.Find("T1", 386);
            var syncInfo_T1_387 = repo.Find("T1", 387);
            var syncInfo_T1_388 = repo.Find("T1", 388);
            var syncInfo_T1_389 = repo.Find("T1", 389);
            var syncInfo_T2_386 = repo.Find("T2", 386);
            var syncInfo_T3_386 = repo.Find("T3", 386);
            var syncInfo_NULL_386 = repo.Find(null, 386);
            var syncInfo_T1_0 = repo.Find("T1", 0);
            var syncInfo_NULL_0 = repo.Find(null, 0);

            //Assert
            Assert.AreEqual(1, syncInfo_T1_386.ToList().Count);
            Assert.AreEqual(1, syncInfo_T1_387.ToList().Count);
            Assert.AreEqual(1, syncInfo_T1_388.ToList().Count);
            Assert.AreEqual(1, syncInfo_T1_389.ToList().Count);
            Assert.AreEqual(1, syncInfo_T2_386.ToList().Count);
            Assert.AreEqual(0, syncInfo_T3_386.ToList().Count);
            Assert.AreEqual(0, syncInfo_NULL_386.ToList().Count);
            Assert.AreEqual(0, syncInfo_T1_0.ToList().Count);
            Assert.AreEqual(0, syncInfo_NULL_0.ToList().Count);
            mockDataContext.VerifyAllExpectations();
        }

        #endregion

        #region Test Data

        /// <summary>
        /// Fake data for SyncInfo
        /// </summary>
        public IEnumerable<SyncInfoFakeDBSet> SyncInfoData
        {
            get
            {
                // Create DBSet
                var dbSet = new SyncInfoFakeDBSet();
                dbSet.Add(new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 386, Fk_VersionId = 396 });
                dbSet.Add(new SyncInfo() { Pk_SyncId = 2, TerminalName = "T1", Offline_PK_Id = 387, Fk_VersionId = 397 });
                dbSet.Add(new SyncInfo() { Pk_SyncId = 3, TerminalName = "T1", Offline_PK_Id = 388, Fk_VersionId = null });
                dbSet.Add(new SyncInfo() { Pk_SyncId = 4, TerminalName = "T1", Offline_PK_Id = 389, Fk_VersionId = null });
                dbSet.Add(new SyncInfo() { Pk_SyncId = 5, TerminalName = "T2", Offline_PK_Id = 386, Fk_VersionId = 398 });

                yield return dbSet;
            }
        }

        #endregion
    }
}
