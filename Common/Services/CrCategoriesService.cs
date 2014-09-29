using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public class CrCategoriesService
    {
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
                LogManager.Error("[Service] Failed to GetChangeRequestCategories request: " + ex.Message);
                isSuccess = false;
            }
            return new KeyValuePair<bool, List<Enum_CRCategory>>(isSuccess, enumChangeRequestCategorylist);
        }
    }

    public interface ICrCategoriesService
    {
        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>Change request Categories list</returns>
        KeyValuePair<bool, List<Enum_CRCategory>> GetChangeRequestCategories();
    }
}
