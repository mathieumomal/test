using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;
using System.Web.UI.WebControls;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// CR Repository to interact database for CR related activities
    /// </summary>
    public class ChangeRequestRepository : IChangeRequestRepository
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get all change request details.
        /// </summary>
        public IQueryable<ChangeRequest> All
        {
            get
            {
                return UoW.Context.ChangeRequests;
            }
        }

        /// <summary>
        /// Get all change request details including the related entities which are provided
        /// </summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>Change request details query</returns>
        public IQueryable<ChangeRequest> AllIncluding(params System.Linq.Expressions.Expression<System.Func<ChangeRequest, object>>[] includeProperties)
        {
            IQueryable<ChangeRequest> query = UoW.Context.ChangeRequests;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(t => t.ChangeRequestTsgDatas).Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus)).Include(includeProperty);
            }
            return query;
        }

        /// <summary>
        /// Finds the by specification identifier.
        /// </summary>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>string list</returns>
        public List<string> FindCrNumberBySpecificationId(int? specificationId)
        {
            return UoW.Context.ChangeRequests.Where(r => r.Fk_Specification == specificationId).Select(x => x.CRNumber).ToList();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="specificationId"></param>
        /// <param name="crNumber"></param>
        /// <returns></returns>
        public int FindCrMaxRevisionBySpecificationIdAndCrNumber(int? specificationId, string crNumber)
        {
            return UoW.Context.ChangeRequests.Where(r => r.Fk_Specification == specificationId && r.CRNumber == crNumber).Max(x => x.Revision).GetValueOrDefault();
        }

        /// <summary>
        /// Return CR by contribution UID
        /// </summary>
        /// <param name="contributionUid">Contribution Uid</param>
        /// <returns>ChangeRequest entity</returns>
        public ChangeRequest GetChangeRequestByContributionUID(string contributionUid)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus).SingleOrDefault(c => c.ChangeRequestTsgDatas.Any(x => x.TSGTdoc.Equals(contributionUid)) || c.WGTDoc.Equals(contributionUid));
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs">Contribution Uid list</param>
        /// <returns>List of CRs</returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus).Where(c => c.ChangeRequestTsgDatas.Any(x => contributionUiDs.Contains(x.TSGTdoc)) || contributionUiDs.Contains(c.WGTDoc)).ToList();
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="changeRequestId"></param>
        /// <returns>Change request entity</returns>
        public ChangeRequest Find(int changeRequestId)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus).SingleOrDefault(x => x.Pk_ChangeRequest == changeRequestId);
        }

        /// <summary>
        /// Inserts or update the change request entity
        /// </summary>
        /// <param name="entity">Change request entity.</param>
        public void InsertOrUpdate(ChangeRequest entity)
        {
            UoW.Context.SetAdded(entity);

            // Add / edit CRtsgData
            foreach (var crTsgData in entity.ChangeRequestTsgDatas)
            {
                crTsgData.ChangeRequest = entity;
                if (crTsgData.Fk_ChangeRequest == default(int))
                    UoW.Context.SetAdded(crTsgData);
                else
                    UoW.Context.SetModified(crTsgData);
            }
        }

        /// <summary>
        /// Deletes the change request entity based on the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the status by wgtdocument.
        /// </summary>
        /// <param name="wgTDoc">The wgtdocument.</param>
        /// <returns></returns>
        public ChangeRequest FindByWgTDoc(string wgTDoc)
        {
            return UoW.Context.ChangeRequests.SingleOrDefault(wg => wg.WGTDoc == wgTDoc);
        }

        /// <summary>
        /// Gets the change requests.
        /// </summary>
        /// <param name="searchObj">The search object.</param>
        /// <returns>List of crs & count</returns>
        public KeyValuePair<List<ChangeRequest>, int> GetChangeRequests(ChangeRequestsSearch searchObj)
        {
            //Manage null values of search object
            if (searchObj.ReleaseIds == null) searchObj.ReleaseIds = new List<int>();
            if (searchObj.WgStatusIds == null) searchObj.WgStatusIds = new List<int>();
            if (searchObj.TsgStatusIds == null) searchObj.TsgStatusIds = new List<int>();
            if (searchObj.MeetingIds == null) searchObj.MeetingIds = new List<int>();
            if (searchObj.WorkItemIds == null) searchObj.WorkItemIds = new List<int>();

            var query = AllIncluding(x => x.Specification, x => x.Release, x => x.NewVersion, x => x.CurrentVersion, x => x.WgStatus);

            //Filter crs based on search criteria
            query = query.Where(x => (((String.IsNullOrEmpty(searchObj.SpecificationNumber)) || (x.Specification.Number.ToLower().Contains(searchObj.SpecificationNumber.ToLower())))
                                   && ((searchObj.VersionId == 0) || (x.Fk_NewVersion == searchObj.VersionId))
                                   && ((searchObj.ReleaseIds.Count ==  0) || (searchObj.ReleaseIds.Contains(0)) || (searchObj.ReleaseIds.Contains(x.Fk_Release ?? 0)))
                                   && ((searchObj.WgStatusIds.Count == 0) || (searchObj.WgStatusIds.Contains(x.Fk_WGStatus ?? 0)))
                                   && ((searchObj.TsgStatusIds.Count == 0) || (x.ChangeRequestTsgDatas.Any(t => searchObj.TsgStatusIds.Contains(t.Fk_TsgStatus ?? 0))))
                                   && ((searchObj.MeetingIds.Count == 0) || (x.ChangeRequestTsgDatas.Any(t => searchObj.MeetingIds.Contains(t.TSGMeeting ?? 0))) || (searchObj.MeetingIds.Contains(x.WGMeeting ?? 0)))
                                   && ((searchObj.WorkItemIds.Count == 0) || (searchObj.WorkItemIds.Any(wiId => x.CR_WorkItems.Any(crWi => crWi.Fk_WIId == wiId))))));

            //Order by
            query = query.OrderBy(x => x.Specification.Number).ThenByDescending(y => y.CRNumber).ThenByDescending(z => z.Revision);
            
            return searchObj.PageSize != 0 ? 
                new KeyValuePair<List<ChangeRequest>, int>(query.Skip(searchObj.SkipRecords).Take(searchObj.PageSize).ToList(), query.Count()) : 
                new KeyValuePair<List<ChangeRequest>, int>(query.ToList(), query.Count());
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
            return UoW.Context.ChangeRequests.Any(x => x.CRNumber == crNumber.Trim() && x.Revision == revision && x.Fk_Specification == specId);
        }

        /// <summary>
        /// Get CRs by keys
        /// </summary>
        /// <param name="crKeys">The spec# / cr# / revision / TsgTdocNumber combination list.</param>
        /// <returns>Matching Crs for given key combination</returns>
        public List<ChangeRequest> GetCrsByKeys(List<CrKeyFacade> crKeys)
        {
            var specIds = crKeys.Select(x => x.SpecId).Distinct().ToList();
            var specNumbers = crKeys.Select(x => x.SpecNumber).Distinct().ToList();
            var crNumbers = crKeys.Select(x => x.CrNumber).Distinct().ToList();
            var revisionNumbers = crKeys.Select(x => x.Revision).Distinct().ToList();

            //Search CR to process : 
            //1) Search all CR which match with Keys values (Spec ID, CR number, Revision)
            //2) Inside this list search which one match to the exact key value of each one
            var matchingCombinations = AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus)
                .Where(individualMatch => (specIds.Contains(individualMatch.Fk_Specification ?? 0) || specNumbers.Contains(individualMatch.Specification == null ? String.Empty : individualMatch.Specification.Number))
                                          && crNumbers.Contains(individualMatch.CRNumber)
                                          && revisionNumbers.Contains(individualMatch.Revision ?? 0))
                .ToList();
            matchingCombinations = matchingCombinations
                .Where(combinationMatch => crKeys.Any(x => (((x.SpecId == combinationMatch.Fk_Specification) || (x.SpecNumber == (combinationMatch.Specification == null ? String.Empty : combinationMatch.Specification.Number)))
                                                                    && (x.CrNumber == combinationMatch.CRNumber)
                                                                    && (x.Revision == (combinationMatch.Revision ?? 0)))))
                //If TSG Tdoc number is not define we return all CRs which matching to to the tuple Spec id, CR number, revision
                //Else ; if TSG TDOC number is define we return CR which matching exactly to the complete unique key
                .ToList();

            return matchingCombinations;
        }

        /// <summary>
        /// Gets the cr by key.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change Request</returns>
        public ChangeRequest GetCrByKey(CrKeyFacade crKey)
        {
            var query = AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus);
            query = query.Where(x => (((x.Fk_Specification ?? 0) == crKey.SpecId) || (x.Specification.Number == crKey.SpecNumber))
                                     && x.CRNumber == crKey.CrNumber
                                     && ((x.Revision ?? 0) == crKey.Revision));
            return query.FirstOrDefault();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change Request</returns>
        public List<ChangeRequest> GetCrsBySpecAndCrNumberAndTsgTdoc(CrKeyFacade crKey)
        {
            var query = AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus);
            query = query.Where(x => (((x.Fk_Specification ?? 0) == crKey.SpecId) || (x.Specification.Number == crKey.SpecNumber))
                                     && x.CRNumber == crKey.CrNumber
                                     && x.ChangeRequestTsgDatas.Any(y => y.TSGTdoc == crKey.TsgTdocNumber));
            return query.ToList();
        }
    }

    /// <summary>
    /// CR Repository interface to work with db related activities
    /// </summary>
    public interface IChangeRequestRepository : IEntityRepository<ChangeRequest>
    {
        
        /// <summary>
        /// Finds the by specification identifier.
        /// </summary>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns></returns>
        List<string> FindCrNumberBySpecificationId(int? specificationId);

        /// <summary>
        /// Return CR by contribution UID
        /// </summary>
        /// <param name="contributionUid">Contribution Uid</param>
        /// <returns>ChangeRequest entity</returns>
        ChangeRequest GetChangeRequestByContributionUID(string contributionUid);

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs"></param>
        /// <returns>List of CRs</returns>
        List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs);

        /// <summary>
        /// Find max CR revision by specId and CR number
        /// </summary>
        /// <param name="specificationId"></param>
        /// <param name="crNumber"></param>
        /// <returns></returns>
        int FindCrMaxRevisionBySpecificationIdAndCrNumber(int? specificationId, string crNumber);

        /// <summary>
        /// Finds the status by wgtdocument.
        /// </summary>
        /// <param name="wgTDoc">The wgtdocument.</param>
        /// <returns></returns>
        ChangeRequest FindByWgTDoc(string wgTDoc);

        /// <summary>
        /// Returns a list of change request given the specified criteria.
        /// </summary>
        /// <param name="searchObj">Cr search object</param>
        /// <returns>List of crs & count</returns>
        KeyValuePair<List<ChangeRequest>, int> GetChangeRequests(ChangeRequestsSearch searchObj);

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
        /// <returns>Change Request</returns>
        ChangeRequest GetCrByKey(CrKeyFacade crKey);

        /// <summary>
        /// Gets the crs by Spec, Cr number and Tsg Tdoc number. To be able to find all revisions of a CR.
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change Request</returns>
        List<ChangeRequest> GetCrsBySpecAndCrNumberAndTsgTdoc(CrKeyFacade crKey);
    }
}
