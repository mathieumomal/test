using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Tests.FakeManagers;
using Etsi.Ultimate.DataAccess;
using Rhino.Mocks;
using System.Data.Entity;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.Services
{
    public class PersonServiceTest : BaseTest
    {
        #region Tests
        [Test, TestCaseSource("GetPersonsWithIdsList")]
        public void GetByIdsTest_TestUknownIdAndNoPrimary(List<int> idsList,IDbSet<View_Persons> persons, int countExpected)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.GetByIds(idsList);

            Assert.AreEqual(countExpected, results.Count);
        }

        [Test, TestCaseSource("GetPersonsWithIdsListVerifyOrder")]
        public void GetByIdsTest_TestUknownIdAndNoPrimary(List<int> idsList, IDbSet<View_Persons> persons, List<int> listId)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.GetByIds(idsList);

            var temp = new List<int>();
            foreach (View_Persons person in results)
            {
                temp.Add(person.PERSON_ID);
            }
            temp.Reverse();
            Assert.AreEqual(listId, temp);
        }

        [Test, TestCaseSource("GetPersonsBySearchNoDeleted")]
        public void LookForTestNoDeletedPerson(String keywords, IDbSet<View_Persons> persons, int personFoundExpected)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.LookFor(keywords);

            Assert.AreEqual(personFoundExpected, results.Count);
        }

        [Test, TestCaseSource("GetPersonsBySearch")]
        public void LookForTest(String keywords, IDbSet<View_Persons> persons, int personFoundExpected)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.LookFor(keywords);

            Assert.AreEqual(personFoundExpected, results.Count);
        }

        [Test, TestCaseSource("GetPersonById")]
        public void FindPersonTest(int id, IDbSet<View_Persons> persons, String nameExpected)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.FindPerson(id);

            Assert.AreEqual(nameExpected, results.LASTNAME);
        }

        [Test]
        public void GetPersonDisplayNameTest()
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return((IDbSet<View_Persons>)GetPersonList());
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.GetPersonDisplayName(1);

            Assert.AreEqual("Un Martine", results);
        }

        [Test]
        public void GetChairmanIdByCommityId()
        {
            var userRoleRepo = MockRepository.GenerateMock<IUserRolesRepository>(); 
            userRoleRepo.Stub(x => x.GetChairmanIdByCommitteeId(1)).Return(2);
            RepositoryFactory.Container.RegisterInstance(userRoleRepo);

            var service = new PersonService();
            var results = service.GetChairmanIdByCommityId(1);

            Assert.AreEqual(2, results);
        }
        #endregion

        #region Datas
        private IEnumerable<object[]> GetPersonsWithIdsList
        {
            get
            {
                var persons = GetPersonList();
                var idsListTree = new List<int>()
                {
                    1,4,6
                };
                var idsListFour = new List<int>()
                {
                    1,4,2,6
                };
                var idsListWithUnknownId = new List<int>()
                {
                    78,4,800,90
                };
                yield return new object[] { idsListTree, (IDbSet<View_Persons>)persons, 3 };
                yield return new object[] { idsListFour, (IDbSet<View_Persons>)persons, 4 };
                yield return new object[] { idsListWithUnknownId, (IDbSet<View_Persons>)persons, 1 };
            }
        }

        private IEnumerable<object[]> GetPersonsWithIdsListVerifyOrder
        {
            get
            {
                var persons = GetPersonList();
                var idsListTree = new List<int>()
                {
                    1,4,6
                };
                var idsListFour = new List<int>()
                {
                    1,4,2,6
                };
                var idsListWithUnknownId = new List<int>()
                {
                    78,4,800,90
                };
                yield return new object[] { idsListTree, (IDbSet<View_Persons>)persons, new List<int>(){1,4,6} };
                yield return new object[] { idsListFour, (IDbSet<View_Persons>)persons, new List<int>() { 1, 4, 2, 6 } };
                yield return new object[] { idsListWithUnknownId, (IDbSet<View_Persons>)persons, new List<int>() { 4 } };
            }
        }

        private IEnumerable<object[]> GetPersonsBySearch
        {
            get
            {
                var persons = GetPersonList();
                yield return new object[] { "un", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "uN", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "justine", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "DeUx", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "capgemini", (IDbSet<View_Persons>)persons, 7 };
                yield return new object[] { "Un ma", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "paul de", (IDbSet<View_Persons>)persons, 1 };
                yield return new object[] { "bernard t", (IDbSet<View_Persons>)persons, 2 };
            }
        }

        private IEnumerable<object[]> GetPersonsBySearchNoDeleted
        {
            get
            {
                var persons = GetPersonList();
                yield return new object[] { "franck", (IDbSet<View_Persons>)persons, 1 };
            }
        }

        private IEnumerable<object[]> GetPersonById
        {
            get
            {
                var persons = GetPersonList();
                yield return new object[] { 3, (IDbSet<View_Persons>)persons, "bernard" };
            }
        }

        private static PersonFakeDBSet GetPersonList()
        {
            var persons = new PersonFakeDBSet { 
                new View_Persons { PERSON_ID = 1, FIRSTNAME = "Un", LASTNAME = "Martine", Email = "martine@capgemini.com", DELETED_FLG="N"  },
                new View_Persons { PERSON_ID = 2, FIRSTNAME = "Deux", LASTNAME = "paul", Email = "paul@capgemini.com", DELETED_FLG="N"  },
                new View_Persons { PERSON_ID = 3, FIRSTNAME = "Trois", LASTNAME = "bernard", Email = "bernard@capgemini.com", DELETED_FLG="N"  },
                new View_Persons { PERSON_ID = 4, FIRSTNAME = "Quatre", LASTNAME = "justine", Email = "justine@capgemini.com", DELETED_FLG="N"  },
                new View_Persons { PERSON_ID = 5, FIRSTNAME = "Cinq", LASTNAME = "pauline", Email = "pauline@capgemini.com", DELETED_FLG="N"  },
                new View_Persons { PERSON_ID = 6, FIRSTNAME = "Six", LASTNAME = "franck", Email = "franck@capgemini.com", DELETED_FLG="N" },
                new View_Persons { PERSON_ID = 7, FIRSTNAME = "sept", LASTNAME = "franck", Email = "delete@capgemini.com", DELETED_FLG="Y" },
                new View_Persons { PERSON_ID = 8, FIRSTNAME = "tom", LASTNAME = "bernard", Email = "bernard.tom@capgemini.com", DELETED_FLG="N" }
            };
            return persons;
        }
        #endregion
    }
}
