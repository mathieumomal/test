using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.WCF.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.ObjectBuilder2;
using UltimateEntities = Etsi.Ultimate.DomainClasses;
using UltimateServiceEntities = Etsi.Ultimate.WCF.Interface.Entities;
using Etsi.Ultimate.WCF.Interface.Entities;
using System.Linq;

namespace Etsi.Ultimate.WCF.Service
{
    /// <summary>
    /// Helper class to provide necessary information to service
    /// </summary>
    public class ServiceHelper
    {
        #region Constants

        private const string ConstErrorTemplateGetReleases = "Ultimate Service Error [GetReleases]: {0}";
        private const string ConstErrorTemplateGetWorkitemsByIds = "Ultimate Service Error [GetWorkItemsByIds]: {0}";
        private const string ConstErrorTemplateGetWorkitemsByKeyword = "Ultimate Service Error [GetWorkItemsByKeyWord]: {0}";
        private const string ConstErrorTemplateGetWorkItemsByKeyWords = "Ultimate Service Error [GetWorkItemsByKeyWords]: {0}";
        private const string ConstErrorTemplateGetSpecificationsByKeyword = "Ultimate Service Error [GetSpecificationsByKeyWord]: {0}";
        private const string ConstErrorTemplateGetSpecificationById = "Ultimate Service Error [GetSpecificationById]: {0}";
        private const string ConstErrorTemplateGetSpecificationsByIds = "Ultimate Service Error [GetSpecificationsByIds]: {0}";
        private const string ConstErrorTemplateGetSpecificationsBySpecNumbers = "Ultimate Service Error [GetSpecificationsBySpecNumbers]: {0}";
        private const string ConstErrorTemplateCreateChangeRequest = "Ultimate Service Error [CreateChangeRequest]: {0}";
        private const string ConstErrorTemplateEditChangeRequest = "Ultimate Service Error [EditChangeRequest]: {0}";
        private const string ConstErrorTemplateUpdateChangeRequestPackRelatedCrs = "Ultimate Service Error [UpdateChangeRequestPackRelatedCrs]: {0}";
        private const string ConstErrorTemplateCreateChangeRequestCategories = "Ultimate Service Error [GetChangeRequestCategories]: {0}";
        private const string ConstErrorTemplateCreateChangeRequestById = "Ultimate Service Error [GetChangeRequestById]: {0}";
        private const string ConstErrorTemplateGetChangeRequestByContribUid = "Ultimate Service Error [GetChangeRequestByContributionUID]: {0}";
        private const string ConstErrorTemplateGetLightChangeRequestForMinuteMan = "Ultimate Service Error [GetLightChangeRequestForMinuteMan]: {0}";
        private const string ConstErrorTemplateGetLightChangeRequestsForMinuteMan = "Ultimate Service Error [GetLightChangeRequestsForMinuteMan]: {0}";
        private const string ConstErrorTemplateGetLightChangeRequestsInsideCrPackForMinuteMan = "Ultimate Service Error [GetLightChangeRequestsInsideCrPackForMinuteMan]: {0}";
        private const string ConstErrorTemplateGetLightChangeRequestsInsideCrPacksForMinuteMan = "Ultimate Service Error [GetLightChangeRequestsInsideCrPacksForMinuteMan]: {0}";
        private const string ConstErrorTemplateChangeSpecificationsStatusToUnderChangeControl = "Ultimate Service Error [ChangeSpecificationsStatusToUnderChangeControl]: {0}";
        private const string ConstErrorTemplateSetCrsAsFinal = "Ultimate Service Error [SetCrsAsFinal]: {0}";
        private const string ConstErrorIsExistCrNumberRevisionCouple = "Ultimate Service Error [IsExistCrNumberRevisionCouple]: {0}";
        private const string ConstErrorTemplateGetVersionsForSpecRelease = "Ultimate Service Error [GetVersionsForSpecRelease]: {0}";
        private const string ConstErrorTemplateUpdateVersionRelatedTdoc = "Ultimate Service Error [UpdateVersionRelatedTdoc]: {0}";
        private const string ConstErrorTemplateCheckDraftCreationOrAssociation = "Ultimate Service Error [CheckDraftCreationOrAssociation]: {0}";
        private const string ConstErrorTemplateCheckVersionForUpload = "Ultimate Service Error [CheckVersionForUpload]: {0}";
        private const string ConstErrorTemplateUploadVersion = "Ultimate Service Error [UploadVersion]: {0}";
        private const string ConstErrorTemplateReIssueCr = "Ultimate Service Error [ReIssueCr]: {0}";
        private const string ConstErrorTemplateReviseCr = "Ultimate Service Error [ReviseCr]: {0}";
        private const string ConstErrorTemplateGetCrsByKeys = "Ultimate Service Error [GetCrsByKeys]: {0}";
        private const string ConstErrorTemplateGetCrByKey = "Ultimate Service Error [GetCrByKey]: {0}";
        private const string ConstErrorTemplateGetCrWgTdocNumberOfParent = "Ultimate Service Error [GetCrWgTdocNumberOfParent]: {0}";
        private const string ConstErrorTemplateRemoveCrsFromCrPack = "Ultimate Service Error [RemoveCrsFromCrPack]: {0}";
        private const string ConstErrorTemplateGetLatestVersionsBySpecIds = "Ultimate Service Error [GetLatestVersionsBySpecIds]: {0}";
        private const string ConstErrorTemplateUpdateCrStatus = "Ultimate Service Error [UpdateCrStatus]: {0}";
        private const string ConstErrorTemplateUpdateCrsStatusOfCrPack = "Ultimate Service Error [UpdateCrsStatusOfCrPack]: {0}";
        #endregion

        #region Release

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>List of releases</returns>
        internal List<UltimateServiceEntities.Release> GetReleases(int personId)
        {
            var releases = new List<UltimateServiceEntities.Release>();

            try
            {
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObjects = svc.GetAllReleases(personId);
                if (releaseRightsObjects.Key != null)
                    releaseRightsObjects.Key.ForEach(x => releases.Add(ConvertUltimateReleaseToServiceRelease(x)));
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetReleases, "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return releases;
        }


        /// <summary>
        /// Gets the releases filtered by status.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="releaseStatus">release status id</param>
        /// <returns>List of releases</returns>
        internal List<UltimateServiceEntities.Release> GetReleasesByStatus(int personId, ReleaseStatus releaseStatus)
        {
            var releases = new List<UltimateServiceEntities.Release>();

            try
            {
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObjects = svc.GetAllReleasesByStatus(personId,
                    ConvertUltimateReleaseStatusEnumToDomainReleaseStatusEnum(releaseStatus));

                if (releaseRightsObjects.Key != null)
                    releaseRightsObjects.Key.ForEach(x => releases.Add(ConvertUltimateReleaseToServiceRelease(x)));
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetReleases, "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return releases;
        }

        /// <summary>
        /// Gets the release by identifier.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns>Return release</returns>
        internal UltimateServiceEntities.Release GetReleaseById(int personId, int releaseId)
        {
            var release = new UltimateServiceEntities.Release();

            try
            {
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObject = svc.GetReleaseById(personId, releaseId);
                if (releaseRightsObject.Key != null)
                    release = ConvertUltimateReleaseToServiceRelease(releaseRightsObject.Key);
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetReleases,
                        "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return release;
        }

        #endregion 

        #region WIs

        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns></returns>
        internal List<UltimateServiceEntities.WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds)
        {
            var workItems = new List<UltimateServiceEntities.WorkItem>();

            try
            {                
                var svc = ServicesFactory.Resolve<IWorkItemService>();               
                var workItemRightsObject = svc.GetWorkItemByIds(personId, workItemIds);
                if (workItemRightsObject.Key != null)
                {
                    workItemRightsObject.Key.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                }
                else
                {
                    LogManager.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, 
                        "Unable to get workitem for work item id=" + workItemIds.FindAll(x=>string.IsNullOrEmpty(x.ToString(CultureInfo.InvariantCulture)))));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, ex.Message));
            }

            return workItems;
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        /// <returns>
        /// List of work items
        /// </returns>
        internal List<WorkItem> GetWorkItemsByKeyWord(int personId, string keyword, bool shouldHaveAcronym = false)
        {
            var workItems = new List<WorkItem>();

            if (string.IsNullOrEmpty(keyword))
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    var svc = ServicesFactory.Resolve<IWorkItemService>();
                    var workItemRightsObjects = svc.GetWorkItemsBySearchCriteria(personId, keyword, shouldHaveAcronym);
                    if (workItemRightsObjects.Key != null)
                        workItemRightsObjects.Key.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                    else
                        LogManager.Error(string.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Failed to get workitem details"));
                }
                catch (Exception ex)
                {
                    LogManager.Error(string.Format(ConstErrorTemplateGetWorkitemsByKeyword, ex.Message));
                }
            }
            return workItems;
        }

        /// <summary>
        /// Gets the work items by key words.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keywords">The keywords to identify work items.</param>
        /// <returns>List of work items</returns>
        internal List<UltimateServiceEntities.WorkItem> GetWorkItemsByKeyWords(int personId, List<string> keywords)
        {
            var workItems = new List<UltimateServiceEntities.WorkItem>();

            try
            {
                var svc = ServicesFactory.Resolve<IWorkItemService>();
                var svcWorkItems = svc.GetWorkItemsByKeywords(personId, keywords);
                if (svcWorkItems != null)
                    svcWorkItems.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetWorkItemsByKeyWords,
                        "Unable to get workitems for keywords=" + keywords.FindAll(x => string.IsNullOrEmpty(x.ToString(CultureInfo.InvariantCulture)))));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetWorkItemsByKeyWords, ex.Message));
            }

            return workItems;
        }

        #endregion

        #region Specs
        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        internal List<UltimateServiceEntities.Specification> GetSpecificationsByKeyWord(int personId, string keyword)
        {
            var specifications = new List<UltimateServiceEntities.Specification>();

            if (String.IsNullOrEmpty(keyword))
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    var svc = ServicesFactory.Resolve<ISpecificationService>();
                    var specificationsObjects = svc.GetSpecificationBySearchCriteria(personId, keyword);
                    if (specificationsObjects != null)
                        specificationsObjects.ForEach(x => specifications.Add(ConvertUltimateSpecificationToServiceSpecification(x)));
                    else
                        LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Failed to get specification details"));
                }
                catch (Exception ex)
                {
                    LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, ex.Message));
                }
            }
            return specifications;
        }

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>
        /// Specification entity
        /// </returns>
        internal UltimateServiceEntities.Specification GetSpecificationById(int personId, int specificationId)
        {
            var specification = new UltimateServiceEntities.Specification();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var specificationRightsObjects = svc.GetSpecificationDetailsById(personId, specificationId);
                if (specificationRightsObjects.Key != null)
                    specification = ConvertUltimateSpecificationToServiceSpecification(specificationRightsObjects.Key);
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationById, "Failed to get specification details"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationById, ex.Message));
            }
            return specification;
        }

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        internal List<UltimateServiceEntities.Specification> GetSpecificationsByIds(int personId, List<int> specificationIds)
        {
            var specifications = new List<UltimateServiceEntities.Specification>();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var specsFound = svc.GetSpecifications(personId, specificationIds).Key;
                if (specsFound != null)
                    specsFound.ForEach(spec => specifications.Add(ConvertUltimateSpecificationToServiceSpecification(spec)));
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, "Failed to get specs"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, ex.Message));
            }

            return specifications;
        }

        /// <summary>
        /// Gets the specifications by numbers.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specNumbers">The specification numbers.</param>
        /// <returns>List of specifications</returns>
        internal List<UltimateServiceEntities.Specification> GetSpecificationsBySpecNumbers(int personId, List<string> specNumbers)
        {
            var specifications = new List<UltimateServiceEntities.Specification>();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var specsFound = svc.GetSpecificationsByNumbers(personId, specNumbers);
                if (specsFound != null)
                    specsFound.ForEach(spec => specifications.Add(ConvertUltimateSpecificationToServiceSpecification(spec)));
                else
                    LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsBySpecNumbers, "Failed to get specs"));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetSpecificationsBySpecNumbers, ex.Message));
            }

            return specifications;
        }

        /// <summary>
        /// Changes the specifications status to under change control.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specIdsForUcc">The spec ids</param>
        /// <returns>Status report</returns>
        internal ServiceResponse<bool> ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIdsForUcc)
        {
            var statusChangeReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var ultimateStatusResponse = svc.ChangeSpecificationsStatusToUnderChangeControl(personId, specIdsForUcc);
                statusChangeReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateChangeSpecificationsStatusToUnderChangeControl, ex.Message));
                statusChangeReport.Result = false;
                statusChangeReport.Report.ErrorList.Add("Specifications status change process failed");
            }
            return statusChangeReport;
        }

        #endregion

        #region Crs

        /// <summary>
        /// Sets the CRS as final.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="tdocNumbers">The tdoc numbers.</param>
        /// <returns>Status report</returns>
        internal ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.SetCrsAsFinal(personId, tdocNumbers);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateSetCrsAsFinal, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Finalizing CRs process failed");
            }
            return statusReport;
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>Change Request entity</returns>
        internal UltimateServiceEntities.ChangeRequest GetChangeRequestById(int personId, int changeRequestId)
        {
            var changeRequest = new UltimateServiceEntities.ChangeRequest();
            try
            {
                var svcChangeRequestById = ServicesFactory.Resolve<IChangeRequestService>();
                var svcResult = svcChangeRequestById.GetChangeRequestById(personId, changeRequestId);
                if (svcResult.Key)
                    changeRequest = ConvertUltimateCRToServiceCR(svcResult.Value);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateCreateChangeRequestById, ex.Message));
            }
            LogManager.Error(String.Format(ConstErrorTemplateCreateChangeRequestById, changeRequest.Pk_ChangeRequest));
            return changeRequest;
        }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        internal ServiceResponse<int> CreateChangeRequest(int personId, UltimateServiceEntities.ChangeRequest changeRequest)
        {
            var response = new ServiceResponse<int> { Report = new ServiceReport() };
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var createCrResponse = svc.CreateChangeRequest(personId, ConvertServiceCRToUltimateCR(changeRequest));
                if (createCrResponse.Report.GetNumberOfErrors() == 0)
                    response.Result = createCrResponse.Result;
                else
                {
                    response.Report.ErrorList = createCrResponse.Report.ErrorList;
                    LogManager.Error(String.Format(ConstErrorTemplateCreateChangeRequest, "Failed to get create CR record"));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateCreateChangeRequest, ex.Message));
            }
            return response;
        }

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        internal bool EditChangeRequest(int personId, UltimateServiceEntities.ChangeRequest changeRequest)
        {
            bool isSuccess;
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                isSuccess = svc.EditChangeRequest(personId, ConvertServiceCRToUltimateCR(changeRequest));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateEditChangeRequest, ex.Message));
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <returns>UltimateService ChangeRequestCategories</returns>
        internal List<UltimateServiceEntities.ChangeRequestCategory> GetChangeRequestCategories()
        {
            var changeRequestCategories = new List<UltimateServiceEntities.ChangeRequestCategory>();
            try
            {
                var svc = ServicesFactory.Resolve<ICrCategoriesService>();
                var svcChangeRequestCategories = svc.GetChangeRequestCategories();
                if (svcChangeRequestCategories.Key)
                    svcChangeRequestCategories.Value.ForEach(x => changeRequestCategories.Add(ConvertUltimateCRCategoriesToServiceCategories(x)));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateCreateChangeRequestCategories, ex.Message));
            }
            return changeRequestCategories;
        }

        /// <summary>
        /// Returns a contribution's CR data
        /// </summary>
        /// <param name="contributionUid">Contribution UID</param>
        /// <returns></returns>
        internal UltimateServiceEntities.ChangeRequest GetChangeRequestByContributionUid(string contributionUid)
        {
            UltimateServiceEntities.ChangeRequest cr = null;
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetContributionCrByUid(contributionUid);
                if (result.Key)
                    cr = ConvertUltimateCRToServiceCR(result.Value);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
            }
            return cr;
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <returns></returns>
        internal List<UltimateServiceEntities.ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUids)
        {
            var crList = new List<UltimateServiceEntities.ChangeRequest>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetChangeRequestListByContributionUidList(contributionUids);
                if (result.Key)
                {
                    result.Value.ForEach(e => crList.Add(ConvertUltimateCRToServiceCR(e, true)));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
            }
            return crList;
        }

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Key value pair with bool (success status), and the change request</returns>
        public ChangeRequest GetLightChangeRequestForMinuteMan(string uid)
        {
            ChangeRequest cr = null;
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetLightChangeRequestForMinuteMan(uid);
                if (result.Key)
                {
                    cr = ConvertUltimateCRToLightServiceCR(result.Value, true);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetLightChangeRequestForMinuteMan, ex.Message), ex);
            }
            return cr;
        }

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        public List<ChangeRequest> GetLightChangeRequestsForMinuteMan(List<string> uids)
        {
            List<ChangeRequest> crs = new List<ChangeRequest>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetLightChangeRequestsForMinuteMan(uids);
                if (result.Key)
                {
                    result.Value.ForEach(x => crs.Add(ConvertUltimateCRToLightServiceCR(x, true)));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetLightChangeRequestsForMinuteMan, ex.Message), ex);
            }
            return crs;
        }

        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        public List<ChangeRequest> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid)
        {
            var crs = new List<ChangeRequest>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetLightChangeRequestsInsideCrPackForMinuteMan(uid);
                if (result.Key)
                {
                    result.Value.ForEach(x => crs.Add(ConvertUltimateCRToLightServiceCR(x, true)));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetLightChangeRequestsInsideCrPackForMinuteMan, ex.Message));
            }
            return crs;
        }

        /// <summary>
        /// Same method than for GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        public List<ChangeRequest> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids)
        {
            var crs = new List<ChangeRequest>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetLightChangeRequestsInsideCrPacksForMinuteMan(uids);
                if (result.Key)
                {
                    result.Value.ForEach(x => crs.Add(ConvertUltimateCRToLightServiceCR(x, true)));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetLightChangeRequestsInsideCrPacksForMinuteMan, ex.Message));
            }
            return crs;
        }

        /// <summary>
        /// Returns all the change requests statuses
        /// </summary>
        /// <returns></returns>
        internal List<UltimateServiceEntities.ChangeRequestStatus> GetAllChangeRequestStatuses()
        {
            var crStatusesList = new List<UltimateServiceEntities.ChangeRequestStatus>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var result = svc.GetChangeRequestStatuses();
                if (result.Key)
                {
                    result.Value.ForEach(e => crStatusesList.Add(ConvertToServiceCrStatus(e)));
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
            }
            return crStatusesList;

        }

        /// <summary>
        /// Updates the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackDecision"></param>
        internal bool UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackDecision)
        {
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                if (crPackDecision.Count > 0)
                {
                    var crUltimateKeys = new List<KeyValuePair<UltimateEntities.CrKeyFacade, string>>();
                    crPackDecision.ForEach(x => crUltimateKeys.Add(new KeyValuePair<UltimateEntities.CrKeyFacade, string>(ConvertToUltimateCrKeyFacade(x.Key), x.Value)));

                    var response = svc.UpdateChangeRequestPackRelatedCrs(crUltimateKeys);
                    if (response.Report.GetNumberOfErrors() <= 0)
                        return true;
                    foreach (var error in response.Report.ErrorList)
                    {
                        LogManager.Error("UpdateChangeRequestPackRelatedCrs failed cause by : " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateUpdateChangeRequestPackRelatedCrs, ex.Message));
                return false;
            }
            return false;
        }

        /// <summary>
        /// Test if a couple Cr # / Revision already exist
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public bool DoesCrNumberRevisionCoupleExist(int personId, int specId, string crNumber, int revision)
        {
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var response = svc.DoesCrNumberRevisionCoupleExist(specId, crNumber, revision);
                if (response.Report.GetNumberOfErrors() <= 0)
                    return response.Result;
                foreach (var error in response.Report.ErrorList)
                {
                    LogManager.Error("IsExistCrNumberRevisionCouple failed cause by : " + error);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorIsExistCrNumberRevisionCouple, ex.Message));
            }
            return true;
        }

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        internal List<ChangeRequest> GetCrsByKeys(List<CrKeyFacade> crKeys)
        {
            var changeRequests = new List<ChangeRequest>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();

                var crUltimateKeys = new List<UltimateEntities.CrKeyFacade>();
                crKeys.ForEach(x => crUltimateKeys.Add(ConvertToUltimateCrKeyFacade(x)));

                var response = svc.GetCrsByKeys(crUltimateKeys);
                if (response.Result != null)
                    response.Result.ForEach(e => changeRequests.Add(ConvertUltimateCRToServiceCR(e)));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetCrsByKeys, ex.Message));
            }
            return changeRequests;
        }

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        internal ChangeRequest GetCrByKey(CrKeyFacade crKey)
        {
            var changeRequest = new ChangeRequest();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();

                var response = svc.GetCrByKey(ConvertToUltimateCrKeyFacade(crKey));
                if (response.Result != null)
                    changeRequest = ConvertUltimateCRToServiceCR(response.Result);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetCrByKey, ex.Message));
            }
            return changeRequest;
        }

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        internal List<KeyValuePair<CrKeyFacade, string>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys)
        {
            var response = new List<KeyValuePair<CrKeyFacade, string>>();
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();

                var crUltimateKeys = new List<UltimateEntities.CrKeyFacade>();
                crKeys.ForEach(x => crUltimateKeys.Add(ConvertToUltimateCrKeyFacade(x)));

                var result = svc.GetCrWgTdocNumberOfParent(crUltimateKeys);
                if (result.Result != null && result.Report.GetNumberOfErrors() == 0)
                    result.Result.ForEach(e => response.Add(new KeyValuePair<CrKeyFacade, string>(ConvertToWcfCrKeyFacade(e.Key),e.Value)));
                else
                    throw new Exception(string.Format("Error occured when system trying to GetCrWgTdocNumberOfParent: {0}", string.Join(",", result.Report.ErrorList)));
            }
            catch (Exception ex)
            {
                LogManager.Error(string.Format(ConstErrorTemplateGetCrWgTdocNumberOfParent, ex.Message));
            }
            return response;
        }

        /// <summary>
        /// Reissue the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        internal ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var crKeyForUltimate = ConvertToUltimateCrKeyFacade(crKey);
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.ReIssueCr(crKeyForUltimate, newTsgTdoc, newTsgMeetingId, newTsgSource);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateReIssueCr, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Change request failed to reissue");
            }
            return statusReport;
        }

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        internal ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var crKeyForUltimate = ConvertToUltimateCrKeyFacade(crKey);
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.ReviseCr(crKeyForUltimate, newTsgTdoc, newTsgMeetingId, newTsgSource);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateReviseCr, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Change request failed to revise");
            }
            return statusReport;
        }

        /// <summary>
        /// Remove Crs from Cr-Pack
        /// </summary>
        /// <param name="crPack">Uid of Cr-Pack</param>
        /// <param name="crIds">List of Cr Ids</param>
        /// <returns>Success/Failure</returns>
        public ServiceResponse<bool> RemoveCrsFromCrPack(string crPack, List<int> crIds)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.RemoveCrsFromCrPack(crPack, crIds);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateRemoveCrsFromCrPack, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Cr Pack failed to remove Crs");
            }
            return statusReport;
        }

        public ServiceResponse<bool> UpdateCrStatus(string uid, string status)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.UpdateCrStatus(uid, status);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateUpdateCrStatus, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Error occured when trying to update CR status");
            }
            return statusReport;
        }

        public ServiceResponse<bool> UpdateCrsStatusOfCrPack(List<CrOfCrPackFacade> CrsOfCrPack)
        {
            var statusReport = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var CrsOfCrPackForUltimate = ConvertToUltimateCrsOfCrPackFacade(CrsOfCrPack);
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var ultimateStatusResponse = svc.UpdateCrsOfCrPackStatus(CrsOfCrPackForUltimate);
                statusReport = ConvertUltimateServiceResponseToWcfServiceResponse(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateUpdateCrsStatusOfCrPack, ex.Message));
                statusReport.Result = false;
                statusReport.Report.ErrorList.Add("Error occured when trying to update CRs status of CR Pack");
            }
            return statusReport;
        } 
        #endregion

        #region versions and draft

        /// <summary>
        /// Get versions related to specification & release
        /// </summary>
        /// <param name="specId">Specification Identifier</param>
        /// <param name="releaseId">Release Identifier</param>
        /// <returns>List of versions</returns>
        public List<UltimateServiceEntities.SpecVersion> GetVersionsForSpecRelease(int specId, int releaseId)
        {
            var specVersions = new List<UltimateServiceEntities.SpecVersion>();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();
                var result = svc.GetVersionsForSpecRelease(specId, releaseId);
                if (result != null)
                    result.ForEach(e => specVersions.Add(ConvertToServiceSpecVersion(e)));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetVersionsForSpecRelease, ex.Message));
            }
            return specVersions;
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
        public ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc)
        {
            var serviceReport = new ServiceReport();
            var svcResponse = new ServiceResponse<bool> { Report = serviceReport };

            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();
                var specVersionResponse = svc.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion, editorialVersion, relatedTdoc);

                svcResponse.Result = specVersionResponse.Result;
                svcResponse.Report.ErrorList.AddRange(specVersionResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(specVersionResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(specVersionResponse.Report.InfoList);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateUpdateVersionRelatedTdoc, ex.Message));
                svcResponse.Result = false;
                svcResponse.Report.ErrorList.Add(ex.Message);
            }

            return svcResponse;
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
        internal ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = new ServiceResponse<bool> { Report = new ServiceReport() };

            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();
                var specVersionResponse = svc.CheckDraftCreationOrAssociation(personId, specId, releaseId, majorVersion, technicalVersion, editorialVersion);

                svcResponse.Result = specVersionResponse.Result;
                svcResponse.Report.ErrorList.AddRange(specVersionResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(specVersionResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(specVersionResponse.Report.InfoList);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateCheckDraftCreationOrAssociation, ex.Message));
                svcResponse.Result = false;
                svcResponse.Report.ErrorList.Add(ex.Message);
            }

            return svcResponse;
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
        internal ServiceResponse<string> CheckVersionForUpload(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string filePath)
        {
            var svcResponse = new ServiceResponse<string> { Report = new ServiceReport() };

            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();

                //Construct version object
                var version = new UltimateEntities.SpecVersion()
                {
                    Fk_SpecificationId = specId,
                    Fk_ReleaseId = releaseId,
                    Source = meetingId,
                    MajorVersion = majorVersion,
                    TechnicalVersion = technicalVersion,
                    EditorialVersion = editorialVersion,
                    DocumentUploaded = DateTime.UtcNow,
                    ProvidedBy = personId
                };

                var specVersionResponse = svc.CheckVersionForUpload(personId, version, filePath);

                svcResponse.Result = specVersionResponse.Result;
                svcResponse.Report.ErrorList.AddRange(specVersionResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(specVersionResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(specVersionResponse.Report.InfoList);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateCheckVersionForUpload, ex.Message));
                svcResponse.Report.ErrorList.Add(ex.Message);
            }

            return svcResponse;
        }

        /// <summary>
        /// Uploads the version.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="token">The token.</param>
        /// <returns>Success/Failure</returns>
        internal ServiceResponse<bool> UploadVersion(int personId, string token)
        {
            var svcResponse = new ServiceResponse<bool> { Report = new ServiceReport() };

            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();

                var specVersionResponse = svc.UploadVersion(personId, token);

                svcResponse.Result = (specVersionResponse.Report.ErrorList.Count <= 0);
                svcResponse.Report.ErrorList.AddRange(specVersionResponse.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(specVersionResponse.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(specVersionResponse.Report.InfoList);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateUploadVersion, ex.Message));
                svcResponse.Result = false;
                svcResponse.Report.ErrorList.Add(ex.Message);
            }

            return svcResponse;
        }

        /// <summary>
        /// Get latest version of each relaease for the given spec ids
        /// </summary>
        /// <param name="specIds">The specification identifiders</param>
        /// <returns>List of Spec Versions</returns>
        internal List<UltimateServiceEntities.SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds)
        {
            var specVersions = new List<UltimateServiceEntities.SpecVersion>();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecVersionService>();
                var result = svc.GetLatestVersionsBySpecIds(specIds);

                if (result != null)
                    result.ForEach(e => specVersions.Add(ConvertToServiceSpecVersion(e)));
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format(ConstErrorTemplateGetLatestVersionsBySpecIds, ex.Message));
            }
            return specVersions;
        }

        /// <summary>
        /// Unlink tdoc from related version
        /// </summary>
        /// <param name="uid">Tdoc uid</param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        public ServiceResponse<bool> UnlinkTdocFromVersion(string uid, int personId)
        {
            var svcResponse = new ServiceResponse<bool> { Report = new ServiceReport() };
            try
            {
                var specVersionSvc = ServicesFactory.Resolve<ISpecVersionService>();
                var response = specVersionSvc.UnlinkTdocFromVersion(uid, personId);

                svcResponse.Result = (response.Report.ErrorList.Count <= 0);
                svcResponse.Report.ErrorList.AddRange(response.Report.ErrorList);
                svcResponse.Report.WarningList.AddRange(response.Report.WarningList);
                svcResponse.Report.InfoList.AddRange(response.Report.InfoList);
            }
            catch (Exception ex)
            {
                svcResponse.Result = false;
                svcResponse.Report.ErrorList.Add(ex.Message);
                LogManager.Error("Unexpected error occured when system trying to unlink tdoc from version", ex);
            }
            return svcResponse;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the ultimate cr category to service cr category.
        /// </summary>
        /// <param name="ultimateCrCategory">The list.</param>
        /// <returns>UltimateService ChangeRequestCategory</returns>      
        private UltimateServiceEntities.ChangeRequestCategory ConvertUltimateCRCategoriesToServiceCategories(UltimateEntities.Enum_CRCategory ultimateCrCategory)
        {
            var svcCrCategory = new UltimateServiceEntities.ChangeRequestCategory
            {
                Pk_EnumCRCategory = ultimateCrCategory.Pk_EnumCRCategory,
                Code = ultimateCrCategory.Code,
                Description = ultimateCrCategory.Description
            };
            return svcCrCategory;
        }

        /// <summary>
        /// Converts release status enum to domain release status.
        /// </summary>
        /// <param name="releaseStatus">the release status</param>
        /// <returns>Service release entity</returns>
        private string ConvertUltimateReleaseStatusEnumToDomainReleaseStatusEnum(ReleaseStatus releaseStatus)
        {
            string result = string.Empty;

            switch (releaseStatus)
            {
                case ReleaseStatus.Closed:
                    result = UltimateEntities.Enum_ReleaseStatus.Closed;
                    break;
                case ReleaseStatus.Frozen:
                    result = UltimateEntities.Enum_ReleaseStatus.Frozen;
                    break;
                case ReleaseStatus.Open:
                    result = UltimateEntities.Enum_ReleaseStatus.Open;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts the ultimate release to service release.
        /// </summary>
        /// <param name="ultimateRelease">The ultimate release.</param>
        /// <returns>Service release entity</returns>
        private UltimateServiceEntities.Release ConvertUltimateReleaseToServiceRelease(UltimateEntities.Release ultimateRelease)
        {
            var serviceRelease = new UltimateServiceEntities.Release();
            if (ultimateRelease != null)
            {
                serviceRelease.Pk_ReleaseId = ultimateRelease.Pk_ReleaseId;
                serviceRelease.Name = ultimateRelease.Name;
                serviceRelease.ShortName = ultimateRelease.ShortName;
                serviceRelease.Status = ultimateRelease.Enum_ReleaseStatus.Description;
                serviceRelease.SortOrder = ultimateRelease.SortOrder;
            }
            return serviceRelease;
        }

        /// <summary>
        /// Converts the ultimate work item to service work item.
        /// </summary>
        /// <param name="ultimateWorkItem">The ultimate work item.</param>
        /// <returns>Service work item entity</returns>
        private UltimateServiceEntities.WorkItem ConvertUltimateWorkItemToServiceWorkItem(UltimateEntities.WorkItem ultimateWorkItem)
        {
            var serviceWorkItem = new UltimateServiceEntities.WorkItem();
            if (ultimateWorkItem != null)
            {
                serviceWorkItem.Pk_WorkItemUid = ultimateWorkItem.Pk_WorkItemUid;
                serviceWorkItem.UID = ultimateWorkItem.UID;
                serviceWorkItem.Acronym = ultimateWorkItem.Acronym;
                serviceWorkItem.Name = ultimateWorkItem.Name;
                serviceWorkItem.ResponsibleGroups = ultimateWorkItem.ResponsibleGroups;
            }
            return serviceWorkItem;
        }

        /// <summary>
        /// Converts the ultimate work item to service work item.
        /// </summary>
        /// <param name="ultimateSpecification">The ultimate specification.</param>
        /// <returns>Service specification entity</returns>
        private UltimateServiceEntities.Specification ConvertUltimateSpecificationToServiceSpecification(UltimateEntities.Specification ultimateSpecification)
        {
            var serviceSpecification = new UltimateServiceEntities.Specification();
            if (ultimateSpecification != null)
            {
                serviceSpecification.Pk_SpecificationId = ultimateSpecification.Pk_SpecificationId;
                serviceSpecification.Title = ultimateSpecification.Title;
                serviceSpecification.Number = ultimateSpecification.Number;
                serviceSpecification.SpecNumberAndTitle = ultimateSpecification.SpecNumberAndTitle;
                if (ultimateSpecification.PrimeResponsibleGroup != null)
                    serviceSpecification.PrimaryResponsibleGroup_CommunityId = ultimateSpecification.PrimeResponsibleGroup.Fk_commityId;
                serviceSpecification.IsActive = ultimateSpecification.IsActive;
                serviceSpecification.IsUcc = ultimateSpecification.IsUnderChangeControl;
            }
            return serviceSpecification;
        }

        /// <summary>
        /// Converts the service cr to ultimate CR.
        /// </summary>
        /// <param name="serviceCr">The service CR.</param>
        /// <returns>Ultimate CR</returns>
        private UltimateEntities.ChangeRequest ConvertServiceCRToUltimateCR(UltimateServiceEntities.ChangeRequest serviceCr)
        {
            var ultimateCr = new UltimateEntities.ChangeRequest();
            if (serviceCr != null)
            {
                ultimateCr.Pk_ChangeRequest = serviceCr.Pk_ChangeRequest;
                ultimateCr.CRNumber = serviceCr.CRNumber;
                ultimateCr.Revision = serviceCr.Revision;
                ultimateCr.Subject = serviceCr.Subject;
                ultimateCr.Fk_WGStatus = serviceCr.Fk_WGStatus;
                ultimateCr.CreationDate = serviceCr.CreationDate;
                ultimateCr.WGSourceOrganizations = serviceCr.WGSourceOrganizations;
                ultimateCr.WGSourceForTSG = serviceCr.WGSourceForTSG;
                ultimateCr.WGMeeting = serviceCr.WGMeeting;
                ultimateCr.WGTarget = serviceCr.WGTarget;
                ultimateCr.Fk_Enum_CRCategory = serviceCr.Fk_Enum_CRCategory;
                ultimateCr.Fk_Specification = serviceCr.Fk_Specification;
                ultimateCr.Fk_Release = serviceCr.Fk_Release;
                ultimateCr.Fk_CurrentVersion = serviceCr.Fk_CurrentVersion;
                ultimateCr.Fk_NewVersion = serviceCr.Fk_NewVersion;
                ultimateCr.Fk_Impact = serviceCr.Fk_Impact;
                ultimateCr.WGTDoc = serviceCr.WGTDoc;
                ultimateCr.RevisionOf = serviceCr.RevisionOf;
                ultimateCr.IsAutoNumberingOff = serviceCr.IsAutoNumberingOff;
                if ((serviceCr.Fk_WorkItemIds != null) && (serviceCr.Fk_WorkItemIds.Count > 0))
                    serviceCr.Fk_WorkItemIds.ForEach(x => ultimateCr.CR_WorkItems.Add(new UltimateEntities.CR_WorkItems { Fk_WIId = x }));

                ultimateCr.ChangeRequestTsgDatas = new List<UltimateEntities.ChangeRequestTsgData>();
                if (serviceCr.TsgData != null)
                {
                    serviceCr.TsgData.ForEach(x => ultimateCr.ChangeRequestTsgDatas.Add(new UltimateEntities.ChangeRequestTsgData
                    {
                        Pk_ChangeRequestTsgData = x.PkChangeRequestTsgData,
                        Fk_TsgStatus = x.FkTsgStatus,
                        TSGMeeting = x.TsgMeeting,
                        TSGSourceOrganizations = x.TsgSourceOrganizations,
                        TSGTarget = x.TsgTarget,
                        TSGTdoc = x.TsgTdoc
                    }));
                }
            }
            return ultimateCr;
        }

        /// <summary>
        /// Converts the ultimate cr to service CR.
        /// </summary>
        /// <param name="ultimateCr"></param>
        /// <param name="withWis"></param>
        /// <returns>Ultimate Entities to Service Entities</returns>
        private UltimateServiceEntities.ChangeRequest ConvertUltimateCRToServiceCR(UltimateEntities.ChangeRequest ultimateCr, bool withWis = false)
        {
            var serviceCr = new UltimateServiceEntities.ChangeRequest();
            if (ultimateCr != null)
            {
                serviceCr.Pk_ChangeRequest = ultimateCr.Pk_ChangeRequest;
                serviceCr.CRNumber = ultimateCr.CRNumber;
                serviceCr.Revision = ultimateCr.Revision;
                serviceCr.Subject = ultimateCr.Subject;
                serviceCr.Fk_WGStatus = ultimateCr.Fk_WGStatus;
                serviceCr.WGStatus = (ultimateCr.WgStatus == null) ? String.Empty : ultimateCr.WgStatus.Description;
                serviceCr.CreationDate = ultimateCr.CreationDate;
                serviceCr.WGSourceOrganizations = ultimateCr.WGSourceOrganizations;
                serviceCr.WGSourceForTSG = ultimateCr.WGSourceForTSG;
                serviceCr.WGMeeting = ultimateCr.WGMeeting;
                serviceCr.WGTarget = ultimateCr.WGTarget;
                serviceCr.Fk_Enum_CRCategory = ultimateCr.Fk_Enum_CRCategory;
                serviceCr.Fk_Specification = ultimateCr.Fk_Specification;
                serviceCr.SpecificationNumber = (ultimateCr.Specification == null) ? String.Empty : ultimateCr.Specification.Number;
                serviceCr.Fk_Release = ultimateCr.Fk_Release;
                serviceCr.ReleaseShortName = (ultimateCr.Release == null) ? String.Empty : ultimateCr.Release.ShortName;
                serviceCr.Fk_CurrentVersion = ultimateCr.Fk_CurrentVersion;
                serviceCr.CurrentVersion = (ultimateCr.CurrentVersion == null) ? String.Empty : ultimateCr.CurrentVersion.Version;
                serviceCr.Fk_NewVersion = ultimateCr.Fk_NewVersion;
                serviceCr.NewVersion = (ultimateCr.NewVersion == null) ? String.Empty : ultimateCr.NewVersion.Version;
                serviceCr.Fk_Impact = ultimateCr.Fk_Impact;
                serviceCr.WGTDoc = ultimateCr.WGTDoc;
                if (withWis)
                {
                    serviceCr.Fk_WorkItemIds = ultimateCr.CR_WorkItems.Select(x => x.Fk_WIId ?? 0).Distinct().ToList();
                }

                //Referenced objects
                if (ultimateCr.Enum_CRCategory != null)
                {
                    serviceCr.Category = new UltimateServiceEntities.ChangeRequestCategory
                    {
                        Pk_EnumCRCategory = ultimateCr.Enum_CRCategory.Pk_EnumCRCategory,
                        Code = ultimateCr.Enum_CRCategory.Code,
                        Description = ultimateCr.Enum_CRCategory.Description
                    };
                }
                serviceCr.TsgData = new List<UltimateServiceEntities.ChangeRequestTsgData>();

                if (ultimateCr.ChangeRequestTsgDatas != null)
                {
                    ultimateCr.ChangeRequestTsgDatas.ForEach(x => serviceCr.TsgData.Add(new UltimateServiceEntities.ChangeRequestTsgData
                    {
                        PkChangeRequestTsgData = x.Pk_ChangeRequestTsgData,
                        TsgTdoc = x.TSGTdoc,
                        TsgMeeting = x.TSGMeeting,
                        TsgSourceOrganizations = x.TSGSourceOrganizations,
                        TsgTarget = x.TSGTarget,
                        TsgStatus = (x.TsgStatus == null) ? String.Empty : x.TsgStatus.Description,
                        FkTsgStatus = x.Fk_TsgStatus,
                        FkChangeRequest = x.Fk_ChangeRequest
                    }));
                }
            }
            return serviceCr;
        }

        /// <summary>
        /// Converts the ultimate cr to LIGHT service CR.
        /// </summary>
        /// <param name="ultimateCr"></param>
        /// <param name="withWis"></param>
        /// <returns>Ultimate Entities to Service Entities</returns>
        private UltimateServiceEntities.ChangeRequest ConvertUltimateCRToLightServiceCR(UltimateEntities.ChangeRequest ultimateCr, bool withWis = false)
        {
            var serviceCr = new UltimateServiceEntities.ChangeRequest();
            if (ultimateCr != null)
            {
                serviceCr.Pk_ChangeRequest = ultimateCr.Pk_ChangeRequest;
                serviceCr.CRNumber = ultimateCr.CRNumber;
                serviceCr.Revision = ultimateCr.Revision;
                serviceCr.Subject = ultimateCr.Subject;
                serviceCr.Fk_WGStatus = ultimateCr.Fk_WGStatus;
                serviceCr.CreationDate = ultimateCr.CreationDate;
                serviceCr.WGSourceOrganizations = ultimateCr.WGSourceOrganizations;
                serviceCr.WGSourceForTSG = ultimateCr.WGSourceForTSG;
                serviceCr.WGMeeting = ultimateCr.WGMeeting;
                serviceCr.WGTarget = ultimateCr.WGTarget;
                serviceCr.Fk_Enum_CRCategory = ultimateCr.Fk_Enum_CRCategory;
                serviceCr.Fk_Specification = ultimateCr.Fk_Specification;
                serviceCr.Fk_Release = ultimateCr.Fk_Release;
                serviceCr.Fk_CurrentVersion = ultimateCr.Fk_CurrentVersion;
                serviceCr.Fk_NewVersion = ultimateCr.Fk_NewVersion;
                serviceCr.Fk_Impact = ultimateCr.Fk_Impact;
                serviceCr.WGTDoc = ultimateCr.WGTDoc;

                //Referenced objects
                serviceCr.TsgData = new List<UltimateServiceEntities.ChangeRequestTsgData>();
                if (ultimateCr.Specification != null)
                {
                    serviceCr.SpecificationNumber = ultimateCr.Specification.Number;
                }

                if (ultimateCr.ChangeRequestTsgDatas != null)
                {
                    ultimateCr.ChangeRequestTsgDatas.ForEach(x => serviceCr.TsgData.Add(new UltimateServiceEntities.ChangeRequestTsgData
                    {
                        PkChangeRequestTsgData = x.Pk_ChangeRequestTsgData,
                        TsgTdoc = x.TSGTdoc,
                        TsgMeeting = x.TSGMeeting,
                        TsgSourceOrganizations = x.TSGSourceOrganizations,
                        TsgTarget = x.TSGTarget,
                        FkTsgStatus = x.Fk_TsgStatus,
                        FkChangeRequest = x.Fk_ChangeRequest
                    }));
                }
            }
            return serviceCr;
        }

        /// <summary>
        /// Converts an ultimate status to a service status
        /// </summary>
        /// <returns></returns>
        private UltimateServiceEntities.ChangeRequestStatus ConvertToServiceCrStatus(UltimateEntities.Enum_ChangeRequestStatus ultimateCrStatus)
        {
            var serviceCrStatus = new UltimateServiceEntities.ChangeRequestStatus();
            if (ultimateCrStatus != null)
            {
                serviceCrStatus.Pk_ChangeRequestStatus = ultimateCrStatus.Pk_EnumChangeRequestStatus;
                serviceCrStatus.Code = ultimateCrStatus.Code;
                serviceCrStatus.Description = ultimateCrStatus.Description;
            }
            return serviceCrStatus;
        }

        /// <summary>
        /// Converts the ultimate service response to WCF service response.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ultimateServiceResponse">The ultimate service response.</param>
        /// <returns>The Wcf compatiable service response</returns>
        private ServiceResponse<T> ConvertUltimateServiceResponseToWcfServiceResponse<T>(UltimateEntities.ServiceResponse<T> ultimateServiceResponse)
        {
            var serviceReport = new ServiceReport { ErrorList = new List<string>(), InfoList = new List<string>(), WarningList = new List<string>() };
            var wcfServiceResponse = new ServiceResponse<T> { Report = serviceReport };

            if (ultimateServiceResponse != null)
            {
                wcfServiceResponse.Result = ultimateServiceResponse.Result;
                wcfServiceResponse.Report.ErrorList = ultimateServiceResponse.Report.ErrorList;
                wcfServiceResponse.Report.WarningList = ultimateServiceResponse.Report.WarningList;
                wcfServiceResponse.Report.InfoList = ultimateServiceResponse.Report.InfoList;
            }

            return wcfServiceResponse;
        }

        /// <summary>
        /// Converts the ultimate spec version to WCF spec version.
        /// </summary>
        /// <param name="specVersion">The ultimate spec version</param>
        /// <returns>The wcf compatiable spec version</returns>
        private UltimateServiceEntities.SpecVersion ConvertToServiceSpecVersion(UltimateEntities.SpecVersion specVersion)
        {
            var serviceSpecVersion = new UltimateServiceEntities.SpecVersion();
            if (specVersion != null)
            {
                serviceSpecVersion.Pk_VersionId = specVersion.Pk_VersionId;
                serviceSpecVersion.MajorVersion = specVersion.MajorVersion;
                serviceSpecVersion.TechnicalVersion = specVersion.TechnicalVersion;
                serviceSpecVersion.EditorialVersion = specVersion.EditorialVersion;
                serviceSpecVersion.RelatedTDoc = specVersion.RelatedTDoc;
                serviceSpecVersion.Fk_SpecificationId = specVersion.Fk_SpecificationId ?? 0;
                serviceSpecVersion.Fk_ReleaseId = specVersion.Fk_ReleaseId ?? 0;
            }
            return serviceSpecVersion;
        }

        /// <summary>
        /// Converts an WCF CrKeyFacade to Ultimate CrKeyFacade
        /// </summary>
        /// <returns></returns>
        private UltimateEntities.CrKeyFacade ConvertToUltimateCrKeyFacade(CrKeyFacade crKeyFacade)
        {
            var ultimateCrKeyFacade = new UltimateEntities.CrKeyFacade
            {
                CrNumber = crKeyFacade.CrNumber,
                SpecId = crKeyFacade.SpecId,
                SpecNumber = crKeyFacade.SpecNumber,
                Revision = crKeyFacade.Revision,
                TsgTdocNumber = crKeyFacade.TsgTdocNumber,
                TsgMeetingId = crKeyFacade.TsgMeetingId,
                TsgSourceOrganization = crKeyFacade.TsgSourceOrganization
            };

            return ultimateCrKeyFacade;
        }

        /// <summary>
        /// Converts an Ultimate CrKeyFacade to WCF CrKeyFacade
        /// </summary>
        /// <returns></returns>
        private CrKeyFacade ConvertToWcfCrKeyFacade(UltimateEntities.CrKeyFacade crKeyFacade)
        {
            var ultimateCrKeyFacade = new CrKeyFacade
            {
                CrNumber = crKeyFacade.CrNumber,
                SpecId = crKeyFacade.SpecId,
                SpecNumber = crKeyFacade.SpecNumber,
                Revision = crKeyFacade.Revision,
                TsgTdocNumber = crKeyFacade.TsgTdocNumber,
                TsgMeetingId = crKeyFacade.TsgMeetingId,
                TsgSourceOrganization = crKeyFacade.TsgSourceOrganization
            };

            return ultimateCrKeyFacade;
        }

        /// <summary>
        /// Converts an WCF CrKeyFacade to Ultimate CrKeyFacade
        /// </summary>
        /// <returns></returns>
        private List<UltimateEntities.CrOfCrPackFacade> ConvertToUltimateCrsOfCrPackFacade(List<CrOfCrPackFacade> crsOfCrPack)
        {
            var result = new List<UltimateEntities.CrOfCrPackFacade>();

            foreach (var item in crsOfCrPack)
            {
                result.Add(new UltimateEntities.CrOfCrPackFacade
                {
                    CrNumber = item.CrNumber,
                    SpecId = item.SpecId,
                    TsgTdocNumber = item.TsgTdocNumber,
                    Status = item.Status,
                    RevisionNumber = item.RevisionNumber
                });

            }

            return result;
        }

        #endregion
    }
}