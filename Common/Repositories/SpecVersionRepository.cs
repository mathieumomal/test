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
using System.Data.Entity.Core.Objects;

namespace Etsi.Ultimate.Repositories
{
    public class SpecVersionRepository : ISpecVersionRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public SpecVersionRepository()
        {
        }

        #region IEntityRepository<SpecVersion> Membres

        public IQueryable<SpecVersion> All
        {
            get
            {
                return AllIncluding(t => t.Release, t => t.Remarks, t => t.Specification);
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
            return AllIncluding(t => t.Release, t => t.Remarks, t => t.Specification).Where(x => x.Pk_VersionId == id).FirstOrDefault();
        }

        public void InsertOrUpdate(SpecVersion specification)
        {
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete SpecVersion entity");
        }

        /// <summary>
        /// Set entity state to deleted
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="Entity">Entity</param>
        public void MarkDeleted<T>(T Entity)
        {
            UoW.Context.SetDeleted(Entity);
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            return All.Where(x => (x.Fk_SpecificationId != null) ? x.Fk_SpecificationId.Value == specificationId : false).ToList();
        }
    }

    public interface ISpecVersionRepository : IEntityRepository<SpecVersion>
    {
        List<SpecVersion> GetVersionsBySpecId(int specificationId);
    }
}
