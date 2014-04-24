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
    public class SpecificationWorkItemRepository : ISpecificationWorkItemRepository
    {
        private IUltimateContext context;

        public SpecificationWorkItemRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        #region IEntityRepository<SpecificationWorkItemRepository> Membres

        public IQueryable<Specification_WorkItem> All
        {
            get {
                return AllIncluding(e => e.WorkItem);
            }
        }

        public IQueryable<Specification_WorkItem> AllIncluding(params System.Linq.Expressions.Expression<Func<Specification_WorkItem, object>>[] includeProperties)
        {
            IQueryable<Specification_WorkItem> query = UoW.Context.Specification_WorkItem;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Specification_WorkItem Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Specification_WorkItem entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        #endregion

         #region IDisposable Membres

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface ISpecificationWorkItemRepository : IEntityRepository<Specification_WorkItem>
    {
    }
}
