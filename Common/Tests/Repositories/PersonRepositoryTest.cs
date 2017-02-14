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
    public class PersonRepositoryTest : BaseTest
    {
        [Test]
        public void PersonRepository_GetAll()
        {
            var repo = new PersonRepository();
            repo.UoW = GetUnitOfWork();
            Assert.AreEqual(3, repo.All.ToList().Count);
            
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void PersonRepository_AllIncluding()
        {
            var repo = new PersonRepository();
            repo.UoW = GetUnitOfWork();
            repo.AllIncluding();
        }

        [Test]
        public void PersonRepository_Find()
        {
            var repo = new PersonRepository();
            repo.UoW = GetUnitOfWork();
            Assert.AreEqual("mangion", repo.Find(27904).Username);
        }

        [Test]
        public void PersonRepository_FindDeletedOrNot()
        {
            var repo = new PersonRepository {UoW = GetUnitOfWork()};
            Assert.AreEqual("johndoe", repo.FindDeletedOrNot(22).Username);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PersonRepository_InsertOrUpdate()
        {
            var repo = new PersonRepository();
            repo.UoW = GetUnitOfWork();
            repo.InsertOrUpdate(new View_Persons());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PersonRepository_Delete()
        {
            var repo = new PersonRepository();
            repo.UoW = GetUnitOfWork();
            repo.Delete(2);
        }

        [Test]
        public void GetPeopleEmails()
        {
            var repo = new PersonRepository {UoW = GetUnitOfWork()};
            var emails = repo.GetPeopleEmails(new List<int> {27904, 9568, 22});

            Assert.Contains("mathieu.mangion@etsi.org", emails);
            Assert.Contains("laurent.vreck@etsi.org", emails);
            Assert.Contains("john@etsi.org", emails);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new PersonFakeDBSet
            {
                new View_Persons()
                {
                    PERSON_ID = 27904,
                    Email = "mathieu.mangion@etsi.org",
                    FIRSTNAME = "Mathieu",
                    LASTNAME = "Mangion",
                    ORGA_ID = 10,
                    ORGA_NAME = "ETSI",
                    Username = "mangion",
                    DELETED_FLG = "N"
                },
                new View_Persons()
                {
                    PERSON_ID = 9568,
                    Email = "laurent.vreck@etsi.org",
                    FIRSTNAME = "Laurent",
                    LASTNAME = "Vreck",
                    ORGA_ID = 10,
                    ORGA_NAME = "ETSI",
                    Username = "vreck",
                    DELETED_FLG = "N"
                },
                new View_Persons()
                {
                    PERSON_ID = 22,
                    Email = "john@etsi.org",
                    FIRSTNAME = "doe",
                    LASTNAME = "john",
                    ORGA_ID = 10,
                    ORGA_NAME = "ETSI",
                    Username = "johndoe",
                    DELETED_FLG = "Y"
                }
            };

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.View_Persons).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
