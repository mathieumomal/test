using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class ChangeRequestsSearch
    {

        public enum ChangeRequestOrder {  };

        /// <summary>
        /// Specifies the order of the sort.
        /// </summary>
        public ChangeRequestOrder OrderBy { get; set; }

        /// <summary>
        /// Number of records to skip in the search. 
        /// Should be generally set to (Page - 1)*#Pagesize#
        /// </summary>
        public int SkipRecords { get; set; }

        /// <summary>
        /// Number of records to get. If = 0, take all. 
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the specification number.
        /// </summary>
        public string SpecificationNumber { get; set; }
    }
}
