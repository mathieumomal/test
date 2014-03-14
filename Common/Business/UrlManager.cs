using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Business
{
    public class UrlManager
    {
        public const string DnnTabIdGetParam = "TabID";

        public IUltimateUnitOfWork UoW { get; set; }

        public UrlManager() { }


        public KeyValuePair<int, string> GetPageIdAndFullAddressForModule(int moduleId, string baseAddress, Dictionary<string, string> urlParams)
        {
            // First, search the data concerning the module
            var shortUrlRepo = RepositoryFactory.Resolve<IUrlRepository>();
            shortUrlRepo.UoW = UoW;
            var result = shortUrlRepo.GetTabIdAndPageNameForModuleId(moduleId);

            if (result.Key == 0)
                return new KeyValuePair<int, string>(0, null);

            // Transform the result into something legible.
            var address = new StringBuilder(baseAddress);
            address.Append(result.Value.Replace("//","/")+".aspx");

            // Add the UrlParams. Do not consider tabId param.
            urlParams.Remove(DnnTabIdGetParam);
            if (urlParams.Count > 0)
                address.Append( "?"+ string.Join("&", urlParams.Select( x => x.Key + "=" + x.Value).ToArray()));

            return new KeyValuePair<int,string>(result.Key, address.ToString());
        }

        public string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> urlParams)
        {
            IUrlRepository repo = RepositoryFactory.Resolve<IUrlRepository>();
            repo.UoW = UoW;

            ShortUrl shortUrlDb = new ShortUrl();
            shortUrlDb.Url = GetPageIdAndFullAddressForModule(moduleId, baseAddress, urlParams).Value;
            var exceptionthrown = true;
            do{
                shortUrlDb.Token = Guid.NewGuid().ToString().Substring(0, 8);
                try
                {
                    repo.FindByToken(shortUrlDb.Token);
                }
                catch (Exception)
                {
                    exceptionthrown = false;
                    throw;
                }
            } while (exceptionthrown);
            
            
            repo.InsertOrUpdate(shortUrlDb);
            return new StringBuilder().Append(baseAddress).Append("/shorturl/").Append(shortUrlDb.Token).ToString();
        }

        public string GetFullUrlForToken(string token)
        {
            IUrlRepository repo = RepositoryFactory.Resolve<IUrlRepository>();
            repo.UoW = UoW;
            return repo.FindByToken(token).Url;
        }
    }
}
