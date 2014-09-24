using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;

namespace Etsi.Ultimate.Business
{
    public class CRManager : ICRManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

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
    }
}
