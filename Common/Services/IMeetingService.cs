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
        /// <param name="personID"></param>
        /// <returns></returns>
        List<DomainClasses.Meeting> GetMatchingMeetings(string SearchText);

        /// <summary>
        /// Abstract method to get list of latest Meetings
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        List<DomainClasses.Meeting> GetLatestMeetings();

        /// <summary>
        /// Abstract method to get list of latest Meetings including given MeetingId
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        List<Meeting> GetLatestMeetings(int MeetingId);
    }
}
