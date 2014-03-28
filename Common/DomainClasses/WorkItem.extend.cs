using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class WorkItem
    {
        public string UID
        {
            get
            {
                return (this.WiLevel == 0) ? string.Empty : this.Pk_WorkItemUid.ToString();
            }
        }
        public bool IsNew { get; set; }
        public DisplayStatus Display { get; set; }
        public string LatestRemark
        {
            get
            {
                var tmpRemarks = this.Remarks.FirstOrDefault();
                return (tmpRemarks != null) ? tmpRemarks.RemarkText : string.Empty;
            }
        }
        public string ShortLatestRemark
        {
            get
            {
                return (this.LatestRemark != null && this.LatestRemark.Length > 28) ? this.LatestRemark.Remove(28) + "..." : this.LatestRemark;
            }
        }
        public string ResponsibleGroups
        {
            get
            {
                return string.Join(",", this.WorkItems_ResponsibleGroups.Select(p => p.ResponsibleGroup));
            }
        }
        public string RapporteurName { set; get; }

        public enum DisplayStatus
        {
            none,
            include,
            matched
        }
    }
}
