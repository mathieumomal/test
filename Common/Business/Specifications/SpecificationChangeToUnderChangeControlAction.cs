using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Specifications
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
            ExtensionLogger.Info("CHANGE SPECS AS UCC: System is trying to make spec UCC...", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("personId", personId),
                new KeyValuePair<string, object>("specIdsForUcc", specIdsForUcc)
            });

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
                var specNumbers = new List<string>();
                specsToUpdate.ForEach(x =>
                {
                    var uid = x.Number + ": " + x.Title;
                    if (!x.IsUnderChangeControl.GetValueOrDefault() && x.IsActive)
                    {
                        specNumbers.Add(uid);
                        x.IsUnderChangeControl = true;
                        x.Histories.Add(new History
                        {
                            Fk_SpecificationId = x.Pk_SpecificationId,
                            CreationDate = DateTime.UtcNow,
                            Fk_PersonId = personId,
                            HistoryText = Localization.History_Specification_Status_Changed_UnderChangeControl
                        });
                        LogManager.Info("CHANGE SPEC AS UCC:    Spec -> " + uid + " is now UCC");
                    }
                    else
                    {
                        LogManager.Info("CHANGE SPEC AS UCC:    Spec -> " + uid + " is already UCC or is not active.");
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
                LogManager.Info("CHANGE SPECS AS UCC:    User doesn't have rights: Specification_EditFull or Specification_EditLimitted");
                statusChangeReport.Result = false;
                statusChangeReport.Report.LogError(Localization.GenericError);
            }

            if (statusChangeReport.Result)
            {
                LogManager.Info("CHANGE SPECS AS UCC: Done successfully. END.");
            }
            else
            {
                LogManager.Info("CHANGE SPECS AS UCC: Done with errors. END. " + string.Join(", ", statusChangeReport.Report.ErrorList));
            }
            return statusChangeReport;
        }
    }
}
