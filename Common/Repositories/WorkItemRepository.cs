using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        #region IWorkItemRepository Members

        public IUltimateUnitOfWork UoW
        {
            get;
            set;
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
            //Remove generated proxies to avoid Referential Integrity Errors
            entity.Release = null;
            entity.ParentWi = null;

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
        /// <param name="granularity">Granularity Level</param>
        /// <param name="wiAcronym">Acronym, we should correspond to the "Effective_Acronym" in database</param>
        /// <param name="wiName">Name</param>
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>List of workitems</returns>
        public List<WorkItem> GetWorkItemsBySearchCriteria(List<int> releaseIds, int granularity, string wiAcronym, string wiName, List<int> tbIds)
        {
            int WorkItemId = -1;
            int.TryParse(wiName, out WorkItemId);

            return AllIncluding(t => t.Release, t => t.Remarks, t => t.ChildWis, t => t.ParentWi, t => t.WorkItems_ResponsibleGroups)
                .Where(x => !x.DeletedFlag
                            && releaseIds.Contains(x.Fk_ReleaseId == null ? -1 : x.Fk_ReleaseId.Value)
                            && (x.Name.ToLower().Contains(wiName.Trim().ToLower()) || x.Pk_WorkItemUid == WorkItemId || String.IsNullOrEmpty(wiName.Trim()))
                            && ((x.Effective_Acronym != null && x.Effective_Acronym.ToLower().Contains(wiAcronym.Trim().ToLower())) || (String.IsNullOrEmpty(wiAcronym.Trim())))
                            && (x.WiLevel != null && x.WiLevel <= granularity)
                            && (tbIds.Count == 0 || x.WorkItems_ResponsibleGroups.Any(y => tbIds.Contains(y.Fk_TbId.Value)))).OrderBy(x=>x.WorkplanId).ToList();
        }

        /// <summary>
        /// Get the list of workitems based on
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        /// <returns>List of WorkItems</returns>
        public List<WorkItem> GetWorkItemsBySearchCriteria(string searchString, bool shouldHaveAcronym = false)
        {
            int workItemId;
            int.TryParse(searchString, out workItemId);

            return AllIncluding(t => t.Release, t => t.Remarks, t => t.ChildWis, t => t.ParentWi, t => t.WorkItems_ResponsibleGroups)
                .Where(x => (x.Name.ToLower().Contains(searchString.ToLower())
                    || x.Acronym.ToLower().Contains(searchString.ToLower())
                    || x.Pk_WorkItemUid == workItemId)
                    && !x.DeletedFlag
                    && x.WiLevel != 0
                    && (!shouldHaveAcronym || !string.IsNullOrEmpty(x.Acronym.Trim()))).OrderBy(x=>x.WorkplanId).ToList();
        }

        /// <summary>
        /// Gets the list of all workitems for a given list of release, regardless of any other criteria.
        /// </summary>
        /// <param name="releaseIds">List of releases IDs</param>
        /// <returns>List of work items.</returns>
        public List<WorkItem> GetAllWorkItemsForReleases(List<int> releaseIds)
        {
            return AllIncluding(t => t.Release, t => t.Remarks, t => t.WorkItems_ResponsibleGroups)
                .Where(x => !x.DeletedFlag
                            && releaseIds.Contains(x.Fk_ReleaseId == null ? -1 : x.Fk_ReleaseId.Value))
                .OrderBy(x =>x.WorkplanId).ToList();
        }

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <param name="granularity">Granularity Level</param>
        /// <param name="hidePercentComplete">Percentage Complete</param>
        /// <param name="wiAcronym">Acronym, we should correspond to the "Effective_Acronym" in database</param>
        /// <param name="wiName">Name</param>
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            return UoW.Context.WorkItems.Where(x => 
                            !x.DeletedFlag
                            && releaseIds.Contains(x.Fk_ReleaseId == null ? -1 : x.Fk_ReleaseId.Value)
                            && (x.Name.ToLower().Contains(wiName.Trim().ToLower()) || String.IsNullOrEmpty(wiName.Trim()))
                            && ((x.Effective_Acronym != null && x.Effective_Acronym.ToLower().Contains(wiAcronym.Trim().ToLower())) || (String.IsNullOrEmpty(wiAcronym.Trim())))
                            && (x.WiLevel != null && x.WiLevel <= granularity)
                            && (hidePercentComplete ? !(x.Completion >= 100 && x.WiLevel == 1) : true)
                            && (tbIds.Count == 0 || x.WorkItems_ResponsibleGroups.Any(y => tbIds.Contains(y.Fk_TbId.Value)))).Count();
        }

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        public List<string> GetAllAcronyms()
        {
            return UoW.Context.WorkItems.Where(x => ((!String.IsNullOrEmpty(x.Acronym)) && (x.Fk_ReleaseId != null))).Select(y => y.Acronym).Distinct().ToList();
        }

        /// <summary>
        /// Look for acronyms which start with (keyword)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>List of acronyms found</returns>
        public List<string> LookForAcronyms(string keyword)
        {
            return UoW.Context.WorkItems.Where(x => ((!String.IsNullOrEmpty(x.Acronym)) && (x.Fk_ReleaseId != null) && x.Acronym.ToLower().StartsWith(keyword.ToLower()))).Select(y => y.Acronym).Distinct().ToList();
        }

        /// <summary>
        /// Gets the work items.
        /// </summary>
        /// <param name="workItems">The work items.</param>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsByIds(List<int> workItems)
        {
            return AllIncluding(wi => wi.WorkItems_ResponsibleGroups).Where(x => workItems.Contains(x.Pk_WorkItemUid)).ToList();
        }

        /// <summary>
        /// Get work items by keywords (acronyms, uids)
        /// </summary>
        /// <param name="keywords">keywords to identify workitems</param>
        /// <returns>List of workitems</returns>
        public List<WorkItem> GetWorkItemsByKeywords(List<string> keywords)
        {
            var keywordsInLower = keywords.ConvertAll(x => x.ToLower());
            return AllIncluding(wi => wi.WorkItems_ResponsibleGroups).Where(x => 
                !x.DeletedFlag && 
                (keywordsInLower.Contains(x.Acronym.ToLower())
                || keywordsInLower.Contains(x.Pk_WorkItemUid.ToString().ToLower()))
                ).OrderBy(x=>x.WorkplanId).ToList();
        }

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        public WorkItem GetPrimeWorkItemBySpecificationID(int specificationID)
        {
            var workItemBySpecID = UoW.Context.Specification_WorkItem.Include(wi => wi.WorkItem).Include(x => x.WorkItem.WorkItems_ResponsibleGroups)
                .Where(x => 
                    x.Fk_SpecificationId == specificationID 
                    && x.isPrime == true 
                    && !x.WorkItem.DeletedFlag).FirstOrDefault();
            return workItemBySpecID != null ? workItemBySpecID.WorkItem : null;
        }

        /// <summary>
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        public Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(List<int> workitemsIds)
        {
            var oneOfWiWithFirstLevel =  UoW.Context.WorkItems.Include(x => x.Release.Enum_ReleaseStatus)
                .Where(x => workitemsIds.Contains(x.Pk_WorkItemUid) && !x.DeletedFlag).OrderBy(x => x.WiLevel).ThenByDescending(x => x.WorkplanId).FirstOrDefault();
            if (oneOfWiWithFirstLevel == null || oneOfWiWithFirstLevel.Release == null)
                return null;
            return oneOfWiWithFirstLevel.Release;
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
        /// <param name="granularity">Granularity Level</param>
        /// <param name="wiAcronym">Acronym</param>
        /// <param name="wiName">Name</param>
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>List of workitems</returns>
        List<WorkItem> GetWorkItemsBySearchCriteria(List<int> releaseIds, int granularity, string wiAcronym, string wiName, List<int> tbIds);

        ///  <summary>
        /// Get the list of workitems based on searchString
        ///  </summary>
        ///  <param name="searchString"></param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        /// <returns>List of workitems</returns>
        List<WorkItem> GetWorkItemsBySearchCriteria(string searchString, bool shouldHaveAcronym = false);

        /// <summary>
        /// Gets the list of all workitems for a given list of release, regardless of any other criteria.
        /// </summary>
        /// <param name="releaseIds">List of releases IDs</param>
        /// <returns>List of work items.</returns>
        List<WorkItem> GetAllWorkItemsForReleases(List<int> releaseIds);

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <param name="granularity">Granularity Level</param>
        /// <param name="hidePercentComplete">Percentage Complete</param>
        /// <param name="wiAcronym">Acronym</param>
        /// <param name="wiName">Name</param>
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>Work Item Count</returns>
        int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds);

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        List<string> GetAllAcronyms();

        /// <summary>
        /// Look for acronyms which start with (keyword)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>List of acronyms found</returns>
        List<string> LookForAcronyms(string keyword);

        /// <summary>
        /// Gets the work items.
        /// </summary>
        /// <param name="workitemIds">The workitem ids.</param>
        /// <returns></returns>
        List<WorkItem> GetWorkItemsByIds(List<int> workitemIds);

        /// <summary>
        /// Get work items by keywords (acronyms, uids)
        /// </summary>
        /// <param name="keywords">keywords to identify workitems</param>
        /// <returns>List of workitems</returns>
        List<WorkItem> GetWorkItemsByKeywords(List<string> keywords);

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        WorkItem GetPrimeWorkItemBySpecificationID(int specificationID);

        /// <summary>
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(List<int> workitemsIds);
    }
}
