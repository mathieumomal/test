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

namespace Etsi.Ultimate.Tests.Services
{
    class ReleaseServiceTest
    {
        [Test]
        public void Test_GetAllReleases()
        {
            string releaseCacheKey = "ULT_REPO_RELEASES_ALL";
            CacheManager.Clear(releaseCacheKey);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases();


            Assert.AreEqual(3, releases.Count);
            Assert.AreEqual(2, releases.Where(t => t.Enum_ReleaseStatus.ReleaseStatus == "Frozen").ToList().Count);


        }

        [Test]
        public void Test_GetAllReleases_Cache()
        {
            // Clear the cache to ensure the test is not wrong.
            string releaseCacheKey = "ULT_REPO_RELEASES_ALL";
            string fakeDescription = "A Fake description";
            CacheManager.Clear(releaseCacheKey);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases();

            List<Release> cachedReleases = (List<Release>) CacheManager.Get(releaseCacheKey);
            Assert.IsNotNull(releases);

            // modify the cache
            cachedReleases.First().Description = fakeDescription;
            CacheManager.Insert(releaseCacheKey, cachedReleases);

            // Call again the code
            
            var newReleases = releaseService.GetAllReleases();

            // Check that the returned releases are taken from cache
            Assert.IsNotNull(newReleases);
            Assert.AreEqual(fakeDescription, newReleases.First().Description);
        }
    }
}
