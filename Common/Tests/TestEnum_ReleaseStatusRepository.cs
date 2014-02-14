using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;


namespace Tests
{
    //TextFixture <=> class contains Test
    [TestFixture]
    public class TestEnum_ReleaseStatusRepository
    {
        [Test]
        public void ReleaseStatus_GetAll()
        {
            var repo = new EnumReleaseRepository(GetUnitOfWork());
            Assert.AreEqual(3, repo.All.ToList().Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ReleaseStatus_AllIncluding()
        {
            var repo = new EnumReleaseRepository(GetUnitOfWork());
            repo.AllIncluding();
        }

        [Test]
        public void ReleaseStatus_Find()
        {
            var repo = new EnumReleaseRepository(GetUnitOfWork());
            Assert.AreSame("Frozen", repo.Find(2).ReleaseStatus);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ReleaseStatus_InsertOrUpdate()
        {
            var repo = new EnumReleaseRepository(GetUnitOfWork());
            repo.InsertOrUpdate(new Enum_ReleaseStatus());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReleaseStatus_Delete()
        {
            var repo = new EnumReleaseRepository(GetUnitOfWork());
            repo.Delete(2);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new Enum_ReleaseStatusFakeDBSet();
            dbSet.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" });
            dbSet.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" });
            dbSet.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" });

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
