﻿using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class WorkItemManager
    {
        private IUltimateUnitOfWork _uoW;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemManager"/> class.
        /// </summary>
        /// <param name="uoW">The Unit of work</param>
        public WorkItemManager(IUltimateUnitOfWork uoW)
        {
            _uoW = uoW;
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
            ExtensionLogger.Info("WORKPLAN RESEARCH: System is searching for Workitems with following search criterias...", new List<KeyValuePair<string, object>>{
                new KeyValuePair<string, object>("personId", personId),
                new KeyValuePair<string, object>("releaseIds", releaseIds),
                new KeyValuePair<string, object>("granularity", granularity),
                new KeyValuePair<string, object>("hidePercentComplete", hidePercentComplete),
                new KeyValuePair<string, object>("wiAcronym", wiAcronym),
                new KeyValuePair<string, object>("wiName", wiName),
                new KeyValuePair<string, object>("tbIds", tbIds)
            });
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;

            // Search simplification:
            // In case only the Releases and the granularity are specified, we anyway will need to retrieve all the work items
            // from the releases. Thus, we retrieve them directly, to speed up performances.
            // Additionally, if TSG/WG are passed, we will clean up the mess.
            if (string.IsNullOrEmpty(wiAcronym) && string.IsNullOrEmpty(wiName) && !hidePercentComplete)
            {
                LogManager.Info("WORKPLAN RESEARCH:     FAST research is running...");
                var wiList = repo.GetAllWorkItemsForReleases(releaseIds);
                if (tbIds.Count > 0)
                {
                    // Get all the WIs for current granularity who are not using the TBs.
                    var nonEligibleWis = wiList.Where(x => x.WiLevel == granularity && !x.WorkItems_ResponsibleGroups.Any(y => tbIds.Contains(y.Fk_TbId.Value))).ToList();
                    List<WorkItem> children = new List<WorkItem>();
                    while (nonEligibleWis.Count != 0)
                    {
                        children = nonEligibleWis.SelectMany(x => x.ChildWis).Distinct().ToList();
                        nonEligibleWis.ForEach(x => wiList.Remove(x));
                        wiList.RemoveAll(x => nonEligibleWis.Select(nw => nw.Pk_WorkItemUid).ToList().Contains(x.Pk_WorkItemUid));
                        nonEligibleWis = children;
                    }
                    // Clean up parents
                    for (var i = granularity - 1; i >= 0; --i)
                    {
                        var wiToDelete = wiList.Where(wi => wi.WiLevel == i && !wi.ChildWis.Any(x => wiList.Contains(x)) && !wi.WorkItems_ResponsibleGroups.Any(x => tbIds.Contains(x.Fk_TbId.Value))).Select(wi => wi.Pk_WorkItemUid);
                        wiList.RemoveAll(wi => wiToDelete.Contains(wi.Pk_WorkItemUid));
                    }
                }

                // TDoc link init
                wiList.ForEach(wi => {
                    if (!string.IsNullOrWhiteSpace(wi.Wid))
                    {
                        wi.TdocLink = "javascript:openTdoc('" + string.Format(ConfigVariables.TdocDetailsUrl, wi.Wid) + "','" + wi.Wid + "')";
                    }
                });
                LogManager.Info("WORKPLAN RESEARCH: Research for workitems is finished. END.");
                return new KeyValuePair<List<WorkItem>, UserRightsContainer>(wiList, GetRights(personId));
            }

            LogManager.Info("WORKPLAN RESEARCH:     LONG research is running...");
            List<WorkItem> AllWorkItems = new List<WorkItem>();

            var workItemsOfSearchCriteria = repo.GetWorkItemsBySearchCriteria(releaseIds, granularity, wiAcronym, wiName, tbIds);
            AllWorkItems.AddRange(workItemsOfSearchCriteria);

            //Get Parents
            List<WorkItem> parentWorkItems = new List<WorkItem>();
            LogManager.Info("WORKPLAN RESEARCH:     System is gathering info about parents of WIs...");
            foreach (var workItem in workItemsOfSearchCriteria)
            {
                GetParentWorkItems(workItem, AllWorkItems, parentWorkItems, repo);
            }
            AllWorkItems.AddRange(parentWorkItems);

            //Remove 100% records at Level 1
            if (hidePercentComplete)
            {
                LogManager.Info("WORKPLAN RESEARCH:     System is removing WIs with 100% of completion at level 1...");
                List<WorkItem> workItemsToRemove = new List<WorkItem>();
                AllWorkItems.Where(x => x.WiLevel == 1 && x.Completion >= 100).ToList().ForEach(x =>
                    {
                        workItemsToRemove.Add(x); //Collect Level 1 records
                        GetChildWorkItems(x, workItemsToRemove, repo); //Collect Child records
                    });
                workItemsToRemove.ForEach(x => AllWorkItems.Remove(x));
            }

            //Get child records for the given granularity
            LogManager.Info("WORKPLAN RESEARCH:     System is gathering info about childs of WIs for the given granularity...");
            List<WorkItem> childRecordsForGranularityLevel = new List<WorkItem>();
            AllWorkItems.Where(x => x.WiLevel == granularity).ToList().ForEach(x =>
                {
                    GetChildWorkItems(x, childRecordsForGranularityLevel, repo); //Collect Child records
                });

            AllWorkItems.AddRange(childRecordsForGranularityLevel);

            LogManager.Info("WORKPLAN RESEARCH: Research for workitems is finished. END.");
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

        /// <summary>
        /// Gets the work items by search criteria.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="searchString">The search string.</param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        /// <returns>Work items</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, string searchString, bool shouldHaveAcronym = false)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;

            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(repo.GetWorkItemsBySearchCriteria(searchString, shouldHaveAcronym), GetRights(personId));
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
        /// Gets the work item by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns></returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemByIds(int personId, List<int> workItemIds)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            var workItems = repo.GetWorkItemsByIds(workItemIds);
            return new KeyValuePair<List<WorkItem>, UserRightsContainer>(workItems, GetRights(personId));
        }

        /// <summary>
        /// Get work items by keywords (acronyms, uids)
        /// </summary>
        /// <param name="personId">The person identifier</param>
        /// <param name="keywords">keywords to identify workitems</param>
        /// <returns>List of workitems</returns>
        public List<WorkItem> GetWorkItemsByKeywords(int personId, List<string> keywords)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            var workItems = repo.GetWorkItemsByKeywords(keywords);
            return workItems;
        }

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        public WorkItem GetPrimeWorkItemBySpecificationID(int specificationID)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            var workItemBySpecID = repo.GetPrimeWorkItemBySpecificationID(specificationID);
            return workItemBySpecID;
        }

        /// <summary>
        /// Gets the work item by identifier extend.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemId">The work item identifier.</param>
        /// <returns></returns>
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
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        public Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(int personId, List<int> workitemsIds)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            return repo.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(workitemsIds);
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

        /// <summary>
        /// Look for acronyms which start with (keyword)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>List of acronyms found</returns>
        public List<string> LookForAcronyms(string keyword)
        {
            IWorkItemRepository repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = _uoW;
            return repo.LookForAcronyms(keyword);
        }

        /// <summary>
        /// Gets the rights.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>User rights container</returns>
        private UserRightsContainer GetRights(int personId)
        {
            //Computes the rights of the user. These are independant from the workitems.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            return rightManager.GetRights(personId);
        }
    }
}
