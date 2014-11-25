using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    if (changeRequest.RevisionOf != null)
                        ManageChangeRequestRevision(changeRequest, repo);
                    else
                    {
                        changeRequest.CRNumber = GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
                        changeRequest.Revision = null;
                    }
                }
                //Verify that values provided are correct
                else
                {
                    var existingCr = repo.All.Where(cr => cr.Fk_Specification == changeRequest.Fk_Specification && cr.CRNumber == changeRequest.CRNumber && cr.Revision == changeRequest.Revision).FirstOrDefault();
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
                return null;
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns></returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUIDList(List<string> contributionUiDs)
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
        /// <param name="tsgTdocNumber"></param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<string, string>> crPackTsgDecisions, string tsgTdocNumber)
        {
            var response = new ServiceResponse<bool>{Result = true};
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var crStatusRepository = RepositoryFactory.Resolve<IChangeRequestStatusRepository>();
            crStatusRepository.UoW = UoW;
            var crStatuses = crStatusRepository.All.ToList();
            foreach (var crPackDecision in crPackTsgDecisions)
            {
                var changeRequest = crRepository.FindByWgTDoc(crPackDecision.Key);
                if (changeRequest == null)
                {
                    response.Report.LogError("Change request not found : " + crPackDecision.Key);
                    break;
                }
                //Update status
                // If status is empty, it means we need to empty it.
                if (String.IsNullOrEmpty(crPackDecision.Value))
                    changeRequest.Fk_TSGStatus = null;
                else
                {
                    var crStatus = crStatuses.Find(x => x.Code == crPackDecision.Value);
                    if (crStatus == null)
                    {
                        response.Report.LogError("Status not found : " + crPackDecision.Value);
                        break;
                    }
                    if ((crStatus != null) && (changeRequest.Fk_TSGStatus != crStatus.Pk_EnumChangeRequestStatus))
                        changeRequest.Fk_TSGStatus = crStatus.Pk_EnumChangeRequestStatus;
                }
                //Update TSG TDoc number
                if (changeRequest.TSGTDoc != tsgTdocNumber)
                    changeRequest.TSGTDoc = tsgTdocNumber;

            }

            if (response.Report.GetNumberOfErrors() > 0)
                response.Result = false;
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
                if (dbChangeRequest.Fk_TSGStatus != uiChangeRequest.Fk_TSGStatus)
                    dbChangeRequest.Fk_TSGStatus = uiChangeRequest.Fk_TSGStatus;
                if (dbChangeRequest.Fk_WGStatus != uiChangeRequest.Fk_WGStatus)
                    dbChangeRequest.Fk_WGStatus = uiChangeRequest.Fk_WGStatus;
                if (dbChangeRequest.CreationDate != uiChangeRequest.CreationDate)
                    dbChangeRequest.CreationDate = uiChangeRequest.CreationDate;
                if (dbChangeRequest.TSGSourceOrganizations != uiChangeRequest.TSGSourceOrganizations)
                    dbChangeRequest.TSGSourceOrganizations = uiChangeRequest.TSGSourceOrganizations;
                if (dbChangeRequest.WGSourceOrganizations != uiChangeRequest.WGSourceOrganizations)
                    dbChangeRequest.WGSourceOrganizations = uiChangeRequest.WGSourceOrganizations;
                if (dbChangeRequest.TSGMeeting != uiChangeRequest.TSGMeeting)
                    dbChangeRequest.TSGMeeting = uiChangeRequest.TSGMeeting;
                if (dbChangeRequest.TSGTarget != uiChangeRequest.TSGTarget)
                    dbChangeRequest.TSGTarget = uiChangeRequest.TSGTarget;
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
                if (dbChangeRequest.TSGTDoc != uiChangeRequest.TSGTDoc)
                    dbChangeRequest.TSGTDoc = uiChangeRequest.TSGTDoc;
                if (dbChangeRequest.WGTDoc != uiChangeRequest.WGTDoc)
                    dbChangeRequest.WGTDoc = uiChangeRequest.WGTDoc;

                //CR WorkItems (Insert / Delete)
                var crWorkItemsToInsert = uiChangeRequest.CR_WorkItems.ToList().Where(x => dbChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToInsert.ToList().ForEach(x => dbChangeRequest.CR_WorkItems.Add(x));
                var crWorkItemsToDelete = dbChangeRequest.CR_WorkItems.ToList().Where(x => uiChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToDelete.ToList().ForEach(x => UoW.MarkDeleted<CR_WorkItems>(x));
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
                else if (newCr.RevisionOf.Equals(parentCr.TSGTDoc) && parentCr.Fk_TSGStatus.GetValueOrDefault() == 0)
                {
                    parentCr.Fk_TSGStatus = revisedStatus.Pk_EnumChangeRequestStatus;
                }
            }

        }
       
        #endregion

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj"></param>
        /// <returns></returns>
        public ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj)
        {
            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var repoResponse = crRepository.GetChangeRequests(searchObj);

            var crsListFacade = new List<ChangeRequestListFacade>();
            repoResponse.Key.ForEach(x => crsListFacade.Add(ConvertChangeRequestToChangeRequestListFacades(x)));

            return new ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> { Result = new KeyValuePair<List<ChangeRequestListFacade>, int>(crsListFacade, repoResponse.Value) };
        }

        /// <summary>
        /// Convert Change request to change request list facade
        /// </summary>
        /// <returns></returns>
        private ChangeRequestListFacade ConvertChangeRequestToChangeRequestListFacades(ChangeRequest cr)
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
                TsgTdocNumber = cr.TSGTDoc,
                WgStatus = cr.WgStatus != null ? cr.WgStatus.Description : null,
                TsgStatus = cr.TsgStatus != null ? cr.TsgStatus.Description : null,
                NewVersion = cr.NewVersion != null ? string.Format("{0}.{1}.{2}", 
                                                cr.NewVersion.MajorVersion, 
                                                cr.NewVersion.TechnicalVersion,
                                                cr.NewVersion.EditorialVersion) : null,
                SpecId = cr.Fk_Specification ?? 0,
                TargetReleaseId = cr.Release != null ? cr.Release.Pk_ReleaseId : 0,
                NewVersionPath = cr.NewVersion != null ? cr.NewVersion.Location : null
            };
        }
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
        List<ChangeRequest> GetChangeRequestListByContributionUIDList(List<string> contributionUiDs);


        /// <summary>
        /// Updates the CRs related to a CR Pack (TSG decision and TsgTdocNumber)
        /// </summary>
        /// <param name="crPackTsgDecisions">The cr pack TSG decisionlst.</param>
        /// <param name="tsgTdocNumber"></param>
        /// <returns></returns>
        ServiceResponse<bool> UpdateChangeRequestPackRelatedCrs(List<KeyValuePair<string, string>> crPackTsgDecisions, string tsgTdocNumber);

        /// <summary>
        /// Returns a list of change requests given the specified criteria.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj"></param>
        /// <returns></returns>
        ServiceResponse<KeyValuePair<List<ChangeRequestListFacade>, int>> GetChangeRequests(int personId, ChangeRequestsSearch searchObj);
    }
}
