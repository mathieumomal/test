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
        /// <param name="token"></param>
        /// <returns></returns>
        bool ImportWorkPlan(string token);

        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="releaseIds">Release Ids</param>
        /// <returns>List of workitems along with rights container</returns>
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsByRelease(int personId, List<int> releaseIds);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId);
    }
}
