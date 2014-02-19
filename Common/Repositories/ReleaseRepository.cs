using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using System.Web;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        public IUltimateUnitOfWork UoW{ get; set; }
        public ReleaseRepository(){}

        #region IEntityRepository<Release> Membres

        /// <summary>
        /// Returns the list of all releases, including the release status.
        /// Stores the data in the cache, as this will often be requested.
        /// </summary>
        public IQueryable<Release> All
        {
            get {
                return AllIncluding(t => t.Enum_ReleaseStatus);
            }
        }

        /// <summary>
        /// Returns the list of all releases, including additional data that might be needed.
        /// 
        /// This performs no caching.
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public IQueryable<Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Release, object>>[] includeProperties)
        {
            IQueryable<Release> query = UoW.Context.Releases;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty).OrderByDescending(c => c.Pk_ReleaseId);
            }
            return query;
        }

        public Release Find(int id)
        {
            return UoW.Context.Releases.Find(id);
        }

        public void InsertOrUpdate(Release entity)
        {
            if (entity.Pk_ReleaseId == default(int))
            {
                UoW.Context.SetAdded(entity);
            }
            else
            {
                UoW.Context.SetModified(entity);
            }
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release entity");
        }

        public void Save()
        {
            UoW.Context.SaveChanges();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            UoW.Context.Dispose();
        }

        #endregion
    }

    public interface IReleaseRepository : IEntityRepository<Release>
    {
    }
}
