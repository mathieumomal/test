using SyncInterface.DataContract;
using System.ServiceModel;

namespace SyncInterface
{
    /// <summary>
    /// SyncService interface to update offline changes
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(SpecVersion))]
    public interface ISyncService
    {
        /// <summary>
        /// Insert record
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Primary Key of inserted entity</returns>
        [OperationContract]
        int InsertRecord(object entity, EnumEntity entityType, string terminalName);

        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        bool UpdateRecord(object entity, EnumEntity entityType);

        /// <summary>
        /// Delete record
        /// </summary>
        /// <param name="primaryKeyID">Primary Key of entity</param>
        /// <param name="entityType">Entity Type</param>
        /// <returns>Success/Failure</returns>
        [OperationContract]
        bool DeleteRecord(int primaryKeyID, EnumEntity entityType);
    }
}
