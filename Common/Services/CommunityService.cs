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
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW ;
                return communityManager.GetCommunities();
            }
        }

        /// <summary>
        /// Return short name of a community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>Short name of the community</returns>
        public string GetCommmunityshortNameById(int id)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW;
                return communityManager.GetCommmunityshortNameById(id);
            }
        }

        #endregion
    }
}
