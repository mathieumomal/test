using SyncInterface;
using System.Collections.Generic;

namespace SyncClient.DataHelpers
{
    /// <summary>
    /// Interface to fetch Offline data
    /// </summary>
    public interface IDataHelper
    {
        /// <summary>
        /// Get inserted data (offline)
        /// </summary>
        /// <returns>Inserted objects</returns>
        KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>> GetInsertData();

        /// <summary>
        /// Get updated data (offline)
        /// </summary>
        /// <returns>Updated objects</returns>
        KeyValuePair<EnumEntity, List<KeyValuePair<int, object>>> GetUpdateData();

        /// <summary>
        /// Get deleted data (offline)
        /// </summary>
        /// <returns>Deleted objects</returns>
        KeyValuePair<EnumEntity, List<KeyValuePair<int, int>>> GetDeleteData();
    }
}
