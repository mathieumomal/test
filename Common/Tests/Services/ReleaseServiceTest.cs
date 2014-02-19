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

namespace Etsi.Ultimate.Tests.Services
{
    class ReleaseServiceTest
    {
        [Test]
        public void Test_GetAllReleases()
        {
            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Setup a mock for the release repository.
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases();


            Assert.AreEqual(3, releases.Count);
            Assert.AreEqual(2, releases.Where(t => t.Enum_ReleaseStatus.ReleaseStatus == "Frozen").ToList().Count);


        }
    }
}
