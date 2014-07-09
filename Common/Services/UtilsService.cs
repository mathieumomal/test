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
        public string EncodeToBase36Digits2(long input)
        {
            var utilsMgr = new UtilsManager();
            return utilsMgr.EncodeToBase36Digits2(input);
        }
    }
}
