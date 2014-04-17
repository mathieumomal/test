using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Utils;

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

        public string RapporteurDetailsAddress
        {
            get
            {
                return new StringBuilder()
                    .Append("<a href='")
                    .Append(ConfigVariables.RapporteurDetailsAddress)
                    .Append(this.PERSON_ID.ToString())
                    .Append("'>")
                    .Append(this.FIRSTNAME)
                    .Append(" ")
                    .Append(this.LASTNAME)
                    .Append("</a>")
                    .ToString();
            }
        }
    }
}
