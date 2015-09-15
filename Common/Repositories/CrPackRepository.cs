using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// CR pack repository
    /// </summary>
    public class CrPackRepository : ICrPackRepository
    {

        #region ICrPackRepository Membres
        /// <summary>
        /// See interface
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public View_CrPacks Find(int id)
        {
            return UoW.Context.View_CrPacks.FirstOrDefault(x => x.pk_Contribution == id);
        }

        /// <summary>
        /// Search for CR-Packs by TbId and searching keyword
        /// </summary>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        public List<View_CrPacks> GetCrPacksByTbIdAndKeywords(int tbId, string keywords)
        {
            var formatKeywords = keywords.ToLower().Trim();
            IQueryable<View_CrPacks> query = UoW.Context
                                                .View_CrPacks
                                                .Where(x =>  (x.uid.ToLower().Trim().Contains(formatKeywords) || x.title.ToLower().Trim().Contains(formatKeywords))
                                                          && (x.TB_ID == tbId))
                                                .OrderBy(x => x.uid);

            return query.ToList();
        }

        #endregion
    }

    public interface ICrPackRepository
    {
        /// <summary>
        /// Unit of Work
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Find CR Pack by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        View_CrPacks Find(int id);

        /// <summary>
        /// Search for CR-Packs by TbId and searching keyword
        /// </summary>
        /// <param name="tbId">Technical body identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <returns>Reserved CrPacks for the given Technical body</returns>
        List<View_CrPacks> GetCrPacksByTbIdAndKeywords(int tbId, string keywords);
    }
}
