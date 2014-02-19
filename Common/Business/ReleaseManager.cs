using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class ReleaseManager
    {
        // Used for the caching of the releases.
        private static string CACHE_KEY = "ULT_REPO_RELEASES_ALL";
        
        public IUltimateUnitOfWork UoW {get; set;}
        
        public ReleaseManager() { }

        

        /// <summary>
        /// Retrieves all the data for the releases.
        /// </summary>
        /// <returns></returns>
        public List<Release> GetAllReleases()
        {
            // Check in the cache
            var cachedData = (List<Release>)CacheManager.Get(CACHE_KEY);
            if (cachedData != null)
                return cachedData;

            // if nothing in the cache, ask the repository, then cache it
            IReleaseRepository repo = RepositoryFactory.Resolve<IReleaseRepository>();
            repo.UoW = UoW;
            cachedData = repo.All.ToList();

            // Check that cache is still empty
            if (CacheManager.Get(CACHE_KEY) == null)
                CacheManager.Insert(CACHE_KEY, cachedData);
            
            return cachedData; 
        }


    }
}
