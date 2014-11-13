using System;
using Etsi.Ultimate.Services;
using System.Collections.Generic;
using UltimateEntities = Etsi.Ultimate.DomainClasses;
using UltimateServiceEntities = Etsi.Ultimate.WCF.Interface.Entities;
using Etsi.Ultimate.WCF.Interface;
using Etsi.Ultimate.WCF.Service.Logger;

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
        private const string ConstErrorTemplateGetSpecificationsByKeyword = "Ultimate Service Error [GetSpecificationsByKeyWord]: {0}";
        private const string ConstErrorTemplateGetSpecificationById = "Ultimate Service Error [GetSpecificationById]: {0}";
        private const string ConstErrorTemplateGetSpecificationsByIds = "Ultimate Service Error [GetSpecificationsByIds]: {0}";
        private const string ConstErrorTemplateCreateChangeRequest = "Ultimate Service Error [CreateChangeRequest]: {0}";
        private const string ConstErrorTemplateEditChangeRequest = "Ultimate Service Error [EditChangeRequest]: {0}";
        private const string ConstErrorTemplateUpdateChangeRequestPackRelatedCrs = "Ultimate Service Error [UpdateChangeRequestPackRelatedCrs]: {0}";
        private const string ConstErrorTemplateCreateChangeRequestCategories = "Ultimate Service Error [GetChangeRequestCategories]: {0}";
        private const string ConstErrorTemplateCreateChangeRequestById = "Ultimate Service Error [GetChangeRequestById]: {0}";
        private const string ConstErrorTemplateGetChangeRequestByContribUid = "Ultimate Service Error [GetChangeRequestByContributionUID]: {0}";
        private const string ConstErrorTemplateChangeSpecificationsStatusToUnderChangeControl = "Ultimate Service Error [ChangeSpecificationsStatusToUnderChangeControl]: {0}";
        #endregion

        #region Internal Methods

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
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
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
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases,
                        "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return release;
        }


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
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, "Unable to get workitem for work item id=" + workItemIds.FindAll(x=>string.IsNullOrEmpty(x.ToString()))));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, ex.Message));
            }

            return workItems;
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        internal List<UltimateServiceEntities.WorkItem> GetWorkItemsByKeyWord(int personId, string keyword)
        {
            var workItems = new List<UltimateServiceEntities.WorkItem>();

            if (String.IsNullOrEmpty(keyword))
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    var svc = ServicesFactory.Resolve<IWorkItemService>();
                    var workItemRightsObjects = svc.GetWorkItemsBySearchCriteria(personId, keyword);
                    if (workItemRightsObjects.Key != null)
                        workItemRightsObjects.Key.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Failed to get workitem details"));
                }
                catch (Exception ex)
                {
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, ex.Message));
                }
            }
            return workItems;
        }

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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Keyword should not empty"));
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
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Failed to get specification details"));
                }
                catch (Exception ex)
                {
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, ex.Message));
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
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationById, "Failed to get specification details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationById, ex.Message));
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
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, "Failed to get specs"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, ex.Message));
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
            var statusChangeReport = new ServiceResponse<bool>();
            try
            {
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var ultimateStatusResponse = svc.ChangeSpecificationsStatusToUnderChangeControl(personId, specIdsForUcc);
                statusChangeReport = ConvertUltimateServiceResponseToWcfServiceResponse<bool>(ultimateStatusResponse);
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateChangeSpecificationsStatusToUnderChangeControl, ex.Message));
                statusChangeReport.Result = false;
                statusChangeReport.Report.ErrorList.Add("Specifications status change process failed");
            }
            return statusChangeReport;
        }

        /// <summary>
        /// Sets the CRS as final.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="tdocNumbers">The tdoc numbers.</param>
        /// <returns>Status report</returns>
        internal ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers)
        {
            var statusReport = new ServiceResponse<bool>();
            try
            {

            }
            catch (Exception ex)
            {

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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequestById, ex.Message));
            }
            LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequestById, changeRequest.Pk_ChangeRequest));
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
            var response = new ServiceResponse<int>();
            var primaryKeyOfNewCr = 0;
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                var createCrResponse = svc.CreateChangeRequest(personId, ConvertServiceCRToUltimateCR(changeRequest));
                if (createCrResponse.Report.GetNumberOfErrors() == 0)
                    response.Result = createCrResponse.Result;
                else
                {
                    response.Report.ErrorList = createCrResponse.Report.ErrorList;
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequest, "Failed to get create CR record"));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequest, ex.Message));
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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateEditChangeRequest, ex.Message));
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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequestCategories, ex.Message));
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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
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
                    result.Value.ForEach(e => crList.Add(ConvertUltimateCRToServiceCR(e)));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
            }
            return crList;
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
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetChangeRequestByContribUid, ex.Message));
            }
            return crStatusesList;

        }

        /// <summary>
        /// Updates the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackDecision"></param>
        /// <param name="tsgTdocNumber"></param>
        internal bool UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<string, string>> crPackDecision, string tsgTdocNumber)
        {
            try
            {
                var svc = ServicesFactory.Resolve<IChangeRequestService>();
                if (crPackDecision.Count > 0)
                {
                    var response = svc.UpdateChangeRequestPackRelatedCrs(crPackDecision, tsgTdocNumber);
                    if (response.Report.GetNumberOfErrors() <= 0)
                        return true;
                    foreach (var error in response.Report.ErrorList)
                    {
                        LogManager.UltimateServiceLogger.Error("UpdateChangeRequestPackRelatedCrs failed cause by : " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateUpdateChangeRequestPackRelatedCrs, ex.Message));
                return false;
            }
            return false;
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
                ultimateCr.Fk_TSGStatus = serviceCr.Fk_TSGStatus;
                ultimateCr.Fk_WGStatus = serviceCr.Fk_WGStatus;
                ultimateCr.Subject = serviceCr.Subject;
                ultimateCr.CreationDate = serviceCr.CreationDate;
                ultimateCr.TSGSourceOrganizations = serviceCr.TSGSourceOrganizations;
                ultimateCr.WGSourceOrganizations = serviceCr.WGSourceOrganizations;
                ultimateCr.TSGMeeting = serviceCr.TSGMeeting;
                ultimateCr.TSGTarget = serviceCr.TSGTarget;
                ultimateCr.WGSourceForTSG = serviceCr.WGSourceForTSG;
                ultimateCr.WGMeeting = serviceCr.WGMeeting;
                ultimateCr.WGTarget = serviceCr.WGTarget;
                ultimateCr.Fk_Enum_CRCategory = serviceCr.Fk_Enum_CRCategory;
                ultimateCr.Fk_Specification = serviceCr.Fk_Specification;
                ultimateCr.Fk_Release = serviceCr.Fk_Release;
                ultimateCr.Fk_CurrentVersion = serviceCr.Fk_CurrentVersion;
                ultimateCr.Fk_NewVersion = serviceCr.Fk_NewVersion;
                ultimateCr.Fk_Impact = serviceCr.Fk_Impact;
                ultimateCr.TSGTDoc = serviceCr.TSGTDoc;
                ultimateCr.WGTDoc = serviceCr.WGTDoc;
                ultimateCr.RevisionOf = serviceCr.RevisionOf;
                if ((serviceCr.Fk_WorkItemIds != null) && (serviceCr.Fk_WorkItemIds.Count > 0))
                    serviceCr.Fk_WorkItemIds.ForEach(x => ultimateCr.CR_WorkItems.Add(new UltimateEntities.CR_WorkItems { Fk_WIId = x }));
            }
            return ultimateCr;
        }

        /// <summary>
        /// Converts the ultimate cr to service CR.
        /// </summary>
        /// <param name="ultimateCr"></param>
        /// <returns>Ultimate Entities to Service Entities</returns>
        private UltimateServiceEntities.ChangeRequest ConvertUltimateCRToServiceCR(UltimateEntities.ChangeRequest ultimateCr)
        {
            var serviceCr = new UltimateServiceEntities.ChangeRequest();
            if (ultimateCr != null)
            {
                serviceCr.Pk_ChangeRequest = ultimateCr.Pk_ChangeRequest;
                serviceCr.CRNumber = ultimateCr.CRNumber;
                serviceCr.Revision = ultimateCr.Revision;
                serviceCr.Subject = ultimateCr.Subject;
                serviceCr.Fk_TSGStatus = ultimateCr.Fk_TSGStatus;
                serviceCr.TSGStatus = (ultimateCr.TsgStatus == null) ? String.Empty : ultimateCr.TsgStatus.Description;
                serviceCr.Fk_WGStatus = ultimateCr.Fk_WGStatus;
                serviceCr.WGStatus = (ultimateCr.WgStatus == null) ? String.Empty : ultimateCr.WgStatus.Description;
                serviceCr.Subject = ultimateCr.Subject;
                serviceCr.CreationDate = ultimateCr.CreationDate;
                serviceCr.TSGSourceOrganizations = ultimateCr.TSGSourceOrganizations;
                serviceCr.WGSourceOrganizations = ultimateCr.WGSourceOrganizations;
                serviceCr.TSGMeeting = ultimateCr.TSGMeeting;
                serviceCr.TSGTarget = ultimateCr.TSGTarget;
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
                serviceCr.TSGTDoc = ultimateCr.TSGTDoc;
                serviceCr.WGTDoc = ultimateCr.WGTDoc;

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
            }
            return serviceCr;
        }

        /// <summary>
        /// Converts an ultimate status to a service status
        /// </summary>
        /// <param name="e"></param>
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

        private KeyValuePair<string, string> ConvertCrpackToService(KeyValuePair<string, string> crPackDecision)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the ultimate service response to WCF service response.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ultimateServiceResponse">The ultimate service response.</param>
        /// <returns>The Wcf compatiable service response</returns>
        private ServiceResponse<T> ConvertUltimateServiceResponseToWcfServiceResponse<T>(UltimateEntities.ServiceResponse<T> ultimateServiceResponse)
        {
            var wcfServiceResponse = new ServiceResponse<T>();

            wcfServiceResponse.Result = ultimateServiceResponse.Result;
            wcfServiceResponse.Report.ErrorList = ultimateServiceResponse.Report.ErrorList;
            wcfServiceResponse.Report.WarningList = ultimateServiceResponse.Report.WarningList;
            wcfServiceResponse.Report.InfoList = ultimateServiceResponse.Report.InfoList;

            return wcfServiceResponse;
        }

        #endregion
    }
}