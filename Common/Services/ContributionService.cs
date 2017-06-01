using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class ContributionService : IContributionService
    {

        #region IContributionService Membres

        /// <summary>
        /// Search for CR-Packs
        /// </summary>
        /// <param name="personId">Persion identifier</param>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        public ServiceResponse<List<View_CrPacks>> GetCrPacksByTbIdAndKeywords(int personId, int tbId, string keywords)
        {
            var response = new ServiceResponse<List<View_CrPacks>>{Result = new List<View_CrPacks>()}; 
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var manager = ManagerFactory.Resolve<IContributionManager>();
                    manager.UoW = uoW;

                    response = manager.GetCrPacksByTbIdAndKeywords(personId, tbId, keywords);
                }
            }
            catch (Exception ex)
            {
                ExtensionLogger.Exception(ex, new List<object> { personId, tbId, keywords }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                response.Report.LogError(Localization.GenericError);
            }
            return response;
        }

        /// <summary>
        /// Get CRs and CR Pack uids and call Contribution External Service 
        /// in order to generate Tdoc list asynchronously
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="crPackId"></param>
        /// <param name="crsIds"></param>
        public ServiceResponse<bool> GenerateTdocListsAfterSendingCrsToCrPack(int personId, int crPackId, List<int> crsIds)
        {
            var response = new ServiceResponse<bool> { Result = false };
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var contributionManager = ManagerFactory.Resolve<IContributionManager>();
                    contributionManager.UoW = uoW;
                    List<string> uidsToGenerateTdocList = contributionManager.GetUidsForCRs(crsIds).Result;
                    if (crPackId != 0)
                    {
                        uidsToGenerateTdocList.Add(contributionManager.GetUidForCrPack(crPackId).Result);
                    }
                    var tmp =
                        contributionManager.GenerateMeetingTdocListsAfterSendingCrsToCrPack(
                            uidsToGenerateTdocList.ToArray(), personId);
                    response.Result = tmp.Result;

                    if (response.Result)
                    {
                        LogManager.Info("[Service] Tdoc list generated after sending CRs to CR pack");
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                ExtensionLogger.Exception(ex, new List<object> { personId, crPackId, crsIds }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                response.Report.LogError("Failed to generate Tdoc List after sending CRs to CR pack");
                return response;
            }


        }

        #endregion
    }

    public interface IContributionService
    {
        /// <summary>
        /// Search for CR-Packs
        /// </summary>
        /// <param name="personId">Persion identifier</param>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        ServiceResponse<List<View_CrPacks>> GetCrPacksByTbIdAndKeywords(int personId, int tbId, string keywords);


        /// <summary>
        /// Get CRs and CR Pack uids and call Contribution External Service 
        /// in order to generate Tdoc list asynchronously
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="crPackId"></param>
        /// <param name="crsIds"></param>
        ServiceResponse<bool> GenerateTdocListsAfterSendingCrsToCrPack(int personId, int crPackId, List<int> crsIds);
    }
}
