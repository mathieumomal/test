using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Repositories
{
    public class UrlRepository : IShortUrlRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public UrlRepository(){}

        #region IEntityRepository<ShortUrl> Membres


        public IQueryable<ShortUrl> All
        {
            get { return UoW.Context.ShortUrls; }
        }

        public IQueryable<ShortUrl> AllIncluding(params System.Linq.Expressions.Expression<Func<ShortUrl, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public ShortUrl Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(ShortUrl entity)
        {
            if (entity.Pk_Id == default(int))
            {
                UoW.Context.SetAdded(entity);
            }
            else
            {
                UoW.Context.SetModified(entity);
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IShortUrlRepository Membres

        //Find shortUrl by token
        public ShortUrl FindByToken(string token)
        {
            //This will throw an exception if the query does not return at least one item.
            var ShortUrl = UoW.Context.ShortUrls.Where(f => f.Token == token).FirstOrDefault();
            if(ShortUrl == null)
                throw new KeyNotFoundException();
            return ShortUrl;
        }

        #endregion
    }

    public interface IShortUrlRepository : IEntityRepository<ShortUrl>
    {
        ShortUrl FindByToken(String token);
    }
}
