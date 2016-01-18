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
        private const int MEETING_FROM_DATE = -15;
        private const int MEETING_TO_DATE = 3;

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
                    .OrderByDescending(d => d.START_DATE)
                    .ToList();
        }

        /// <summary>
        /// Retrieves latest meetings data
        /// </summary>
        /// <returns></returns>
        public List<Meeting> GetLatestMeetings(int includeMeetingId, List<int> tbsIds = null)
        {
            IMeetingRepository repo = RepositoryFactory.Resolve<IMeetingRepository>();
            repo.UoW = UoW;

            DateTime fromDate = DateTime.UtcNow.AddMonths(MEETING_FROM_DATE);
            DateTime toDate = DateTime.UtcNow.AddMonths(MEETING_TO_DATE);

            var query = repo.All
                            .Where(m => m.START_DATE >= fromDate && m.START_DATE < toDate);

            if (tbsIds != null && tbsIds.Count() > 0)
            {
                query = query.Where(m => m.TB_ID.HasValue && tbsIds.Contains((int)m.TB_ID.Value));
            }

            var meetings = query.OrderByDescending(m => m.START_DATE).ToList();

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
