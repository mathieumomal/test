using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;

namespace Etsi.Ultimate.Business
{
    public class ChangeRequestManager : IChangeRequestManager
    {
        #region IChangeRequestManager Members
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        public ServiceResponse<bool> CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var response = new ServiceResponse<bool> { Result = true };
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                if (string.IsNullOrEmpty(changeRequest.CRNumber) || (changeRequest.RevisionOf != null && !changeRequest.Revision.HasValue))
                {
                    if (!changeRequest.IsAutoNumberingOff) //Assign cr# & revision only when auto numbering is On
                    {
                        if (changeRequest.RevisionOf != null)
                            ManageChangeRequestRevision(changeRequest, repo);
                        else
                        {
                            changeRequest.CRNumber = GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
                            changeRequest.Revision = null;
                        }
                    }
                }
                //Verify that values provided are correct
                else
                {
                    var existingCr = repo.All.FirstOrDefault(cr => cr.Fk_Specification == changeRequest.Fk_Specification && cr.CRNumber == changeRequest.CRNumber && cr.Revision == changeRequest.Revision);
                    if (existingCr != null)
                    {
                        response.Report.LogError(string.Format(Localization.ChangeRequest_Create_AlreadyExists, changeRequest.CRNumber, changeRequest.Revision.HasValue? changeRequest.Revision.Value.ToString(): "none"));
                        response.Result = false;
                        return response;
                    }
                }
                repo.InsertOrUpdate(changeRequest);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Business] Failed to create change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        }

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        public bool EditChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var isSuccess = true;
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var dbChangeRequest = repo.Find(changeRequest.Pk_ChangeRequest);
                CompareAndUpdate(changeRequest, dbChangeRequest);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Business] Failed to edit change request: {0} - {1}{2}", "WcfBusinessCrEditFailed", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Generates the cr number.
        /// </summary>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>Cr number</returns>
        public string GenerateCrNumberBySpecificationId(int? specificationId)
        {
            var crNumberList = new List<int>();

            var heighestCrNumber = 0;
            if (specificationId > 0)
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var crNumber = repo.FindCrNumberBySpecificationId(specificationId);

                foreach (var num in crNumber)
                {
                    int tmpCrNumber;
                    if (int.TryParse(num, out tmpCrNumber))
                        crNumberList.Add(tmpCrNumber);
                }
                if (crNumberList.Count != 0)
                {
                    heighestCrNumber = crNumberList.Max();
                }
                heighestCrNumber++;
            }
            return heighestCrNumber.ToString(new String('0', 4));
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>change Request object</returns>
        public ChangeRequest GetChangeRequestById(int personId, int changeRequestId)
        {
            var changeRequest = new ChangeRequest();
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                changeRequest = repo.Find(changeRequestId);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Business] Failed to get change request id : {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
            }
            return changeRequest;
        }

        /// <summary>
        /// Returns a contribution's CR data
        /// </summary>
        /// <param name="contributionUid">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        public ChangeRequest GetChangeRequestByContributionUid(string contributionUid)
        {
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var result = repo.GetChangeRequestByContributionUID(contributionUid);
                return result;
            }
            catch (Exception ex)
            {
                LogManager.Error("[Business] Failed to GetContributionCRByUid:" + ex.Message);
                LogManager.Error(ex.StackTrace);
                while (ex.InnerException != null)
                {
                    LogManager.Error("InnerException: " + ex.InnerException.Message + "\n"+ex.InnerException.StackTrace);
                    ex = ex.InnerException;
                }
                    
                return null;
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns></returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs)
        {
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var result = repo.GetChangeRequestListByContributionUidList(contributionUiDs);
                return result;
            }
            catch (Exception ex)
            {
                LogManager.Error("[Business] Failed to GetChangeRequestListByContributionUIDList:" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="crPackTsgDecisions">The cr pack TSG decisions.</param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackTsgDecisions)
        {
            var response = new ServiceResponse<bool>{Result = true};
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var crStatusRepository = RepositoryFactory.Resolve<IChangeRequestStatusRepository>();
            crStatusRepository.UoW = UoW;
            var crStatuses = crStatusRepository.All.ToList();
            foreach (var crPackDecision in crPackTsgDecisions)
            {
                //[Step 1]:: Check for valid status
                int? statusId = null;
                if (!String.IsNullOrEmpty(crPackDecision.Value))
                {
                    var crStatus = crStatuses.Find(x => x.Code == crPackDecision.Value.Trim());
                    if (crStatus == null)
                    {
                        response.Report.LogError(String.Format(Localization.CR_Status_Not_Found, crPackDecision.Value));
                        break;
                    }
                    statusId = crStatus.Pk_EnumChangeRequestStatus;
                }

                //[Step 2]:: (a) Create Revision / (b) Create Reissue / (C) Update Decision
                var changeRequest = crRepository.GetCrByKey(crPackDecision.Key);
                var changeRequestRevisions = crRepository.GetCrsBySpecAndCrNumberAndTsgTdoc(crPackDecision.Key);
                //[Step 2a]:: Create Revision (if applicable)
                if (changeRequest == null)
                {
                    var crkeyFacade = new CrKeyFacade { SpecId = crPackDecision.Key.SpecId, SpecNumber = crPackDecision.Key.SpecNumber, CrNumber = crPackDecision.Key.CrNumber };
                    var revisedCr = crRepository.GetCrByKey(crkeyFacade);
                    if (revisedCr == null)
                    {
                        response.Report.LogError(String.Format(Localization.CR_Not_Found, crPackDecision.Key.GetIdentifierForLog()));
                        break;
                    }
                    else if (changeRequestRevisions.Any(x => x.ChangeRequestTsgDatas.Any(y => y.TSGTdoc == crPackDecision.Key.TsgTdocNumber)))
                    {
                        response.Report.LogError(String.Format(Localization.CR_Cannot_Revise_For_Same_TsgTdocNumber, crPackDecision.Key.GetIdentifierForLog(), crPackDecision.Key.TsgTdocNumber));
                        break;
                    }
                    else
                    {
                        var revisionCr = new ChangeRequest()
                        {
                            CRNumber = crPackDecision.Key.CrNumber,
                            Revision = crPackDecision.Key.Revision,
                            Subject = revisedCr.Subject,
                            Fk_WGStatus = null,
                            CreationDate = DateTime.UtcNow,
                            WGSourceOrganizations = revisedCr.WGSourceOrganizations,
                            WGSourceForTSG = revisedCr.WGSourceForTSG,
                            WGMeeting = revisedCr.WGMeeting,
                            WGTarget = revisedCr.WGTarget,
                            WGTDoc = String.Empty,
                            Fk_Enum_CRCategory = revisedCr.Fk_Enum_CRCategory,
                            Fk_Specification = revisedCr.Fk_Specification,
                            Fk_Release = revisedCr.Fk_Release,
                            Fk_CurrentVersion = revisedCr.Fk_CurrentVersion,
                            Fk_NewVersion = revisedCr.Fk_NewVersion,
                            Fk_Impact = revisedCr.Fk_Impact,
                            ChangeRequestTsgDatas = new List<ChangeRequestTsgData> { new ChangeRequestTsgData { TSGTdoc = crPackDecision.Key.TsgTdocNumber, 
                                                                                                                TSGMeeting = crPackDecision.Key.TsgMeetingId,
                                                                                                                Fk_TsgStatus = statusId } }
                        };

                        crRepository.InsertOrUpdate(revisionCr);
                    }
                }
                else
                {
                    var tsgDataToChange = changeRequest.ChangeRequestTsgDatas.FirstOrDefault(x => (x.TSGTdoc ?? "").Trim() == (crPackDecision.Key.TsgTdocNumber ?? "").Trim());
                    //[Step 2b]:: Create Reissue
                    if (tsgDataToChange == null)
                    {
                        var newTsgData = new ChangeRequestTsgData
                        {
                            TSGTdoc = crPackDecision.Key.TsgTdocNumber,
                            TSGMeeting = crPackDecision.Key.TsgMeetingId,
                            Fk_TsgStatus = statusId
                        };

                        changeRequest.ChangeRequestTsgDatas.Add(newTsgData);
                    }
                    else //[Step 2c]:: Update Decision
                    {
                        if (tsgDataToChange.Fk_TsgStatus != statusId)
                            tsgDataToChange.Fk_TsgStatus = statusId;
                    }
                }
            }

            if (response.Report.GetNumberOfErrors() > 0)
                response.Result = false;
            return response;
        }

        /// <summary>
        /// Returns a list of change requests given the specified criteria.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="searchObj">The search object.</param>
        /// <returns>List of change requests</returns>
        public ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj)
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var repoResponse = crRepository.GetChangeRequests(searchObj);

            var crsListFacade = new List<ChangeRequestListFacade>();
            repoResponse.Key.ForEach(x =>
            {
                if (x.ChangeRequestTsgDatas != null && x.ChangeRequestTsgDatas.Count > 0)
                    x.ChangeRequestTsgDatas.ForEach(y =>
                    {
                        if (searchObj.TsgStatusIds == null || searchObj.TsgStatusIds.Count == 0 || searchObj.TsgStatusIds.Contains(y.Fk_TsgStatus ?? 0))
                            crsListFacade.Add(ConvertChangeRequestToChangeRequestListFacade(x, y));
                    });
                else
                    crsListFacade.Add(ConvertChangeRequestToChangeRequestListFacade(x, null));
            });

            return new ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> { Result = new KeyValuePair<List<ChangeRequestListFacade>, int>(crsListFacade, repoResponse.Value) };
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public bool DoesCrNumberRevisionCoupleExist(int specId, string crNumber, int revision)
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            return crRepository.DoesCrNumberRevisionCoupleExist(specId, crNumber, revision);
        }

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        public List<ChangeRequest> GetCrsByKeys(List<CrKeyFacade> crKeys)
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            return crRepository.GetCrsByKeys(crKeys);
        }

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        public ChangeRequest GetCrByKey(CrKeyFacade crKey)
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            return crRepository.GetCrByKey(crKey);
        }

        /// <summary>
        /// Res the issue cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <returns>Success/Failure</returns>
        public ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var dbChangeRequest = repo.GetCrByKey(crKey);
                if (dbChangeRequest == null)
                {
                    response.Report.LogError(String.Format("Change request not found : Spec#: {0}, Cr#: {1}, Revision: {2}", crKey.SpecNumber, crKey.CrNumber, crKey.Revision));
                    return response;
                }

                if (dbChangeRequest.ChangeRequestTsgDatas == null)
                    dbChangeRequest.ChangeRequestTsgDatas = new List<ChangeRequestTsgData>();

                var newTsgData = new ChangeRequestTsgData
                {
                    TSGTdoc = newTsgTdoc,
                    TSGMeeting = newTsgMeetingId
                };

                dbChangeRequest.ChangeRequestTsgDatas.Add(newTsgData);
                response.Result = true;
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
        /// <returns>Success/Failure</returns>
        public ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;

                var dbChangeRequest = repo.GetCrByKey(crKey);
                if (dbChangeRequest == null)
                {
                    response.Report.LogError(String.Format("Change request not found : Spec#: {0}, Cr#: {1}, Revision: {2}", crKey.SpecNumber, crKey.CrNumber, crKey.Revision));
                    return response;
                }

                //Create new Change Request (Without WgTdoc)
                var revisionMaxFound = repo.FindCrMaxRevisionBySpecificationIdAndCrNumber(dbChangeRequest.Fk_Specification, dbChangeRequest.CRNumber);

                var newTsgData = new ChangeRequestTsgData { TSGTdoc = newTsgTdoc, TSGMeeting = newTsgMeetingId };
                var newChangeRequest = new ChangeRequest()
                {
                    CRNumber = dbChangeRequest.CRNumber,
                    Revision = revisionMaxFound + 1,
                    Subject = dbChangeRequest.Subject,
                    Fk_WGStatus = null,
                    CreationDate = DateTime.UtcNow,
                    WGSourceOrganizations = dbChangeRequest.WGSourceOrganizations,
                    WGSourceForTSG = dbChangeRequest.WGSourceForTSG,
                    WGMeeting = dbChangeRequest.WGMeeting,
                    WGTarget = dbChangeRequest.WGTarget,
                    WGTDoc = String.Empty,
                    Fk_Enum_CRCategory = dbChangeRequest.Fk_Enum_CRCategory,
                    Fk_Specification = dbChangeRequest.Fk_Specification,
                    Fk_Release = dbChangeRequest.Fk_Release,
                    Fk_CurrentVersion = dbChangeRequest.Fk_CurrentVersion,
                    Fk_NewVersion = dbChangeRequest.Fk_NewVersion,
                    Fk_Impact = dbChangeRequest.Fk_Impact,
                    ChangeRequestTsgDatas = new List<ChangeRequestTsgData> { newTsgData }
                };

                repo.InsertOrUpdate(newChangeRequest);

                response.Result = true;
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message + e.StackTrace);
                response.Result = false;
                response.Report.LogError(e.Message);
            }
            return response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Compares the and update.
        /// </summary>
        /// <param name="uiChangeRequest">The UI change request.</param>
        /// <param name="dbChangeRequest">The database change request.</param>
        private void CompareAndUpdate(ChangeRequest uiChangeRequest, ChangeRequest dbChangeRequest)
        {
            if (uiChangeRequest != null && dbChangeRequest != null)
            {
                if (dbChangeRequest.CRNumber != uiChangeRequest.CRNumber)
                    dbChangeRequest.CRNumber = uiChangeRequest.CRNumber;
                if (dbChangeRequest.Revision != uiChangeRequest.Revision)
                    dbChangeRequest.Revision = uiChangeRequest.Revision;
                if (dbChangeRequest.Subject != uiChangeRequest.Subject)
                    dbChangeRequest.Subject = uiChangeRequest.Subject;
                if (dbChangeRequest.Fk_WGStatus != uiChangeRequest.Fk_WGStatus)
                    dbChangeRequest.Fk_WGStatus = uiChangeRequest.Fk_WGStatus;
                if (dbChangeRequest.CreationDate != uiChangeRequest.CreationDate)
                    dbChangeRequest.CreationDate = uiChangeRequest.CreationDate;
                if (dbChangeRequest.WGSourceOrganizations != uiChangeRequest.WGSourceOrganizations)
                    dbChangeRequest.WGSourceOrganizations = uiChangeRequest.WGSourceOrganizations;
                if (dbChangeRequest.WGSourceForTSG != uiChangeRequest.WGSourceForTSG)
                    dbChangeRequest.WGSourceForTSG = uiChangeRequest.WGSourceForTSG;
                if (dbChangeRequest.WGMeeting != uiChangeRequest.WGMeeting)
                    dbChangeRequest.WGMeeting = uiChangeRequest.WGMeeting;
                if (dbChangeRequest.WGTarget != uiChangeRequest.WGTarget)
                    dbChangeRequest.WGTarget = uiChangeRequest.WGTarget;
                if (dbChangeRequest.Fk_Enum_CRCategory != uiChangeRequest.Fk_Enum_CRCategory)
                    dbChangeRequest.Fk_Enum_CRCategory = uiChangeRequest.Fk_Enum_CRCategory;
                if (dbChangeRequest.Fk_Specification != uiChangeRequest.Fk_Specification)
                    dbChangeRequest.Fk_Specification = uiChangeRequest.Fk_Specification;
                if (dbChangeRequest.Fk_Release != uiChangeRequest.Fk_Release)
                    dbChangeRequest.Fk_Release = uiChangeRequest.Fk_Release;
                if (dbChangeRequest.Fk_CurrentVersion != uiChangeRequest.Fk_CurrentVersion)
                    dbChangeRequest.Fk_CurrentVersion = uiChangeRequest.Fk_CurrentVersion;
                if (dbChangeRequest.Fk_NewVersion != uiChangeRequest.Fk_NewVersion)
                    dbChangeRequest.Fk_NewVersion = uiChangeRequest.Fk_NewVersion;
                if (dbChangeRequest.Fk_Impact != uiChangeRequest.Fk_Impact)
                    dbChangeRequest.Fk_Impact = uiChangeRequest.Fk_Impact;
                if (dbChangeRequest.WGTDoc != uiChangeRequest.WGTDoc)
                    dbChangeRequest.WGTDoc = uiChangeRequest.WGTDoc;

                //CR WorkItems (Insert / Delete)
                var crWorkItemsToInsert = uiChangeRequest.CR_WorkItems.ToList().Where(x => dbChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToInsert.ToList().ForEach(x => dbChangeRequest.CR_WorkItems.Add(x));
                var crWorkItemsToDelete = dbChangeRequest.CR_WorkItems.ToList().Where(x => uiChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToDelete.ToList().ForEach(x => UoW.MarkDeleted(x));

                //Tsg Data (Insert / Update)
                var tsgDataToInsert = uiChangeRequest.ChangeRequestTsgDatas.ToList().Where(x => dbChangeRequest.ChangeRequestTsgDatas.ToList().All(y => y.TSGTdoc != x.TSGTdoc));
                tsgDataToInsert.ToList().ForEach(x => dbChangeRequest.ChangeRequestTsgDatas.Add(x));
                var tsgDataToUpdate = uiChangeRequest.ChangeRequestTsgDatas.ToList().Where(x => dbChangeRequest.ChangeRequestTsgDatas.ToList().Any(y => y.TSGTdoc == x.TSGTdoc && y.Fk_TsgStatus != x.Fk_TsgStatus));
                tsgDataToUpdate.ToList().ForEach(x => dbChangeRequest.ChangeRequestTsgDatas.ToList().Find(y => y.TSGTdoc == x.TSGTdoc).Fk_TsgStatus = x.Fk_TsgStatus);
            }
        }

        /// <summary>
        /// Set CR number and Revision of the new CR according to its "revision of" CR
        /// </summary>
        private void ManageChangeRequestRevision(ChangeRequest newCr, IChangeRequestRepository repo)
        {
            //Get CRNumber
            var parentCr = GetChangeRequestByContributionUid(newCr.RevisionOf);
            if (parentCr == null)
                throw new InvalidOperationException(String.Format("Change request with contribution UID : {0} not found", newCr.RevisionOf));
            newCr.CRNumber = parentCr.CRNumber;

            //Get revision
            var revisionMaxFound = repo.FindCrMaxRevisionBySpecificationIdAndCrNumber(parentCr.Fk_Specification, parentCr.CRNumber);
            newCr.Revision = revisionMaxFound + 1;

            //Put the parent CR contribution as Revised if not yet decided
            var crStatusMgr = ManagerFactory.Resolve<IChangeRequestStatusManager>();
            crStatusMgr.UoW = UoW;
            var allChangeRequestStatuses = crStatusMgr.GetAllChangeRequestStatuses();
            var revisedStatus = allChangeRequestStatuses.FirstOrDefault(x => x.Code == Enum_ChangeRequestStatuses.Revised.ToString());
            if (revisedStatus != null)
            {
                if (newCr.RevisionOf.Equals(parentCr.WGTDoc) && parentCr.Fk_WGStatus.GetValueOrDefault() == 0)
                {
                    parentCr.Fk_WGStatus = revisedStatus.Pk_EnumChangeRequestStatus;
                }
                else if (parentCr.ChangeRequestTsgDatas != null && 
                    parentCr.ChangeRequestTsgDatas.Any() && 
                    newCr.RevisionOf.Equals(parentCr.ChangeRequestTsgDatas.First().TSGTdoc) && 
                    parentCr.ChangeRequestTsgDatas.First().Fk_TsgStatus.GetValueOrDefault() == 0)
                {
                    parentCr.ChangeRequestTsgDatas.First().Fk_TsgStatus = revisedStatus.Pk_EnumChangeRequestStatus;
                }
            }
        }
       
        /// <summary>
        /// Convert Change request to change request list facade
        /// </summary>
        /// <returns>Converted crs for search list</returns>
        private ChangeRequestListFacade ConvertChangeRequestToChangeRequestListFacade(ChangeRequest cr, ChangeRequestTsgData tsgData)
        {
            return new ChangeRequestListFacade
            {
                ChangeRequestId = cr.Pk_ChangeRequest,
                SpecNumber = cr.Specification != null ? cr.Specification.Number : null,
                ChangeRequestNumber = cr.CRNumber,
                Revision = (cr.Revision == null ? "-" : cr.Revision.ToString()),
                ImpactedVersion = cr.CurrentVersion != null ? string.Format("{0}.{1}.{2}", 
                                                        cr.CurrentVersion.MajorVersion, 
                                                        cr.CurrentVersion.TechnicalVersion,
                                                        cr.CurrentVersion.EditorialVersion) : null,
                TargetRelease = cr.Release != null ? cr.Release.ShortName : null,
                Title = cr.Subject,
                WgTdocNumber = cr.WGTDoc,
                TsgTdocNumber = (tsgData != null) ? tsgData.TSGTdoc : null,
                WgStatus = cr.WgStatus != null ? cr.WgStatus.Description : null,
                TsgStatus = (tsgData != null) ? ((tsgData.TsgStatus != null) ? tsgData.TsgStatus.Description : null) : null,
                NewVersion = cr.NewVersion != null ? string.Format("{0}.{1}.{2}", 
                                                cr.NewVersion.MajorVersion, 
                                                cr.NewVersion.TechnicalVersion,
                                                cr.NewVersion.EditorialVersion) : null,
                SpecId = cr.Fk_Specification ?? 0,
                TargetReleaseId = cr.Release != null ? cr.Release.Pk_ReleaseId : 0,
                ImpactedVersionPath =  cr.CurrentVersion != null ? cr.CurrentVersion.Location : null,
                NewVersionPath = cr.NewVersion != null ? cr.NewVersion.Location : null
            };
        }

        #endregion
    }

    /// <summary>
    /// IChangeRequestManager
    /// </summary>
    public interface IChangeRequestManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        ServiceResponse<bool> CreateChangeRequest(int personId, ChangeRequest changeRequest);

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
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>change Request object</returns>
        ChangeRequest GetChangeRequestById(int personId, int changeRequestId);

        /// <summary>
        /// Returns a contribution's CR data
        /// </summary>
        /// <param name="contributionUid">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        ChangeRequest GetChangeRequestByContributionUid(string contributionUid);

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns></returns>
        List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs);

        /// <summary>
        /// Updates the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackTsgDecisions">The cr pack TSG decisionlst.</param>
        /// <returns></returns>
        ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<CrKeyFacade, string>> crPackTsgDecisions);

        /// <summary>
        /// Returns a list of change requests given the specified criteria.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="searchObj">The search object.</param>
        /// <returns>List of change requests</returns>
        ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj);

        /// <summary>
        /// Test if CR # / revision couple already exist
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="crNumber"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        bool DoesCrNumberRevisionCoupleExist(int specId, string crNumber, int revision);

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        List<ChangeRequest> GetCrsByKeys(List<CrKeyFacade> crKeys);

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change request</returns>
        ChangeRequest GetCrByKey(CrKeyFacade crKey);

        /// <summary>
        /// Reissue the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <returns>Success/Failure</returns>
        ServiceResponse<bool> ReIssueCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId);

        /// <summary>
        /// Revise the cr.
        /// </summary>
        /// <param name="crKey">The cr identifier.</param>
        /// <param name="newTsgTdoc">The new TSG tdoc.</param>
        /// <param name="newTsgMeetingId">The new TSG meeting identifier.</param>
        /// <returns>Success/Failure</returns>
        ServiceResponse<bool> ReviseCr(CrKeyFacade crKey, string newTsgTdoc, int newTsgMeetingId);
    }
}
