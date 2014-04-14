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
        [Test, TestCaseSource("GetPersonsWithIdsListNoPrimary")]
        public void GetByIdsTest_TestUknownIdAndNoPrimary(List<KeyValuePair<int,bool>> idsList,IDbSet<View_Persons> persons, int countExpected)
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


        [Test, TestCaseSource("GetPersonsWithIdsListWithPrimary")]
        public void GetByIdsTest_WithPrimary(List<KeyValuePair<int,bool>> idsList,IDbSet<View_Persons> persons, int firstIdExpected)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.View_Persons).Return(persons);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);


            var service = new PersonService();
            var results = service.GetByIds(idsList);

            Assert.AreEqual(firstIdExpected, results.First().PERSON_ID);
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
        #endregion

        #region Datas
        private IEnumerable<object[]> GetPersonsWithIdsListNoPrimary
        {
            get
            {
                var persons = GetPersonList();
                var idsListTree = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(1, false),
                    new KeyValuePair<int, bool>(4, false),
                    new KeyValuePair<int, bool>(6, false)
                };
                var idsListFour = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(1, false),
                    new KeyValuePair<int, bool>(4, false),
                    new KeyValuePair<int, bool>(2, false),
                    new KeyValuePair<int, bool>(6, false)
                };
                var idsListWithUnknownId = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(78, false),
                    new KeyValuePair<int, bool>(4, false),
                    new KeyValuePair<int, bool>(800, true),
                    new KeyValuePair<int, bool>(90, false)
                };
                yield return new object[] { idsListTree, (IDbSet<View_Persons>)persons, 3 };
                yield return new object[] { idsListFour, (IDbSet<View_Persons>)persons, 4 };
                yield return new object[] { idsListWithUnknownId, (IDbSet<View_Persons>)persons, 1 };
            }
        }

        private IEnumerable<object[]> GetPersonsWithIdsListWithPrimary
        {
            get
            {
                var persons = GetPersonList();
                var idsListTree = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(1, false),
                    new KeyValuePair<int, bool>(4, false),
                    new KeyValuePair<int, bool>(6, true)
                };
                var idsListFour = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(1, false),
                    new KeyValuePair<int, bool>(4, false),
                    new KeyValuePair<int, bool>(2, true),
                    new KeyValuePair<int, bool>(6, false)
                };
                var idsListWithUnknownId = new List<KeyValuePair<int, bool>>()
                {
                    new KeyValuePair<int, bool>(3, false),
                    new KeyValuePair<int, bool>(4, true),
                    new KeyValuePair<int, bool>(800, true),
                    new KeyValuePair<int, bool>(1, true)
                };
                yield return new object[] { idsListTree, (IDbSet<View_Persons>)persons, 6 };
                yield return new object[] { idsListFour, (IDbSet<View_Persons>)persons, 2 };
                yield return new object[] { idsListWithUnknownId, (IDbSet<View_Persons>)persons, 1 };
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
                yield return new object[] { "capgemini", (IDbSet<View_Persons>)persons, 6 };
            }
        }

        private static PersonFakeDBSet GetPersonList()
        {
            var persons = new PersonFakeDBSet { 
                new View_Persons { PERSON_ID = 1, FIRSTNAME = "Un", LASTNAME = "Martine", Email = "martine@capgemini.com" },
                new View_Persons { PERSON_ID = 2, FIRSTNAME = "Deux", LASTNAME = "paul", Email = "paul@capgemini.com" },
                new View_Persons { PERSON_ID = 3, FIRSTNAME = "Trois", LASTNAME = "bernard", Email = "bernard@capgemini.com" },
                new View_Persons { PERSON_ID = 4, FIRSTNAME = "Quatre", LASTNAME = "justine", Email = "justine@capgemini.com" },
                new View_Persons { PERSON_ID = 5, FIRSTNAME = "Cinq", LASTNAME = "pauline", Email = "pauline@capgemini.com" },
                new View_Persons { PERSON_ID = 6, FIRSTNAME = "Six", LASTNAME = "franck", Email = "franck@capgemini.com" },
            };
            return persons;
        }
        #endregion
    }
}
