﻿using Etsi.Ultimate.WCF.Interface.Entities;
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
        /// <param name="personId">The person identifier.</param>
        /// <returns>
        /// List of Releases
        /// </returns>
        [OperationContract]
        List<Release> GetReleases(int personId);

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
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds);

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsByKeyWord(int personId, string keyword);

        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>List of specifications</returns>
        [OperationContract]
        List<Specification> GetSpecificationsByKeyWord(int personId, string keyword);

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>Specification entity</returns>
        [OperationContract]
        Specification GetSpecificationById(int personId, int specificationId);

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>List of specifications</returns>
        [OperationContract]
        List<Specification> GetSpecificationsByIds(int personId, List<int> specificationIds);

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>Change Request entity</returns>
        [OperationContract]
        ChangeRequest GetChangeRequestById(int personId, int changeRequestId);

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        [OperationContract]
        int CreateChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Edit the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        bool EditChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Changes the request categories.
        /// </summary>
        /// <returns>Change request categories list</returns>
        [OperationContract]
        List<ChangeRequestCategory> GetChangeRequestCategories();
    }
}
