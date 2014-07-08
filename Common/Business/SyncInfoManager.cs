using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Collections.Generic;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// SyncInfo manager class to get the SyncInfo data from repository
    /// </summary>
    public class SyncInfoManager
    {
        #region Variables

        private IUltimateUnitOfWork _uoW;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of SyncInfo Manager
        /// </summary>
        /// <param name="UoW">ultimate UnitOfWork</param>
        public SyncInfoManager(IUltimateUnitOfWork UoW) 
        {
            _uoW = UoW;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the list of SyncInfo objects for the given criteria
        /// </summary>
        /// <param name="terminalName">Terminal Name</param>
        /// <param name="offline_PkID">Offline Primary Key</param>
        /// <returns>List of Processed SyncInfo objects</returns>
        public List<SyncInfo> GetSyncInfo(string terminalName, int offline_PkID)
        {          
            var syncInfoRepo = RepositoryFactory.Resolve<ISyncInfoRepository>();
            syncInfoRepo.UoW = _uoW;
            return syncInfoRepo.Find(terminalName, offline_PkID);
        }

        #endregion        
    }   
}
