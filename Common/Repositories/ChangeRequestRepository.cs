using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;
using System.Linq;

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
            throw new System.NotImplementedException();
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
        /// <param name="contributionUID"></param>
        /// <returns></returns>
        public ChangeRequest GetChangeRequestByContributionUID(string contributionUID)
        {            
            var result = UoW.Context.ChangeRequests.SingleOrDefault(c => (c.TSGTDoc.Equals(contributionUID) || c.WGTDoc.Equals(contributionUID)) );
            return result;
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Change request entity</returns>
        public ChangeRequest Find(int changeRequestId)
        {
            return UoW.Context.ChangeRequests.Find(changeRequestId);
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
        ChangeRequest GetChangeRequestByContributionUID(string contributionUID);
    }
}
