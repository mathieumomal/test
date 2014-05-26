using System.Collections.Generic;

namespace OfflineSetup.Entities
{
    public class TrackingTable
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string PrimaryKey { get; set; }
        public List<string> Columns { get; set; }
        public List<TrackingForeignKey> ForeignKeys { get; set; }

        public TrackingTable()
        {
            this.Columns = new List<string>();
            this.ForeignKeys = new List<TrackingForeignKey>();
        }
    }
}
