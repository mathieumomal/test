
namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Offline Repository class to update online context with offline changes
    /// </summary>
    public class OfflineRepository : IOfflineRepository
    {
        #region Properties

        /// <summary>
        /// Unit of Work
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region IOfflineRepository Members

        /// <summary>
        /// Insert offline entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void InsertOfflineEntity(object entity)
        {
            UoW.Context.SetAdded(entity);
        }

        /// <summary>
        /// Update offline entity
        /// </summary>
        /// <param name="entity">entity</param>
        public void UpdateOfflineEntity(object entity)
        {
            UoW.Context.SetModified(entity);
        }

        /// <summary>
        /// Delete offline entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void DeleteOfflineEntity(object entity)
        {
            UoW.Context.SetDeleted(entity);
        }

        #endregion
    }

    /// <summary>
    /// Offline Repository interface to update online context with offline changes
    /// </summary>
    public interface IOfflineRepository
    {
        /// <summary>
        /// Unit of Work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Insert offline entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void InsertOfflineEntity(object entity);

        /// <summary>
        /// Update offline entity
        /// </summary>
        /// <param name="entity">entity</param>
        void UpdateOfflineEntity(object entity);

        /// <summary>
        /// Delete offline entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void DeleteOfflineEntity(object entity);
    }
}
