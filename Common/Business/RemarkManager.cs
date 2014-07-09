using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// This class is in charge of all the business logic concerning the remarks.
    /// </summary>
    public class RemarkManager
    {
        #region Variables

        private IUltimateUnitOfWork _uoW { get; set; } 

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor which will assign Ultimate unit of work 
        /// </summary>
        /// <param name="uow">Unit of Work</param>
        public RemarkManager(IUltimateUnitOfWork uow)
        {
            _uoW = uow;
        } 

        #endregion

        #region Offline Sync Methods

        /// <summary>
        /// Insert Remark entity
        /// </summary>
        /// <param name="entity">Remark</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Success/Failure</returns>
        public bool InsertEntity(Remark entity, string terminalName)
        {
            bool isSuccess = true;

            try
            {
                if (entity != null)
                {
                    SyncInfo syncInfo = new SyncInfo();
                    syncInfo.TerminalName = terminalName;
                    syncInfo.Offline_PK_Id = entity.Pk_RemarkId;
                    entity.SyncInfoes.Add(syncInfo);

                    IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                    offlineRepo.UoW = _uoW;
                    offlineRepo.InsertOfflineEntity(entity);
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Update Remark entity
        /// </summary>
        /// <param name="entity">Remark</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateEntity(Remark entity)
        {
            bool isSuccess = true;

            try
            {
                if (entity != null)
                {
                    //[1] Get the DB Version Entity
                    IRemarkRepository remarkRepo = RepositoryFactory.Resolve<IRemarkRepository>();
                    remarkRepo.UoW = _uoW;
                    Remark dbEntity = remarkRepo.Find(entity.Pk_RemarkId);

                    //Record may be deleted in serverside, while changes happen at offline
                    //So, priority is serverside, hence no more changes will update
                    if (dbEntity != null)
                    {
                        //[2] Compare & Update SpecVersion Properties
                        UpdateModifications(dbEntity, entity);

                        //[3] Update modified entity in Context
                        IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                        offlineRepo.UoW = _uoW;
                        offlineRepo.UpdateOfflineEntity(dbEntity);
                    }
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
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
            bool isSuccess = true;

            try
            {
                //[1] Get the DB Version Entity
                IRemarkRepository remarkRepo = RepositoryFactory.Resolve<IRemarkRepository>();
                remarkRepo.UoW = _uoW;
                Remark dbEntity = remarkRepo.Find(primaryKey);

                //Record may be deleted in serverside, while changes happen at offline
                //So, priority is serverside, hence no more changes will update
                if (dbEntity != null)
                {
                    //[2] Update modified entity in Context
                    IOfflineRepository offlineRepo = RepositoryFactory.Resolve<IOfflineRepository>();
                    offlineRepo.UoW = _uoW;
                    offlineRepo.DeleteOfflineEntity(dbEntity);
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update modified properties
        /// </summary>
        /// <param name="targetRemark">Target Remark</param>
        /// <param name="sourceRemark">Source Remark</param>
        private void UpdateModifications(Remark targetRemark, Remark sourceRemark)
        {
            if (targetRemark.Fk_PersonId != sourceRemark.Fk_PersonId)
                targetRemark.Fk_PersonId = sourceRemark.Fk_PersonId;
            if (targetRemark.IsPublic != sourceRemark.IsPublic)
                targetRemark.IsPublic = sourceRemark.IsPublic;
            if (targetRemark.CreationDate != sourceRemark.CreationDate)
                targetRemark.CreationDate = sourceRemark.CreationDate;
            if (targetRemark.RemarkText != sourceRemark.RemarkText)
                targetRemark.RemarkText = sourceRemark.RemarkText;
            if (targetRemark.Fk_VersionId != sourceRemark.Fk_VersionId)
                targetRemark.Fk_VersionId = sourceRemark.Fk_VersionId;
        }

        #endregion
    }
}
