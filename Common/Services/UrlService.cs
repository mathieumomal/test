using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class UrlService: IUrlService
    {
        #region IUrlService Members

        public KeyValuePair<int, string> GetPageIdAndFullAddressForModule(int moduleId, string baseAddress, Dictionary<string, string> getParams)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var urlManager = new UrlManager();
                urlManager.UoW = uoW;
                return urlManager.GetPageIdAndFullAddressForModule(moduleId, baseAddress, getParams);
            }
        }

        public string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> getParams)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var urlManager = new UrlManager();
                urlManager.UoW = uoW;
                return urlManager.CreateShortUrl(moduleId, baseAddress, getParams);
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
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion
    }
}
