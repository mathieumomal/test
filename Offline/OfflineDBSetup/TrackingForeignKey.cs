
namespace OfflineDBSetup
{
    /// <summary>
    /// Foreignkey entity
    /// </summary>
    public class TrackingForeignKey
    {
        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName { get; set; }
    }
}
