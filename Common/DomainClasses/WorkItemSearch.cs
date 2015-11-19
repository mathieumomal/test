using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    /// <summary>
    /// Helper class to carry search values
    /// </summary>
    [Serializable]
    public class WorkItemSearch
    {
        public WorkItemSearch()
        {
            //initialize list to default value
            this.SelectedReleaseIds = new List<int>();
        }

        public string NameUID { get; set; }
        public string Acronym { get; set; }
        public bool HideCompletedItems { get; set; }
        public string GranularityId { get; set; }
        public List<int> SelectedReleaseIds { get; set; }
    }
}
