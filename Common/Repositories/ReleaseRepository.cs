using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        public IUltimateUnitOfWork UoW{ get; set; }
        public ReleaseRepository(){}


        #region IEntityRepository<Release> Membres

        public IQueryable<Release> All
        {
            get { return UoW.Context.Releases; }
        }

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
