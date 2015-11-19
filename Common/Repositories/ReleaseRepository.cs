using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        public IUltimateUnitOfWork UoW{ get; set; }
        private const string CacheUltReleasesId = "ULT_RELEASES_ID_{0}";

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
            return AllIncluding(t => t.Enum_ReleaseStatus, t => t.Remarks, t=> t.Histories).Where(x => x.Pk_ReleaseId == id).FirstOrDefault();
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
                CacheManager.Clear(String.Format(CacheUltReleasesId, entity.Pk_ReleaseId));
            }

            // Add / edit remarks
            foreach (var rk in entity.Remarks)
            {
                rk.Release = entity;
                if (rk.Pk_RemarkId == default(int))
                    UoW.Context.SetAdded(rk);
                else
                    UoW.Context.SetModified(rk);
            }

            // Add / edit history
            foreach (var hi in entity.Histories)
            {
                if (hi.Pk_HistoryId == default(int))
                {
                    hi.Release = entity;
                    UoW.Context.SetAdded(hi);
                }
            }
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release entity");
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion

        #region inherited methods
        /// <summary>
        /// Get releases linked to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <returns>List of releases linked to spec provided</returns>
        public List<Release> GetReleasesLinkedToASpec(int specId)
        {
            return UoW.Context.Specification_Release.Where(x => x.Fk_SpecificationId == specId).Select(x => x.Release).OrderByDescending(x => x.SortOrder).ToList();
        }
        #endregion
    }

    public interface IReleaseRepository : IEntityRepository<Release>
    {
        /// <summary>
        /// Get releases linked to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <returns>List of releases linked to spec provided</returns>
        List<Release> GetReleasesLinkedToASpec(int specId);
    }
}
