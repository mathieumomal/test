using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// CrCategoriesService
    /// </summary>
    public class CrCategoriesService : ICrCategoriesService
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }
        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>
        /// Change request Categories list</returns>

        public KeyValuePair<bool, List<Enum_CRCategory>> GetChangeRequestCategories()
        {
            List<Enum_CRCategory> enumChangeRequestCategorylist = new List<Enum_CRCategory>();
            var isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<ICrCategoriesManager>();
                    manager.UoW = uoW;
                    enumChangeRequestCategorylist = manager.GetChangeRequestCategories();
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogManager.Error("[Service] Failed to GetChangeRequestCategories request: " + ex.Message);                
            }
            return new KeyValuePair<bool, List<Enum_CRCategory>>(isSuccess, enumChangeRequestCategorylist);
        }
    }

    /// <summary>
    /// ICrCategoriesService
    /// </summary>
    public interface ICrCategoriesService
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }
        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>Change request Categories list</returns>
        KeyValuePair<bool, List<Enum_CRCategory>> GetChangeRequestCategories();
    }
}
