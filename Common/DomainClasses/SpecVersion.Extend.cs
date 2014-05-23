using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class SpecVersion
    {
        public string LatestRemark
        {
            get
            {
                string remark = string.Empty;
                if (Remarks.Count > 0)
                    remark = this.Remarks.OrderBy(x => x.CreationDate).FirstOrDefault().RemarkText;
                return remark;
            }
        }


        public string Version
        {
            get
            {
                string version = string.Empty;
                version = string.Format("{0}.{1}.{2}", (this.MajorVersion ?? default(int)).ToString(),
                                                       (this.TechnicalVersion ?? default(int)).ToString(),
                                                       (this.EditorialVersion ?? default(int)).ToString());
                return version;
            }
        }
    }
}