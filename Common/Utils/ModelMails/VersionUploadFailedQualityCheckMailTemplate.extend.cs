using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Utils.ModelMails
{
    public partial class VersionUploadFailedQualityCheckMailTemplate
    {
        public String UserName { get; set; }
        public String SpecNumber { get; set; }
        public String VersionNumber { get; set; }
        public List<String> WarningList { get; set; }

        public VersionUploadFailedQualityCheckMailTemplate(String UserName, String SpecNumber, String VersionNumber, List<String> WarningList)
        {
            this.UserName = UserName;
            this.SpecNumber = SpecNumber;
            this.VersionNumber = VersionNumber;
            this.WarningList = WarningList;
        }
    }
}
