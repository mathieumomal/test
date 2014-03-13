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

        public List<Meeting> GetLatestMeetings()
        {
            return GetLatestMeetings(0);
        }

        public List<Meeting> GetLatestMeetings(int MeetingId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var meetingManager = new MeetingManager();
                meetingManager.UoW = uoW;
                //Get list of meetings
                return meetingManager.GetLatestMeetings(MeetingId);
            }
        }

        #endregion
    }
}
