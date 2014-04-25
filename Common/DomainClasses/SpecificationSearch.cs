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
    public class SpecificationSearch
    {

        public SpecificationSearch()
        {
            //initialize list to default value
            this.SelectedCommunityIds = new List<int>();
            this.SelectedReleaseIds = new List<int>();
            this.Technologies = new List<int>();
            this.Series = new List<int>();
        }

        public string Title { get; set; }
        public List<int> Series { get; set; }
        public bool? Type { get; set; }
        public bool NumberNotYetAllocated { get; set; }
        public List<int> SelectedCommunityIds { get; set; }

        public List<int> SelectedReleaseIds { get; set;}
        public bool? IsForPublication { get; set; }
        public List<int> Technologies { get; set; }
        public bool IsDraft { get; set; }
        public bool IsUnderCC { get; set; }
        public bool IsWithACC { get; set; }
        public bool IsWithBCC { get; set; }
    }
}
