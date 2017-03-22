using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public class ChangeRequestService : IChangeRequestService
    {
        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>
        /// Primary key of newly inserted change request along with the status (success/failure)
        /// </returns>
        public ServiceResponse<int> CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var response = new ServiceResponse<int>();
            ServiceResponse<bool> isSuccess;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;

                    isSuccess = manager.CreateChangeRequest(personId, changeRequest);
                    if (isSuccess.Result)
                    {
                        uoW.Save();
                        response.Result = changeRequest.Pk_ChangeRequest;
                    }
                    else
                    {
                        response.Report = isSuccess.Report;
                        response.Result = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Report.LogError(Localization.GenericError);
                LogManager.Error(String.Format("[Service] Failed to create change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
            }

            return response;
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId"></param>
        /// <returns>ChangeRequest object</returns>
        public KeyValuePair<bool, ChangeRequest> GetChangeRequestById(int personId, int changeRequestId)
        {
            var changeRequest = new ChangeRequest();
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequest = manager.GetChangeRequestById(personId, changeRequestId);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogManager.Error(String.Format("[Service] Failed to get change request id : {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
            }
            return new KeyValuePair<bool, ChangeRequest>(isSuccess, changeRequest);
        }

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        public bool EditChangeRequest(int personId, ChangeRequest changeRequest)
        {
            bool isSuccess;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;

                    isSuccess = manager.EditChangeRequest(personId, changeRequest);
                    if (isSuccess)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Service] Failed to edit change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="contributionUid">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        public KeyValuePair<bool, ChangeRequest> GetContributionCrByUid(string contributionUid)
        {
            ChangeRequest cr = null;
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    cr = manager.GetChangeRequestByContributionUid(contributionUid);
                    if (cr == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetContributionCrByUid request: " + ex.Message);
                isSuccess = false;
            }
            return new KeyValuePair<bool, ChangeRequest>(isSuccess, cr);
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns></returns>    
        public KeyValuePair<bool, List<ChangeRequest>> GetChangeRequestListByContributionUidList(List<string> contributionUiDs)
        {
            List<ChangeRequest> crList = null;
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    crList = manager.GetChangeRequestListByContributionUidList(contributionUiDs);
                    if (crList == null || crList.Count == 0)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetChangeRequestListByContributionUidList request: " + ex.Message, ex);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<ChangeRequest>>(isSuccess, crList);
        }

        public List<ChangeRequest> GetWgCrsByWgTdocList(List<string> contribUids)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    return manager.GetWgCrsByWgTdocList(contribUids);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetWgCrsByWgTdocList request: " + ex.Message, ex);
            }
            return new List<ChangeRequest>();
        }

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Key value pair with bool (success status), and the change request</returns>
        public KeyValuePair<bool, ChangeRequest> GetLightChangeRequestForMinuteMan(string uid)
        {
            var isSuccess = true;
            ChangeRequest changeRequest = null;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequest = manager.GetLightChangeRequestForMinuteMan(uid);
                    if (changeRequest == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetLightChangeRequestForMinuteMan request: ", ex);
                isSuccess = false;
            }
            return new KeyValuePair<bool, ChangeRequest>(isSuccess, changeRequest);
        }

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        public KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsForMinuteMan(List<string> uids)
        {
            var isSuccess = true;
            List<ChangeRequest> changeRequests = null;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequests = manager.GetLightChangeRequestsForMinuteMan(uids);
                    if (changeRequests == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetLightChangeRequestsForMinuteMan request: ", ex);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<ChangeRequest>>(isSuccess, changeRequests);
        }

        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        public KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid)
        {
            var isSuccess = true;
            List<ChangeRequest> changeRequests = null;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequests = manager.GetLightChangeRequestsInsideCrPackForMinuteMan(uid);
                    if (changeRequests == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetLightChangeRequestsInsideCrPackForMinuteMan request: ", ex);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<ChangeRequest>>(isSuccess, changeRequests);
        }

        /// <summary>
        /// Same method than GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        public KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids)
        {
            var isSuccess = true;
            List<ChangeRequest> changeRequests = null;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequests = manager.GetLightChangeRequestsInsideCrPacksForMinuteMan(uids);
                    if (changeRequests == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetLightChangeRequestsInsideCrPacksForMinuteMan request: ", ex);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<ChangeRequest>>(isSuccess, changeRequests);
        }

        /// <summary>
        /// Fetches the list of change request statuses via the manager
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, List<Enum_ChangeRequestStatus>> GetChangeRequestStatuses()
        {
            var crStatusList = new List<Enum_ChangeRequestStatus>();
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestStatusManager>();
                    manager.UoW = uoW;
                    crStatusList = manager.GetAllChangeRequestStatuses();
                    if (crStatusList == null || crStatusList.Count == 0)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to get list of change request status: " + ex.Message);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<Enum_ChangeRequestStatus>>(isSuccess, crStatusList);

        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="crPackDecisionlst"></param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackDecisionlst)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;

                    response = manager.UpdateChangeRequestPackRelatedCrs(crPackDecisionlst);
                    if (response.Result)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Service] Failed to UpdateChangeRequestPackRelatedCrs: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                return response;
            }
            return response;
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="tdocNumbers"></param>
        /// <returns></returns>
        public ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers)
        {
            var response = new ServiceResponse<bool> { Result = false };

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var finalizeAction = new FinalizeCrsAction() { UoW = uoW };
                    response = finalizeAction.FinalizeCrs(personId, tdocNumbers);

                    if (response.Result)
                    {
                        uoW.Save();
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        }

        /// <summary>
        /// Returns the list of change requests, given the specific criteria.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="searchObj">The search object.</param>
        /// <returns>List of change requests</returns>
        public ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var crManager = ManagerFactory.Resolve<IChangeRequestManager>();
                crManager.UoW = uow;
                return crManager.GetChangeRequests(personId, searchObj);
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public ServiceResponse<bool> DoesCrNumberRevisionCoupleExist(int specId, string crNumber, int revision)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var crManager = ManagerFactory.Resolve<IChangeRequestManager>();
                crManager.UoW = uow;
                return new ServiceResponse<bool>
                {
                    Result = crManager.DoesCrNumberRevisionCoupleExist(specId, crNumber, revision)
                };
            }
        }

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        public ServiceResponse<List<ChangeRequest>> GetCrsByKeys(List<CrKeyFacade> crKeys)
        {
            var response = new ServiceResponse<List<ChangeRequest>>();

            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var crManager = ManagerFactory.Resolve<IChangeRequestManager>();
                    crManager.UoW = uow;
                    response.Result = crManager.GetCrsByKeys(crKeys);
                }
                catch (Exception ex)
                {
                    response.Report.LogError("Failed to get matching Spec# / CR # / Revision");
                    LogManager.Error(String.Format("[Service] Failed to get matching Spec# / CR # / Revision: {0}{1}{2}{3}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty), ex.StackTrace, ex.Source));
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        public ServiceResponse<ChangeRequest> GetCrByKey(CrKeyFacade crKey)
        {
            var response = new ServiceResponse<ChangeRequest>();

            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var crManager = ManagerFactory.Resolve<IChangeRequestManager>();
                    crManager.UoW = uow;
                    response.Result = crManager.GetCrByKey(crKey);
                }
                catch (Exception ex)
                {
                    response.Report.LogError("Failed to get matching Spec# / CR # / Revision");
                    LogManager.Error(String.Format("[Service] Failed to get matching Spec# / CR # / Revision: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                }
            }

            return response;        
        }

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        public ServiceResponse<List<KeyValuePair<CrKeyFacade, string>>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys)
        {
            var response = new ServiceResponse<List<KeyValuePair<CrKeyFacade, string>>>();

            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var crManager = ManagerFactory.Resolve<IChangeRequestManager>();
                    crManager.UoW = uow;
                    response.Result = crManager.GetCrWgTdocNumberOfParent(crKeys);
                }
                catch (Exception ex)
                {
                    response.Report.LogError("Failed to get WgTdoc number of parent CRs");
                    LogManager.Error("[Service] Failed to get WgTdoc number of parent CRs (GetCrWgTdocNumberOfParent):", ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Res the issue cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        public ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource)
        {
            var response = new ServiceResponse<bool> { Result = false };

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    response = manager.ReIssueCr(crKey, newTsgTdoc, newTsgMeetingId, newTsgSource);

                    if (response.Result)
                        uoW.Save();
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(e.Message);
            }
            return response;        
        }

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        public ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource)
        {
            var response = new ServiceResponse<bool> { Result = false };

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    response = manager.ReviseCr(crKey, newTsgTdoc, newTsgMeetingId, newTsgSource);

                    if (response.Result)
                        uoW.Save();
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(e.Message);
            }
            return response;        
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="crsIds"></param>
        /// <param name="crPackId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> SendCrsToCrPack(int personId, List<int> crsIds, int crPackId)
        {
            var response = new ServiceResponse<bool> { Result = false };

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    response = manager.SendCrsToCrPack(personId, crsIds, crPackId);

                    if (response.Result)
                    {
                        uoW.Save();
                        var contributionService = ServicesFactory.Resolve<IContributionService>();
                        contributionService.GenerateTdocListsAfterSendingCrsToCrPack(personId, crPackId, crsIds);
                    }
                        
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        }

        public ServiceResponse<bool> UpdateCrStatus(string uid, string status)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestStatusManager>();
                    manager.UoW = uoW;
                    response = manager.UpdateCrStatus(uid, status);

                    if (response.Result)
                        uoW.Save();
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        }

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="crsOfCrPack"></param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateCrsOfCrPackStatus(List<CrOfCrPackFacade> crsOfCrPack)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestStatusManager>();
                    manager.UoW = uoW;
                    response = manager.UpdateCrsStatusOfCrPack(crsOfCrPack);

                    if (response.Result)
                        uoW.Save();
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        } 
    }

    /// <summary>
    /// IChangeRequestService
    /// </summary>
    public interface IChangeRequestService
    {
        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Response containing success/failure information.</returns>
        ServiceResponse<int> CreateChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        bool EditChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId"></param>
        /// <returns>ChangeRequest object</returns>
        KeyValuePair<bool, ChangeRequest> GetChangeRequestById(int personId, int changeRequestId);

        /// <summary>
        /// Returns a contribution's CR data
        /// </summary>
        /// <param name="contributionUid">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        KeyValuePair<bool, ChangeRequest> GetContributionCrByUid(string contributionUid);

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns></returns>
        KeyValuePair<bool, List<ChangeRequest>> GetChangeRequestListByContributionUidList(List<string> contributionUiDs);

        List<ChangeRequest> GetWgCrsByWgTdocList(List<string> contribUids);

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Change request</returns>
        KeyValuePair<bool, ChangeRequest> GetLightChangeRequestForMinuteMan(string uid);

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsForMinuteMan(List<string> uids);

        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid);

        /// <summary>
        /// Same method than GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        KeyValuePair<bool, List<ChangeRequest>> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids);

        /// <summary>
        /// Returns the list of all change request statuses.
        /// </summary>
        /// <returns></returns>
        KeyValuePair<bool, List<Enum_ChangeRequestStatus>> GetChangeRequestStatuses();

        /// <summary>
        /// Updates the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackDecisionlst">The cr pack decisionlst.</param>
        ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackDecisionlst);

        /// <summary>
        /// Allocates versions for all the TSG approved CRs that are related to the provided TDoc Numbers
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="tdocNumbers"></param>
        /// <returns></returns>
        ServiceResponse<bool> SetCrsAsFinal(int personId, List<string> tdocNumbers);

        /// <summary>
        /// Returns the list of change requests, given the specific criteria.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="searchObj">The search object.</param>
        /// <returns>List of change requests</returns>
        ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj);

        /// <summary>
        /// Test if Cr # / Revision couple already exist
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        ServiceResponse<bool> DoesCrNumberRevisionCoupleExist(int specId, string crNumber, int revision);

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        ServiceResponse<List<ChangeRequest>> GetCrsByKeys(List<CrKeyFacade> crKeys);

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        ServiceResponse<ChangeRequest> GetCrByKey(CrKeyFacade crKey);

        /// <summary>
        /// Reissue the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource);

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <param name="newTsgSource"></param>
        /// <returns>Success/Failure</returns>
        ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId, string newTsgSource);

        /// <summary>
        /// Send Crs to Cr-Pack
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="crsIds"></param>
        /// <param name="crPackId"></param>
        /// <returns></returns>
        ServiceResponse<bool> SendCrsToCrPack(int personId, List<int> crsIds, int crPackId);

        ServiceResponse<bool> UpdateCrStatus(string uid, string status);

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="crsOfCrPack"></param>
        /// <returns></returns>
        ServiceResponse<bool> UpdateCrsOfCrPackStatus(List<CrOfCrPackFacade> crsOfCrPack);

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        ServiceResponse<List<KeyValuePair<CrKeyFacade, string>>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys);
    }
}

