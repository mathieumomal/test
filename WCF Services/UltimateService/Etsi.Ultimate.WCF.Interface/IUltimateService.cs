using Etsi.Ultimate.WCF.Interface.Entities;
using System.Collections.Generic;
using System.ServiceModel;

namespace Etsi.Ultimate.WCF.Interface
{
    /// <summary>
    /// Provide the information which is related to ultimate database
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(Release))]
    [ServiceKnownType(typeof(WorkItem))]
    public interface IUltimateService
    {
        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <returns>
        /// List of Releases
        /// </returns>
        [OperationContract]
        List<Release> GetReleases(int personID);

        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsByIds(int personID, List<int> workItemIds);

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsByKeyWord(int personID, string keyword);

        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>List of specifications</returns>
        [OperationContract]
        List<Specification> GetSpecificationsByKeyWord(int personID, string keyword);
    }
}
