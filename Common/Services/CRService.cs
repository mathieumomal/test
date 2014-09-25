using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public class CRService : ICRService
    {
        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>
        /// Primary key of newly inserted change request along with the status (success/failure)
        /// </returns>
        public KeyValuePair<bool, int> CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            int primaryKeyOfChangeRequest = 0;
            bool isSuccess = false;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<ICRManager>();
                    manager.UoW = uoW;

                    isSuccess = manager.CreateChangeRequest(personId, changeRequest);
                    if (isSuccess)
                    {
                        uoW.Save();
                        primaryKeyOfChangeRequest = changeRequest.Pk_ChangeRequest;
                    }
                }
            }
            catch (Exception)
            {
                //LogManager.Error("[Service] Failed to create change request: " + ex.Message);
                isSuccess = false;
            }

            return new KeyValuePair<bool, int>(isSuccess, primaryKeyOfChangeRequest);
        }

        public KeyValuePair<bool, List<Enum_CRCategory>> GetChangeRequestCategories(int personId)
        {
            List<Enum_CRCategory> enumChangeRequestCategorylist = new List<Enum_CRCategory>();
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<ICRManager>();
                    manager.UoW = uoW;
                    enumChangeRequestCategorylist = manager.GetChangeRequestCategories(personId);
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

    public interface ICRService
    {
        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request along with the status (success/failure)</returns>
        KeyValuePair<bool, int> CreateChangeRequest(int personId, ChangeRequest changeRequest);


        /// <summary>
        /// Gets the change request category.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>Change request Category list</returns>
        KeyValuePair<bool, List<Enum_CRCategory>> GetChangeRequestCategories(int personId);
    }
}

