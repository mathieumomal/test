using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private const string CACHE_KEY = "ULT_REPO_MODULE_TAB";

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
            return UoW.Context.ShortUrls.Where(f => f.Token == token).FirstOrDefault();
        }

        /// <summary>
        /// Looks for the module Id in order to get the tab where it has been installed, and the path to this tab in the application.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns>
        /// 0, null if moduleId can't be found
        /// the tabId and the TabPath (form /Full/Path, //PageName if root)
        /// </returns>
        public KeyValuePair<int, string> GetTabIdAndPageNameForModuleId(int moduleId)
        {
            if(moduleId == 0)
                return new KeyValuePair<int, string>(0, null);

            // First check in cache if exists.
            var cachedData = (Dictionary<int,View_ModulesPages>) CacheManager.Get(CACHE_KEY);
            if (cachedData != null && cachedData.ContainsKey(moduleId))
            {
                var data = cachedData[moduleId];
                return new KeyValuePair<int, string>(data.TabID, data.TabPath);
            }
            
            // Else, compute it
            var matchingModule = UoW.Context.View_ModulesPages.Where(m => m.ModuleID == moduleId).FirstOrDefault();
            if (matchingModule == null)
                return new KeyValuePair<int,string>(0,null);

            // And put it in cache.
            if (cachedData == null)
                cachedData = new Dictionary<int, View_ModulesPages>();
            cachedData.Add(moduleId, matchingModule);
            CacheManager.Insert(CACHE_KEY, cachedData);

            return new KeyValuePair<int,string>(matchingModule.TabID,matchingModule.TabPath);
        }

        #endregion
    }

    public interface IUrlRepository : IEntityRepository<ShortUrl>
    {
        ShortUrl FindByToken(String token);
        KeyValuePair<int, string> GetTabIdAndPageNameForModuleId(int moduleId);
    }
}
