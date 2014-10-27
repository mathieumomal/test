using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport.ModuleImport
{
    public class MeetingHelper
    {
        private Dictionary<string, int> MeetingIds;

        public MeetingHelper(ITmpDb iLegacyContext, IUltimateContext iUltimateContext)
        {
            MeetingIds = new Dictionary<string,int>();
            
            iLegacyContext.plenary_meetings_with_end_dates.ToList().ForEach(m => 
                MeetingIds.Add(m.meeting, m.mtg_id.GetValueOrDefault())
            );

            iUltimateContext.Meetings.ToList().ForEach( m =>
                MeetingIds.Add(m.MtgShortRef, m.MTG_ID)
            );

        }

        public int? FindMeetingId(string uid)
        {
            if (MeetingIds.ContainsKey(uid))
            {
                return (MeetingIds[uid]);
            }
            return null;
        }

    }
}
