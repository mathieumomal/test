using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class UtilsManager
    {
        /// <summary>
        /// Convert to Base36 string
        /// </summary>
        /// <param name="input">Base10 number</param>
        /// <returns>Base36 string</returns>
        public string EncodeToBase36Digits2(long input)
        {
            if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");
            var CharList = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] clistarr = CharList.ToCharArray();
            var result = new Stack<char>();
            if (input == 0)
                result.Push('0');
            else
            {
                while (input != 0)
                {
                    result.Push(clistarr[input % 36]);
                    input /= 36;
                }
            }
            var strResult = String.Empty;
            if (result.Count == 0)
                strResult = "00";
            else if (result.Count == 1)
                strResult = new StringBuilder().Append("0").Append(result.ToArray()).ToString();
            else
                strResult = new String(result.ToArray());
            return strResult;
        }
    }
}
