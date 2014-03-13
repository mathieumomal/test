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
        public IUltimateUnitOfWork UoW { get; set; }

        public UrlManager() { }


        public KeyValuePair<int, string> GetPageIdAndFullAddressForModule()
        {
            throw new NotImplementedException();
        }

        public string CreateShortUrl(int moduleId, string baseAddress, Dictionary<string, string> urlParams)
        {
            IShortUrlRepository repo = RepositoryFactory.Resolve<IShortUrlRepository>();
            repo.UoW = UoW;

            StringBuilder url = new StringBuilder().Append(baseAddress).Append("?");
            foreach (KeyValuePair<string, string> entry in urlParams)
            {
                url
                    .Append(entry.Key)
                    .Append("=")
                    .Append(entry.Value)
                    .Append("&");
            }
            
            ShortUrl shortUrl = new ShortUrl();
            shortUrl.Url = url.ToString();
            shortUrl.Token = Guid.NewGuid().ToString().Substring(0,8);
            repo.InsertOrUpdate(shortUrl);
            return new StringBuilder().Append(baseAddress).Append("/shorturl/").Append(shortUrl.Token).ToString();
        }

        public string GetFullUrlForToken(string token)
        {
            IShortUrlRepository repo = RepositoryFactory.Resolve<IShortUrlRepository>();
            repo.UoW = UoW;
            return repo.FindByToken(token).Url;
        }
    }
}
