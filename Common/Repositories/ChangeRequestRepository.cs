using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

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
                query = query.Include(includeProperty);
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
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).SingleOrDefault(c => c.TSGTDoc.Equals(contributionUid) || c.WGTDoc.Equals(contributionUid));
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUiDs">Contribution Uid list</param>
        /// <returns>List of CRs</returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUiDs)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).Where(x => contributionUiDs.Contains(x.TSGTDoc) || contributionUiDs.Contains(x.WGTDoc)).ToList();
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="changeRequestId"></param>
        /// <returns>Change request entity</returns>
        public ChangeRequest Find(int changeRequestId)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).SingleOrDefault(x => x.Pk_ChangeRequest == changeRequestId);
        }

        /// <summary>
        /// Inserts or update the change request entity
        /// </summary>
        /// <param name="entity">Change request entity.</param>
        public void InsertOrUpdate(ChangeRequest entity)
        {
            UoW.Context.SetAdded(entity);
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
            return UoW.Context.ChangeRequests.Where(wg => wg.WGTDoc == wgTDoc).SingleOrDefault();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="searchObj"></param>
        /// <returns></returns>
        public List<ChangeRequest> GetChangeRequests(ChangeRequestsSearch searchObj)
        {
            return UoW.Context.ChangeRequests.Skip(20).Take(100).ToList();
        }
    }

    /// <summary>
    /// CR Repository interface to work with db related activities
    /// </summary>
    public interface IChangeRequestRepository : IEntityRepository<ChangeRequest>
    {
        /// <summary>
        /// Gets or sets the uo w.
        /// </summary>
        /// <value>
        /// The uo w.
        /// </value>
        IUltimateUnitOfWork UoW { get; set; }

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
        /// <param name="searchObj"></param>
        /// <returns></returns>
        List<ChangeRequest> GetChangeRequests(ChangeRequestsSearch searchObj);
    }
}
