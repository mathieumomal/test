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
                    remark = this.Remarks.OrderByDescending(x => x.CreationDate).FirstOrDefault().RemarkText;
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

        public string MtgShortRef { get; set; }

        /// <summary>
        /// Get Reference and date of a transposed version
        /// </summary>
        public string TranspositionReferenceAndDate {
            get
            {
                var val = new StringBuilder();
                if(this.ETSI_WKI_ID != null && this.ETSI_WKI_ID != 0)
                {
                    if (!String.IsNullOrEmpty(this.ETSI_WKI_Ref))
                    {
                        val.Append(this.ETSI_WKI_Ref);
                    }
                    if (this.DocumentPassedToPub != null)
                    {
                        val
                            .Append("\nTransposed on : ")
                            .Append(this.DocumentPassedToPub);
                    }
                }
                return val.ToString();
            }
        }
    }
}