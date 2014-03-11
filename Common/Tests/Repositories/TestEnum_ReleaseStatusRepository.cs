using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web;


namespace Etsi.Ultimate.Tests.Repositories
{
    //TextFixture <=> class contains Test
    [TestFixture]
    public class TestEnum_ReleaseStatusRepository : BaseTest
    {
        [Test]
        public void ReleaseStatus_GetAll()
        {
            var repo = new Enum_ReleaseStatusRepository(GetUnitOfWork());
            Assert.AreEqual(3, repo.All.ToList().Count);
            
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ReleaseStatus_AllIncluding()
        {
            var repo = new Enum_ReleaseStatusRepository(GetUnitOfWork());
            repo.AllIncluding();
        }

        [Test]
        public void ReleaseStatus_Find()
        {
            var repo = new Enum_ReleaseStatusRepository(GetUnitOfWork());
            Assert.AreSame("Frozen", repo.Find(2).ReleaseStatus);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ReleaseStatus_InsertOrUpdate()
        {
            var repo = new Enum_ReleaseStatusRepository(GetUnitOfWork());
            repo.InsertOrUpdate(new Enum_ReleaseStatus());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReleaseStatus_Delete()
        {
            var repo = new Enum_ReleaseStatusRepository(GetUnitOfWork());
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
