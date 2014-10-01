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
                //LogManager.Error("[Business] Failed to create change request: " + ex.Message);
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
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>change Request object</returns>
        ChangeRequest GetChangeRequestById(int personId, int changeRequestId);
    }
}
