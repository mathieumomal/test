using Etsi.Ultimate.WCF.Interface;
using Etsi.Ultimate.WCF.Interface.Entities;
using System.Collections.Generic;

namespace Etsi.Ultimate.WCF.Service
{
    /// <summary>
    /// Provide the information which is related to ultimate database
    /// </summary>
    public class UltimateService : IUltimateService
    {
        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <returns>List of Releases</returns>
        public List<Release> GetReleases(int personID)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetReleases(personID);
        }

        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<WorkItem> GetWorkItemsByIds(int personID, List<int> workItemIds)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetWorkItemsByIds(personID, workItemIds);
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<WorkItem> GetWorkItemsByKeyWord(int personID, string keyword)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetWorkItemsByKeyWord(personID, keyword);
        }
    }
}