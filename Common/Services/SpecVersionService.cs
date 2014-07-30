﻿using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.SpecVersionBusiness;

namespace Etsi.Ultimate.Services
{
    public class SpecVersionService : ISpecVersionService, IOfflineService<SpecVersion>
    {

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager();
                specVersionManager.UoW = uoW;
                return specVersionManager.GetVersionsBySpecId(specificationId);
            }
        }

        public List<SpecVersion> GetVersionsForSpecRelease(int specificationId, int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager();
                specVersionManager.UoW = uoW;
                return specVersionManager.GetVersionsForASpecRelease(specificationId, releaseId);
            }
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int VersionId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager();
                specVersionManager.UoW = uoW;
                return specVersionManager.GetSpecVersionById(VersionId, personId);
            }
        }

        //TO REMOVE -----------------------------------------
        /// <summary>
        /// OLD : Allocate/Upload a version
        /// </summary>
        /// <param name="version">Version to allocate/upload</param>
        /// <returns>Result of the operation</returns>
        public Report UploadOrAllocateVersion(SpecVersion version, bool isDraft, int personId, Report report = null)
        {
            Report result = new Report();
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager();
                    specVersionManager.UoW = uoW;
                    result = specVersionManager.UploadOrAllocateVersion(version, isDraft, personId, report);

                    if (result.ErrorList.Count == 0)
                        uoW.Save();
                }
                catch (Exception ex)
                {
                    result.LogError(ex.Message);
                }
            }
            return result;
        }
        //TO REMOVE -----------------------------------------

        /// <summary>
        /// Allocate a version
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public Report AllocateVersion(int personId, SpecVersion version)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionAllocateAction = new SpecVersionAllocateAction();
                specVersionAllocateAction.UoW = uoW;
                var result = specVersionAllocateAction.AllocateVersion(personId, version);
                uoW.Save();

                return result;
            }
        }

        /// <summary>
        /// Check a version before upload it
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionUploadAction = new SpecVersionUploadAction();
                specVersionUploadAction.UoW = uoW;
                return specVersionUploadAction.CheckVersionForUpload(personId, version, path);
            }
        }

        /// <summary>
        /// Upload a version
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public ServiceResponse<string> UploadVersion(int personId, SpecVersion version, string token)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionUploadAction = new SpecVersionUploadAction();
                specVersionUploadAction.UoW = uoW;
                return specVersionUploadAction.UploadVersion(personId, version, token);
            }
        }


        /// <summary>
        /// Validate Uploaded version document & provide validation summary
        /// </summary>
        /// <param name="fileExtension">File Extension (.doc/.docx)</param>
        /// <param name="memoryStream">Memory Stream</param>
        /// <param name="temporaryFolder">Temporary Folder</param>
        /// <param name="version">Specification Version</param>
        /// <param name="title">Specification Title</param>
        /// <param name="release">Specification Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <param name="tsgTitle">Technical Specificaion Group Title</param>
        /// <param name="isTS">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>Validation Summary</returns>
        public Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string temporaryFolder, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS)
        {
            Report validationReport;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager();
                    specVersionManager.UoW = uoW;
                    validationReport = specVersionManager.ValidateVersionDocument(fileExtension, memoryStream, temporaryFolder, version, title, release, meetingDate, tsgTitle, isTS);
                }
                catch (Exception ex)
                {
                    string errorMessage = "Version Document Validation Error: " + ex.Message;
                    validationReport = new Report();
                    validationReport.LogError(errorMessage);
                    Utils.LogManager.Error(errorMessage);
                }
            }
            return validationReport;
        }

        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager();
                specVersionManager.UoW = uoW;
                return specVersionManager.CountVersionsPendingUploadByReleaseId(releaseId);
            }
        }

        #region IOfflineService Members

        /// <summary>
        /// Insert SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Inserted Identity</returns>
        public int InsertEntity(SpecVersion entity, string terminalName)
        {
            int primaryKeyID = 0;

            if (entity != null)
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    try
                    {
                        var specVersionManager = new SpecVersionsManager();
                        specVersionManager.UoW = uoW;
                        //Check whether insert already processed
                        var syncInfoManager = new SyncInfoManager(uoW);
                        List<SyncInfo> syncInfos = syncInfoManager.GetSyncInfo(terminalName, entity.Pk_VersionId);
                        var syncInfo = syncInfos.Where(x => x.Fk_VersionId != null).FirstOrDefault();
                        if (syncInfo != null)
                        {
                            primaryKeyID = syncInfo.Fk_VersionId ?? 0;
                        }
                        else if (specVersionManager.InsertEntity(entity, terminalName))
                        {
                            uoW.Save();
                            primaryKeyID = entity.Pk_VersionId;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.LogManager.Error("[Offline] Specification Insert Error: " + ex.Message);
                        if (ex.InnerException != null)
                            Utils.LogManager.Error("Inner Exception: " + ex.InnerException);
                    }
                }
            }

            return primaryKeyID;
        }

        /// <summary>
        /// Update SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <returns>Success/Failure</returns>
        public bool UpdateEntity(SpecVersion entity)
        {
            bool isSuccess = false;

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager();
                    specVersionManager.UoW = uoW;
                    if (specVersionManager.UpdateEntity(entity))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogManager.Error("[Offline] Specification Update Error: " + ex.Message);
                    if (ex.InnerException != null)
                        Utils.LogManager.Error("Inner Exception: " + ex.InnerException);
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// Delete SpecVersion entity
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
                    var specVersionManager = new SpecVersionsManager();
                    specVersionManager.UoW = uoW;
                    if (specVersionManager.DeleteEntity(primaryKey))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    Utils.LogManager.Error("[Offline] Specification Delete Error: " + ex.Message);
                    if (ex.InnerException != null)
                        Utils.LogManager.Error("Inner Exception: " + ex.InnerException);
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        #endregion

        #region ISpecVersionService Members


        public ServiceResponse<SpecVersionCurrentAndNew> GetNextVersionForSpec(int personId, int specId, int releaseId, bool forUpload)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var versionMgr = new GetNextReleaseAction();
                versionMgr.UoW = uoW;
                return versionMgr.GetNextVersionForSpec(personId, specId, releaseId, forUpload);
            }
        }

        #endregion
    }

    public interface ISpecVersionService
    {
        /// <summary>
        /// Returns a list of specVersion of a specification
        /// </summary>
        /// <param name="specificationId">Specification Id</param>
        /// <returns>List of SpecVersions including related releases</returns>
        List<SpecVersion> GetVersionsBySpecId(int specificationId);

        /// <summary>
        /// Returns the list of versions of a specification release
        /// </summary>
        /// <param name="specificationId">The specification identifier</param>
        /// <param name="releaseId">The identifier of the specification's release</param>
        /// <returns>List of versions objects</returns>
        List<SpecVersion> GetVersionsForSpecRelease(int specificationId, int releaseId);

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <returns>A couple (version,userrights)</returns>
        KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int VersionId, int personId);

        /// <summary>
        /// Returns the next version which is candidate for upload or allocation.
        /// 
        /// This system will :
        /// - In case of allocation (upload = false), return the next available version for allocation
        /// - in case of upload (upload = true), compute the next version that should be uploaded.
        /// </summary>
        /// <param name="personId">Person requesting the update.</param>
        /// <param name="SpecId">The target specification ID</param>
        /// <param name="ReleaseId">The target Release ID</param>
        /// <param name="forUpload">true if next version is to be uploaded, false if not.</param>
        /// <returns></returns>
        ServiceResponse<SpecVersionCurrentAndNew> GetNextVersionForSpec(int personId, int SpecId, int ReleaseId, bool forUpload);

        /// <summary>
        /// Enables user to allocate version.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        Report AllocateVersion(int personId, SpecVersion version);

        /// <summary>
        /// Allocate/Upload a version
        /// </summary>
        /// <param name="version">Version to allocate/upload</param>
        /// <returns>Result of the operation</returns>
        Report UploadOrAllocateVersion(SpecVersion version, bool isDraft, int personId, Report report);

        /// <summary>
        /// Validate Uploaded version document & provide validation summary
        /// </summary>
        /// <param name="fileExtension">File Extension (.doc/.docx)</param>
        /// <param name="memoryStream">Memory Stream</param>
        /// <param name="temporaryFolder">Temporary Folder</param>
        /// <param name="version">Specification Version</param>
        /// <param name="title">Specification Title</param>
        /// <param name="release">Specification Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <param name="tsgTitle">Technical Specificaion Group Title</param>
        /// <param name="isTS">True - Technical Specificaiton / False - Technical Report</param>
        /// <returns>Validation Summary</returns>
        Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string temporaryFolder, string version, string title, string release, DateTime meetingDate, string tsgTitle, bool isTS);

        /// <summary>
        /// Count the number of (latest) versions which pending upload
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        int CountVersionsPendingUploadByReleaseId(int releaseId);

        ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path);
        ServiceResponse<string> UploadVersion(int personId, SpecVersion version, string token);
    }
}

