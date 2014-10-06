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
        /// Return CR by contribution UID
        /// </summary>
        /// <param name="contributionUID">Contribution Uid</param>
        /// <returns>ChangeRequest entity</returns>
        public ChangeRequest GetChangeRequestByContributionUID(string contributionUID)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).SingleOrDefault(c => c.TSGTDoc.Equals(contributionUID) || c.WGTDoc.Equals(contributionUID));
        }

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUIDs">Contribution Uid list</param>
        /// <returns>List of CRs</returns>
        public List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUIDs)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).Where(x => contributionUIDs.Contains(x.TSGTDoc) || contributionUIDs.Contains(x.WGTDoc)).ToList();
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Change request entity</returns>
        public ChangeRequest Find(int changeRequestId)
        {
            return AllIncluding(t => t.Enum_CRCategory, t => t.Specification, t => t.Release, t => t.CurrentVersion, t => t.NewVersion, t => t.TsgStatus, t => t.WgStatus).SingleOrDefault( x => x.Pk_ChangeRequest == changeRequestId);
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
        /// <param name="contributionUID">Contribution Uid</param>
        /// <returns>ChangeRequest entity</returns>
        ChangeRequest GetChangeRequestByContributionUID(string contributionUID);

        /// <summary>
        /// Returns list of CRs using list of contribution UIDs. 
        /// </summary>
        /// <param name="contributionUIDs">Contribution Uid list</param>
        /// <returns>List of CRs</returns>
        List<ChangeRequest> GetChangeRequestListByContributionUidList(List<string> contributionUids);
    }
}
