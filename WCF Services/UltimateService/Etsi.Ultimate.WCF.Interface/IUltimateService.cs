using System;
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
    [ServiceKnownType(typeof(CrOfCrPackFacade))]
    public interface IUltimateService
    {
        #region release
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
        /// Gets the releases filtered by status.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="releaseStatus">release status id</param>
        /// <returns>
        /// List of Releases
        /// </returns>
        [OperationContract]
        List<Release> GetReleasesByStatus(int personId, ReleaseStatus releaseStatus);

        /// <summary>
        /// Gets release by identifier
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        [OperationContract]
        Release GetReleaseById(int personId, int releaseId);

        [OperationContract]
        Release GetHighestNonClosedReleaseLinkedToASpec(int specId);

        #endregion

        #region wi
        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns></returns>
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
        /// Gets the work items with acronym by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsWithAcronymByKeyWord(int personId, string keyword);

        /// <summary>
        /// Gets the work items by key words.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keywords">The keywords to identify work items.</param>
        /// <returns>List of work items</returns>
        [OperationContract]
        List<WorkItem> GetWorkItemsByKeyWords(int personId, List<string> keywords);

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        [OperationContract]
        WorkItem GetPrimeWorkItemBySpecificationID(int specificationID);

        /// <summary>
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        [OperationContract]
        Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(int personId, List<int> workitemsIds);
        #endregion

        #region specs
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
        /// Changes the specifications status to under change control.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specIdsForUcc">The spec ids.</param>
        /// <returns>Status report</returns>
        [OperationContract]
        ServiceResponse<bool> ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIdsForUcc);

        /// <summary>
        /// Gets the specifications by numbers.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specNumbers">The specification numbers.</param>
        /// <returns>List of specifications</returns>
        [OperationContract]
        List<Specification> GetSpecificationsBySpecNumbers(int personId, List<string> specNumbers);

        #endregion

        #region crs
        [OperationContract]
        ServiceResponse<bool> UpdateCrStatus(string uid, string status);

        /// <summary>
        /// Sets the CRS as final.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="tdocNumbers">The tdoc numbers.</param>
        /// <returns>Status report</returns>
        [OperationContract]
        ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers);

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
        ServiceResponse<int> CreateChangeRequest(int personId, ChangeRequest changeRequest);

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

        /// <summary>
        /// Returns the CR data for a contribution using it's UID
        /// </summary>
        /// <param name="contributionUid">ContributionUID</param>
        /// <returns></returns>
        [OperationContract]
        ChangeRequest GetChangeRequestByContributionUid(string contributionUid);

        /// <summary>
        /// Return list of CRs using list of contribution unique identifiers
        /// </summary>
        /// <param name="contributionUids"></param>
        /// <returns></returns>
        [OperationContract]
        List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUids);

        [OperationContract]
        List<ChangeRequest> GetWgCrsByWgTdocList(List<string> contribUids);

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Key value pair with bool (success status), and the change request</returns>
        [OperationContract]
        ChangeRequest GetLightChangeRequestForMinuteMan(string uid);

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        [OperationContract]
        List<ChangeRequest> GetLightChangeRequestsForMinuteMan(List<string> uids);

        /// <summary>
        /// Return the list of all change request categories, that can be used to fill the ChangeRequest.Fk_TSGStatus and ChangeRequest.Fk_WGStatus fields
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ChangeRequestStatus> GetAllChangeRequestStatuses();

        /// <summary>
        /// The aim of this method is to be able to update the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackDecisionlist"></param>
        [OperationContract]
        bool UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackDecisionlist);

        /// <summary>
        /// Test if a couple Cr # / Revision already exist
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        [OperationContract]
        bool DoesCrNumberRevisionCoupleExist(int personId, int specId, string crNumber, int revision);

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        [OperationContract]
        List<ChangeRequest> GetCrsByKeys(List<CrKeyFacade> crKeys);

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        [OperationContract]
        ChangeRequest GetCrByKey(CrKeyFacade crKey);

        /// <summary>
        /// Reissue the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource);

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource);

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        [OperationContract]
        List<KeyValuePair<CrKeyFacade, string>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys);
        #endregion

        #region cr pack
        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        [OperationContract]
        List<ChangeRequest> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid);

        /// <summary>
        /// Same method than for GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        [OperationContract]
        List<ChangeRequest> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids);

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="CrsOfCrPack"></param>
        /// <returns></returns>
        [OperationContract]
        ServiceResponse<bool> UpdateCrsStatusOfCrPack(List<CrOfCrPackFacade> CrsOfCrPack);

        [OperationContract]
        ServiceResponse<bool> UpdateCrsInsideCrPack(ChangeRequestPackFacade crPack,
            List<ChangeRequestInsideCrPackFacade> crs, int personId);
        #endregion

        #region Versions

        [OperationContract]
        List<SpecVersion> GetVersionsBySpecId(int specificationId);

        /// <summary>
        /// Get versions related to specification & release
        /// </summary>
        /// <param name="specId">Specification Identifier</param>
        /// <param name="releaseId">Release Identifier</param>
        /// <returns>List of versions</returns>
        [OperationContract]
        List<SpecVersion> GetVersionsForSpecRelease(int specId, int releaseId);

        /// <summary>
        /// Link TDoc to Version (by associate or allocate if needed)
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId">The specification identifier</param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <param name="relatedTdoc">Related Tdoc</param>
        /// <param name="releaseId"></param>
        /// <returns>Success/Failure status</returns>
        [OperationContract]
        ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc);

        /// <summary>
        /// Checks the draft creation or association.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specId">The spec identifier.</param>
        /// <param name="releaseId">The release identifier.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <returns>Draft creation or association status along with validation failures</returns>
        [OperationContract]
        ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion);

        /// <summary>
        /// Checks the version for upload.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specId">The spec identifier.</param>
        /// <param name="releaseId">The release identifier.</param>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Return cached token for version upload</returns>
        [OperationContract]
        ServiceResponse<string> CheckVersionForUpload(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string filePath);

        /// <summary>
        /// Uploads the version.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        ServiceResponse<bool> UploadVersion(int personId, string token);

        /// <summary>
        /// Get latest version of each relaease for the given spec ids
        /// </summary>
        /// <param name="specIds">The specification identifiders</param>
        /// <returns>List of Spec Versions</returns>
        [OperationContract]
        List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds);

        /// <summary>
        /// Unlink tdoc from related version
        /// </summary>
        /// <param name="uid">Tdoc uid</param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        [OperationContract]
        ServiceResponse<bool> UnlinkTdocFromVersion(string uid, int personId);

        /// <summary>
        /// Create version for pCR tdoc if necessary (if doesn't already exist)
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <returns></returns>
        [OperationContract]
        ServiceResponse<bool> CreatepCrDraftVersionIfNecessary(int personId, int specId, int releaseId,
            int meetingId, int majorVersion, int technicalVersion, int editorialVersion);

        #endregion

        #region finalize approved drafts

        [OperationContract]
        ServiceResponse<bool> FinalizeApprovedDrafts(int personId, int mtgId, List<Tuple<int, int, int>> approvedDrafts);

        #endregion

        #region communities

        [OperationContract]
        List<Community> GetCommunitiesByIds(List<int> ids);

        #endregion
    }
}
