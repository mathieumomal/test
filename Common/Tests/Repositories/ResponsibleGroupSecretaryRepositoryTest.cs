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
    public class ResponsibleGroupSecretaryRepositoryTest
    {
        [TestFixture]
        public class PersonRepositoryTest : BaseTest
        {
            [Test]
            public void ResponsibleGroupSecretaryRepository_GetAll()
            {
                var repo = new ResponsibleGroupSecretaryRepository();
                repo.UoW = GetUnitOfWork();
                Assert.AreEqual(3, repo.All.ToList().Count);
            }

            [Test]
            [TestCase(2, 1)]//Two secretaries for the comittee id = 1 (TbId)
            [TestCase(1, 2)]//One secretary for the commitee id = 2
            [TestCase(0, 3)]//0 secretary for the ommitee id which isn't in the database
            public void ResponsibleGroupSecretaryRepository_FindAllByCommiteeId(int result, int tbId)
            {
                var repo = new ResponsibleGroupSecretaryRepository();
                repo.UoW = GetUnitOfWork();
                Assert.AreEqual(result, repo.FindAllByCommiteeId(tbId).Count());
            }

            [Test]
            [ExpectedException(typeof(NotImplementedException))]
            public void ResponsibleGroupSecretaryRepository_AllIncluding()
            {
                var repo = new ResponsibleGroupSecretaryRepository();
                repo.UoW = GetUnitOfWork();
                repo.AllIncluding();
            }

            [Test]
            [ExpectedException(typeof(NotImplementedException))]
            public void ResponsibleGroupSecretaryRepository_Find()
            {
                var repo = new ResponsibleGroupSecretaryRepository();
                repo.UoW = GetUnitOfWork();
                repo.Find(1);
            }

            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void ResponsibleGroupSecretaryRepository_InsertOrUpdate()
            {
                var repo = new ResponsibleGroupSecretaryRepository();
                repo.UoW = GetUnitOfWork();
                repo.InsertOrUpdate(new ResponsibleGroup_Secretary());
            }

            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void ResponsibleGroupSecretaryRepository_Delete()
            {
                var repo = new ResponsibleGroupSecretaryRepository();
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

                var dbSet = new ResponsibleGroupSecretaryFakeDBSet();
                dbSet.Add(new ResponsibleGroup_Secretary() { TbId = 1, Email = "one@capgemini.com", PersonId = 1 });
                dbSet.Add(new ResponsibleGroup_Secretary() { TbId = 1, Email = "onebis@capgemini.com", PersonId = 43 });
                dbSet.Add(new ResponsibleGroup_Secretary() { TbId = 2, Email = "two@capgemini.com", PersonId = 2 });

                iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
                iUltimateContext.Stub(ctx => ctx.ResponsibleGroupSecretaries).Return(dbSet);
                return iUnitOfWork;
            }
        }
    }
}
