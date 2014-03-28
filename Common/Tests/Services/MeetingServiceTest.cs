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
    class MeetingServiceTest : BaseTest
    {
        #region Tests

        [Test, TestCaseSource("GetMeetingsWithSearchString")]
        public void Test_GetMatchingMeetings(string SearchText, IDbSet<Meeting> meetings, int exptectedMeetingCount)
        {
            var mockUoW = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);
            mockUoW.Stub(s => s.Context).Return(mockDataContext);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateUnitOfWork), mockUoW);

            var service = new MeetingService();
            var results = service.GetMatchingMeetings(SearchText);

            Assert.AreEqual(exptectedMeetingCount, results.Count);
        }

        [Test, TestCaseSource("GetMeetings")]
        public void Test_GetLatestMeetings(IDbSet<Meeting> meetings)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var service = new MeetingService();
            var results = service.GetLatestMeetings();

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(1, results.Where(x => x.MTG_ID == 1).Count());
            Assert.AreEqual(0, results.Where(x => x.START_DATE == null).Count());
        }

        [Test, TestCaseSource("GetMeetingsWithMeetingId")]
        public void Test_GetLatestMeetings_WithId(IDbSet<Meeting> meetings, int includedMeetingId, int exptectedMeetingCount)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var meetingService = new MeetingService();
            var results = meetingService.GetLatestMeetings(includedMeetingId);

            Assert.AreEqual(exptectedMeetingCount, results.Count);
        }

        [Test, TestCaseSource("GetMeetings")]
        public void Test_GetMeetingById(IDbSet<Meeting> meetings)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var service = new MeetingService();

            Assert.AreEqual(1, service.GetMeetingById(1).MTG_ID);
            Assert.AreEqual(default(Meeting), service.GetMeetingById(0));
        }

        [Test, TestCaseSource("GetMeetings")]
        public void Test_GetMeetingOrderedByStartDate(IDbSet<Meeting> meetings)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var service = new MeetingService();
            List<Meeting> result = service.GetLatestMeetings();
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(3, result.ElementAt(0).MTG_ID);
            Assert.AreEqual(1, result.ElementAt(1).MTG_ID);
            Assert.AreEqual(6, result.ElementAt(2).MTG_ID);

            List<Meeting> resultSearch = service.GetMatchingMeetings("S");
            Assert.AreEqual(4, resultSearch.Count());
            Assert.AreEqual(4, resultSearch.ElementAt(0).MTG_ID);
            Assert.AreEqual(3, resultSearch.ElementAt(1).MTG_ID);
            Assert.AreEqual(1, resultSearch.ElementAt(2).MTG_ID);
            Assert.AreEqual(6, resultSearch.ElementAt(3).MTG_ID);
        }

        [Test, TestCaseSource("GetMeetingsWithSearchStringCaseInsensitive")]
        public void Test_GetMeetingCaseInsensitive(String searchText, IDbSet<Meeting> meetings)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Meetings).Return(meetings);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var service = new MeetingService();
            List<Meeting> resultSearch = service.GetMatchingMeetings(searchText);
            Assert.AreEqual(4, resultSearch.Count());
        }
        #endregion

        #region Data
        private IEnumerable<IDbSet<Meeting>> GetMeetings
        {
            get
            {
                var meeting = GetMeetingList();

                yield return (IDbSet<Meeting>)meeting;
            }
        }
        private IEnumerable<object[]> GetMeetingsWithSearchStringCaseInsensitive
        {
            get
            {
                var meetings = GetMeetingList();
                yield return new object[] { "S", (IDbSet<Meeting>)meetings};
                yield return new object[] { "s", (IDbSet<Meeting>)meetings};
            }
        }
        private IEnumerable<object[]> GetMeetingsWithSearchString
        {
            get
            {
                var meetings = GetMeetingList();
                yield return new object[] { "S5", (IDbSet<Meeting>)meetings, 2 };
                yield return new object[] { "", (IDbSet<Meeting>)meetings, 6 };
                yield return new object[] { "2014-12", (IDbSet<Meeting>)meetings, 0 };
                yield return new object[] { "TH", (IDbSet<Meeting>)meetings, 1 };

            }
        }
        private IEnumerable<object[]> GetMeetingsWithMeetingId
        {
            get
            {
                var meetings = GetMeetingList();
                yield return new object[] { (IDbSet<Meeting>)meetings, 1, 3 };
                yield return new object[] { (IDbSet<Meeting>)meetings, 2, 4 };
                yield return new object[] { (IDbSet<Meeting>)meetings, 3, 3 };
                yield return new object[] { (IDbSet<Meeting>)meetings, 0, 3 };
            }
        }

        private static MeetingFakeDBSet GetMeetingList()
        {
            var meeting = new MeetingFakeDBSet { 
                new Meeting { MTG_ID = 1, MtgShortRef = "S5-31", END_DATE = DateTime.Now, LOC_CITY = "Brussels", LOC_CTY_CODE = "BE", START_DATE = DateTime.Now },
                new Meeting { MTG_ID = 2, MtgShortRef = "R1-28", END_DATE = null, LOC_CITY = "Bangkok", LOC_CTY_CODE = "TH", START_DATE = DateTime.Now.AddDays(-40) },
                new Meeting { MTG_ID = 3, MtgShortRef = "S3-48", END_DATE = DateTime.Now, LOC_CITY = "New York", LOC_CTY_CODE = "US", START_DATE = DateTime.Now.AddDays(-20) },
                new Meeting { MTG_ID = 4, MtgShortRef = "S5-31", END_DATE = DateTime.Now, LOC_CITY = "", LOC_CTY_CODE = "US", START_DATE = null},
                new Meeting { MTG_ID = 5, MtgShortRef = "R1-28", END_DATE = null, LOC_CITY = "Berlin", LOC_CTY_CODE = "", START_DATE = DateTime.Now.AddDays(-40) },
                new Meeting { MTG_ID = 6, MtgShortRef = "S3-48", END_DATE = DateTime.Now, LOC_CITY = "", LOC_CTY_CODE = null, START_DATE = DateTime.Now.AddDays(50) }
                };
            return meeting;
        }
        #endregion
    }
}
