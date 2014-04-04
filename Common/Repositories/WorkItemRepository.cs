using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{
    public class WorkItemRepository: IWorkItemRepository
    {
        #region IWorkItemRepository Members

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
            return AllIncluding(t => t.Release, t => t.Remarks, t => t.ChildWis, t => t.ParentWi, t => t.WorkItems_ResponsibleGroups).Where(x => x.Pk_WorkItemUid == id).FirstOrDefault();
        }

        public void InsertOrUpdate(WorkItem entity)
        {
            //[1] Add Existing Childs First
            entity.WorkItems_ResponsibleGroups.ToList().ForEach(x =>
            {
                if (x.Pk_WorkItemResponsibleGroups != default(int))
                    UoW.Context.SetModified(x);
            });

            entity.Remarks.ToList().ForEach(x =>
            {
                if (x.Pk_RemarkId != default(int))
                    UoW.Context.SetModified(x);
            });

            //[2] Add the Entity (It will add the childs as well)
            UoW.Context.SetAdded(entity);

            if (!entity.IsNew)
                UoW.Context.SetModified(entity);

            //[3] Set Deleted status for deleted records
            entity.WorkItems_ResponsibleGroups.ToList().ForEach(x =>
            {
                if (x.IsDeleted)
                    UoW.Context.SetDeleted(x);
            });
        }

        public void Delete(int id)
        {
            var wi = UoW.Context.WorkItems.Find(id);
            if (wi != null)
                UoW.Context.WorkItems.Remove(wi);
        }

        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="releaseIds">Release Ids</param>
        /// <returns>List of workitems</returns>
        public List<WorkItem> GetWorkItemsByRelease(List<int> releaseIds)
        {
            return AllIncluding(t => t.Release, t => t.Remarks, t => t.ChildWis, t=> t.ParentWi, t=> t.WorkItems_ResponsibleGroups).Where(x => releaseIds.Contains(x.Fk_ReleaseId == null ? -1 : x.Fk_ReleaseId.Value)).ToList();
        }

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountByRelease(List<int> releaseIds)
        {
            return UoW.Context.WorkItems.Where(x => releaseIds.Contains(x.Fk_ReleaseId == null ? -1 : x.Fk_ReleaseId.Value)).Count();
        }

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        public List<string> GetAllAcronyms()
        {
            return UoW.Context.WorkItems.Where(x => !String.IsNullOrEmpty(x.Acronym)).Select(y => y.Acronym).Distinct().ToList();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
            
    public interface IWorkItemRepository : IEntityRepository<WorkItem>
    {
        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="releaseIds">Release Ids</param>
        /// <returns>List of workitems</returns>
        List<WorkItem> GetWorkItemsByRelease(List<int> releaseIds);

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <returns>Work Item Count</returns>
        int GetWorkItemsCountByRelease(List<int> releaseIds);

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        List<string> GetAllAcronyms();
    }
}
