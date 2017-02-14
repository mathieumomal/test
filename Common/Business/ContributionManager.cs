using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business.ExternalContributionService;
using Etsi.Ultimate.Business.UserRightsService;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Contribution Manager
    /// </summary>
    public class ContributionManager : IContributionManager
    {
        #region IContributionBusiness Membres

        /// <summary>
        /// See interface
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Search for CR-Packs by TbId and searching keyword
        /// </summary>
        /// <param name="personId">Persion identifier</param>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        public ServiceResponse<List<View_CrPacks>> GetCrPacksByTbIdAndKeywords(int personId, int tbId, string keywords)
        {
            var crpackRepo = RepositoryFactory.Resolve<ICrPackRepository>();
            crpackRepo.UoW = UoW;
            return new ServiceResponse<List<View_CrPacks>>
            {
                Result = crpackRepo.GetCrPacksByTbIdAndKeywords(tbId, keywords)
            };
        }

        /// <summary>
        /// Get CR uids (WGTDoc) from ids 
        /// </summary>
        /// <param name="crsId"></param>
        /// <returns></returns>
        public ServiceResponse<List<string>> GetUidsForCRs(List<int> crsId)
        {
            var response = new ServiceResponse<List<string>> { Result = new List<string>() };

            var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            repo.UoW = UoW;

            var crs = repo.FindCrsByIds(crsId);
            foreach (var item in crs)
            {
                response.Result.Add(item.WGTDoc);
            }
            return response;
        }

        /// <summary>
        /// Get the CRPack uid from its id
        /// </summary>
        /// <param name="crPackId"></param>
        /// <returns></returns>
        public ServiceResponse<string> GetUidForCrPack(int crPackId)
        {
            var response = new ServiceResponse<string> { Result = "" };

            var crPackRepo = RepositoryFactory.Resolve<ICrPackRepository>();
            crPackRepo.UoW = UoW;

            try
            {
                var crPack = crPackRepo.Find(crPackId);
                response.Result = crPack.uid;
            }
            catch (Exception e)
            {
                LogManager.ErrorFormat("Error while getting UID for CrPack : {0}",e);
            }
            return response;
        }

        /// <summary>
        /// Call external contribution service in order to generate tdoc list
        /// after sending Crs to a Cr pack
        /// </summary>
        /// <param name="uids"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> GenerateMeetingTdocListsAfterSendingCrsToCrPack(string[] uids, int personId)
        {
            var response = new ServiceResponse<bool> { Result = false };
            var externalContributionService = ManagerFactory.Resolve<IExtContributionService>();
            response.Result = externalContributionService.GenerateMeetingTdocListsAsynchronously(uids, personId).Result;
            return response;
        }

        #endregion
    }

    public interface IContributionManager
    {
        /// <summary>
        /// Unit of Work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Search for CR-Packs by TbId and searching keyword
        /// </summary>
        /// <param name="personId">Persion identifier</param>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        ServiceResponse<List<View_CrPacks>> GetCrPacksByTbIdAndKeywords(int personId, int tbId, string keywords);

        /// <summary>
        /// Get CR uids (WGTDoc) from ids 
        /// </summary>
        /// <param name="crsId"></param>
        /// <returns></returns>
        ServiceResponse<List<string>> GetUidsForCRs(List<int> crsId);

        /// <summary>
        /// Get the CRPack uid from its id
        /// </summary>
        /// <param name="crPackId"></param>
        /// <returns></returns>
        ServiceResponse<string> GetUidForCrPack(int crPackId);
        /// <summary>
        /// Call external contribution service in order to generate tdoc list
        /// after sending Crs to a Cr pack
        /// </summary>
        /// <param name="uids"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        ServiceResponse<bool> GenerateMeetingTdocListsAfterSendingCrsToCrPack(string[] uids, int personId);
    }
}
