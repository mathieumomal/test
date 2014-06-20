
namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// Interface - Offline Services
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public interface IOfflineService<T>
    {
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Primary Key of Inserted Entity</returns>
        int InsertEntity(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Success/Failure</returns>
        bool UpdateEntity(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="primaryKey">Primary Key ID</param>
        /// <returns>Success/Failure</returns>
        bool DeleteEntity(int primaryKey);
    }
}
