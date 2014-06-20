using SyncInterface;

namespace SyncService
{
    /// <summary>
    /// SyncService to apply offline changes to online server
    /// </summary>
    public class SyncService : ISyncService
    {
        #region ISyncService Members

        /// <summary>
        /// Insert record
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Primary Key of inserted entity</returns>
        public int InsertRecord(object entity, EnumEntity entityType)
        {
            return ServiceHelper.InsertEntity(entity, entityType);
        }

        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateRecord(object entity, EnumEntity entityType)
        {
            return ServiceHelper.UpdateEntity(entity, entityType);
        }

        /// <summary>
        /// Delete record
        /// </summary>
        /// <param name="primaryKeyID">Primary Key of entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        public bool DeleteRecord(int primaryKeyID, EnumEntity entityType)
        {
            return ServiceHelper.DeleteEntity(primaryKeyID, entityType);
        }

        #endregion
    }
}
