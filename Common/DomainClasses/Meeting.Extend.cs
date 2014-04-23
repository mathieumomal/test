using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class Meeting
    {

        public string MtgDdlText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.MtgShortRef);
                if (this.START_DATE != null)
                    sb.Append(" (" + this.START_DATE.Value.ToString("yyyy-MM-dd"));
                if (!String.IsNullOrEmpty(this.LOC_CITY))
                    sb.Append(" - " + this.LOC_CITY);
                if (this.LOC_CTY_CODE != null)
                    sb.Append("(" + this.LOC_CTY_CODE + ")");
                if (this.START_DATE != null)
                    sb.Append(")");
                return sb.ToString();
            }
        }

        public string MtgDdlValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(this.MTG_ID);
                if (this.END_DATE != null)
                    sb.Append("|" + this.END_DATE.Value.ToString("yyyy-MM-dd"));
                else
                    sb.Append("|-");
                return sb.ToString();
            }
        }
    }
}
