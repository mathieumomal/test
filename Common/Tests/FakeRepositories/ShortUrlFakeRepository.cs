using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    public class ShortUrlFakeRepository : IUrlRepository
    {
        public ShortUrlFakeRepository() { }


        #region IEntityRepository<ShortUrl> Membres

        public IUltimateUnitOfWork UoW
        {
            get;
            set;
        }

        public IQueryable<ShortUrl> All
        {
            get { return GenerateList();  }
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

        public ShortUrl FindByToken(string token)
        {
            //This will throw an exception if the query does not return at least one item.
            var ShortUrl = GenerateList().Where(f => f.Token == token).FirstOrDefault();
            if (ShortUrl == null)
                throw new KeyNotFoundException();
            return ShortUrl;
        }

        #endregion

        private IQueryable<ShortUrl> GenerateList()
        {
            var shorturls = new ShortUrlFakeDBSet();
            shorturls.Add(new ShortUrl() { Token="azer1", Pk_Id=1, Url="address1"});
            shorturls.Add(new ShortUrl() { Token = "azer2", Pk_Id = 2, Url = "address2" });
            shorturls.Add(new ShortUrl() { Token = "azer3", Pk_Id = 3, Url = "address3" });
            return shorturls.AsQueryable();
        }

        public KeyValuePair<int, string> GetTabIdAndPageNameForModuleId(int moduleId)
        {
            return new KeyValuePair<int,string>(0, "");
        }

    }
}
