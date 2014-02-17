using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using Etsi.Ultimate.Utils;


namespace Tests
{
    [TestFixture]
    class TestReleaseRepository
    {
        [Test]
        public void Release_GetAll()
        {
            //var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            var repo = DependencyFactory.Resolve<IReleaseRepository>();
            //DependencyFactory.Container.RegisterType<IReleaseRepository, ReleaseMockRepository>
            repo.UoW = GetUnitOfWork(); 

            Assert.AreEqual(3, repo.All.ToList().Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Release_GetAllIncluding()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            repo.AllIncluding();
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
            var releaseStatus = new EnumReleaseRepository(GetUnitOfWork());
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

        [Test]
        public void Release_GetAllReleaseByIdReleaseStatus()
        {
            var repo = new ReleaseRepository() { UoW = GetUnitOfWork() };
            var releaseStatus = new EnumReleaseRepository(GetUnitOfWork());
            var releasesFrozen = repo.GetAllReleaseByIdReleaseStatus(releaseStatus.Find(2));
            var releasesOpen = repo.GetAllReleaseByIdReleaseStatus(releaseStatus.Find(1));

            Assert.AreEqual(2, releasesFrozen.Count);
            Assert.AreEqual(1, releasesOpen.Count);

            Assert.AreEqual("First release", releasesOpen.First<Release>().Name);
        }
        

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet_release = new ReleaseFakeDBSet();
            //Just essentials informations for the tests
            dbSet_release.Add(new Release() { Pk_ReleaseId = 1, Name = "First release", Fk_ReleaseStatus = 1 });
            dbSet_release.Add(new Release() { Pk_ReleaseId = 2, Name = "Second release", Fk_ReleaseStatus = 2 });
            dbSet_release.Add(new Release() { Pk_ReleaseId = 3, Name = "Third release", Fk_ReleaseStatus = 2 });

            var dbSet_releaseStatus = new Enum_ReleaseStatusFakeDBSet();
            //Just essentials informations for the tests
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" });
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" });
            dbSet_releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" });


            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.Releases).Return(dbSet_release);
            iUltimateContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(dbSet_releaseStatus);
            return iUnitOfWork;
        }

    }
}
