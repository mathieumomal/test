using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.Business
{
    [TestFixture]
    public class CommunityManagerTest : BaseTest
    {
        private static string cacheKey = "ULT_COMMUNITY_MANAGER_ALL";
        private static int CHILDTBID = 46;
        private static int PARENTTBID = 189;

        [TearDown]
        public void CleanUpCache()
        {
            CacheManager.Clear(cacheKey);
        }

        [Test]
        public void CommunityManager_GetAll()
        {
            CacheManager.Clear(cacheKey);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = ManagerFactory.Resolve<ICommunityManager>();
            manager.UoW = uow;
            Assert.AreEqual(3, manager.GetCommunities().Count);

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void CommunityManager_GetAll_WithCache()
        {
            CacheManager.Clear(cacheKey);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = ManagerFactory.Resolve<ICommunityManager>();
            manager.UoW = uow;

            Assert.AreEqual(3, manager.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));
            Assert.AreEqual(3, manager.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));
            Assert.AreEqual(3, manager.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));

            mockDataContext.VerifyAllExpectations();
        } 
       
        [Test]
        public void GetEnumCommunityShortNameByCommunityId_Test()
        {
            Enum_CommunitiesShortNameFakeDbSet comShortNamefkDbSet = new Enum_CommunitiesShortNameFakeDbSet();
            comShortNamefkDbSet.Add(new Enum_CommunitiesShortName()
            {
                Fk_TbId = 1,
                Pk_EnumCommunitiesShortNames = 1,
                ShortName = "EN",
                WpmProjectId = 3
            });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Enum_CommunitiesShortName).Return((IDbSet<Enum_CommunitiesShortName>)comShortNamefkDbSet);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = ManagerFactory.Resolve<ICommunityManager>();
            manager.UoW = uow;

            var comShortNameFound = manager.GetEnumCommunityShortNameByCommunityId(1);

            Assert.AreEqual(comShortNamefkDbSet.Find(1), comShortNameFound);
        }

        [Test]
        public void GetParentCommunityByCommunityId_Test()
        {
            CommunityFakeDBSet comfkDbSet = new CommunityFakeDBSet();
            comfkDbSet.Add(new Community()
            {
                ShortName = "S1",
                TbId = CHILDTBID,
                ParentTbId = PARENTTBID
            });
            comfkDbSet.Add(new Community()
            {
                ShortName = "SP",
                TbId = PARENTTBID,
                ParentTbId = null
            });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)comfkDbSet);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new CommunityManager();
            manager.UoW = uow;

            var comParentFound = manager.GetParentCommunityByCommunityId(CHILDTBID);

            Assert.AreEqual(PARENTTBID, comParentFound.TbId);
        }
    }
}
