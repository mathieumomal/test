using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class WorkItemRepository: IWorkItemRepository
    {

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        public IQueryable<WorkItem> All
        {
            get { return UoW.Context.WorkItems; }
        }

        public IQueryable<WorkItem> AllIncluding(params System.Linq.Expressions.Expression<Func<WorkItem, object>>[] includeProperties)
        {
            IQueryable<WorkItem> query = UoW.Context.WorkItems;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public WorkItem Find(int id)
        {
            return UoW.Context.WorkItems.Find(id);
        }

        public void InsertOrUpdate(WorkItem entity)
        {
            if (entity.Pk_WorkItemUid == default(int))
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
            var wi = UoW.Context.WorkItems.Find(id);
            if (wi != null)
                UoW.Context.WorkItems.Remove(wi);
        }


        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
    
    
    
    public interface IWorkItemRepository : IEntityRepository<WorkItem>
    {
    }
}
