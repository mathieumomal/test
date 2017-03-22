using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// CR Repository to interact database for CR related activities
    /// PERFORMANCE IMPROVMENTS : to improve performance two requests are send potentially to find a CR at WG or TSG level.
    /// Actually, if we are trying to manage LEFT join with where clause with OR statement the request and 20 time more long... 
    /// For more information please see UpdateCrStatus method
    /// </summary>
    public class ChangeRequestRepository : IChangeRequestRepository
    {
        private int _limitOfTdocsPerRequest;
        public int LimitOfTdocsPerRequest
        {
            get
            {
                if (_limitOfTdocsPerRequest == 0)
                    return ConfigVariables.LimitOfTdocsToSearchPerRequest;
                return _limitOfTdocsPerRequest;
            }
            set { _limitOfTdocsPerRequest = value; }
        }

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
        public IQueryable<ChangeRequest> AllIncluding(params System.Linq.Expressions.Expression<Func<ChangeRequest, object>>[] includeProperties)
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
            //Search for WG CR
            var wgCrFound = (from cr in UoW.Context.ChangeRequests
                         where cr.WGTDoc == contributionUid
                         select cr)
                .Include(x => x.ChangeRequestTsgDatas)
                .Include(x => x.Specification)
                .Include(x => x.ChangeRequestTsgDatas)
                .Include(x => x.Enum_CRCategory)
                .Include(x => x.Release)
                .Include(x => x.CurrentVersion)
                .Include(x => x.NewVersion)
                .Include(x => x.WgStatus)
                .Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus))
                .Distinct().SingleOrDefault();

            if (wgCrFound != null)
                return wgCrFound;

            //Search for TSG CR
            var tsgCrFound = (from cr in UoW.Context.ChangeRequests
                            join cre in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals cre.Fk_ChangeRequest
                            where cre.TSGTdoc == contributionUid
                            select cr)
            .Include(x => x.Specification)
            .Include(x => x.ChangeRequestTsgDatas)
            .Include(x => x.Enum_CRCategory)
            .Include(x => x.Release)
            .Include(x => x.CurrentVersion)
            .Include(x => x.NewVersion)
            .Include(x => x.WgStatus)
            .Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus))
            .Distinct().SingleOrDefault();

            return tsgCrFound;
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// Performance improvment : to avoid any timeout, we split request in multiple requests each LimitOfTdocsPerRequest (integer) tdocs
        /// </summary>
        /// <param name="contributionUiDs">Contribution Uid list</param>
        /// <returns>List of CRs</returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs)
        {
            var crs = new List<ChangeRequest>();

            //Total count of crs that we are looking for
            var totalCount = contributionUiDs.Count;
            //Get required number of iterations according to the LimitOfTdocsPerRequest variable
            var iterations = Math.Ceiling((double)totalCount / LimitOfTdocsPerRequest);
            //For each iterations : execute request to get crs in DB
            for (var i = 0; i < iterations; i++)
            {
                //Define subUids list for current iteration
                var subUids = contributionUiDs.Skip(i * LimitOfTdocsPerRequest).Take(LimitOfTdocsPerRequest).ToList();
                //Execute request and add tdocs to the final list
                //Search for WG CR
                var queryWgCrFound = (from cr in UoW.Context.ChangeRequests
                         where subUids.Contains(cr.WGTDoc)
                         select cr)
                    .Include(x => x.Specification)
                    .Include(x => x.ChangeRequestTsgDatas)
                    .Include(x => x.Enum_CRCategory)
                    .Include(x => x.CR_WorkItems)
                    .Include(x => x.Release)
                    .Include(x => x.CurrentVersion)
                    .Include(x => x.NewVersion)
                    .Include(x => x.WgStatus)
                    .Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus));

                crs.AddRange(queryWgCrFound.ToList());

                //Search for TSG CR
                var queryTsgCrFound = (from cr in UoW.Context.ChangeRequests
                             join cre in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals cre.Fk_ChangeRequest
                    where subUids.Contains(cre.TSGTdoc)
                    select cr)
                    .Include(x => x.Specification)
                    .Include(x => x.ChangeRequestTsgDatas)
                    .Include(x => x.Enum_CRCategory)
                    .Include(x => x.CR_WorkItems)
                    .Include(x => x.Release)
                    .Include(x => x.CurrentVersion)
                    .Include(x => x.NewVersion)
                    .Include(x => x.WgStatus)
                    .Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus));

                crs.AddRange(queryTsgCrFound.ToList());
            }

            return crs.Distinct().ToList();
        }

        public List<ChangeRequest> GetWgCrsByWgTdocList(List<string> contribUids)
        {
            var crs = new List<ChangeRequest>();

            //Total count of crs that we are looking for
            var totalCount = contribUids.Count;
            //Get required number of iterations according to the LimitOfTdocsPerRequest variable
            var iterations = Math.Ceiling((double)totalCount / LimitOfTdocsPerRequest);
            //For each iterations : execute request to get crs in DB
            for (var i = 0; i < iterations; i++)
            {
                //Define subUids list for current iteration
                var subUids = contribUids.Skip(i * LimitOfTdocsPerRequest).Take(LimitOfTdocsPerRequest).ToList();
                //Execute request and add tdocs to the final list
                //Search for WG CR
                var queryWgCrFound = (from cr in UoW.Context.ChangeRequests
                                      where subUids.Contains(cr.WGTDoc)
                                      select cr)
                    .Include(x => x.Specification)
                    .Include(x => x.ChangeRequestTsgDatas)
                    .Include(x => x.Enum_CRCategory)
                    .Include(x => x.CR_WorkItems)
                    .Include(x => x.Release)
                    .Include(x => x.CurrentVersion)
                    .Include(x => x.NewVersion)
                    .Include(x => x.WgStatus)
                    .Include(t => t.ChangeRequestTsgDatas.Select(x => x.TsgStatus));

                crs.AddRange(queryWgCrFound.ToList());
            }

            return crs.Distinct().ToList();
        }

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects (except spec) because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Change request</returns>
        public ChangeRequest GetLightChangeRequestForMinuteMan(string uid)
        {
            //Search for CR at WG level
            var crWgFound = (from cr in UoW.Context.ChangeRequests
                             where cr.WGTDoc == uid
                             select cr)
                             .Include(x => x.Specification)
                             .Include(x => x.ChangeRequestTsgDatas)
                             .FirstOrDefault();

            if (crWgFound != null)
                return crWgFound;

            //Search for CR at TSG level
            var crTsgFound = (from cr in UoW.Context.ChangeRequests
                              join crtsg in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals crtsg.Fk_ChangeRequest
                              where crtsg.TSGTdoc == uid
                              select cr)
                              .Include(x => x.Specification)
                              .Include(x => x.ChangeRequestTsgDatas)
                              .FirstOrDefault();
            return crTsgFound;
        }

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        public List<ChangeRequest> GetLightChangeRequestsForMinuteMan(List<string> uids)
        {
            var crs = new List<ChangeRequest>();
            //Search for CR at WG level
            var crWgFound = (from cr in UoW.Context.ChangeRequests
                             where uids.Contains(cr.WGTDoc)
                             select cr)
                             .Include(x => x.Specification)
                             .Include(x => x.ChangeRequestTsgDatas)
                             .ToList();
            crs.AddRange(crWgFound);

            //Search for CR at TSG level
            var crTsgFound = (from cr in UoW.Context.ChangeRequests
                              join crtsg in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals crtsg.Fk_ChangeRequest
                              where uids.Contains(crtsg.TSGTdoc)
                              select cr)
                              .Include(x => x.Specification)
                              .Include(x => x.ChangeRequestTsgDatas)
                              .ToList();
            crs.AddRange(crTsgFound);

            return crs.Distinct().ToList();
        }

        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects (except spec) because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        public List<ChangeRequest> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid)
        {
            //Get CRs with related TSG data
            var crsFound = UoW.Context.ChangeRequests.Include(x => x.ChangeRequestTsgDatas)
                .Where(c => c.ChangeRequestTsgDatas.Any(x => uid == x.TSGTdoc)).ToList();

            //Get related specifications
            var crsSpecIds = crsFound.Select(x => x.Fk_Specification).Distinct().ToList();
            var specificationNumbers = UoW.Context.Specifications.Where(x => crsSpecIds.Contains(x.Pk_SpecificationId)).Select(
                x =>
                new
                {
                    x.Pk_SpecificationId,
                    x.Number
                }).ToList();
            crsFound.ForEach(cr =>
            {
                var spec = specificationNumbers.FirstOrDefault(x => x.Pk_SpecificationId == cr.Fk_Specification);
                cr.Specification = spec != null ? new Specification { Number = spec.Number } : new Specification();
            });

            return crsFound;
        }

        /// <summary>
        /// Same method than GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        public List<ChangeRequest> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids)
        {
            //Get CRs with related TSG data
            var crsFound = (from cr in UoW.Context.ChangeRequests
                join crtsg in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals crtsg.Fk_ChangeRequest
                where uids.Contains(crtsg.TSGTdoc)
                select cr).Include(x => x.ChangeRequestTsgDatas).ToList();

            //Get related specifications
            var crsSpecIds = crsFound.Select(x => x.Fk_Specification).Distinct().ToList();
            var specificationNumbers = UoW.Context.Specifications.Where(x => crsSpecIds.Contains(x.Pk_SpecificationId)).Select(
                x =>
                new
                {
                    x.Pk_SpecificationId,
                    x.Number
                }).ToList();
            crsFound.ForEach(cr =>
            {
                var spec = specificationNumbers.FirstOrDefault(x => x.Pk_SpecificationId == cr.Fk_Specification);
                cr.Specification = spec != null ? new Specification { Number = spec.Number } : new Specification();
            });

            return crsFound;
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
        /// See interface
        /// </summary>
        /// <param name="crsIds"></param>
        /// <returns></returns>
        public List<ChangeRequest> FindCrsByIds(List<int> crsIds)
        {
            return AllIncluding(x => x.ChangeRequestTsgDatas).Where(x => crsIds.Contains(x.Pk_ChangeRequest)).ToList();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
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
                                   && ((searchObj.TsgStatusIds.Count == 0) || (searchObj.TsgStatusIds.Contains(0) && x.ChangeRequestTsgDatas.Count == 0) || (x.ChangeRequestTsgDatas.Any(t => searchObj.TsgStatusIds.Contains(t.Fk_TsgStatus ?? 0))))
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
            var specIds = crKeys.Where(x => x.SpecId > 0).Select(y => y.SpecId).Distinct().ToList();
            var specNumbers = crKeys.Where(x => x.SpecNumber != null).Select(y => y.SpecNumber).Distinct().ToList();
            var crNumbers = crKeys.Select(x => x.CrNumber).Distinct().ToList();
            var revisionNumbers = crKeys.Select(x => x.Revision).Distinct().ToList();

            //Search CR to process : 
            //1) Search all CR which match with Keys values (Spec ID, CR number, Revision)
            //2) Inside this list search which one match to the exact key value of each one
            var matchingCombinations = AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus)
                .Where(individualMatch => (specIds.Contains(individualMatch.Fk_Specification ?? 0) || specNumbers.Contains(individualMatch.Specification.Number.Trim()))
                                          && crNumbers.Contains(individualMatch.CRNumber)
                                          && revisionNumbers.Contains(individualMatch.Revision ?? 0))
                .ToList();
            matchingCombinations = matchingCombinations
                .Where(combinationMatch => crKeys.Any(x => (((x.SpecId == combinationMatch.Fk_Specification) || (x.SpecNumber == combinationMatch.Specification.Number.Trim()))
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
            query = query.Where(x => (((crKey.SpecId != 0 && (x.Fk_Specification ?? 0) == crKey.SpecId)) || (x.Specification.Number == crKey.SpecNumber))
                                     && x.CRNumber == crKey.CrNumber
                                     && ((x.Revision ?? 0) == crKey.Revision));
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        public List<KeyValuePair<CrKeyFacade, string>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys)
        {
            var specNumbers = crKeys.Select(x => x.SpecNumber).ToList();
            var crNumbers = crKeys.Select(x => x.CrNumber).ToList();

            var query = (from cr in UoW.Context.ChangeRequests
                join spec in UoW.Context.Specifications on cr.Fk_Specification equals spec.Pk_SpecificationId
                where (cr.Revision == 0 || cr.Revision == null) && specNumbers.Contains(spec.Number) && crNumbers.Contains(cr.CRNumber)
                select cr);

            var parentCrsFound = query.ToList();

            return (from crKey in crKeys 
                    let parentCrFound = parentCrsFound.FirstOrDefault(x => x.CRNumber == crKey.CrNumber && x.Specification.Number == crKey.SpecNumber) 
                    where parentCrFound != null 
                    select 
                        new KeyValuePair<CrKeyFacade, string>(
                            new CrKeyFacade {CrNumber = crKey.CrNumber, SpecNumber = crKey.SpecNumber, Revision = 0}, 
                            parentCrFound.WGTDoc))
                    .ToList();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="crKey">The cr key.</param>
        /// <returns>Change Request</returns>
        public List<ChangeRequest> GetCrsBySpecAndCrNumberAndTsgTdoc(CrKeyFacade crKey)
        {
            var query = AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.WgStatus);
            query = query.Where(x => (((crKey.SpecId != 0 && (x.Fk_Specification ?? 0) == crKey.SpecId)) || (x.Specification.Number == crKey.SpecNumber))
                                     && x.CRNumber == crKey.CrNumber
                                     && x.ChangeRequestTsgDatas.Any(y => y.TSGTdoc == crKey.TsgTdocNumber));
            return query.ToList();
        }

        /// <summary>
        /// Get Tsg information for a given crPack
        /// </summary>
        /// <param name="crPack">Uid of Cr-Pack</param>
        /// <returns>Tsg information</returns>
        public List<ChangeRequestTsgData> GetTsgDataForCrPack(string crPack)
        {
            return UoW.Context.ChangeRequestTsgDatas.Where(x => x.TSGTdoc == crPack).ToList();
        }

        /// <summary>
        /// Update CR status. 
        /// Actually, this request seems strange. 
        /// The fact is that a CR (NOT a CR link to a CR-Pack) could be at WG level or TSG level.
        /// - CR at WG level : the uid is unique inside the ChangeRequest table
        /// - CR at TSG level : the uid is unique inside the ChangeRequestTsgData table
        /// Rule 1 : a CR UID cannot be found at WG level and at TSG level in the same time. 
        /// Rule 2 : a CR UID cannot be present multiple time on ChangeRequest table (same for ChangeRequestTsgData table)
        /// 
        /// So, the request has been transform to improve performance by searching first of all inside ChangeRequest table 
        /// and if the UID is not found as a WGTdoc number, the system will search inside ChangeRequestTsgData table (as a TSGTdoc number)
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pkStatus"></param>
        /// <returns></returns>
        public bool UpdateCrStatus(string uid, int? pkStatus)
        {
            //Search for CR at WG level
            var crWgFound = (from cr in UoW.Context.ChangeRequests
                where cr.WGTDoc == uid
                select cr).FirstOrDefault();

            if (crWgFound != null)
            {
                crWgFound.Fk_WGStatus = pkStatus;//Change status of WG CR
            }
            else
            {
                //Search for CR at TSG level
                var crTsgFound = (from cr in UoW.Context.ChangeRequests
                    join crtsg in UoW.Context.ChangeRequestTsgDatas on cr.Pk_ChangeRequest equals crtsg.Fk_ChangeRequest
                    where crtsg.TSGTdoc == uid
                    select cr).FirstOrDefault();

                if (crTsgFound == null)//If CR not found at WG and TSG level return false
                    return false;

                crTsgFound.ChangeRequestTsgDatas.First().Fk_TsgStatus = pkStatus;//Change status of TSG CR
            }
            return true;
        }

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="crsOfCrPack"></param>
        /// <returns></returns>
        public bool UpdateCrsStatusOfCrPack(List<CrOfCrPackFacade> crsOfCrPack)
        {
            foreach (var item in crsOfCrPack)
            {
                var cr = UoW.Context.ChangeRequests.FirstOrDefault(x => x.Specification.Pk_SpecificationId == item.SpecId
                                && x.CRNumber.ToUpper() == item.CrNumber.ToUpper()
                                    && (x.Revision ?? 0) == item.RevisionNumber);

                if (cr == null)
                    continue;

                var tsgCr = UoW.Context.ChangeRequestTsgDatas.FirstOrDefault(x => x.Fk_ChangeRequest == cr.Pk_ChangeRequest
                    && x.TSGTdoc.ToUpper() == item.TsgTdocNumber.ToUpper());

                if (tsgCr != null)
                {
                    tsgCr.Fk_TsgStatus = item.PkEnumStatus;
                }
            }
           
            return true;
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

        List<ChangeRequest> GetWgCrsByWgTdocList(List<string> contribUids);

        /// <summary>
        /// Get light change request for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR UID</param>
        /// <returns>Change request</returns>
        ChangeRequest GetLightChangeRequestForMinuteMan(string uid);

        /// <summary>
        /// Same method than GetLightChangeRequestForMinuteMan but for multiple CRs
        /// </summary>
        /// <param name="uids">CRs UIDs</param>
        /// <returns>List of Change requests</returns>
        List<ChangeRequest> GetLightChangeRequestsForMinuteMan(List<string> uids);

        /// <summary>
        /// Get light change requests inside CR packs for MinuteMan. Actually, for performance reason, MM no need to have all related objects because :
        /// - will not change during a meeting
        /// - and/or data will be loaded and cache by MM
        /// </summary>
        /// <param name="uid">CR pack UID</param>
        /// <returns>List of Change requests</returns>
        List<ChangeRequest> GetLightChangeRequestsInsideCrPackForMinuteMan(string uid);

        /// <summary>
        /// Same method than GetLightChangeRequestsInsideCrPackForMinuteMan but for multiple CR-Packs
        /// </summary>
        /// <param name="uids">List of CR-Pack uids</param>
        /// <returns>List of CRs inside CR-Packs</returns>
        List<ChangeRequest> GetLightChangeRequestsInsideCrPacksForMinuteMan(List<string> uids);

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

        /// <summary>
        /// Get Tsg information for a given crPack
        /// </summary>
        /// <param name="crPack">Uid of Cr-Pack</param>
        /// <returns>Tsg information</returns>
        List<ChangeRequestTsgData> GetTsgDataForCrPack(string crPack);

        /// <summary>
        /// Get CRs by Ids
        /// </summary>
        /// <param name="crsIds"></param>
        /// <returns></returns>
        List<ChangeRequest> FindCrsByIds(List<int> crsIds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="pkStatus"></param>
        /// <returns></returns>
        bool UpdateCrStatus(string uid, int? pkStatus);

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="crsOfCrPack"></param>
        /// <returns></returns>
        bool UpdateCrsStatusOfCrPack(List<CrOfCrPackFacade> crsOfCrPack);

        /// <summary>
        /// Find WgTdoc number of Crs which have been revised 
        /// Parent with revision 0 : WgTdoc = CP-1590204 -> have a WgTdoc number 
        /// ..
        /// Child with revision x : WgTdoc = ??? -> don't have WgTdoc number, we will find it thanks to its parent 
        /// </summary>
        /// <param name="crKeys">CrKeys with Specification number and CR number</param>
        /// <returns>List of CRKeys and related WgTdoc number</returns>
        List<KeyValuePair<CrKeyFacade, string>> GetCrWgTdocNumberOfParent(List<CrKeyFacade> crKeys);
    }
}
