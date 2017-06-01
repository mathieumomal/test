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
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                    communityManager.UoW = uoW;
                    return communityManager.GetCommunities();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object>(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public string GetCommmunityshortNameById(int id)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                    communityManager.UoW = uoW;
                    return communityManager.GetCommmunityshortNameById(id);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { id }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
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
                ExtensionLogger.Exception(e, new List<object> { ids }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
