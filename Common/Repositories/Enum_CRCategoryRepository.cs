using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Repositories
{
    public class Enum_CRCategoryRepository : IEnum_CRCategoryRepository
    {

        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get all change request details.
        /// </summary>
        public IQueryable<Enum_CRCategory> All
        {
            get
            {
                return UoW.Context.Enum_CRCategory;
            }
        }

        /// <summary>
        /// Get all change request details including the related entities which are provided
        /// </summary>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>Change request details query</returns>
        public IQueryable<Enum_CRCategory> AllIncluding(params System.Linq.Expressions.Expression<System.Func<Enum_CRCategory, object>>[] includeProperties)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Finds the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Change request entity</returns>
        public Enum_CRCategory Find(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Inserts or update the change request entity
        /// </summary>
        /// <param name="entity">Change request entity.</param>
        public void InsertOrUpdate(Enum_CRCategory entity)
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
    /// 
    /// </summary>
    public interface IEnum_CRCategoryRepository : IEntityRepository<Enum_CRCategory>
    {
      
    }
}
