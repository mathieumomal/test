using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the remarks.
    /// </summary>
    public class RemarkService : IRemarkService, IOfflineService<Remark>
    {
        #region IRemarkService

        /// <summary>
        /// Delegats the GetRemarks to the repositories
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<List<Remark>> GetRemarks(string entityName, int primaryKey, int personId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var remarkMgr = new RemarkManager(uoW);
                    return remarkMgr.GetRemarks(entityName, primaryKey, personId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { entityName, primaryKey, personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public ServiceResponse<bool> UpdateRemarks(List<Remark> remarks, int personId)
        {
            try
            {

                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var remarkMgr = new RemarkManager(uoW);

                    var mgrResponse = remarkMgr.UpdateRemarks(remarks, personId);
                    if (mgrResponse.Result)
                    {
                        uoW.Save();
                    }
                    return mgrResponse;
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { remarks, personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                var response = new ServiceResponse<bool> { Result = false };
                response.Report.LogError(Localization.GenericError);
                return response;
            }
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
                        ExtensionLogger.Exception(ex, new List<object> { entity, terminalName }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                    ExtensionLogger.Exception(ex, new List<object> { entity }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                    ExtensionLogger.Exception(ex, new List<object> { primaryKey }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        #endregion
    }

    public interface IRemarkService
    {
        /// <summary>
        /// This method returns the remark for a given Version of SpecRelease entry
        /// </summary>
        /// <param name="entityName">"version" or "specrelease". Any other value will return an empty list</param>
        /// <param name="primaryKey">The id of the version or the specrelease object</param>
        /// <param name="personId">The id of the person performing the action</param>
        /// <returns>The list of remarks concerning the record.
        /// IMPORTANT: if user does not have right to see private remarks, the system will not provide them.</returns>
        ServiceResponse<List<Remark>> GetRemarks(string entityName, int primaryKey, int personId);

        /// <summary>
        /// Enables to update a list of remarks for specrelease or versions records. 
        /// In case of an update of an existing record, the system will only fetch the IsPublic parameter of the remark.
        /// 
        /// Note that it is not necessary that all the records concern the same version of specrelease.
        /// </summary>
        /// <param name="remarks"></param>
        /// <param name="personId"></param>
        /// <returns> true if things went well, else false</returns>
        ServiceResponse<bool> UpdateRemarks(List<Remark> remarks, int personId);
    }
}

