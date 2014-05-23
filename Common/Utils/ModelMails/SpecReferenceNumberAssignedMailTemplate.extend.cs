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

        public SpecReferenceNumberAssignedMailTemplate(String Recipient, String SpecNumber, String SpecTitle, List<String> WorkItems)
        {
            this.Recipient = Recipient;
            this.SpecNumber = SpecNumber;
            this.SpecTitle = SpecTitle;
            this.WorkItems = WorkItems;
        }
    }
}
