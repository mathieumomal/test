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
    public class CommunityRepositoryTest
    {
        [Test]
        public void CommunityRepository_GetAll()
        {
            var repo = new CommunityRepository(GetUnitOfWork());
            Assert.AreEqual(2, repo.All.ToList().Count);
            
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void CommunityRepository_AllIncluding()
        {
            var repo = new CommunityRepository(GetUnitOfWork());
            repo.AllIncluding();
        }

        [Test]
        public void CommunityRepository_Find()
        {
            var repo = new CommunityRepository(GetUnitOfWork());
            Assert.AreEqual("S1", repo.Find(2).ShortName);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommunityRepository_InsertOrUpdate()
        {
            var repo = new CommunityRepository(GetUnitOfWork());
            repo.InsertOrUpdate(new Community());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommunityRepository_Delete()
        {
            var repo = new CommunityRepository(GetUnitOfWork());
            repo.Delete(2);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new CommunityFakeDBSet();
            dbSet.Add(new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" });
            dbSet.Add(new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" });

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.Communities).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
