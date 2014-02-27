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
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Tests.Services
{
    class ReleaseServiceTest
    {
        private static readonly string CACHE_VALUE = "ULT_REPO_RIGHTS_ALL";
        private static readonly string RELEASE_CACHE_KEY = "ULT_BIZ_RELEASES_ALL";

        [Test]
        public void Test_GetAllReleases()
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases(1);


            Assert.AreEqual(3, releases.Key.Count);
            Assert.AreEqual(2, releases.Key.Where(t => t.Enum_ReleaseStatus.ReleaseStatus == "Frozen").ToList().Count);


        }

        [Test]
        public void Test_GetAllReleases_Rights()
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases(1);

            Assert.AreEqual(false, releases.Value.HasRight(Enum_UserRights.Release_ViewCompleteDetails));
            Assert.AreEqual(true, releases.Value.HasRight(Enum_UserRights.Release_Close));
            Assert.AreEqual(true, releases.Value.HasRight(Enum_UserRights.Release_Freeze));

        }

        [Test]
        public void Test_GetAllReleases_Cache()
        {
            // Clear the cache to ensure the test is not wrong.
            string fakeDescription = "A Fake description";
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases(1);

            List<Release> cachedReleases = (List<Release>)CacheManager.Get(RELEASE_CACHE_KEY);
            Assert.IsNotNull(releases);

            // modify the cache
            cachedReleases.First().Description = fakeDescription;
            CacheManager.Insert(RELEASE_CACHE_KEY, cachedReleases);

            // Call again the code
            
            var newReleases = releaseService.GetAllReleases(1);

            // Check that the returned releases are taken from cache
            Assert.IsNotNull(newReleases);
            Assert.AreEqual(fakeDescription, newReleases.Key.First().Description);
        }
    }
}
