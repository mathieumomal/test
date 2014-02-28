using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Etsi.Ultimate.Utils
{
    public class ServerTopology
    {

        public static string GetServerRootPath()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Request.PhysicalApplicationPath;
            return "";
        }
    }
}
