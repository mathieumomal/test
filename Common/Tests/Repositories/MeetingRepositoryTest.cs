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
    public class MeetingRepositoryTest : BaseTest
    {
        [Test]
        public void MeetingRepository_GetAll()
        {
            var repo = new MeetingRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual(1, repo.All.ToList().Count);
            
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void MeetingRepository_AllIncluding()
        {
            var repo = new MeetingRepository() { UoW = GetUnitOfWork() };
            repo.AllIncluding();
        }

        [Test]
        public void MeetingRepository_Find()
        {
            var repo = new MeetingRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual("3GPPSA#12", repo.Find(1).MTG_REF);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MeetingRepository_InsertOrUpdate()
        {
            var repo = new MeetingRepository() { UoW = GetUnitOfWork() };
            repo.InsertOrUpdate(new Meeting());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MeetingRepository_Delete()
        {
            var repo = new MeetingRepository() { UoW = GetUnitOfWork() };
            repo.Delete(2);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new MeetingFakeDBSet();
            dbSet.Add(new Meeting() { MTG_ID = 1, MTG_REF = "3GPPSA#12", MTG_SEQNO = 12 });

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.Meetings).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
