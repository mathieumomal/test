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
    class HistoryRepositoryTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("GetHistoryData")]
        public void GetAll_Test(IDbSet<History> historyData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Histories).Return(historyData);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new HistoryRepository() { UoW = uow };
            var results = repo.All.ToList();

            Assert.AreEqual(8, results.Count);
            Assert.AreEqual(3, results.Where(x => x.Fk_ReleaseId == 1).Count());
            Assert.AreEqual(2, results.Where(x => x.Fk_ReleaseId == 2).Count());
            Assert.AreEqual(3, results.Where(x => x.Fk_ReleaseId == 3).Count());
            Assert.AreEqual(8, results.Where(x => x.Release == null).Count());
        }

        [Test, TestCaseSource("GetHistoryDataWithRelease")]
        public void GetAllIncluding_Test(IDbSet<History> historyData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Histories).Return(historyData);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new HistoryRepository() { UoW = uow };
            var results = repo.AllIncluding(t => t.Release).ToList();

            Assert.AreEqual(8, results.Count);
            Assert.AreEqual(3, results.Where(x => x.Fk_ReleaseId == 1).Count());
            Assert.AreEqual(2, results.Where(x => x.Fk_ReleaseId == 2).Count());
            Assert.AreEqual(3, results.Where(x => x.Fk_ReleaseId == 3).Count());
            Assert.AreEqual(8, results.Where(x => x.Release != null).Count());
        }

        [Test, TestCaseSource("GetHistoryDataWithRelease")]
        public void Find_Test(IDbSet<History> historyData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Histories).Return(historyData);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new HistoryRepository() { UoW = uow };
            var history = repo.Find(1);
            Assert.AreEqual(history, historyData.Find(1));

            history = repo.Find(6);
            Assert.AreEqual(history, historyData.Find(6));
        }

        [Test]
        public void Insert_Test()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            History history = new History() { Fk_ReleaseId = 3, Fk_PersonId = 16, HistoryText = "Release Closed" };

            var repo = new HistoryRepository() { UoW = uow };
            repo.InsertOrUpdate(history);

            mockDataContext.AssertWasCalled(x => x.SetAdded(history));
            mockDataContext.AssertWasNotCalled(x => x.SetModified(history));
        }

        [Test]
        public void Update_Test()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            History history = new History() { Pk_HistoryId = 2, Fk_ReleaseId = 3, Fk_PersonId = 16, HistoryText = "Release Closed" };

            var repo = new HistoryRepository() { UoW = uow };
            repo.InsertOrUpdate(history);

            mockDataContext.AssertWasNotCalled(x => x.SetAdded(history));
            mockDataContext.AssertWasCalled(x => x.SetModified(history));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot delete History entity")]
        public void Delete_Test()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new HistoryRepository() { UoW = uow };
            repo.Delete(1);
        }

        [Test]
        public void Dispose_Test()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var repo = new HistoryRepository() { UoW = uow };
            repo.Dispose();

            mockDataContext.AssertWasCalled(x => x.Dispose());
        }

        #endregion

        #region Data

        /// <summary>
        /// Provide History Data
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDbSet<History>> GetHistoryData
        {
            get
            {
                var historyDBSet = new HistoryFakeDBSet();

                historyDBSet.Add(new History() { Pk_HistoryId = 1, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Open", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis" });
                historyDBSet.Add(new History() { Pk_HistoryId = 2, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Frozen", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis" });
                historyDBSet.Add(new History() { Pk_HistoryId = 3, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Closed", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis" });
                historyDBSet.Add(new History() { Pk_HistoryId = 4, Fk_ReleaseId = 2, Fk_PersonId = 9, HistoryText = "Relese 2 Open", CreationDate = DateTime.Now, PersonName = "R. Cassaro" });
                historyDBSet.Add(new History() { Pk_HistoryId = 5, Fk_ReleaseId = 2, Fk_PersonId = 9, HistoryText = "Relese 2 Frozen", CreationDate = DateTime.Now, PersonName = "R. Cassaro" });
                historyDBSet.Add(new History() { Pk_HistoryId = 6, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Open", CreationDate = DateTime.Now, PersonName = "Michael Crosse" });
                historyDBSet.Add(new History() { Pk_HistoryId = 7, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Frozen", CreationDate = DateTime.Now, PersonName = "Michael Crosse" });
                historyDBSet.Add(new History() { Pk_HistoryId = 8, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Closed", CreationDate = DateTime.Now, PersonName = "Michael Crosse" });

                yield return (IDbSet<History>)historyDBSet;
            }
        }

        /// <summary>
        /// Provide History Data Along with Release
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDbSet<History>> GetHistoryDataWithRelease
        {
            get
            {
                var historyDBSet = new HistoryFakeDBSet();
                var release1 = new Release() { Pk_ReleaseId = 1, Name = "Release 1" };
                var release2 = new Release() { Pk_ReleaseId = 2, Name = "Release 2" };
                var release3 = new Release() { Pk_ReleaseId = 3, Name = "Release 3" };

                historyDBSet.Add(new History() { Pk_HistoryId = 1, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Open", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis", Release = release1 });
                historyDBSet.Add(new History() { Pk_HistoryId = 2, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Frozen", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis", Release = release1 });
                historyDBSet.Add(new History() { Pk_HistoryId = 3, Fk_ReleaseId = 1, Fk_PersonId = 3, HistoryText = "Relese 1 Closed", CreationDate = DateTime.Now, PersonName = "Gilbert Dupuis", Release = release1 });
                historyDBSet.Add(new History() { Pk_HistoryId = 4, Fk_ReleaseId = 2, Fk_PersonId = 9, HistoryText = "Relese 2 Open", CreationDate = DateTime.Now, PersonName = "R. Cassaro", Release = release2 });
                historyDBSet.Add(new History() { Pk_HistoryId = 5, Fk_ReleaseId = 2, Fk_PersonId = 9, HistoryText = "Relese 2 Frozen", CreationDate = DateTime.Now, PersonName = "R. Cassaro", Release = release2 });
                historyDBSet.Add(new History() { Pk_HistoryId = 6, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Open", CreationDate = DateTime.Now, PersonName = "Michael Crosse", Release = release3 });
                historyDBSet.Add(new History() { Pk_HistoryId = 7, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Frozen", CreationDate = DateTime.Now, PersonName = "Michael Crosse", Release = release3 });
                historyDBSet.Add(new History() { Pk_HistoryId = 8, Fk_ReleaseId = 3, Fk_PersonId = 21, HistoryText = "Relese 3 Closed", CreationDate = DateTime.Now, PersonName = "Michael Crosse", Release = release3 });

                yield return (IDbSet<History>)historyDBSet;
            }
        }

        #endregion
    }
}
