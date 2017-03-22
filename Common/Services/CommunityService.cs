using System;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Collections.Generic;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class CommunityService : ICommunityService
    {
        public List<Community> GetCommunities()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW ;
                return communityManager.GetCommunities();
            }
        }

        public string GetCommmunityshortNameById(int id)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW;
                return communityManager.GetCommmunityshortNameById(id);
            }
        }

        public List<Community> GetCommunitiesByIds(List<int> ids)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                    communityManager.UoW = uoW;
                    return communityManager.GetCommmunityByIds(ids);
                }
            }
            catch (Exception e)
            {
                LogManager.Error("CommunityService - GetCommunitiesByIds - IDs: " + string.Join(", ", ids), e);
                return null;
            }
        }
    }

    public interface ICommunityService
    {
        List<Community> GetCommunities();

        List<Community> GetCommunitiesByIds(List<int> ids); 

        string GetCommmunityshortNameById(int id);
    }
}
