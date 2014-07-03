using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class ResponsibleGroupChairmanRepositoryTest
    {
        [Test]
        public void ResponsibleGroupChairmanRepository_GetAll()
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            Assert.AreEqual(3, repo.All.ToList().Count);
        }

        [Test]
        [TestCase(2, 1)]//Two chairmans for the comittee id = 1 (TbId)
        [TestCase(1, 2)]//One chairman for the commitee id = 2
        [TestCase(0, 3)]//0 chairman for the ommitee id which isn't in the database
        public void ResponsibleGroupChairmanRepository_FindAllByCommiteeId(int result, int tbId)
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            Assert.AreEqual(result, repo.FindAllByCommiteeId(tbId).Count());
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ResponsibleGroupChairmanRepository_AllIncluding()
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            repo.AllIncluding();
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ResponsibleGroupChairmanRepository_Find()
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            repo.Find(1);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ResponsibleGroupChairmanRepository_InsertOrUpdate()
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            repo.InsertOrUpdate(new ResponsibleGroup_Chairman());
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ResponsibleGroupChairmanRepository_Delete()
        {
            var repo = new ResponsibleGroupChairmanRepository();
            repo.UoW = GetUnitOfWork();
            repo.Delete(2);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new ResponsibleGroupChairmanFakeDBSet();
            dbSet.Add(new ResponsibleGroup_Chairman() { TbId = 1, Email = "one@capgemini.com", PersonId = 1 });
            dbSet.Add(new ResponsibleGroup_Chairman() { TbId = 1, Email = "onebis@capgemini.com", PersonId = 43 });
            dbSet.Add(new ResponsibleGroup_Chairman() { TbId = 2, Email = "two@capgemini.com", PersonId = 2 });

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.ResponsibleGroupChairmans).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
