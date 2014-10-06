using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Default implementation of the IChangeRequestStatusManager
    /// </summary>
    public class ChangeRequestStatusManager: IChangeRequestStatusManager
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        private const string CacheKey = "ULTIMATE_BIZ_CHANGEREQUEST_STATUSES";

        /// <summary>
        /// Fetches the list of all change request statuses by the repository. 
        /// Caches the information.
        /// </summary>
        /// <returns></returns>
        public List<Enum_ChangeRequestStatus> GetAllChangeRequestStatuses()
        {
            var cachedData = (List<Enum_ChangeRequestStatus>)CacheManager.Get(CacheKey);
            if (cachedData == null)
            {
                var crStatusRepo = RepositoryFactory.Resolve<IChangeRequestStatusRepository>();
                crStatusRepo.UoW = UoW;

                cachedData = crStatusRepo.All.ToList();
                CacheManager.Insert(CacheKey, cachedData);
            }
            return cachedData;
        }

    }
}
