using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public class ChangeRequestService : IChangeRequestService
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
            var primaryKeyOfChangeRequest = 0;
            bool isSuccess;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;

                    isSuccess = manager.CreateChangeRequest(personId, changeRequest);
                    if (isSuccess)
                    {
                        uoW.Save();
                        primaryKeyOfChangeRequest = changeRequest.Pk_ChangeRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogManager.Error(String.Format("[Service] Failed to create change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
            }

            return new KeyValuePair<bool, int>(isSuccess, primaryKeyOfChangeRequest);
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId"></param>
        /// <returns>ChangeRequest object</returns>
        public KeyValuePair<bool, ChangeRequest> GetChangeRequestById(int personId, int changeRequestId)
        {
            var changeRequest = new ChangeRequest();
            bool isSuccess=true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    changeRequest = manager.GetChangeRequestById(personId, changeRequestId);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogManager.Error(String.Format("[Service] Failed to get change request id : {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
            }
            return new KeyValuePair<bool, ChangeRequest>(isSuccess, changeRequest);
        }

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        public bool EditChangeRequest(int personId, ChangeRequest changeRequest)
        {
            bool isSuccess;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;

                    isSuccess = manager.EditChangeRequest(personId, changeRequest);
                    if (isSuccess)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Service] Failed to edit change request: {0}{1}", ex.Message, ((ex.InnerException != null)? "\n InnterException:" + ex.InnerException : String.Empty)));
                isSuccess = false;
            }

            return isSuccess;
        }
        
        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="ContributionUID">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        public KeyValuePair<bool, ChangeRequest> GetContributionCrByUid(string ContributionUID)
        {
            ChangeRequest cr = null;
            bool isSuccess = true;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IChangeRequestManager>();
                    manager.UoW = uoW;
                    cr = manager.GetContributionCrByUid(ContributionUID);
                    if (cr == null)
                        isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Service] Failed to GetContributionCrByUid request: " + ex.Message);
                isSuccess = false;
            }
            return new KeyValuePair<bool, ChangeRequest>(isSuccess, cr);
        }

    }

    /// <summary>
    /// IChangeRequestService
    /// </summary>
    public interface IChangeRequestService
    {
        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request along with the status (success/failure)</returns>
        KeyValuePair<bool, int> CreateChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        bool EditChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId"></param>
        /// <returns>ChangeRequest object</returns>
        KeyValuePair<bool, ChangeRequest> GetChangeRequestById(int personId, int changeRequestId);

        /// <summary>
        /// Returns a contribution's CR data
        /// </summary>
        /// <param name="ContributionUID">Contribution UID</param>
        /// <returns>ChangeRequest entity</returns>
        KeyValuePair<bool, ChangeRequest> GetContributionCrByUid(string ContributionUID);
    }
}

