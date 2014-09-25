using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class CRManager : ICRManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }
        private const string CACHE_KEY = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        public bool CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            bool isSuccess = true;
            try
            {
                var repo = RepositoryFactory.Resolve<ICRRepository>();
                repo.UoW = UoW;
                repo.InsertOrUpdate(changeRequest);
            }
            catch (Exception)
            {
                //LogManager.Error("[Business] Failed to create change request: " + ex.Message);
                isSuccess = false;
            }
            return isSuccess;
        }

        public List<Enum_CRCategory> GetChangeRequestCategories(int personId)
        {
            //cheeck cache data need to check User right
            var cachedData = (List<Enum_CRCategory>)CacheManager.Get(CACHE_KEY);
            try
            {
                if (cachedData == null)
                {
                    var repo = RepositoryFactory.Resolve<IEnum_CRCategoryRepository>();
                    repo.UoW = UoW;
                   cachedData = repo.All.ToList();
                    
                    if (CacheManager.Get(CACHE_KEY) == null)
                    {
                        CacheManager.Insert(CACHE_KEY, cachedData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Business] Failed to GetChangeRequestCategories:" + ex.Message);
            }
            return cachedData;
        }
    }

    public interface ICRManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        bool CreateChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>CR Category list</returns>
        List<Enum_CRCategory> GetChangeRequestCategories(int personId);
    }
}
