using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class CommunityManager : ICommunityManager
    {
        #region Constants

        private const string CACHE_KEY = "ULT_COMMUNITY_MANAGER_ALL";

        #endregion

        #region Properties


        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region Constructor


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
                repo.UoW = UoW;
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
            repo.UoW = UoW;

            var community = GetCommunities().Where(c => c.TbId == id).FirstOrDefault();
            return ((community == null) ? String.Empty : community.ShortName);
        }


        /// <summary>
        /// Return community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>community</returns>
        public Community GetCommmunityById(int id)
        {
            ICommunityRepository repo = RepositoryFactory.Resolve<ICommunityRepository>();
            repo.UoW = UoW;

            return GetCommunities().Where(c => c.TbId == id).FirstOrDefault();
        }

        public Enum_CommunitiesShortName GetEnumCommunityShortNameByCommunityId(int id)
        {
            IEnum_CommunitiesShortNameRepository repo = RepositoryFactory.Resolve<IEnum_CommunitiesShortNameRepository>();
            repo.UoW = UoW;

            return repo.All.Where(x => x.Fk_TbId == id).FirstOrDefault();
        }

        public Community GetParentCommunityByCommunityId(int childTbId)
        {
            ICommunityRepository repo = RepositoryFactory.Resolve<ICommunityRepository>();
            repo.UoW = UoW;
            var childCommunity = this.GetCommmunityById(childTbId);
            if (childCommunity != null && childCommunity.ParentCommunityId != 0)
            {
                var parentCommunity = this.GetCommmunityById(childCommunity.ParentCommunityId);
                return parentCommunity;
            }
            return null;
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
