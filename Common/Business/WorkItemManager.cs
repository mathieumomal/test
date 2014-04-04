using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
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
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsByRelease(int personId, List<int> releaseIds)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;

            List<WorkItem> workItems = new List<WorkItem>();

            var workItemsOfSearchCriteria = repo.GetWorkItemsByRelease(releaseIds);
            workItems.AddRange(workItemsOfSearchCriteria);
            //Get Parents
            List<WorkItem> parentWorkItems = new List<WorkItem>();
            foreach (var workItem in workItemsOfSearchCriteria)
            {
                GetParentWorkItems(workItem, workItems, parentWorkItems, repo);
            }

            workItems.AddRange(parentWorkItems);

            List<WorkItem> childWorkItems = new List<WorkItem>();
            foreach (var workItem in workItemsOfSearchCriteria)
            {
                GetChildWorkItems(workItem, workItems, childWorkItems, repo);
            }
            workItems.AddRange(childWorkItems);

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(workItems, GetRights(personId));
        }

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

        private void GetChildWorkItems(WorkItem workItem, List<WorkItem> workItems, List<WorkItem> childWorkItems, IWorkItemRepository repo)
        {
            if (workItem.ChildWis != null)
            {
                foreach (var childWorkItem in workItem.ChildWis)
                {
                    if ((!workItems.Exists(x => x.Pk_WorkItemUid == childWorkItem.Pk_WorkItemUid)) &&
                    (!childWorkItems.Exists(x => x.Pk_WorkItemUid == childWorkItem.Pk_WorkItemUid)))
                        childWorkItems.Add(repo.Find(childWorkItem.Pk_WorkItemUid));
                    GetChildWorkItems(childWorkItem, workItems, childWorkItems, repo);
                }
            }
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
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountByRelease(List<int> releaseIds)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            return repo.GetWorkItemsCountByRelease(releaseIds);
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
