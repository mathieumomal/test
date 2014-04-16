using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class WorkItemManager
    {
        private IUltimateUnitOfWork _uoW;

        public WorkItemManager(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="releaseIds">Release Ids</param>
        /// <param name="granularity">Granularity Level</param>
        /// <param name="hidePercentComplete">Percentage Complete</param>
        /// <param name="wiAcronym">Acronym</param>
        /// <param name="wiName">Name</param>
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;

            List<WorkItem> AllWorkItems = new List<WorkItem>();

            var workItemsOfSearchCriteria = repo.GetWorkItemsBySearchCriteria(releaseIds, granularity, wiAcronym, wiName, tbIds);
            AllWorkItems.AddRange(workItemsOfSearchCriteria);

            //Get Parents
            List<WorkItem> parentWorkItems = new List<WorkItem>();
            foreach (var workItem in workItemsOfSearchCriteria)
            {
                GetParentWorkItems(workItem, AllWorkItems, parentWorkItems, repo);
            }
            AllWorkItems.AddRange(parentWorkItems);

            //Remove 100% records at Level 1
            if (hidePercentComplete)
            {
                List<WorkItem> workItemsToRemove = new List<WorkItem>();
                AllWorkItems.Where(x => x.WiLevel == 1 && x.Completion >= 100).ToList().ForEach(x =>
                    {
                        workItemsToRemove.Add(x); //Collect Level 1 records
                        GetChildWorkItems(x, workItemsToRemove, repo); //Collect Child records
                    });
                workItemsToRemove.ForEach(x => AllWorkItems.Remove(x));
            }

            //Get child records for the given granularity
            List<WorkItem> childRecordsForGranularityLevel = new List<WorkItem>();
            AllWorkItems.Where(x => x.WiLevel == granularity).ToList().ForEach(x =>
                {
                    GetChildWorkItems(x, childRecordsForGranularityLevel, repo); //Collect Child records
                });

            AllWorkItems.AddRange(childRecordsForGranularityLevel.Where(x => (x.Name.ToLower().Contains(wiName.Trim().ToLower()) || String.IsNullOrEmpty(wiName.Trim()))
                                                                          && (x.Acronym.ToLower().Contains(wiAcronym.Trim().ToLower()) || (String.IsNullOrEmpty(wiAcronym.Trim())))
                                                                          && (tbIds.Count == 0 || x.WorkItems_ResponsibleGroups.Any(y => tbIds.Contains(y.Fk_TbId.Value)))));

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(AllWorkItems, GetRights(personId));
        }

        /// <summary>
        /// Get Parent WorkItems for the given list of work items
        /// </summary>
        /// <param name="workItem">Work item to search for parent record</param>
        /// <param name="workItems">List of all work items</param>
        /// <param name="parentWorkItems">List of collected parent work items</param>
        /// <param name="repo">Work item repository</param>
        private void GetParentWorkItems(WorkItem workItem, List<WorkItem> workItems, List<WorkItem> parentWorkItems, IWorkItemRepository repo)
        {
            if (workItem.ParentWi != null)
            {
                if ((!workItems.Exists(x => x.Pk_WorkItemUid == workItem.ParentWi.Pk_WorkItemUid)) &&
                (!parentWorkItems.Exists(x => x.Pk_WorkItemUid == workItem.ParentWi.Pk_WorkItemUid)))
                    parentWorkItems.Add(repo.Find(workItem.ParentWi.Pk_WorkItemUid));
                GetParentWorkItems(workItem.ParentWi, workItems, parentWorkItems, repo);
            }
        }

        /// <summary>
        /// Get the child work items for the given work item
        /// </summary>
        /// <param name="workItem">Work item to search for child records</param>
        /// <param name="childWorkItems">List of collected child work items</param>
        /// <param name="repo">Work item repository</param>
        private void GetChildWorkItems(WorkItem workItem, List<WorkItem> childWorkItems, IWorkItemRepository repo)
        {
            if (workItem.ChildWis != null)
            {
                foreach (var childWorkItem in workItem.ChildWis)
                {
                    if (!childWorkItems.Exists(x => x.Pk_WorkItemUid == childWorkItem.Pk_WorkItemUid))
                        childWorkItems.Add(repo.Find(childWorkItem.Pk_WorkItemUid));
                    GetChildWorkItems(childWorkItem, childWorkItems, repo);
                }
            }
        }

        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, string searchString)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(repo.GetWorkItemsBySearchCriteria(searchString), GetRights(personId));
        }

        /// <summary>
        /// Get All Work Items including referenced objects
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetAllWorkItems(int personId)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            var workItems = repo.AllIncluding(x => x.Release, x => x.Remarks, x => x.ChildWis, x => x.ParentWi, x => x.WorkItems_ResponsibleGroups).ToList();

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(workItems, GetRights(personId));
        }

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            var workItem = repo.Find(workItemId);

            return new KeyValuePair<WorkItem, UserRightsContainer>(workItem, GetRights(personId));
        }

        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemByIdExtend(int personId, int workItemId)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            IPersonRepository PersonRepo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = _uoW;
            PersonRepo.UoW = _uoW;

            var workItem = repo.Find(workItemId);

            if (workItem != null & workItem.RapporteurId != null)
            {
                var person = PersonRepo.Find(workItem.RapporteurId.Value);
                workItem.RapporteurName = (person != null) ? person.FIRSTNAME + " " + person.LASTNAME : workItem.RapporteurStr;
            }

            return new KeyValuePair<WorkItem, UserRightsContainer>(workItem, GetRights(personId));
        }

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
        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            return repo.GetWorkItemsCountBySearchCriteria(releaseIds, granularity, hidePercentComplete, wiAcronym, wiName, tbIds);
        }

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        public List<string> GetAllAcronyms()
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            return repo.GetAllAcronyms();
        }

        private UserRightsContainer GetRights(int personId)
        {
            //Computes the rights of the user. These are independant from the workitems.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            return rightManager.GetRights(personId);
        }
    }
}
