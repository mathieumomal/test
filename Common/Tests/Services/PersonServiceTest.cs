using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;

namespace Etsi.Ultimate.Tests.Services
{
    class PersonServiceTest : BaseTest
    {
        #region Tests

        [TestCase(53388, false)]
        [TestCase(27900, true)]  //MCC_MEMBER
        [TestCase(27904, false)]
        [TestCase(27905, true)] //MCC_MEMBER
        [TestCase(13, false)]
        public void IsUserMCCMember(int personID, bool result)
        {
            UserRolesFakeRepository userRolesFakeRepository = new UserRolesFakeRepository();
            var etsiRoles = userRolesFakeRepository.GetAllEtsiBasedRoles();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Users_Groups).Return((IDbSet<Users_Groups>)etsiRoles);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var personService = new PersonService();
            bool isUserMCCMember = personService.IsUserMCCMember(personID);

            Assert.AreEqual(result, isUserMCCMember);
        }

        #endregion
    }
}
