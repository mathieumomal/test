using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    public class CommunityServiceTest
    {
        private static string cacheKey = "ULT_COMMUNITY_MANAGER_ALL";

        [Test]
        public void CommunityService_GetAll()
        {
            CacheManager.Clear(cacheKey);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var service = new CommunityService();
            Assert.AreEqual(3, service.GetCommunities().Count);

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void CommunityService_GetAll_WithCache()
        {
            CacheManager.Clear(cacheKey);
            CommunityFakeRepository fakeRepo = new CommunityFakeRepository();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Communities).Return((IDbSet<Community>)fakeRepo.All).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var service = new CommunityService();

            Assert.AreEqual(3, service.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));
            Assert.AreEqual(3, service.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));
            Assert.AreEqual(3, service.GetCommunities().Count);
            Assert.IsNotNull(CacheManager.Get(cacheKey));

            mockDataContext.VerifyAllExpectations();
        }
    }
}
