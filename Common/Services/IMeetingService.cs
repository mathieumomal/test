using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using System;

namespace Etsi.Ultimate.Services
{

    /// <summary>
    /// Interface description all operations that are allowed regarding meetings.
    /// </summary>
    public interface IMeetingService
    {
        /// <summary>
        /// Abstract method to get list of Meetings
        /// </summary>
        /// <param name="SearchText"></param>
        /// <returns></returns>
        List<DomainClasses.Meeting> GetMatchingMeetings(string SearchText);

        /// <summary>
        /// Abstract method to get list of latest Meetings
        /// </summary>
        /// <returns></returns>
        List<DomainClasses.Meeting> GetLatestMeetings();

        /// <summary>
        /// Abstract method to get list of latest Meetings including given MeetingId
        /// </summary>
        /// <param name="includeMeetingId"></param>
        /// <returns></returns>
        List<Meeting> GetLatestMeetings(int includeMeetingId);

        /// <summary>
        /// Abstract method to get meeting my passed MeetingId
        /// </summary>
        /// <param name="MeetingId"></param>
        /// <returns></returns>
        Meeting GetMeetingById(int MeetingId);

        /// <summary>
        /// Gets the meetings by ids.
        /// </summary>
        /// <param name="meetingIds">The meeting ids.</param>
        /// <returns>List of meetings</returns>
        List<Meeting> GetMeetingsByIds(List<int> meetingIds);
    }
}
