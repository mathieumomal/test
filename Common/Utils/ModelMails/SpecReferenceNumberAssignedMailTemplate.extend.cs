using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Utils.ModelMails
{
    public partial class SpecReferenceNumberAssignedMailTemplate
    {
        public String Recipient { get; set; }
        public String SpecNumber { get; set; }
        public String SpecTitle { get; set; }
        public List<String> WorkItems { get; set; }

        public SpecReferenceNumberAssignedMailTemplate(String recipient, String specNumber, String specTitle, List<String> workItems)
        {
            this.Recipient = recipient;
            this.SpecNumber = specNumber;
            this.SpecTitle = SpecTitle;
            this.WorkItems = workItems;
        }
    }
}
