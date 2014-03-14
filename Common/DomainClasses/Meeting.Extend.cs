using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class Meeting
    {
        /// <summary>
        /// Converts a short reference (example: S1-56) into a full reference (3GPPSA1#56)
        /// </summary>
        /// <param name="shortRef"></param>
        /// <returns></returns>
        public static string ToFullReference(string shortRef)
        {
            if (!shortRef.Contains('-'))
                throw new FormatException("Meeting short reference must contain a dash");

            // Split the meeting into two parts
            var mtgParts = shortRef.Split('-');
            var commitee = mtgParts[0];
            var mtgNumber = mtgParts[1];

            string fullCommittee = "";
            // Let's check the first part
            if (commitee.Substring(commitee.Length - 1) == "P")
            {
                switch (commitee.Substring(0, 1).ToUpper())
                {
                    case "S":
                        fullCommittee = "SA";
                        break;
                    case "C":
                        fullCommittee = "CT";
                        break;
                    case "N":
                        fullCommittee = "CN";
                        break;
                    case "G":
                        fullCommittee = "GERAN";
                        break;
                    case "R":
                        fullCommittee = "RAN";
                        break;
                    case "T":
                        fullCommittee = "T";
                        break;
                    default:
                        throw new FormatException("Invalid input for meeting: " + shortRef + ". Could not determine commitee");
                }
            }
            else if (commitee.ToUpper() == "SMG")
            {
                fullCommittee = "SMG";
            }
            else if (commitee.ToUpper() == "GSM")
            {
                fullCommittee = "GSM";
            }
            else if (commitee.ToUpper() == "PCG")
            {
                fullCommittee = "PCG";
            }
            else
            {
                throw new NotImplementedException("Subgroups are not handled yet");
            }

            return "3GPP" + fullCommittee + "#" + mtgNumber;
        }

        /// <summary>
        /// Converts a full reference (SA1#56) into a short reference (example: S1-56)
        /// </summary>
        /// <param name="fullRef"></param>
        /// <returns></returns>
        public static string ToShortReference(string fullRef)
        {
            if (!fullRef.Contains('#'))
                throw new FormatException("Meeting full reference must contain a hash(#)");

            fullRef = fullRef.Replace("3GPP", "");
            // Split the meeting into two parts
            var mtgParts = fullRef.Split('#');
            var fullCommittee = mtgParts[0];
            var mtgNumber = mtgParts[1];

            string committee = "";
            // Let's check the first part

            switch (fullCommittee.Trim().ToUpper())
            {
                case "SA":
                    committee = "S" + "P";
                    break;
                case "CT":
                    committee = "C" + "P";
                    break;
                case "CN":
                    committee = "N" + "P";
                    break;
                case "GERAN":
                    committee = "G" + "P";
                    break;
                case "RAN":
                    committee = "R" + "P";
                    break;
                case "T":
                    committee = "T" + "P";
                    break;
                case "SMG":
                    committee = "SMG";
                    break;
                case "GSM":
                    committee = "GSM";
                    break;
                case "PCG":
                    committee = "PCG";
                    break;
                default:
                    throw new FormatException("Invalid input for meeting: " + fullRef + ". Could not determine commitee");
            }

            return committee + "-" + mtgNumber;
        }

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
