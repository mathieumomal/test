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
                LogManager.Error(String.Format("[Service] Failed to GetCrPacksByTbIdAndKeywords: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnerException:" + ex.InnerException : String.Empty)));
                response.Report.LogError(Localization.GenericError);
            }
            return response;
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
    }
}
