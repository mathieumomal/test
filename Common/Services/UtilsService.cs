using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Services
{
    public class UtilsService
    {
        /// <summary>
        /// Convert Base10 number to Base36 string
        /// </summary>
        /// <param name="input">Base10 number</param>
        /// <returns>Base36 string</returns>
        public string EncodeToBase36Digits2(long input)
        {
            var utilsMgr = new UtilsManager();
            return utilsMgr.EncodeToBase36Digits2(input);
        }

        /// <summary>
        /// Convert Base36 string to Base10 number
        /// </summary>
        /// <param name="input">Base36 string</param>
        /// <returns>Base10 number</returns>
        public long DecodeBase36ToDecimal(string input)
        {
            var utilsMgr = new UtilsManager();
            return utilsMgr.DecodeBase36ToDecimal(input);
        }
    }
}
