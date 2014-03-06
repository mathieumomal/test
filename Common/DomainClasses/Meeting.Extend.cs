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
        /// Converts a short reference (example: S1-56) into a full reference (SA1#56)
        /// </summary>
        /// <param name="shortRef"></param>
        /// <returns></returns>
        public static string ToFullReference(string shortRef)
        {
            if (!shortRef.Contains('-'))
                throw new FormatException("Meeting short reference must contain a dash");

            // Split the meeting into two parts
            var mtgParts = shortRef.Split('-') ;
            var commitee = mtgParts[0];
            var mtgNumber = mtgParts[1]; 

            string fullCommittee ="";
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
                        throw new FormatException("Invalid input for meeting: "+shortRef+". Could not determine commitee");
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

            return "3GPP"+fullCommittee + "#" + mtgNumber;
        }

    }
}
