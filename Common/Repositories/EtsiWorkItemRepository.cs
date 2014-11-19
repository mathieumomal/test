using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Implementation of the IEtsiWorkItemRepository. Seeks data in the U3GPPDB view 
    /// that points to WPMDB
    /// </summary>
    public class EtsiWorkItemRepository: IEtsiWorkItemRepository
    {
        /// <summary>
        /// Unit of work containing the EF context.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="wiIdsList"></param>
        /// <returns></returns>
        public List<ETSI_WorkItem> GetWorkItemsByIds(List<int> wiIdsList)
        {
            return UoW.Context.ETSI_WorkItem.Where(wi => wiIdsList.Contains(wi.WKI_ID)).ToList();
        }
    }

    /// <summary>
    /// Repository that returns ETSI_WorkItems related information
    /// </summary>
    public interface IEtsiWorkItemRepository
    {
        /// <summary>
        /// Unit of work.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Returns the list of WIs corresponding to the provided Ids.
        /// </summary>
        /// <param name="wiIdsList"></param>
        /// <returns></returns>
        List<ETSI_WorkItem> GetWorkItemsByIds(List<int> wiIdsList);
    }
}
