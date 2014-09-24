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
        /// Get a release by its id
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public Release GetReleaseById(int personId, int releaseId)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetReleaseById(personId, releaseId);
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

        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        public List<Specification> GetSpecificationsByKeyWord(int personID, string keyword)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationsByKeyWord(personID, keyword);
        }

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>
        /// Specification entity
        /// </returns>
        public Specification GetSpecificationById(int personID, int specificationId)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationById(personID, specificationId);
        }

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        public List<Specification> GetSpecificationsByIds(int personID, List<int> specificationIds)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationsByIds(personID, specificationIds);
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>
        /// Change Request entity
        /// </returns>
        public ChangeRequest GetChangeRequestById(int personID, int changeRequestId)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetChangeRequestById(personID, changeRequestId);
        }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>
        /// Primary key of newly inserted change request
        /// </returns>
        public int CreateChangeRequest(int personID, ChangeRequest changeRequest)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.CreateChangeRequest(personID, changeRequest);
        }
    }
}