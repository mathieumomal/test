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
            var workItems = repo.GetWorkItemsByRelease(releaseIds);

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(workItems, GetRights(personId));
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
