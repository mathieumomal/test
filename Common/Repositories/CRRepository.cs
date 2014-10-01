﻿using Etsi.Ultimate.DomainClasses;
using System.Linq;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// CR Repository to interact database for CR related activities
    /// </summary>
    public class CRRepository : ICRRepository
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
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Change request entity</returns>
        public ChangeRequest Find(int id)
        {
            throw new System.NotImplementedException();
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
    public interface ICRRepository : IEntityRepository<ChangeRequest>
    {

    }
}