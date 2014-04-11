using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public interface IWorkItemService
    {
        /// <summary>
        /// Analyse imported workItems and return errors and warnings logs + cache token
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        KeyValuePair<string, ImportReport> AnalyseWorkPlanForImport(String path);

        /// <summary>
        /// Import workitems uploaded on the server
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="exportPath">Export Path</param>
        /// <returns>Success/Failure</returns>
        bool ImportWorkPlan(string token, string exportPath);

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
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds);

        KeyValuePair<List<WorkItem>, UserRightsContainer> GetAllWorkItems(int personId);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item, right container and other required properties</returns>
        KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemByIdExtend(int personId, int workItemId);

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
    }
}
