using Etsi.Ultimate.DomainClasses;
using System;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Repository class for Remark entity
    /// </summary>
    public class RemarkRepository : IRemarkRepository
    {
        #region Properties

        /// <summary>
        /// Ultimate UnitOfWork
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region IEntityRepository<Remark> Membres

        /// <summary>
        /// Give access to Remark entities
        /// </summary>
        public IQueryable<Remark> All
        {
            get
            {
                return UoW.Context.Remarks;
            }
        }

        /// <summary>
        /// Give access to Remark entities. Additionally provides a way to include
        /// some fields as Eager Loading to improve performances in loop queries. 
        /// </summary>
        /// <param name="includeProperties">The property to load in the query</param>
        /// <returns>The query that can be executed</returns>
        public IQueryable<Remark> AllIncluding(params System.Linq.Expressions.Expression<Func<Remark, object>>[] includeProperties)
        {
            IQueryable<Remark> query = UoW.Context.Remarks;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        /// <summary>
        /// Retrieve a Remark object
        /// </summary>
        /// <param name="id">Primary Key</param>
        /// <returns>If found entity, otherwise null</returns>
        public Remark Find(int id)
        {
            return UoW.Context.Remarks.Find(id);
        }

        /// <summary>
        /// Enables to insert or update an Remark in the repository. System will add the entity is it has no Id, else update it.
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        public void InsertOrUpdate(Remark entity)
        {
            throw new InvalidOperationException("Cannot add or update a Remark entity as its own");
        }

        /// <summary>
        /// Deletes the entity with provided id. Seeks for the entity if the entity is not already in the context.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Remark entity as its own");
        }

        #endregion

        #region IDisposable Membres

        /// <summary>
        /// Dispose unit of work object
        /// </summary>
        public void Dispose() { }

        #endregion
    }

    /// <summary>
    /// Interface to implement additional methods for Remark repository
    /// </summary>
    public interface IRemarkRepository : IEntityRepository<Remark>
    {
    }
}
