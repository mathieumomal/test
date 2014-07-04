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

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void Test_GetReleaseById(int releaseId)
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var allReleases = releaseService.GetAllReleases(releaseId).Key;
            var returnedRelese = releaseService.GetReleaseById(0,releaseId).Key;
            foreach(Release r in allReleases){
                if (r.Pk_ReleaseId == releaseId)
                {
                    Assert.AreEqual(r.Pk_ReleaseId, returnedRelese.Pk_ReleaseId);
                }
                else
                {
                    Assert.AreNotEqual(r.Pk_ReleaseId, returnedRelese.Pk_ReleaseId);
                }
            }            
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void Test_GetPreviousReleaseCode(int releaseId)
        {
            CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());            

            // Call the code
            var releaseService = new ReleaseService();
            
            int previousReleaseId = releaseService.GetPreviousReleaseCode(releaseId).Key;
            if (releaseId == 1)
            {
                Assert.AreEqual(0, previousReleaseId);
            }
            else
            {
                Assert.AreNotEqual(releaseId+1, previousReleaseId);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        public void Test_GetAllReleasesCodes(int releaseId)
        {
            //CacheManager.Clear(RELEASE_CACHE_KEY);

            // Setup the dependency manager, let's test both Service and business
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            ManagerFactory.Container.RegisterType<IRightsManager, RightsManagerFake>(new TransientLifetimeManager());

            // Call the code
            var releaseService = new ReleaseService();
            var releases = releaseService.GetAllReleases(1).Key.ToList(); 
            var releasesCodes = releaseService.GetAllReleasesCodes(releaseId);
            Assert.AreEqual(releases.Count, releasesCodes.Count);            
            CollectionAssert.DoesNotContain(releasesCodes.Keys, releaseId);
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
                                                                                                    && y.HistoryText == String.Format("Release closed. Changes:<br />Closure date:  (None) => {0} (S6-25)", closureDate.ToString("yyyy-MM-dd")))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void Test_EditRelease()
        {
            int releaseIdToTest = 2;
            int previousReleaseId = 3;
            string editedName = "Second release edited";
            int personID = 12;

            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var releaseStatus = new Enum_ReleaseStatusFakeDBSet();
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, Code = "Closed" });

            var MeetingsSet = new MeetingFakeDBSet();
            MeetingsSet.Add(new Meeting(){  MTG_ID = 1, MtgShortRef = "d5", END_DATE= DateTime.Today.AddDays(1)});
            MeetingsSet.Add(new Meeting(){  MTG_ID = 2, MtgShortRef = "d6", END_DATE= DateTime.Today.AddDays(2)});
            MeetingsSet.Add(new Meeting(){  MTG_ID = 3, MtgShortRef = "d7", END_DATE= DateTime.Today.AddDays(3)});
            MeetingsSet.Add(new Meeting(){  MTG_ID = 4, MtgShortRef = "d8", END_DATE = DateTime.Today.AddDays(4) });
            MeetingsSet.Add(new Meeting(){  MTG_ID = 5, MtgShortRef = "d9", END_DATE = DateTime.Today.AddDays(5) });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releaseFakeRepository.All);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)releaseStatus);
            mockDataContext.Stub(x => x.Meetings).Return((IDbSet<Meeting>)MeetingsSet);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var releaseService = new ReleaseService();
            var ReleaseToTest = new Release()
            {
                Code = "Rel-98",
                Pk_ReleaseId = 2,
                Name = "Second release",
                Fk_ReleaseStatus = 1,
                StartDate = new DateTime(2008, 9, 18),
                ClosureDate = new DateTime(2015, 10, 12),
                Enum_ReleaseStatus = releaseStatus.ElementAt(1), //Frozen
                Stage1FreezeDate = DateTime.Today.AddDays(-5),
                Stage1FreezeMtgRef = "d3",
                Stage2FreezeDate = DateTime.Today.AddDays(-3),
                Stage2FreezeMtgRef = "d4",
                Stage3FreezeDate = DateTime.Today.AddDays(1),
                Stage3FreezeMtgRef = ("a pas afficher"),
                EndMtgRef = "SP-12",
                EndDate = new DateTime(2014,4,7),
                SortOrder = 20
            };
            ReleaseToTest.Name = editedName;
            ReleaseToTest.Stage1FreezeMtgId = MeetingsSet.ElementAt(0).MTG_ID;
            ReleaseToTest.Stage2FreezeMtgId = MeetingsSet.ElementAt(1).MTG_ID;
            ReleaseToTest.Stage3FreezeMtgId = MeetingsSet.ElementAt(2).MTG_ID;
            ReleaseToTest.EndMtgId = MeetingsSet.ElementAt(3).MTG_ID;
            ReleaseToTest.ClosureMtgId = MeetingsSet.ElementAt(4).MTG_ID;

            releaseService.EditRelease(ReleaseToTest, previousReleaseId, personID);

            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<Release>.Matches(y => y.Name == editedName && y.Pk_ReleaseId == ReleaseToTest.Pk_ReleaseId
                                                                && DateTime.Compare(y.Stage1FreezeDate.Value,MeetingsSet.ElementAt(0).END_DATE.Value) == 0 
                                                                && DateTime.Compare(y.Stage2FreezeDate.Value,MeetingsSet.ElementAt(1).END_DATE.Value) == 0 
                                                                && DateTime.Compare(y.Stage3FreezeDate.Value,MeetingsSet.ElementAt(2).END_DATE.Value) == 0
                                                                && DateTime.Compare(y.EndDate.Value, MeetingsSet.ElementAt(3).END_DATE.Value) == 0
                                                                && DateTime.Compare(y.ClosureDate.Value, MeetingsSet.ElementAt(4).END_DATE.Value) == 0
                                                                && y.Histories.Count == 1
                                                                && y.Histories.First().HistoryText.Contains("End date"))));
            
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void Test_CreateRelease()
        {
            int previousReleaseId = 2;
            int personID = 12;

            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var releaseStatus = new Enum_ReleaseStatusFakeDBSet();
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = "Open" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen" });
            releaseStatus.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, Code = "Closed" });

            var meetings = new MeetingFakeDBSet();
            meetings.Add(new Meeting() { MTG_ID = 12, MTG_REF = "SP-25" });

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releaseFakeRepository.All);
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)releaseStatus);
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var releaseService = new ReleaseService();
            var ReleaseToTest = new Release()
            {
                Code = "Rel-999",
                Name = "New release",
                Fk_ReleaseStatus = 1,
                StartDate = new DateTime(2009, 9, 18),
                ClosureDate = new DateTime(2016, 10, 12),
                ClosureMtgId = 12,
                Enum_ReleaseStatus = releaseStatus.ElementAt(0),
                Stage1FreezeDate = DateTime.Today.AddDays(-5),
                Stage1FreezeMtgRef = "d3",
                Stage2FreezeDate = DateTime.Today.AddDays(-3),
                Stage2FreezeMtgRef = "d4",
                Stage3FreezeDate = DateTime.Today.AddDays(1),
                Stage3FreezeMtgRef = ("d5"),
            };

            releaseService.CreateRelease(ReleaseToTest, previousReleaseId, personID);

            // Check that Release that was inserted has correct Values.
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<Release>.Matches(y => y.Code == ReleaseToTest.Code && y.Name == ReleaseToTest.Name
                                                                    && y.StartDate == ReleaseToTest.StartDate && y.ClosureDate == ReleaseToTest.ClosureDate)));
                                                                                

            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<History>.Matches(y => y.Fk_ReleaseId == default(int) && y.Fk_PersonId == personID
                                                                                                    && y.HistoryText == Utils.Localization.History_Release_Created)));
            
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void Test_FreezeRelease()
        {
            //User Right
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            int releaseIdToTest = 1;
            DateTime FreezeDate = DateTime.Now;
            string FreezeRef = "S6-25";
            int FreezeRefId = 21;
            int personID = 12;

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)GetReleases());
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)GetReleaseStatus());
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)GetSpecs());
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleases());
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)GetVersions());

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Freeze a release
            var releaseService = new ReleaseService();
            releaseService.FreezeRelease(releaseIdToTest, FreezeDate, personID, FreezeRefId, FreezeRef);

            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<Release>.Matches(y => y.Fk_ReleaseStatus == 2 && y.Enum_ReleaseStatus == null
                                                                                                                && y.EndDate == FreezeDate)));

            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<History>.Matches(y => y.Fk_ReleaseId == releaseIdToTest && y.Fk_PersonId == personID
                                                                                                    && y.HistoryText == String.Format("Release frozen. Changes:<br />End date:  (None) => {0} (S6-25)", FreezeDate.ToString("yyyy-MM-dd")))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());

            mockDataContext.VerifyAllExpectations();
        }

        #region datas
        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "1", IsUnderChangeControl = true, IsForPublication = true });
            return list;
        }

        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isTranpositionForced = false });
            return list;
        }

        private IDbSet<SpecVersion> GetVersions()
        {
            var list = new SpecVersionFakeDBSet();
            list.Add(new SpecVersion() { Pk_VersionId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1 });
            return list;
        }
        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            list.Add(new Release() { Pk_ReleaseId = 2, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 3, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 4, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            return list;
        }
        private IDbSet<Enum_ReleaseStatus> GetReleaseStatus()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen", Description = "Frozen" });
            return list;
        }
        #endregion datas
    }
}
