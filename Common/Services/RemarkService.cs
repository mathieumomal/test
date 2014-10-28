using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the remarks.
    /// </summary>
    public class RemarkService : IRemarkService, IOfflineService<Remark>
    {
        #region IRemarkService

        public ServiceResponse<List<Remark>> GetRemarks(string entityName, int primaryKey, int personId)
        {
            var remarks = new ServiceResponse<List<Remark>>();

            if (!String.IsNullOrEmpty(entityName))
            {
                switch (entityName.ToLower())
                {
                    case "version":
                        break;
                    case "specrelease":
                        break;
                }
            }

            return remarks;
        }

        public ServiceResponse<bool> UpdateRemarks(List<Remark> remarks, int personId)
        {
            var isSuccess = new ServiceResponse<bool>();

            return isSuccess;
        }

        #endregion

        #region IOfflineService Members

        /// <summary>
        /// Insert Remark entity
        /// </summary>
        /// <param name="entity">Remark</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Inserted Identity</returns>
        public int InsertEntity(Remark entity, string terminalName)
        {
            int primaryKeyID = 0;

            if (entity != null)
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    try
                    {
                        var remarkManager = new RemarkManager(uoW);
                        //Check whether insert already processed
                        var syncInfoManager = new SyncInfoManager(uoW);
                        List<SyncInfo> syncInfos = syncInfoManager.GetSyncInfo(terminalName, entity.Pk_RemarkId);
                        var syncInfo = syncInfos.Where(x => x.Fk_RemarkId != null).FirstOrDefault();
                        if (syncInfo != null)
                        {
                            primaryKeyID = syncInfo.Fk_RemarkId ?? 0;
                        }
                        else if (remarkManager.InsertEntity(entity, terminalName))
                        {
                            uoW.Save();
                            primaryKeyID = entity.Pk_RemarkId;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("[Offline] Remark Insert Error: " + ex.Message);
                        if (ex.InnerException != null)
                            LogManager.Error("Inner Exception: " + ex.InnerException);
                    }
                }
            }

            return primaryKeyID;
        }

        /// <summary>
        /// Update Remark entity
        /// </summary>
        /// <param name="entity">Remark</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateEntity(Remark entity)
        {
            bool isSuccess = false;

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var remarkManager = new RemarkManager(uoW);
                    if (remarkManager.UpdateEntity(entity))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error("[Offline] Remark Update Error: " + ex.Message);
                    if (ex.InnerException != null)
                        LogManager.Error("Inner Exception: " + ex.InnerException);
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Delete Remark entity
        /// </summary>
        /// <param name="primaryKey">Primary Key</param>
        /// <returns>Success/Failure</returns>
        public bool DeleteEntity(int primaryKey)
        {
            bool isSuccess = false;

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var remarkManager = new RemarkManager(uoW);
                    if (remarkManager.DeleteEntity(primaryKey))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error("[Offline] Remark Delete Error: " + ex.Message);
                    if (ex.InnerException != null)
                        LogManager.Error("Inner Exception: " + ex.InnerException);
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        #endregion
    }

    public interface IRemarkService
    {
        ServiceResponse<List<Remark>> GetRemarks(string entityName, int primaryKey, int personId);
        ServiceResponse<bool> UpdateRemarks(List<Remark> remarks, int personId);
    }
}

