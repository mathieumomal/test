using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Utils.ModelMails
{
    public partial class SpecAwaitingReferenceNumberMailTemplate
    {
        public String SecretaryName { get; set; }
        public String SpecTitle { get; set; }
        public String LinkToSpec { get; set; }

        public SpecAwaitingReferenceNumberMailTemplate(String secretaryName, String SpecTitle, String LinkToSpec)
        {
            this.SecretaryName = secretaryName;
            this.SpecTitle = SpecTitle;
            this.LinkToSpec = LinkToSpec;
        }
    }
}
