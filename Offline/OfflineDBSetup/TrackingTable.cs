using System.Collections.Generic;

namespace OfflineDBSetup
{
    /// <summary>
    /// Table entity
    /// </summary>
    public class TrackingTable
    {
        /// <summary>
        /// Schema
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Primary Key
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// List of Columns
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// List of Foreign Keys
        /// </summary>
        public List<TrackingForeignKey> ForeignKeys { get; set; }

        /// <summary>
        /// Constructor to assin columns & foreign keys
        /// </summary>
        public TrackingTable()
        {
            this.Columns = new List<string>();
            this.ForeignKeys = new List<TrackingForeignKey>();
        }
    }
}
