using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    [Serializable]
    public class ChangeRequestsSearch
    {
        public ChangeRequestsSearch()
        {
            MeetingIds = new List<int>();
            WorkItemIds = new List<int>();
            ReleaseIds = new List<int>();
            WgStatusIds = new List<int>();
            TsgStatusIds = new List<int>();
        }

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

        /// <summary>
        /// Gets or sets the meeting ids.
        /// </summary>
        public List<int> MeetingIds { get; set; }

        /// <summary>
        /// Gets or sets the work item ids.
        /// </summary>
        public List<int> WorkItemIds { get; set; }

        /// <summary>
        /// Gets or sets the release ids.
        /// </summary>
        public List<int> ReleaseIds { get; set; }

        /// <summary>
        /// Gets or sets the wg status ids.
        /// </summary>
        public List<int> WgStatusIds { get; set; }

        /// <summary>
        /// Gets or sets the TSG status ids.
        /// </summary>
        public List<int> TsgStatusIds { get; set; }

        /// <summary>
        /// Gets or sets Spec version id
        /// </summary>
        public int VersionId { get; set; }
    }
}
