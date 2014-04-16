using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public class CommunityService : ICommunityService
    {
        #region Public Methods

        /// <summary>
        /// Get All Communities
        /// </summary>
        /// <returns>List of Communities</returns>
        public List<Community> GetCommunities()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var communityManager = new CommunityManager(uoW);
                return communityManager.GetCommunities();
            }
        }

        #endregion
    }
}
