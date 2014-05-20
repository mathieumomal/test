using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Utils;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace Etsi.Ultimate.Repositories
{
    public class SpecVersionsRepository : ISpecVersionsRepository
    {
        private IUltimateContext context;
        public SpecVersionsRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }



        #region IEntityRepository<SpecVersionRepository> Membres

        public IQueryable<SpecVersion> All
        {
            get
            {
                return AllIncluding(v => v.Remarks, v=> v.Release, v => v.Specification);
            }
        }

        public IQueryable<SpecVersion> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecVersion, object>>[] includeProperties)
        {
            IQueryable<SpecVersion> query = UoW.Context.SpecVersions;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SpecVersion Find(int id)
        {
            return AllIncluding(v => v.Remarks, v=> v.Release, v => v.Specification).Where(v => v.Pk_VersionId == id).FirstOrDefault();
        }

        public List<SpecVersion> GetVersionForSpecRelease(int specId, int releaseId)
        {
            return AllIncluding(v => v.Remarks).Where(v => v.Fk_SpecificationId == specId && v.Fk_ReleaseId == releaseId).ToList();
        }

        public void InsertOrUpdate(SpecVersion entity)
        {
            //Remove generated proxies to avoid Referential Integrity Errors
            entity.Release = null;
            entity.Specification = null;
            
            //[1] Add Existing Childs First
            entity.Remarks.ToList().ForEach(x =>
            {
                if (x.Pk_RemarkId != default(int))
                    UoW.Context.SetModified(x);                
            });

            /*if (entity.Fk_SpecificationId != default(int))
                UoW.Context.SetModified(entity.Specification);

            if (entity.Fk_ReleaseId != default(int))
                UoW.Context.SetModified(entity.Release);*/


            //[2] Add the Entity (It will add the childs as well)
            UoW.Context.SetAdded(entity);

            
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Version entity");
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            //context.Dispose();
        }

        #endregion


        public IUltimateUnitOfWork UoW { get; set; }
    }
    public interface ISpecVersionsRepository : IEntityRepository<SpecVersion>
    {
        /// <summary>
        /// Return a list of SpecVersion for a specification release
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <param name="releaseId">Release id</param>
        /// <returns>List of specVersions</returns>
        List<SpecVersion> GetVersionForSpecRelease(int specId, int releaseId);
    }
}
