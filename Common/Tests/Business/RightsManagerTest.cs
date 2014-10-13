using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils.Core;
using NUnit.Framework;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Tests.Business
{
    class RightsManagerTest: BaseTest
    {
        private static readonly string CACHE_STRING = "ULTIMATE_BIZ_USER_RIGHTS_";

        [Test]
        public void GetRights_RetrievesRightsForAnonymous()
        {
            InitializeUserRightsMock();
            CacheManager.Clear(CACHE_STRING + "0");

            var rightsRetriever = new RightsManager();

            var rights = rightsRetriever.GetRights(0);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewLimitedDetails));
        }

        [Test]
        public void GetRights_RetrievesRightsForSuperUser()
        {
            InitializeUserRightsMock();
            CacheManager.Clear(CACHE_STRING + UserRolesFakeRepository.SPECMGR_ID);

            var rightsRetriever = new RightsManager();
            
            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.SPECMGR_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewDetails));
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewCompleteDetails));
        }

        [Test]
        public void GetRights_RetrievesRightsForOfficials()
        {
            InitializeUserRightsMock();
            CacheManager.Clear(CACHE_STRING + UserRolesFakeRepository.CHAIRMAN_ID);

            var rightsRetriever = new RightsManager();

            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.CHAIRMAN_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Versions_Allocate, UserRolesFakeRepository.TB_ID1));
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Versions_Modify_MajorVersion, UserRolesFakeRepository.TB_ID2));
        }

        [Test]
        public void GetRights_LooksInCache()
        {
            InitializeUserRightsMock();
            var cacheKey = CACHE_STRING + UserRolesFakeRepository.ADMINISTRATOR_ID;
            CacheManager.Clear( cacheKey );

            var rightsRetriever = new RightsManager();

            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID);
            Assert.IsNotNull(CacheManager.Get(cacheKey));

            // Update the rights in cache
            var cachedRights = (UserRightsContainer) CacheManager.Get(cacheKey);
            cachedRights.AddRight(Enum_UserRights.Release_Create); 
            
            CacheManager.Insert(cacheKey, cachedRights);

            // Ask again the system
            rights = rightsRetriever.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_Create));
        }

        [Test]
        public void GetRights_ReturnsCopy()
        {
            InitializeUserRightsMock();
            var cacheKey = CACHE_STRING + UserRolesFakeRepository.ADMINISTRATOR_ID;
            CacheManager.Clear(cacheKey);

            var rightsRetriever = new RightsManager();

            // System should have computed the rights, and put them in cache.
            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewDetails));
            rights.RemoveRight(Enum_UserRights.Release_ViewDetails, true);

            // Thus if we call it again, we should still have it.
            rights = rightsRetriever.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewDetails));
        }

        [TestCase(53388, false)] 
        [TestCase(27900, true)]  //MCC_MEMBER
        [TestCase(27904, false)]   
        [TestCase(27905, true)] //MCC_MEMBER
        [TestCase(13, false)]
        public void IsUserMCCMember(int personID, bool result)
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsManager = new RightsManager();
            rightsManager.UoW = GetUnitOfWork();
            bool isUserMCCMember = rightsManager.IsUserMCCMember(personID);

            Assert.AreEqual(result, isUserMCCMember);
        }

        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            return iUnitOfWork;
        }
    }
}
