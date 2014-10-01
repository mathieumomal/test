using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// CrCategoriesManager
    /// </summary>
    public class CrCategoriesManager : ICrCategoriesManager
    {
        #region constants
        private const string CacheKey = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";
        #endregion

        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }
        
        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <returns>CR Categories list </returns>
        public List<Enum_CRCategory> GetChangeRequestCategories()
        {
            //cheeck cache data need to check User right
            var cachedData = (List<Enum_CRCategory>)CacheManager.Get(CacheKey);
            try
            {
                if (cachedData == null)
                {
                    var repo = RepositoryFactory.Resolve<IEnum_CrCategoryRepository>();
                    repo.UoW = UoW;
                    cachedData = repo.All.ToList();

                    if (CacheManager.Get(CacheKey) == null)
                    {
                        CacheManager.Insert(CacheKey, cachedData);
                    }
                }
            }
            catch (Exception ex)
            {
               LogManager.Error("[Ultimate Business] Failed to GetChangeRequestCategories:" + ex.Message);
            }
            return cachedData;
        }
    }

    /// <summary>
    /// ICrCategoriesManager
    /// </summary>
    public interface ICrCategoriesManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <returns>CR Categories list</returns>
        List<Enum_CRCategory> GetChangeRequestCategories();

    }
}
