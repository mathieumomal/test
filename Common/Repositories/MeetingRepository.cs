using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{

    /// <summary>
    /// Default implementation of the IMeetingRepository. Goes fetch in the View_Meetings view.
    /// </summary>
    public class MeetingRepository : IMeetingRepository
    {
        public MeetingRepository()
        {
        }

        #region IEntityRepository<MeetingRepository> Membres

        public IQueryable<Meeting> All
        {
            get
            {
                return UoW.Context.Meetings;
            }
        }

        public IQueryable<Meeting> AllIncluding(params System.Linq.Expressions.Expression<Func<Meeting, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Meeting Find(int id)
        {
            return UoW.Context.Meetings.Find(id);
        }

        public void InsertOrUpdate(Meeting entity)
        {
            throw new InvalidOperationException("Cannot add or update a meeting");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete meeting status entity");
        }

        #endregion

        #region IMeetingRepository Membres

        /// <summary>
        /// Gets the meetings for dropdown.
        /// </summary>
        /// <returns>Meetings list</returns>
        public Dictionary<int, string> GetMeetingsForDropdown()
        {
            return UoW.Context.Meetings.Select(x => new { x.MTG_ID, x.MtgShortRef, x.START_DATE, x.LOC_CITY, x.LOC_CTY_CODE }).ToDictionary(y => y.MTG_ID, y => GetFormattedMeeting(y.MtgShortRef, y.START_DATE, y.LOC_CITY, y.LOC_CTY_CODE));
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gets the formatted meeting.
        /// </summary>
        /// <param name="mtgRef">The MTG reference.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="meetingLocation">The meeting location.</param>
        /// <param name="meetingLocationCode">The meeting location code.</param>
        /// <returns>Formatted meeting string</returns>
        private string GetFormattedMeeting(string mtgRef, DateTime? startDate, string meetingLocation, string meetingLocationCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mtgRef);
            if (startDate != null)
                sb.Append(" (" + startDate.Value.ToString("yyyy-MM-dd"));
            if (!String.IsNullOrEmpty(meetingLocation))
                sb.Append(" - " + meetingLocation);
            if (meetingLocationCode != null)
                sb.Append("(" + meetingLocationCode + ")");
            if (startDate != null)
                sb.Append(")");
            return sb.ToString();
        } 

        #endregion

        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface IMeetingRepository : IEntityRepository<Meeting>
    {
        /// <summary>
        /// Gets the meetings for dropdown.
        /// </summary>
        /// <returns>Meetings list</returns>
        Dictionary<int, string> GetMeetingsForDropdown();
    }
}
