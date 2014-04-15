using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class View_Persons
    {
        public string PersonSearchTxt
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!String.IsNullOrEmpty(this.FIRSTNAME))
                    sb.Append(this.FIRSTNAME).Append(" ");
                if (!String.IsNullOrEmpty(this.ORGA_NAME))
                {
                    sb.Append("(");
                    sb.Append(this.ORGA_NAME);
                    sb.Append(") ");
                }
                if (!String.IsNullOrEmpty(this.Email))
                    sb.Append(this.Email);
                return sb.ToString();
            }
        }
    }
}
