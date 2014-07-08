using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Repository class for SyncInfo entity
    /// </summary>
    public class SyncInfoRepository : ISyncInfoRepository
    {
        #region Properties

        /// <summary>
        /// Ultimate UnitOfWork
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region IEntityRepository<SyncInfo> Membres

        /// <summary>
        /// Give access to SyncInfo entities
        /// </summary>
        public IQueryable<SyncInfo> All
        {
            get { 
                return UoW.Context.SyncInfoes;
            }
        }

        /// <summary>
        /// Give access to SyncInfo entities. Additionally provides a way to include
        /// some fields as Eager Loading to improve performances in loop queries. 
        /// </summary>
        /// <param name="includeProperties">The property to load in the query</param>
        /// <returns>The query that can be executed</returns>
        public IQueryable<SyncInfo> AllIncluding(params System.Linq.Expressions.Expression<Func<SyncInfo, object>>[] includeProperties)
        {
            IQueryable<SyncInfo> query = UoW.Context.SyncInfoes;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        /// <summary>
        /// Retrieve a SyncInfo object
        /// </summary>
        /// <param name="id">Primary Key</param>
        /// <returns>If found entity, otherwise null</returns>
        public SyncInfo Find(int id)
        {
            return UoW.Context.SyncInfoes.Find(id);
        }

        /// <summary>
        /// Enables to insert or update an SyncInfo in the repository. System will add the entity is it has no Id, else update it.
        /// </summary>
        /// <param name="entity">The entity to add or update</param>
        public void InsertOrUpdate(SyncInfo entity)
        {
            throw new InvalidOperationException("Cannot add or update a SyncInfo entity");
        }

        /// <summary>
        /// Deletes the entity with provided id. Seeks for the entity if the entity is not already in the context.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete SyncInfo entity");
        }

        #endregion        

        #region ISyncInfoRepository Membres

        /// <summary>
        /// Retrieve SyncInfo objects for the given criteria
        /// </summary>
        /// <param name="terminalName">Terminal Name</param>
        /// <param name="offline_PkID">Offline Primary Key</param>
        /// <returns>List of SyncInfo objects</returns>
        public List<SyncInfo> Find(string terminalName, int offline_PkID)
        {
            return UoW.Context.SyncInfoes.Where(x => x.TerminalName == terminalName && x.Offline_PK_Id == offline_PkID).ToList();
        }

        #endregion

        #region IDisposable Membres

        /// <summary>
        /// Dispose unit of work object
        /// </summary>
        public void Dispose(){}

        #endregion
    }

    /// <summary>
    /// Interface to implement find method
    /// </summary>
    public interface ISyncInfoRepository : IEntityRepository<SyncInfo> 
    {
        /// <summary>
        /// Retrieve SyncInfo objects for the given criteria
        /// </summary>
        /// <param name="terminalName">Terminal Name</param>
        /// <param name="offline_PkID">Offline Primary Key</param>
        /// <returns>List of SyncInfo objects</returns>
        List<SyncInfo> Find(string terminalName, int offline_PkID);
    }
}
