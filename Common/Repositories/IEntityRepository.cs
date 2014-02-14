using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// The IEntityRepository contains the basic methods that an Entity repository should implement.
    /// 
    /// Note that there is no save mecanism in this Repository: the IUnitOfWork should be in charge of
    /// the save.
    /// </summary>
    /// <typeparam name="T"> A Domain class</typeparam>
    public interface IEntityRepository<T> : IDisposable
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Gives access to all the entities in the repository. The loading of the entities is 
        /// however lazy.
        /// </summary>
        IQueryable<T> All { get; }

        /// <summary>
        /// Gives access to all the entities in the repository. Additionally provides a way to include
        /// some fields as Eager Loading to improve performances in loop queries.
        /// </summary>
        /// <param name="includeProperties">The property to load in the query.</param>
        /// <returns> The query that can be executed.</returns>
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Enables to retrieve a given Entity in the Repository
        /// </summary>
        /// <param name="id">the entity Id</param>
        /// <returns>The entity if found, null if no entity can be found.</returns>
        T Find(int id);

        /// <summary>
        /// Enables to insert or update an entity in the repository. System will add the entity is it 
        /// has no Id, else update it.
        /// 
        /// Caution: the system will not automatically modify the linked objects. Use 
        /// dedicated repositories to add/modify these objects.
        /// (This behaviour has been put into place in order to limit the risk of side effects)
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        void InsertOrUpdate(T entity);

        /// <summary>
        /// Deletes the entity with provided id. Seeks for the entity if the entity is not already in the context.
        /// </summary>
        /// <param name="id">the id of the entity to delete.</param>
        void Delete(int id);

     
    }
}
