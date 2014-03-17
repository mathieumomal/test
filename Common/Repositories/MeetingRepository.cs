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
            get { 
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

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion
        
        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface IMeetingRepository : IEntityRepository<Meeting>
    {

    }
}
