using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Classe managing the change request statuses
    /// </summary>
    public interface IChangeRequestStatusManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Retrieves the change request statuses
        /// </summary>
        List<Enum_ChangeRequestStatus> GetAllChangeRequestStatuses();        
    }
}
