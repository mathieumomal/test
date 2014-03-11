using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static readonly string CACHE_STRING = "BIZ_USER_RIGHTS_";

        [Test]
        public void GetNonCommitteeRelated_RolesReturnAnonymousOnPersonIdZero()
        {
            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(0);
            var roles2 = rightsRetriever.GetNonCommitteeRelatedRoles(-1);

            Assert.AreEqual(1, roles.Count);
            Assert.AreEqual(1, roles2.Count);

            Assert.AreEqual(Enum_UserRoles.Anonymous, roles.FirstOrDefault());
            Assert.AreEqual(Enum_UserRoles.Anonymous, roles2.FirstOrDefault());
        }

        [Test]
        public void GetNonCommitteeRelated_RolesReturnEolOnPersonIdPositive()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(2335);
            Assert.AreEqual(1, roles.Count);
            Assert.AreEqual(Enum_UserRoles.EolAccountOwner, roles.FirstOrDefault());
        }

        [Test]
        public void GetNonCommitteeRelated_RolesReturnStaffMemberIfUserBelongsToEtsi()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(UserRolesFakeRepository.MCC_MEMBER_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.StaffMember, roles);
        }

        [Test]
        public void GetNonCommitteeRelated_RolesChecksForAdministrators()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(UserRolesFakeRepository.ADMINISTRATOR_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.Administrator, roles);
        }

        [Test]
        public void GetNonCommitteeRelated_RolesChecksForWorkplanManagers()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(UserRolesFakeRepository.WORKPLANMGR_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.WorkPlanManager, roles);
        }

        [Test]
        public void GetNonCommitteeRelated_RolesChecksForSpecManager()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();

            var roles = rightsRetriever.GetNonCommitteeRelatedRoles(UserRolesFakeRepository.SPECMGR_ID);
            Assert.AreEqual(2, roles.Count);
            Assert.Contains(Enum_UserRoles.SuperUser, roles);
        }

        [Test]
        public void GetCommitteeRelated_RolesChecksForCommitteeOfficials()
        {
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();

            var chairmanRoles = rightsRetriever.GetCommitteeRelatedRoles(UserRolesFakeRepository.CHAIRMAN_ID);
            Assert.Contains(Enum_UserRoles.CommitteeOfficial, chairmanRoles[15]);
            Assert.Contains(Enum_UserRoles.CommitteeOfficial, chairmanRoles[16]);

            var secretaryRoles = rightsRetriever.GetCommitteeRelatedRoles(UserRolesFakeRepository.SECRETARY_ID);
            Assert.Contains(Enum_UserRoles.CommitteeOfficial, secretaryRoles[15]);

            var viceChairmanRoles = rightsRetriever.GetCommitteeRelatedRoles(UserRolesFakeRepository.VICECHAIRMAN_ID);
            Assert.Contains(Enum_UserRoles.CommitteeOfficial, viceChairmanRoles[15]);
        }

        [Test]
        public void GetRights_RetrievesRightsForAnonymous()
        {
            CacheManager.Clear(CACHE_STRING + "0");
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IUserRightsRepository, RightsFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();
            rightsRetriever.UoW = GetUnitOfWork();

            var rights = rightsRetriever.GetRights(0);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewLimitedDetails));
        }


        [Test]
        public void GetRights_RetrievesRightsForSuperUser()
        {
            CacheManager.Clear(CACHE_STRING + UserRolesFakeRepository.SPECMGR_ID);
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IUserRightsRepository, RightsFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();
            rightsRetriever.UoW = GetUnitOfWork();

            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.SPECMGR_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewDetails));
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_ViewCompleteDetails));
        }

        [Test]
        public void GetRights_RetrievesRightsForOfficials()
        {
            CacheManager.Clear(CACHE_STRING + UserRolesFakeRepository.SPECMGR_ID);
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IUserRightsRepository, RightsFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();
            rightsRetriever.UoW = GetUnitOfWork();

            var rights = rightsRetriever.GetRights(UserRolesFakeRepository.CHAIRMAN_ID);
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_Close, UserRolesFakeRepository.TB_ID1));
            Assert.IsTrue(rights.HasRight(Enum_UserRights.Release_Close, UserRolesFakeRepository.TB_ID2));
        }

        [Test]
        public void GetRights_LooksInCache()
        {
            var cacheKey = CACHE_STRING + UserRolesFakeRepository.ADMINISTRATOR_ID;
            CacheManager.Clear( cacheKey );
            RepositoryFactory.Container.RegisterType<IUserRolesRepository, UserRolesFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IUserRightsRepository, RightsFakeRepository>(new TransientLifetimeManager());

            var rightsRetriever = new RightsManager();
            rightsRetriever.UoW = GetUnitOfWork();

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
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetRights_UoWShouldBeInitialized()
        {

            var rightsRetriever = new RightsManager();
            var rightsUoW = rightsRetriever.GetRights(UserRolesFakeRepository.ADMINISTRATOR_ID); 

        }

        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            return iUnitOfWork;
        }




    }
}
