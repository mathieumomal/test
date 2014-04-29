using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class CommunityManager : ICommunityManager
    {
        #region Constants

        private static string CACHE_KEY = "ULT_COMMUNITY_MANAGER_ALL";

        #endregion

        #region Properties

        private IUltimateUnitOfWork _uoW;
        public IUltimateUnitOfWork _UoW { 
            get {return _uoW;} 
            set{_uoW = value;}
        }

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
                //Data Issue: Root Node Missing for 3GPP. Hence, adding dynamically
                AddMissingParent(cachedData);
                //Set Order for communities based on the parent-child releation
                var rootCommunities = cachedData.Where(x => x.ParentTbId == 0);
                int order = 0;
                foreach (var rootCommunity in rootCommunities)
                {
                    rootCommunity.Order = order;
                    order = UpdateCommunityOrder(cachedData, rootCommunity);
                }

                CacheManager.Insert(CACHE_KEY, cachedData);
            }
            return cachedData;
        }

        /// <summary>
        /// Return short name of a community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>Short name of the community</returns>
        public string GetCommmunityshortNameById(int id)
        {
            ICommunityRepository repo = RepositoryFactory.Resolve<ICommunityRepository>();
            repo.UoW = _uoW;
            //return repo.All.Where(c => c.TbId == id).ToList().FirstOrDefault().ShortName;
            return GetCommunities().Where(c => c.TbId == id).ToList().FirstOrDefault().ShortName;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add missing parent record
        /// </summary>
        /// <param name="DataSource">Datasource</param>
        private void AddMissingParent(List<Community> DataSource)
        {
            List<Community> missingParentCommunities = new List<Community>();

            foreach (var community in DataSource)
            {
                if (community.ParentTbId != null && community.ParentTbId != 0) //Don't process the root nodes
                {
                    var parentCommunity = DataSource.Find(x => x.TbId == community.ParentTbId);
                    if ((parentCommunity == null) && (!missingParentCommunities.Exists(x => x.TbId == community.ParentCommunityId))) //If parent missing, add the same
                    {
                        Community missingParentCommunity = new Community();
                        missingParentCommunity.TbId = community.ParentCommunityId;
                        missingParentCommunity.ParentTbId = 0;
                        missingParentCommunity.TbName = community.TbName.Split(' ')[0];
                        missingParentCommunity.ShortName = community.TbName.Split(' ')[0];
                        missingParentCommunity.ActiveCode = "ACTIVE";
                        missingParentCommunities.Add(missingParentCommunity);
                    }
                }
            }

            DataSource.AddRange(missingParentCommunities);
        }

        /// <summary>
        /// Update Order property of child nodes for the given community (recursively)
        /// </summary>
        /// <param name="dataSource">List of Communities</param>
        /// <param name="currentCommunity">Community to proceed for child levels</param>
        /// <returns>New Order Number</returns>
        private int UpdateCommunityOrder(List<Community> dataSource, Community currentCommunity)
        {
            int nextOrder = currentCommunity.Order + 1;
            List<Community> childDataSource = dataSource.FindAll(x => x.ParentTbId == currentCommunity.TbId);
            foreach (var childCommunity in childDataSource)
            {
                childCommunity.Order = nextOrder;
                nextOrder = UpdateCommunityOrder(dataSource, childCommunity);
            }
            return nextOrder;
        }

        #endregion
    }
}
