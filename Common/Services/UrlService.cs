using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public class UrlService: IUrlService
    {
        #region IUrlService Members

        public KeyValuePair<int, string> GetPageIdAndFullAddressForModule(int moduleId, string baseAddress, Dictionary<string, string> getParams)
        {
            throw new NotImplementedException();
        }

        public string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> urlParams)
        {
            throw new NotImplementedException();
        }

        public string GetFullUrlForToken(string token)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
