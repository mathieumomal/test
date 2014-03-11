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
        private static string CACHE_ULT_RELEASES_ID = "ULT_RELEASES_ID_{0}";

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
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Release Find(int id)
        {
            // Check in the cache
            var cachedData = (Release)CacheManager.Get(String.Format(CACHE_ULT_RELEASES_ID, id));
            if (cachedData != null)
                return cachedData;

            cachedData = AllIncluding(t => t.Enum_ReleaseStatus, t => t.Remarks, t=> t.Histories).Where(x => x.Pk_ReleaseId == id).FirstOrDefault();

            // Check that cache is still empty
            if (CacheManager.Get(String.Format(CACHE_ULT_RELEASES_ID, id)) == null)
                CacheManager.Insert(String.Format(CACHE_ULT_RELEASES_ID, id), cachedData);

            return cachedData; 
            //return UoW.Context.Releases.Find(id);
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
                CacheManager.Clear(String.Format(CACHE_ULT_RELEASES_ID, entity.Pk_ReleaseId));
            }
            CacheManager.Clear("ULT_BIZ_RELEASES_ALL");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release entity");
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
