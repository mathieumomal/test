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
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Tests.FakeManagers;
using Etsi.Ultimate.DataAccess;
using Rhino.Mocks;
using System.Data.Entity;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.Services
{
    class ReleaseServiceTest : BaseTest
    {
        private static readonly string CACHE_VALUE = "ULT_REPO_RIGHTS_ALL";
        private static readonly string RELEASE_CACHE_KEY = "ULT_BIZ_RELEASES_ALL";

        [Test]
        public void Test_GetAllReleases()
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases(1);


            Assert.AreEqual(4, releases.Key.Count);
            Assert.AreEqual(2, releases.Key.Where(t => t.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Frozen).ToList().Count);


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

            Assert.AreEqual(true, releases.Value.HasRight(Enum_UserRights.Release_Close));
            Assert.AreEqual(true, releases.Value.HasRight(Enum_UserRights.Release_Freeze));

        }

        private static Dictionary<string, int> StatusToRelease = new Dictionary<string, int> { 
            { "Open", ReleaseFakeRepository.OPENED_RELEASE_ID }, 
            { "Frozen", ReleaseFakeRepository.FROZEN_RELEASE_ID }, 
            { "Closed", ReleaseFakeRepository.CLOSED_RELEASE_ID } };

        [TestCase("Open", true, true)]
        [TestCase("Frozen", true, false)]
        [TestCase("Closed", false, false)]
        public void Test_GetRelease_Releases(string status, bool closeEnabled, bool freezeEnabled)
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            int releaseId = StatusToRelease[status];            
            
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            // The fake right manager returns all rights, so we can check all the values.
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            var releaseService = new ReleaseService();
            var releaseAndRights = releaseService.GetReleaseById(1, releaseId);

            Assert.IsNotNull(releaseAndRights.Key);
            Assert.IsNotNull(releaseAndRights.Value);

            var rights = releaseAndRights.Value;
            Assert.AreEqual(closeEnabled, rights.HasRight(Enum_UserRights.Release_Close));
            Assert.AreEqual(freezeEnabled, rights.HasRight(Enum_UserRights.Release_Freeze));
        }

        [Test]
        public void Test_GetRelease_ReturnsRemarksAndHistory()
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            int releaseId = StatusToRelease["Open"];

            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            // The fake right manager returns all rights, so we can check all the values.
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            var releaseService = new ReleaseService();
            var releaseAndRights = releaseService.GetReleaseById(1, releaseId);
            Assert.IsNotNull(releaseAndRights.Key);

            var release = releaseAndRights.Key;
            Assert.AreEqual(1, release.Remarks.Count);
            Assert.AreEqual(2, release.Histories.Count);
        }

      

        [Test]
        public void Test_GetAllReleases_Cache()
        {
            // Clear the cache to ensure the test is not wrong.
            string fakeDescription = "A Fake description";
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

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

        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(0, 4)]
        public void Test_GetPreviousReleaseCode(int personID, int releaseId)
        {
            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var previousCode = releaseService.GetPreviousReleaseCode(personID, releaseId);
            if (releaseId == 1)
            {
                Assert.AreEqual(string.Empty, previousCode);
            }
            else
            {
                Assert.AreNotEqual(string.Empty, previousCode);
            }
        }

        [Test]
        public void Test_CloseRelease()
        {
            int releaseIdToTest = 1;
            DateTime closureDate = DateTime.Now;
            string meetingRef = "S6-25";
            int meetingRefId = 21;
            int personID = 12;

            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var releaseStatus = new Enum_ReleaseStatusFakeDBSet();
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, Code = "Closed" });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releaseFakeRepository.All).Repeat.Once();
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)releaseStatus).Repeat.Once();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var releaseService = new ReleaseService();
            releaseService.CloseRelease(releaseIdToTest, closureDate, meetingRef, meetingRefId, personID);

            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<Release>.Matches(y => y.Fk_ReleaseStatus == 3 && y.Enum_ReleaseStatus == null
                                                                                                                && y.ClosureDate == closureDate
                                                                                                                && y.ClosureMtgRef == meetingRef
                                                                                                                && y.ClosureMtgId == meetingRefId)));
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<History>.Matches(y => y.Fk_ReleaseId == releaseIdToTest && y.Fk_PersonId == personID
                                                                                                    && y.HistoryText == "'Rel-1' has been Closed")));
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());

            mockDataContext.VerifyAllExpectations();
        }
    }
}
