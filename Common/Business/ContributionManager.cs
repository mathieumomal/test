using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

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
    }
}
