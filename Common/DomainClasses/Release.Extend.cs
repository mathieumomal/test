using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public partial class Release
    {
        private const string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Version2gBase36{
            get{
                return (Version2g != null) ? Release.Encode((int)Version2g) : String.Empty;
            }
        }

        public string Version3gBase36{
            get{
                return (Version3g != null) ? Release.Encode((int)Version3g) : String.Empty;
            }
        }

        public static String Encode(int input)
        {            
            char[] baseElements = base36.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(baseElements[input % 36]);
                input /= 36;
            }
            return new string(result.ToArray());
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Shortname: {1}, Code: {2}", Pk_ReleaseId, ShortName, Code);
        }
    }
}
