using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class UrlService: IUrlService
    {
        #region IUrlService Members

        public KeyValuePair<int, string> GetPageIdAndFullAddressForModule(int moduleId, string baseAddress, Dictionary<string, string> getParams)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var urlManager = new UrlManager();
                    urlManager.UoW = uoW;
                    return urlManager.GetPageIdAndFullAddressForModule(moduleId, baseAddress, getParams);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { moduleId, baseAddress, getParams }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> getParams)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var urlManager = new UrlManager();
                    urlManager.UoW = uoW;
                    String shortUrl = urlManager.CreateShortUrl(moduleId, baseAddress, getParams);
                    uoW.Save();
                    return shortUrl;
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { moduleId, baseAddress, getParams }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public string GetFullUrlForToken(string token)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var urlManager = new UrlManager();
                urlManager.UoW = uoW;
                try
                {
                    return urlManager.GetFullUrlForToken(token);
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { token }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return null;
                }
            }
        }

        #endregion
    }
}
