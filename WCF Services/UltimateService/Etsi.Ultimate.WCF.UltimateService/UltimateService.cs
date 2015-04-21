﻿using System;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.WCF.Interface;
using Etsi.Ultimate.WCF.Interface.Entities;
using System.Collections.Generic;
using ChangeRequest = Etsi.Ultimate.WCF.Interface.Entities.ChangeRequest;
using Release = Etsi.Ultimate.WCF.Interface.Entities.Release;
using Specification = Etsi.Ultimate.WCF.Interface.Entities.Specification;
using SpecVersion = Etsi.Ultimate.WCF.Interface.Entities.SpecVersion;
using WorkItem = Etsi.Ultimate.WCF.Interface.Entities.WorkItem;

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
            LogManager.Debug("[ServiceCall][GetReleases] PersonId=" + personId);
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
            LogManager.Debug("[ServiceCall][GetReleaseById] PersonId=" + personId+"; ReleaseId="+releaseId);
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
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds)
        {
            LogManager.Debug("[ServiceCall][GetWorkItemsByIds] PersonId=" + personId + "; WorkitemIds=" + string.Join(", ",workItemIds));
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
            LogManager.Debug("[ServiceCall][GetWorkItemsByKeyWord] PersonId=" + personId + "; keyword=" + keyword);
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
            LogManager.Debug("[ServiceCall][GetSpecificationsByKeyWord] PersonId=" + personId + "; keyword=" + keyword);
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
            LogManager.Debug("[ServiceCall][GetSpecificationById] PersonId=" + personId + "; specificationId=" + specificationId);
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
            LogManager.Debug("[ServiceCall][GetSpecificationsByIds] PersonId=" + personId + "; specificationIds=" + string.Join(", ",specificationIds));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetSpecificationsByIds(personId, specificationIds);
        }

        /// <summary>
        /// Changes the specifications status to under change control.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specIdsForUcc">The spec ids</param>
        /// <returns>Status report</returns>
        public Interface.ServiceResponse<bool> ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIdsForUcc)
        {
            LogManager.Debug("[ServiceCall][ChangeSpecificationsStatusToUnderChangeControl] specNumbersForUcc=" + string.Join(", ", specIdsForUcc));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.ChangeSpecificationsStatusToUnderChangeControl(personId, specIdsForUcc);
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
            LogManager.Debug("[ServiceCall][GetChangeRequestById] PersonId=" + personId + "; changeRequestId=" + changeRequestId);
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
        public Interface.ServiceResponse<int> CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            LogManager.Debug("[ServiceCall][CreateChangeRequest] PersonId=" + personId);
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
            LogManager.Debug("[ServiceCall][EditChangeRequest] PersonId=" + personId);
            var serviceHelper = new ServiceHelper();
            return serviceHelper.EditChangeRequest(personId, changeRequest);
        }

        /// <summary>
        /// Changes the request categories.
        /// </summary>
        /// <returns> Change request categories list</returns>      
        public List<ChangeRequestCategory> GetChangeRequestCategories()
        {
            LogManager.Debug("[ServiceCall][GetChangeRequestCategories]");
            var svcHelper = new ServiceHelper();
            return svcHelper.GetChangeRequestCategories();
        }

        /// <summary>
        /// Returns the CR data for a contribution using it's UID
        /// </summary>
        /// <param name="contributionUid">ContributionUID</param>
        /// <returns></returns>
        public ChangeRequest GetChangeRequestByContributionUid(string contributionUid)
        {
            LogManager.Debug("[ServiceCall][GetChangeRequestByContributionUid] contributionUid=" + contributionUid);
            var svcHelper = new ServiceHelper();
            return svcHelper.GetChangeRequestByContributionUid(contributionUid);
        }

        /// <summary>
        /// Returns list of Crs using list of contribution uids
        /// </summary>
        /// <param name="contributionUids"></param>
        /// <returns></returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(
            List<string> contributionUids)
        {
            LogManager.Debug("[ServiceCall][GetChangeRequestListByContributionUidList] contributionUids=" + string.Join(", ",contributionUids));
            var svcHelper = new ServiceHelper();
            return svcHelper.GetChangeRequestListByContributionUidList(contributionUids);
        }

        /// <summary>
        /// Default implementation. Asks the service to return all the possible statuses for a CR
        /// </summary>
        /// <returns></returns>
        public List<ChangeRequestStatus> GetAllChangeRequestStatuses()
        {
            LogManager.Debug("[ServiceCall][GetAllChangeRequestStatuses]");
            var svcHelper = new ServiceHelper();
            return svcHelper.GetAllChangeRequestStatuses();
        }

        /// <summary>
        /// The aim of this method is to be able to update the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crKeys"></param>
        public bool UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<Interface.Entities.CrKeyFacade, string>> crKeys)
        {
            LogManager.Debug("[ServiceCall][UpdateChangeRequestPackRelatedCrs]");
            var svcHelper = new ServiceHelper();
            return svcHelper.UpdateChangeRequestPackRelatedCrs(crKeys);
        }

        /// <summary>
        /// Sets the CRS as final.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="tdocNumbers">The tdoc numbers.</param>
        /// <returns>Status report</returns>
        public Interface.ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers)
        {
            LogManager.Debug("[ServiceCall][SetCrsAsFinal] PersonId=" + personId + "; tdocNumbers=" + string.Join(", ", tdocNumbers));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.SetCrsAsFinal(personId, tdocNumbers);
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        public bool DoesCrNumberRevisionCoupleExist(int personId, int specId, string crNumber, int revision)
        {
            LogManager.Debug("[ServiceCall][IsExistCrNumberRevisionCouple] CrNumber=" + crNumber + "; revision=" + revision);
            var serviceHelper = new ServiceHelper();
            return serviceHelper.DoesCrNumberRevisionCoupleExist(personId, specId, crNumber, revision);
        }

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        public List<ChangeRequest> GetCrsByKeys(List<Interface.Entities.CrKeyFacade> crKeys)
        {
            LogManager.Debug("[ServiceCall][GetCrsByKeys]");
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetCrsByKeys(crKeys);
        }

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        public ChangeRequest GetCrByKey(CrKeyFacade crKey)
        {
            LogManager.Debug(String.Format("[ServiceCall][GetCrByKey] crKey=[Spec# {0}, Cr# {1}, Revision# {2} ];", crKey.SpecNumber, crKey.CrNumber, crKey.Revision));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetCrByKey(crKey);
        }

        /// <summary>
        /// Reissue the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <returns>Success/Failure</returns>
        public Interface.ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId)
        {
            LogManager.Debug(String.Format("[ServiceCall][ReIssueCr] crKey=[Spec# {0}, Cr# {1}, Revision# {2} ]; newTsgTdoc={3}; newTsgMeetingId={4}", crKey.SpecNumber, crKey.CrNumber, crKey.Revision, newTsgTdoc, newTsgMeetingId));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.ReIssueCr(crKey, newTsgTdoc, newTsgMeetingId);
        }

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <returns>Success/Failure</returns>
        public Interface.ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId)
        {
            LogManager.Debug(String.Format("[ServiceCall][ReviseCr] crKey=[Spec# {0}, Cr# {1}, Revision# {2} ]; newTsgTdoc={3}; newTsgMeetingId={4}", crKey.SpecNumber, crKey.CrNumber, crKey.Revision, newTsgTdoc, newTsgMeetingId));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.ReviseCr(crKey, newTsgTdoc, newTsgMeetingId);        
        }

        #endregion

        #region Version services

        /// <summary>
        /// Get versions related to specification & release
        /// </summary>
        /// <param name="specId">Specification Identifier</param>
        /// <param name="releaseId">Release Identifier</param>
        /// <returns>List of versions</returns>
        public List<SpecVersion> GetVersionsForSpecRelease(int specId, int releaseId)
        {
            LogManager.Debug("[ServiceCall][GetVersionsForSpecRelease] Spec Id=" + specId + "; Release Id=" + releaseId);
            var serviceHelper = new ServiceHelper();
            return serviceHelper.GetVersionsForSpecRelease(specId, releaseId);
        }

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
        public Interface.ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc)
        {
            LogManager.Debug(string.Format("[ServiceCall][AllocateOrAssociateDraftVersion] Spec Id={0}; Version={1}; Tdoc={2}",
                                                                 specId, majorVersion + "." + technicalVersion + "." + editorialVersion, relatedTdoc));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion, editorialVersion, relatedTdoc);
        }

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
        public Interface.ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            LogManager.Debug(
                string.Format(
                    "[ServiceCall][CheckDraftCreationOrAssociation]  Person Id = {0}; Spec Id={1}; Release Id={2}; Version={3};",
                    personId, specId, releaseId, String.Format("{0}.{1}.{2}", majorVersion, technicalVersion, editorialVersion)));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.CheckDraftCreationOrAssociation(personId, specId, releaseId, majorVersion, technicalVersion, editorialVersion);
        }

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
        public Interface.ServiceResponse<string> CheckVersionForUpload(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string filePath)
        {
            LogManager.Debug(
                string.Format(
                    "[ServiceCall][CheckVersionForUpload]  Person Id = {0}; Spec Id = {1}; Release Id = {2}; Meeting Id = {3}; Version = {4}; File Name = {5};",
                    personId, specId, releaseId, meetingId, String.Format("{0}.{1}.{2}", majorVersion, technicalVersion, editorialVersion), filePath));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.CheckVersionForUpload(personId, specId, releaseId, meetingId, majorVersion, technicalVersion, editorialVersion, filePath);        
        }

        /// <summary>
        /// Uploads the version.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Success/Failure</returns>
        public Interface.ServiceResponse<bool> UploadVersion(int personId, string token) 
        {
            LogManager.Debug(
               string.Format(
                   "[ServiceCall][UploadVersion]  Person Id = {0}; token = {1};",
                   personId, token));
            var serviceHelper = new ServiceHelper();
            return serviceHelper.UploadVersion(personId, token);
        }

        #endregion
    }
}