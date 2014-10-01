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
    public class ChangeRequestManager : IChangeRequestManager
    {
        #region IChangeRequestManager Members

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
            var isSuccess = true;
            try
            {
                changeRequest.CRNumber = GenerateCrNumberBySpecificationId(changeRequest.Fk_Specification);
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                repo.InsertOrUpdate(changeRequest);
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Business] Failed to create change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Edits the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Success/Failure</returns>
        public bool EditChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var isSuccess = true;
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var dbChangeRequest = repo.Find(changeRequest.Pk_ChangeRequest);
                CompareAndUpdate(changeRequest, dbChangeRequest);                
            }
            catch (Exception ex)
            {
                LogManager.Error(String.Format("[Business] Failed to edit change request: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Generates the cr number.
        /// </summary>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>Cr number</returns>
        public string GenerateCrNumberBySpecificationId(int? specificationId)
        {
            List<int> crNumberList = new List<int>();

            var heighestCrNumber = 0;
            if (specificationId > 0)
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var crNumber = repo.FindCrNumberBySpecificationId(specificationId);

                int tmpCrNumber;
                foreach (var num in crNumber)
                {
                    if (int.TryParse(num, out tmpCrNumber))
                        crNumberList.Add(tmpCrNumber);
                }
                if (crNumberList.Count != 0)
                { 
                heighestCrNumber = crNumberList.Max();
                }
                heighestCrNumber++;
            }
            return heighestCrNumber.ToString(new String('0', 4));
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>change Request object</returns>
        public ChangeRequest GetChangeRequestById(int personId, int changeRequestId)
        {
            var isSuccess = true;
            var changeRequest = new ChangeRequest();
            try
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                changeRequest = repo.Find(changeRequestId);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                LogManager.Error("[Business] Failed to GetChangeRequestById:" + ex.Message);
            }
            return changeRequest;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Compares the and update.
        /// </summary>
        /// <param name="uiChangeRequest">The UI change request.</param>
        /// <param name="dbChangeRequest">The database change request.</param>
        private void CompareAndUpdate(ChangeRequest uiChangeRequest, ChangeRequest dbChangeRequest)
        {
            if (uiChangeRequest != null && dbChangeRequest != null)
            {
                if (dbChangeRequest.CRNumber != uiChangeRequest.CRNumber)
                    dbChangeRequest.CRNumber = uiChangeRequest.CRNumber;
                if (dbChangeRequest.Revision != uiChangeRequest.Revision)
                    dbChangeRequest.Revision = uiChangeRequest.Revision;
                if (dbChangeRequest.Subject != uiChangeRequest.Subject)
                    dbChangeRequest.Subject = uiChangeRequest.Subject;
                if (dbChangeRequest.Fk_TSGStatus != uiChangeRequest.Fk_TSGStatus)
                    dbChangeRequest.Fk_TSGStatus = uiChangeRequest.Fk_TSGStatus;
                if (dbChangeRequest.Fk_WGStatus != uiChangeRequest.Fk_WGStatus)
                    dbChangeRequest.Fk_WGStatus = uiChangeRequest.Fk_WGStatus;
                if (dbChangeRequest.CreationDate != uiChangeRequest.CreationDate)
                    dbChangeRequest.CreationDate = uiChangeRequest.CreationDate;
                if (dbChangeRequest.TSGSourceOrganizations != uiChangeRequest.TSGSourceOrganizations)
                    dbChangeRequest.TSGSourceOrganizations = uiChangeRequest.TSGSourceOrganizations;
                if (dbChangeRequest.WGSourceOrganizations != uiChangeRequest.WGSourceOrganizations)
                    dbChangeRequest.WGSourceOrganizations = uiChangeRequest.WGSourceOrganizations;
                if (dbChangeRequest.TSGMeeting != uiChangeRequest.TSGMeeting)
                    dbChangeRequest.TSGMeeting = uiChangeRequest.TSGMeeting;
                if (dbChangeRequest.TSGTarget != uiChangeRequest.TSGTarget)
                    dbChangeRequest.TSGTarget = uiChangeRequest.TSGTarget;
                if (dbChangeRequest.WGSourceForTSG != uiChangeRequest.WGSourceForTSG)
                    dbChangeRequest.WGSourceForTSG = uiChangeRequest.WGSourceForTSG;
                if (dbChangeRequest.WGMeeting != uiChangeRequest.WGMeeting)
                    dbChangeRequest.WGMeeting = uiChangeRequest.WGMeeting;
                if (dbChangeRequest.WGTarget != uiChangeRequest.WGTarget)
                    dbChangeRequest.WGTarget = uiChangeRequest.WGTarget;
                if (dbChangeRequest.Fk_Enum_CRCategory != uiChangeRequest.Fk_Enum_CRCategory)
                    dbChangeRequest.Fk_Enum_CRCategory = uiChangeRequest.Fk_Enum_CRCategory;
                if (dbChangeRequest.Fk_Specification != uiChangeRequest.Fk_Specification)
                    dbChangeRequest.Fk_Specification = uiChangeRequest.Fk_Specification;
                if (dbChangeRequest.Fk_Release != uiChangeRequest.Fk_Release)
                    dbChangeRequest.Fk_Release = uiChangeRequest.Fk_Release;
                if (dbChangeRequest.Fk_CurrentVersion != uiChangeRequest.Fk_CurrentVersion)
                    dbChangeRequest.Fk_CurrentVersion = uiChangeRequest.Fk_CurrentVersion;
                if (dbChangeRequest.Fk_NewVersion != uiChangeRequest.Fk_NewVersion)
                    dbChangeRequest.Fk_NewVersion = uiChangeRequest.Fk_NewVersion;
                if (dbChangeRequest.Fk_Impact != uiChangeRequest.Fk_Impact)
                    dbChangeRequest.Fk_Impact = uiChangeRequest.Fk_Impact;
                if (dbChangeRequest.TSGTDoc != uiChangeRequest.TSGTDoc)
                    dbChangeRequest.TSGTDoc = uiChangeRequest.TSGTDoc;
                if (dbChangeRequest.WGTDoc != uiChangeRequest.WGTDoc)
                    dbChangeRequest.WGTDoc = uiChangeRequest.WGTDoc;

                //CR WorkItems (Insert / Delete)
                var crWorkItemsToInsert = uiChangeRequest.CR_WorkItems.ToList().Where(x => dbChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToInsert.ToList().ForEach(x => dbChangeRequest.CR_WorkItems.Add(x));
                var crWorkItemsToDelete = dbChangeRequest.CR_WorkItems.ToList().Where(x => uiChangeRequest.CR_WorkItems.ToList().All(y => y.Fk_WIId != x.Fk_WIId));
                crWorkItemsToDelete.ToList().ForEach(x => UoW.MarkDeleted<CR_WorkItems>(x));
            }
        } 

        #endregion
    }

    /// <summary>
    /// IChangeRequestManager
    /// </summary>
    public interface IChangeRequestManager
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
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>change Request object</returns>
        ChangeRequest GetChangeRequestById(int personId, int changeRequestId);
    }
}
