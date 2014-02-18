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

        private static string CACHE_KEY = "ULT_REPO_RELEASES_ALL";


        #region IEntityRepository<Release> Membres

        /// <summary>
        /// Returns the list of all releases, including the release status.
        /// Stores the data in the cache, as this will often be requested.
        /// </summary>
        public IQueryable<Release> All
        {
            get {
                
                var cachedData = (IQueryable<Release>)CacheManager.Get(CACHE_KEY);
                if (cachedData != null)
                    return cachedData;
                cachedData = AllIncluding(t => t.Enum_ReleaseStatus);
                CacheManager.Insert(CACHE_KEY, cachedData);
                return cachedData; 
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
            HttpRuntime.Cache.Remove(CACHE_KEY);
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
