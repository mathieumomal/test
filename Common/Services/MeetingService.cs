using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Linq;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the meetings.
    /// </summary>
    public class MeetingService : IMeetingService
    {
        #region IMeetingService Membres

        /// <summary>
        /// Get list of Meetings
        /// </summary>
        /// <param name="SearchText">search text</param>
        /// <returns>List of matching meetings</returns>
        public List<Meeting> GetMatchingMeetings(string SearchText)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var meetingManager = new MeetingManager();
                meetingManager.UoW = uoW;
                //Get list of meetings
                return meetingManager.GetMatchingMeetings(SearchText);
            }
        }

        /// <summary>
        /// Get list of latest Meetings
        /// </summary>
        /// <returns>List of meetings</returns>
        public List<Meeting> GetLatestMeetings()
        {
            return GetLatestMeetings(0);
        }

        /// <summary>
        /// Get list of latest Meetings including given MeetingId
        /// </summary>
        /// <param name="includeMeetingId">meeting id to include</param>
        /// <returns>List of latest meetings</returns>
        public List<Meeting> GetLatestMeetings(int includeMeetingId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var meetingManager = new MeetingManager();
                meetingManager.UoW = uoW;
                //Get list of meetings
                return meetingManager.GetLatestMeetings(includeMeetingId);
            }
        }

        /// <summary>
        /// Get meeting by passed MeetingId
        /// </summary>
        /// <param name="MeetingId">meeting id</param>
        /// <returns>Matching details</returns>
        public Meeting GetMeetingById(int MeetingId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var meetingManager = new MeetingManager();
                meetingManager.UoW = uoW;
                
                return meetingManager.GetMeetingById(MeetingId);
            }
        }

        /// <summary>
        /// Gets the meetings by ids.
        /// </summary>
        /// <param name="meetingIds">The meeting ids.</param>
        /// <returns>List of meetings</returns>
        public List<Meeting> GetMeetingsByIds(List<int> meetingIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var meetingManager = new MeetingManager();
                meetingManager.UoW = uoW;

                return meetingManager.GetMeetingsByIds(meetingIds);
            }
        }

        #endregion
    }
}
