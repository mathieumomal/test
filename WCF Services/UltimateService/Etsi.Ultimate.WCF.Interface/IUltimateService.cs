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
    [ServiceKnownType(typeof(ChangeRequest))]
    [ServiceKnownType(typeof(ChangeRequestCategory))]
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
        /// Gets release by identifier
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        [OperationContract]
        Release GetReleaseById(int personId, int releaseId);

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

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>Specification entity</returns>
        [OperationContract]
        Specification GetSpecificationById(int personID, int specificationId);

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>List of specifications</returns>
        [OperationContract]
        List<Specification> GetSpecificationsByIds(int personID, List<int> specificationIds);

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>Change Request entity</returns>
        [OperationContract]
        ChangeRequest GetChangeRequestById(int personID, int changeRequestId);

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        [OperationContract]
        int CreateChangeRequest(int personID, ChangeRequest changeRequest);

        /// <summary>
        /// Changes the request category.
        /// </summary>
        /// <returns>Change request category list</returns>
        [OperationContract]
        List<ChangeRequestCategory> GetChangeRequestCategories(int personId);
    }
}
