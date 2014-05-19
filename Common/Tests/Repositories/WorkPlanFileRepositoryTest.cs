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
    [TestFixture]
    class WorkPlanFileRepositoryTest : BaseTest
    {
        [Test]
        public void WorkPlanFileRepository_GetAll()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new WorkPlanFileRepository(uow);
            repo.UoW = uow;
            Assert.AreEqual(2, repo.All.ToList().Count);

        }

        [Test]
        public void WorkPlanFileRepository_Find()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new WorkPlanFileRepository(uow);
            repo.UoW = uow;
            Assert.AreEqual("Path1", repo.Find(1).WorkPlanFilePath);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WorkPlanFileRepository_Delete()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new WorkPlanFileRepository(uow);
            repo.UoW = uow;
            repo.Delete(0);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void WorkPlanFileRepository_AllIncluding()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new WorkPlanFileRepository(uow);
            repo.UoW = uow;
            repo.AllIncluding();
        }

        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new WorkPlanFileFakeDBSet();
            dbSet.Add(new WorkPlanFile() { Pk_WorkPlanFileId = 1, CreationDate = DateTime.Today.AddDays(-3), WorkPlanFilePath="Path1" });
            dbSet.Add(new WorkPlanFile() { Pk_WorkPlanFileId = 2, CreationDate = DateTime.Today.AddDays(-1), WorkPlanFilePath = "Path2" });

            iUltimateContext.Stub(ctx => ctx.WorkPlanFiles).Return(dbSet);
            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);

            return iUnitOfWork;
        }
    }
}
