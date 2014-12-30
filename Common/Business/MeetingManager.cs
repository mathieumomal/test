using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class MeetingManager
    {
        private const int MEETING_START_DATE = -30;
        private const int NUMBER_OF_MEETINGS_TO_LOAD = 50;

        public IUltimateUnitOfWork UoW { get; set; }

        public MeetingManager() { }

        /// <summary>
        /// Retrieves meetings with matching search text
        /// </summary>
        /// <returns></returns>
        public List<Meeting> GetMatchingMeetings(string SearchText)
        {
            IMeetingRepository repo = RepositoryFactory.Resolve<IMeetingRepository>();
            repo.UoW = UoW;

            return repo
                    .All
                    .Where(x => (x.MtgShortRef != null && x.MtgShortRef.ToLower().Contains(SearchText.ToLower())) ||
                            (x.LOC_CITY != null && x.LOC_CITY.ToLower().Contains(SearchText.ToLower())) ||
                            (x.LOC_CTY_CODE != null && x.LOC_CTY_CODE.ToLower().Contains(SearchText.ToLower())))
                    .OrderBy(d => d.START_DATE)
                    .ToList();
        }

        /// <summary>
        /// Retrieves latest meetings data
        /// </summary>
        /// <returns></returns>
        public List<Meeting> GetLatestMeetings(int includeMeetingId)
        {
            IMeetingRepository repo = RepositoryFactory.Resolve<IMeetingRepository>();
            repo.UoW = UoW;

            DateTime startDate = DateTime.UtcNow.AddDays(MEETING_START_DATE);
            var meetings = repo
                            .All
                            .Where(x => x.START_DATE > startDate)
                            .OrderBy(d => d.START_DATE)
                            .Take(NUMBER_OF_MEETINGS_TO_LOAD)
                            .ToList();

            var requestedMeeting = repo.All.Where(x => x.MTG_ID == includeMeetingId).FirstOrDefault();
            if (requestedMeeting != null && !meetings.Exists(x => x.MTG_ID == requestedMeeting.MTG_ID))
                meetings.Insert(0, requestedMeeting);

            return meetings;
        }

        /// <summary>
        /// Gets the meeting by identifier.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <returns>Meeting details</returns>
        public Meeting GetMeetingById(int meetingId)
        {
            IMeetingRepository repo = RepositoryFactory.Resolve<IMeetingRepository>();
            repo.UoW = UoW;

            return repo.Find(meetingId);
        }

        /// <summary>
        /// Gets the meetings by ids.
        /// </summary>
        /// <param name="meetingIds">The meeting ids.</param>
        /// <returns>List of meetings</returns>
        public List<Meeting> GetMeetingsByIds(List<int> meetingIds)
        {
            IMeetingRepository repo = RepositoryFactory.Resolve<IMeetingRepository>();
            repo.UoW = UoW;

            return repo.GetMeetingsByIds(meetingIds);
        }
    }
}
