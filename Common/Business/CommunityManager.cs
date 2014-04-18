using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class CommunityManager
    {
        #region Constants

        private static string CACHE_KEY = "ULT_COMMUNITY_MANAGER_ALL";

        #endregion

        #region Properties

        private IUltimateUnitOfWork _uoW;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="UoW">Unit Of Work</param>
        public CommunityManager(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get All Communities
        /// </summary>
        /// <returns>List of Communities</returns>
        public List<Community> GetCommunities()
        {
            // Check in the cache
            var cachedData = (List<Community>)CacheManager.Get(CACHE_KEY);
            if (cachedData == null)
            {
                ICommunityRepository repo = RepositoryFactory.Resolve<ICommunityRepository>();
                repo.UoW = _uoW;
                cachedData = repo.All.ToList();

                CacheManager.Insert(CACHE_KEY, cachedData);
            }
            return cachedData;
        }

        #endregion
    }
}
