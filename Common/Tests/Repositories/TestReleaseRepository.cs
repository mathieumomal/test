using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using Etsi.Ultimate.Utils;


namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class TestReleaseRepository
    {
        [Test]
        public void Release_GetAll()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            var results = repo.All.ToList();

            Assert.AreEqual(3, results.Count);
        }

        [Test]
        public void Release_GetAllIncluding()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            var results = repo.AllIncluding( t => t.Enum_ReleaseStatus).ToList();

            Assert.AreEqual(3, results.Count);
            Assert.IsNotNull(results.First().Enum_ReleaseStatus);
            Assert.AreEqual("Open",results.First().Enum_ReleaseStatus.ReleaseStatus);
        }

        [Test]
        public void Release_Find()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual("Second release", repo.Find(2).Name);
        }

        [Test]
        public void Release_InsertOrUpdate()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            var releaseStatus = new Enum_ReleaseStatusRepository(GetUnitOfWork());
            Release myRelease = new Release();
            myRelease.Name = "release number 4";
            myRelease.Enum_ReleaseStatus = releaseStatus.Find(2);
            repo.InsertOrUpdate(myRelease);
            Assert.AreEqual(4, repo.All.ToList().Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Release_Delete()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() }; 
            repo.Delete(3);
        }
                       

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            //var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet_release = new ReleaseFakeDBSet();
            //Just essentials informations for the tests
            var openStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" };
            dbSet_release.Add(new Release() { Pk_ReleaseId = 1, Name = "First release", Fk_ReleaseStatus = 1, Enum_ReleaseStatus = openStatus  });
            dbSet_release.Add(new Release() { Pk_ReleaseId = 2, Name = "Second release", Fk_ReleaseStatus = 2 });
            dbSet_release.Add(new Release() { Pk_ReleaseId = 3, Name = "Third release", Fk_ReleaseStatus = 2 });

            var dbSet_releaseStatus = new Enum_ReleaseStatusFakeDBSet();
            //Just essentials informations for the tests
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" });
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" });
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" });

            iUltimateContext.Releases = dbSet_release;
            iUltimateContext.Enum_ReleaseStatus = dbSet_releaseStatus;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }

    }
}
