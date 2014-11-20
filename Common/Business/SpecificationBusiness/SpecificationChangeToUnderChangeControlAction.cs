using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Business.SpecificationBusiness
{
    public class SpecificationChangeToUnderChangeControlAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Changes the specifications status to under change control.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specIdsForUcc">The spec ids.</param>
        /// <returns>Status report</returns>
        public ServiceResponse<bool> ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIdsForUcc)
        {
            var statusChangeReport = new ServiceResponse<bool>();

            // Check that user has right to perform operation
            var rightsManager = ManagerFactory.Resolve<IRightsManager>();
            rightsManager.UoW = UoW;
            var userRights = rightsManager.GetRights(personId);
            if ((userRights != null) && (userRights.HasRight(Enum_UserRights.Specification_EditFull) || userRights.HasRight(Enum_UserRights.Specification_EditLimitted)))
            {
                var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
                specRepo.UoW = UoW;
                var specsToUpdate = specRepo.GetSpecifications(specIdsForUcc);
                List<string> specNumbers = new List<string>();
                specsToUpdate.ForEach(x =>
                {
                    if (!x.IsUnderChangeControl.GetValueOrDefault() && x.IsActive)
                    {
                        specNumbers.Add(x.Number + ": " + x.Title);
                        x.IsUnderChangeControl = true;
                        x.Histories.Add(new History()
                        {
                            Fk_SpecificationId = x.Pk_SpecificationId,
                            CreationDate = DateTime.UtcNow,
                            Fk_PersonId = personId,
                            HistoryText = Utils.Localization.History_Specification_Status_Changed_UnderChangeControl
                        });
                    }
                });
                statusChangeReport.Result = true;
                if (specNumbers.Count > 0)
                    statusChangeReport.Report.LogInfo("Following specifications changed to Under Change Control.\n\t" + String.Join("\n\t", specNumbers));
                else
                    statusChangeReport.Report.LogInfo("None of the specifications changed to Under Change Control.");
            }
            else
            {
                statusChangeReport.Result = false;
                statusChangeReport.Report.LogError(Localization.GenericError);
            }

            return statusChangeReport;
        }
    }
}
