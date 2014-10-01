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
        #region Releases services        
        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>List of Releases</returns>
        public List<Release> GetReleases(int personId)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetReleases(personId);
        }

        /// <summary>
        /// Get a release by its id
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public Release GetReleaseById(int personId, int releaseId)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetReleaseById(personId, releaseId);
        }
        #endregion

        #region WI services
        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetWorkItemsByIds(personId, workItemIds);
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<WorkItem> GetWorkItemsByKeyWord(int personId, string keyword)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetWorkItemsByKeyWord(personId, keyword);
        }
        #endregion

        #region Specifications services
        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        public List<Specification> GetSpecificationsByKeyWord(int personId, string keyword)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationsByKeyWord(personId, keyword);
        }

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>
        /// Specification entity
        /// </returns>
        public Specification GetSpecificationById(int personId, int specificationId)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationById(personId, specificationId);
        }

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        public List<Specification> GetSpecificationsByIds(int personId, List<int> specificationIds)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationsByIds(personId, specificationIds);
        }
        #endregion

        #region CRs services        
        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>Change Request entity</returns>
        public ChangeRequest GetChangeRequestById(int personId, int changeRequestId)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetChangeRequestById(personId, changeRequestId);
        }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>
        /// Primary key of newly inserted change request
        /// </returns>
        public int CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.CreateChangeRequest(personId, changeRequest);
        }

        /// <summary>
        /// Edit the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        public bool EditChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var serviceHelper = new ServiceHelper();
            return serviceHelper.EditChangeRequest(personId, changeRequest);
        }

        /// <summary>
        /// Changes the request categories.
        /// </summary>
        /// <returns> Change request categories list</returns>      
        public List<ChangeRequestCategory> GetChangeRequestCategories()
        {
            var svcHelper = new ServiceHelper();
            return svcHelper.GetChangeRequestCategories();
        }

        /// <summary>
        /// Returns the CR data for a contribution using it's UID
        /// </summary>
        /// <param name="ContributionUID">ContributionUID</param>
        /// <returns></returns>
        public ChangeRequest GetChangeRequestByContributionUID(string ContributionUID)
        {
            ServiceHelper svcHelper = new ServiceHelper();
            return svcHelper.GetChangeRequestByContributionUID(ContributionUID);
        }
        #endregion
    }
}