using Etsi.Ultimate.DomainClasses;
using System;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        #region Properties

        public IUltimateUnitOfWork UoW{ get; set; }

        #endregion

        #region IEntityRepository<History> Membres

        /// <summary>
        /// Returns the list of all histories, including the release status.
        /// </summary>
        public IQueryable<History> All
        {
            get
            {
                return UoW.Context.Histories;
            }
        }

        /// <summary>
        /// Returns the list of all histories, including additional data that might be needed.
        /// </summary>
        /// <param name="includeProperties">Properties to Include</param>
        /// <returns>History data</returns>
        public IQueryable<History> AllIncluding(params System.Linq.Expressions.Expression<Func<History, object>>[] includeProperties)
        {
            IQueryable<History> query = UoW.Context.Histories;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        /// <summary>
        /// Get the history record based on the id
        /// </summary>
        /// <param name="id">History Id</param>
        /// <returns>History Record</returns>
        public History Find(int id)
        {
            return AllIncluding(t => t.Release).Where(x => x.Pk_HistoryId == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert / Update history entity
        /// </summary>
        /// <param name="entity">History Entity</param>
        public void InsertOrUpdate(History entity)
        {
            if (entity.Pk_HistoryId == default(int))
            {
                UoW.Context.SetAdded(entity);
            }
            else
            {
                UoW.Context.SetModified(entity);
            }
        }

        /// <summary>
        /// Delete History Record
        /// </summary>
        /// <param name="id">History Id</param>
        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete History entity");
        }

        #endregion

        #region IDisposable Membres

        /// <summary>
        /// Dispose context
        /// </summary>
        public void Dispose()
        {
            UoW.Context.Dispose();
        }

        #endregion
    }

    public interface IHistoryRepository : IEntityRepository<History>
    {
    }
}
