using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public static class UtilsManager
    {
        private static string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Encodes the version to base36.
        /// </summary>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <returns>Base36 version</returns>
        public static string EncodeVersionToBase36(int? majorVersion, int? technicalVersion, int? editorialVersion)
        {
            var versionResult = String.Empty;

            var majorVersionString = EncodeToBase36Digits2(majorVersion ?? 0);
            var technicalVersionString = EncodeToBase36Digits2(technicalVersion ?? 0);
            var editorialVersionString = EncodeToBase36Digits2(editorialVersion ?? 0);

            //artf1549683: If all version numbers lessthan 36 then encode single digit base36 string
            if ((majorVersionString.Length == 2 && majorVersionString.StartsWith("0"))
                 && (technicalVersionString.Length == 2 && technicalVersionString.StartsWith("0")) 
                 && (editorialVersionString.Length == 2 && editorialVersionString.StartsWith("0")))
            {
                versionResult = String.Format("{0}{1}{2}", majorVersionString.Substring(1), technicalVersionString.Substring(1), editorialVersionString.Substring(1));
            }
            else //If any one version number having value more than 35 then encode two digit base36 string
            {
                versionResult = String.Format("{0}{1}{2}", majorVersionString, technicalVersionString, editorialVersionString);
            }

            return versionResult;
        }

        /// <summary>
        /// Convert to Base36 string
        /// </summary>
        /// <param name="input">Base10 number</param>
        /// <returns>Base36 string</returns>
        private static string EncodeToBase36Digits2(long input)
        {
            if (input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");
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
