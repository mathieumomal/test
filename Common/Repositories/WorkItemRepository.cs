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
            if (entity.IsNew)
            {
                UoW.Context.SetAdded(entity);

             

            }
            else
            {
                UoW.Context.SetModified(entity);
            }

            // Add possibly the Responsible groups
            foreach (var responsibleGroup in entity.WorkItems_ResponsibleGroups)
            {
                if (responsibleGroup.Pk_WorkItemResponsibleGroups == default(int))
                    UoW.Context.SetAdded(responsibleGroup);
                else
                    UoW.Context.SetModified(responsibleGroup);
            }

            // Add possibly the remarks
            foreach (var remark in entity.Remarks)
            {
                if (remark.Pk_RemarkId == default(int))
                    UoW.Context.SetAdded(remark);
                else
                    UoW.Context.SetModified(remark);
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
