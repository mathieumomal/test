using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class CommunityManager
    {
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
            ICommunityRepository repo = RepositoryFactory.Resolve<ICommunityRepository>();
            repo.UoW = _uoW;
            return repo.All.ToList();
        }

        #endregion
    }
}
