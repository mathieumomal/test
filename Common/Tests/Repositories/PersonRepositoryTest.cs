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
    public class PersonRepositoryTest
    {
        [Test]
        public void PersonRepository_GetAll()
        {
            var repo = new PersonRepository(GetUnitOfWork());
            Assert.AreEqual(2, repo.All.ToList().Count);
            
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void PersonRepository_AllIncluding()
        {
            var repo = new PersonRepository(GetUnitOfWork());
            repo.AllIncluding();
        }

        [Test]
        public void PersonRepository_Find()
        {
            var repo = new PersonRepository(GetUnitOfWork());
            Assert.AreEqual("mangion", repo.Find(27904).Username);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PersonRepository_InsertOrUpdate()
        {
            var repo = new PersonRepository(GetUnitOfWork());
            repo.InsertOrUpdate(new View_Persons());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PersonRepository_Delete()
        {
            var repo = new PersonRepository(GetUnitOfWork());
            repo.Delete(2);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var dbSet = new PersonFakeDBSet();
            dbSet.Add (new View_Persons() { PERSON_ID=27904, Email="mathieu.mangion@etsi.org", FIRSTNAME="Mathieu", LASTNAME="Mangion", ORGA_ID=10, ORGA_NAME="ETSI", Username="mangion"});
            dbSet.Add (new View_Persons() { PERSON_ID=9568, Email="laurent.vreck@etsi.org", FIRSTNAME="Laurent", LASTNAME="Vreck", ORGA_ID=10, ORGA_NAME="ETSI", Username="vreck"});

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            iUltimateContext.Stub(ctx => ctx.View_Persons).Return(dbSet);
            return iUnitOfWork;
        }
    }
}
