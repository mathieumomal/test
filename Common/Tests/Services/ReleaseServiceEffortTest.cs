using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Etsi.Ultimate.Services;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    class ReleaseServiceEffortTest : BaseEffortTest
    {
        [TestCase(136080, 4, 2884, Description = "Should return 4 releases for the spec:136080 and the first one should be the release: 2874")]
        [TestCase(136082, 2, 2884, Description = "Should return 2 releases for the spec:136082 and the first one should be the release: 2883")]
        [TestCase(140002, 0, 0, Description = "Should return 0 releases for the spec:140002 because not linked to any release")]
        [TestCase(99999, 0, 0, Description = "Should return 0 releases for the spec:99999 because doen't exist")]
        public void GetReleasesLinkedToASpec(int spectId, int expectedResult, int expectedFirstReleaseId)
        {
            var releaseService = ServicesFactory.Resolve<IReleaseService>();
            var releases = releaseService.GetReleasesLinkedToASpec(spectId, 0);

            Assert.AreEqual(0, releases.Report.GetNumberOfErrors());
            Assert.IsNotNull(releases.Result);
            Assert.AreEqual(expectedResult, releases.Result.Count);
            if(expectedResult != 0)
                Assert.AreEqual(expectedFirstReleaseId, releases.Result.First().Pk_ReleaseId);
        }

        [Test]
        public void GetHighestNonClosedReleaseLinkedToASpec()
        {
            //GIVEN
            var mockReleaseRepo = MockRepository.GenerateMock<IReleaseRepository>();
            mockReleaseRepo.Stub(x => x.GetHighestNonClosedReleaseLinkedToASpec(Arg<int>.Is.Anything)).Return(new Release{Pk_ReleaseId = 1});
            RepositoryFactory.Container.RegisterInstance(mockReleaseRepo);

            //WHEN
            var releaseService = ServicesFactory.Resolve<IReleaseService>();
            var result = releaseService.GetHighestNonClosedReleaseLinkedToASpec(1);

            //THEN
            Assert.AreEqual(1, result.Pk_ReleaseId);
        }
    }
}
