using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;
using Rhino.Mocks;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.Repositories
{
    public class ShortUrlRepositoryTest : BaseTest
    {
        public const string tokenExample_exist = "azer";
        public const string tokenExample_dontexist = "azex";

        public const string CacheModulePage = "ULT_REPO_MODULE_TAB";

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindByToken_noResult()
        {
            CacheManager.Clear(CacheModulePage);
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var results = repo.FindByToken(tokenExample_dontexist);
        }

        [Test]
        public void FindByToken_oneFind()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var results = repo.FindByToken(tokenExample_exist);
            Assert.AreEqual(results.Token, tokenExample_exist);
        }

        [Test]
        public void InsertOrUpdate_add()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            var shorturl = new ShortUrl();
            shorturl.Token = tokenExample_dontexist;
            shorturl.Url = "adresse";
            repo.InsertOrUpdate(shorturl);
            Assert.AreEqual(3, repo.All.Count());
        }

        [Test]
        public void GetTabIdAndPageNameForModuleId_ReturnsIdWhenFound()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };

            var result = repo.GetTabIdAndPageNameForModuleId(1);

            Assert.AreEqual(12, result.Key);
            Assert.AreEqual("//WorkItem", result.Value);
        }

        [Test]
        public void GetTabIdAndPageNameForModuleId_ReturnsNullWhenNothingFound()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual(0,repo.GetTabIdAndPageNameForModuleId(12134).Key);
        }

        
        [Test]
        public void GetTabIdAndPageNameForModuleId_UsesCache()
        {
            var repo = new UrlRepository() { UoW = GetUnitOfWork() };
            int newValue = 25;

            var result = repo.GetTabIdAndPageNameForModuleId(1);

            Assert.AreEqual(12, result.Key);

            // retrieve cache and check it.
            var cache = (Dictionary<int, View_ModulesPages>) CacheManager.Get(CacheModulePage);
            Assert.IsNotNull(cache);
            Assert.AreEqual(1, cache.Count);

            // Let's modify the cache entry, and call it a second time.
            var cacheResult = cache.First();
            var record = cacheResult.Value;
            record.TabID = newValue;
            cache.Remove(cacheResult.Key);
            cache.Add(cacheResult.Key, record);
            CacheManager.Insert(CacheModulePage, cache);

            // Check that result is modified
            var newResult = repo.GetTabIdAndPageNameForModuleId(1);
            Assert.AreEqual(newValue, newResult.Key);
        }
        



        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();

            var suDbSet = new ShortUrlFakeDBSet();
            suDbSet.Add(new ShortUrl()
            {
                Pk_Id = 1,
                Token = tokenExample_exist,
                Url = "/release.ascx?params"
            });
            suDbSet.Add(new ShortUrl()
            {
                Pk_Id = 2,
                Token = tokenExample_exist,
                Url = "/release.ascx?params"
            });

            var modulesPagesDbSet = new ModulesPagesFakeDbSet();
            modulesPagesDbSet.Add(new View_ModulesPages() { ModuleID = 1, TabName = "WorkItem", TabPath = "//WorkItem", TabID=12 });

            iUltimateContext.ShortUrls = suDbSet;
            iUltimateContext.View_ModulesPages = modulesPagesDbSet;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            
            return iUnitOfWork;
        }
    }
}
