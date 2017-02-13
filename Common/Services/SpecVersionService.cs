using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

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

        /// <summary>
        /// Get latest version of each relaease for the given spec ids
        /// </summary>
        /// <param name="specIds">The specification identifiders</param>
        /// <returns>List of Spec Versions</returns>
        public List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                specVersionManager.UoW = uoW;
                return specVersionManager.GetLatestVersionsBySpecIds(specIds);
            }
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int versionId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager {UoW = uoW};
                return specVersionManager.GetSpecVersionById(versionId, personId);
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public ServiceResponse<VersionForCrListFacade> GetVersionNumberWithSpecNumberByVersionId(int personId, int versionId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager {UoW = uoW};
                return specVersionManager.GetVersionNumberWithSpecNumberByVersionId(personId, versionId);
            }
        }

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
                var specVersionAllocateAction = new SpecVersionAllocateAction {UoW = uoW};
                var result = specVersionAllocateAction.AllocateVersion(personId, version);

                if (result != null && (result.Report.ErrorList == null || result.Report.ErrorList.Count() == 0))
                {
                    uoW.Save();
                }

                return result.Report;
            }
        }

        /// <summary>
        /// Check a version before upload it
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="version"></param>
        /// <param name="path"></param>
        /// <param name="shouldAvoidQualityChecks"></param>
        /// <returns></returns>
        public ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path, bool shouldAvoidQualityChecks = false)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionUploadAction = new SpecVersionUploadAction {UoW = uoW};
                return specVersionUploadAction.CheckVersionForUpload(personId, version, path, shouldAvoidQualityChecks);
            }
        }

        /// <summary>
        /// Upload a version
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ServiceResponse<string> UploadVersion(int personId, string token)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionUploadAction = new SpecVersionUploadAction {UoW = uoW};
                var result = specVersionUploadAction.UploadVersion(personId, token);
                uoW.Save();
                return result;
            }
        }

        /// <summary>
        /// Delete a specification version
        /// </summary>
        /// <param name="personId">UserId to check rights</param>
        /// <param name="versionId">Version Id</param>
        /// <returns>serviceResponse</returns>
        public ServiceResponse<bool> DeleteVersion(int personId, int versionId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var response = new ServiceResponse<bool>();
                try
                {
                    var specVersionManager = new SpecVersionsManager {UoW = uoW};
                    response = specVersionManager.DeleteVersion(personId, versionId);
                    if(response.Result)
                        uoW.Save();
                }
                catch (Exception e)
                {
                    LogManager.Error("SpecVersionService - DeleteVersion : an unexpected error occured", e);
                    response.Result = false;
                    response.Report.LogError(Localization.GenericError);
                }
                return response;
            }
        } 

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager();
                specVersionManager.UoW = uoW;
                return specVersionManager.CountVersionsPendingUploadByReleaseId(releaseId);
            }
        }

        /// <summary>
        /// Update version
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<SpecVersion> UpdateVersion(SpecVersion version, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                specVersionManager.UoW = uoW;
                var result = specVersionManager.UpdateVersion(version, personId);
                if(result.Result != null && result.Report.GetNumberOfErrors() == 0)
                    uoW.Save();
                else
                    LogManager.Error(Localization.GenericError + ": " + string.Join(", ", result.Report.ErrorList));
                return result;
            }
        }

        /// <summary>
        /// Check if user is allowed to edit version numbers
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        public ServiceResponse<bool> CheckVersionNumbersEditAllowed(SpecVersion version, int personId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var manager = ManagerFactory.Resolve<ISpecVersionManager>();
                    manager.UoW = uow;
                    return manager.CheckVersionNumbersEditAllowed(version, personId);
                }
                catch (Exception e)
                {
                    LogManager.Error(string.Format("Error occured when trying to check if user have right to edit version number -> versionId:{0}", version.Pk_VersionId), e);
                    return new ServiceResponse<bool> { Result = false, Report = new Report { ErrorList = new List<string> { Localization.GenericError } } };
                }
            }
        }

        /// <summary>
        /// Create & Fill version Lastest Folder
        /// </summary>
        /// <returns>Service response bool</returns>
        public ServiceResponse<bool> CreateAndFillVersionLatestFolder(string folderName, int personId)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>() { Result = false };

            try
            {
                var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();

                /* Checks */
                using (var UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    ftpFoldersManager.UoW = UoW;
                    response = ftpFoldersManager.CheckLatestFolder(folderName, personId);
                }

                if (response.Result)
                {
                    /* If checks OK, then create latest folder */
                    ftpFoldersManager.CreateAndFillVersionLatestFolder(folderName, personId);
                }
            }
            catch (Exception e)
            {
                response.Result = false;
                response.Report.LogError(Localization.GenericError);
                LogManager.Error("[Service] Failed to Create and fill version latest folder", e);
            }
            
            return response;
        }

        /// <summary>
        /// Get Ftp Folders Manager Status (from cache)
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        public FtpFoldersManagerStatus GetFtpFoldersManagerStatus()
        {
            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            return ftpFoldersManager.GetStatus();
        }

        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        public string GetFTPLatestFolderName()
        {
            try
            {
                using (var UoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
                    ftpFoldersManager.UoW = UoW;
                    return ftpFoldersManager.GetFTPLatestFolderName();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Clear cache of Ftp Folders Manager Status
        /// </summary>
        public void ClearFtpFoldersManagerStatus()
        {
            var ftpFoldersManager = ManagerFactory.Resolve<IFtpFoldersManager>();
            ftpFoldersManager.ClearStatus();
        }

        /// <summary>
        /// return true if a copy is in progress
        /// </summary>
        /// <returns>True or False</returns>
        public bool IsCopyLatestFolderInProgress()
        {
            var result = GetFtpFoldersManagerStatus();
            return result != null && !result.Finished;
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
            int primaryKeyId = 0;

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
                            primaryKeyId = syncInfo.Fk_VersionId ?? 0;
                        }
                        else if (specVersionManager.InsertEntity(entity, terminalName))
                        {
                            uoW.Save();
                            primaryKeyId = entity.Pk_VersionId;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("[Offline] Specification Insert Error: " + ex.Message);
                        if (ex.InnerException != null)
                            LogManager.Error("Inner Exception: " + ex.InnerException);
                    }
                }
            }

            return primaryKeyId;
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
                    LogManager.Error("[Offline] Specification Update Error: " + ex.Message);
                    if (ex.InnerException != null)
                        LogManager.Error("Inner Exception: " + ex.InnerException);
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
                    LogManager.Error("[Offline] Specification Delete Error: " + ex.Message);
                    if (ex.InnerException != null)
                        LogManager.Error("Inner Exception: " + ex.InnerException);
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

        /// <summary>
        /// Link TDoc to Version
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId">The specification identifier</param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <param name="relatedTdoc">Related Tdoc</param>
        /// <param name="releaseId"></param>
        /// <returns>Success/Failure status</returns>
        public ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc)
        {
            var svcResponse = new ServiceResponse<bool>();

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                    specVersionManager.UoW = uoW;
                    svcResponse = specVersionManager.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion, editorialVersion, relatedTdoc);
                    if (svcResponse.Result)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                svcResponse.Result = false;
                svcResponse.Report.LogError(ex.Message);
                LogManager.Error("AllocateOrAssociateDraftVersion : an unexpected error occured", ex);
            }

            return svcResponse;
        }

        /// <summary>
        /// Checks the draft creation or association.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specId">The spec identifier.</param>
        /// <param name="releaseId">The release identifier.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <returns>Draft creation or association status along with validation failures</returns>
        public ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = new ServiceResponse<bool>();

            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                    specVersionManager.UoW = uoW;
                    svcResponse = specVersionManager.CheckDraftCreationOrAssociation(personId, specId, releaseId, majorVersion, technicalVersion, editorialVersion);
                }
            }
            catch (Exception ex)
            {
                svcResponse.Result = false;
                svcResponse.Report.LogError(ex.Message);
            }

            return svcResponse;
        }

        /// <summary>
        /// Unlink tdoc from related version
        /// </summary>
        /// <param name="uid">Tdoc uid</param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        public ServiceResponse<bool> UnlinkTdocFromVersion(string uid, int personId)
        {
            var svcResponse = new ServiceResponse<bool>();
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                    specVersionManager.UoW = uoW;
                    svcResponse = specVersionManager.UnlinkTdocFromVersion(uid, personId);
                    if(svcResponse.Result && svcResponse.Report.GetNumberOfErrors() <= 0)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                svcResponse.Result = false;
                svcResponse.Report.LogError(ex.Message);
                LogManager.Error("Unexpected error occured when system trying to unlink tdoc from version", ex);
            }
            return svcResponse;
        }

        /// <summary>
        /// Create version for pCR tdoc if necessary (if doesn't already exist)
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <returns></returns>
        public ServiceResponse<bool> CreatepCrDraftVersionIfNecessary(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = new ServiceResponse<bool>();
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specVersionManager = ManagerFactory.Resolve<ISpecVersionManager>();
                    specVersionManager.UoW = uoW;
                    svcResponse = specVersionManager.CreatepCrDraftVersionIfNecessary(personId, specId, releaseId, meetingId, majorVersion, technicalVersion, editorialVersion);
                    if (svcResponse.Result && svcResponse.Report.GetNumberOfErrors() <= 0)
                        uoW.Save();
                }
            }
            catch (Exception ex)
            {
                svcResponse.Result = false;
                svcResponse.Report.LogError(ex.Message);
                LogManager.Error("Unexpected error occured when system trying to create a pCR draft version", ex);
            }
            return svcResponse;
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
        /// Get latest version of each relaease for the given spec ids
        /// </summary>
        /// <param name="specIds">The specification identifiders</param>
        /// <returns>List of Spec Versions</returns>
        List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds);

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <param name="personId"></param>
        /// <returns>A couple (version,userrights)</returns>
        KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int versionId, int personId);

        /// <summary>
        /// Get version 'number' and spec number related to a version by a versionId
        /// </summary>
        /// <returns></returns>
        ServiceResponse<VersionForCrListFacade> GetVersionNumberWithSpecNumberByVersionId(int personId, int versionId);

        /// <summary>
        /// Returns the next version which is candidate for upload or allocation.
        /// 
        /// This system will :
        /// - In case of allocation (upload = false), return the next available version for allocation
        /// - in case of upload (upload = true), compute the next version that should be uploaded.
        /// </summary>
        /// <param name="personId">Person requesting the update.</param>
        /// <param name="specId">The target specification ID</param>
        /// <param name="releaseId">The target Release ID</param>
        /// <param name="forUpload">true if next version is to be uploaded, false if not.</param>
        /// <returns></returns>
        ServiceResponse<SpecVersionCurrentAndNew> GetNextVersionForSpec(int personId, int specId, int releaseId, bool forUpload);

        /// <summary>
        /// Enables user to allocate version.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        Report AllocateVersion(int personId, SpecVersion version);

        /// <summary>
        /// Count the number of (latest) versions which pending upload
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        int CountVersionsPendingUploadByReleaseId(int releaseId);

        /// <summary>
        /// Perform validation checks before upload a version 
        /// </summary>
        /// <param name="personId">The person identifier</param>
        /// <param name="version">The version</param>
        /// <param name="path">The upload path</param>
        /// <param name="shouldAvoidQualityChecks">True : Avoid quality checks, False : run quality checks before upload version</param>
        /// <returns>GUID of cached version information</returns>
        ServiceResponse<string> CheckVersionForUpload(int personId, SpecVersion version, string path, bool shouldAvoidQualityChecks = false);

        /// <summary>
        /// Upload version
        /// </summary>
        /// <param name="personId">The person identifier</param>
        /// <param name="token">GUID of cached version information</param>
        /// <returns>Upload status</returns>
        ServiceResponse<string> UploadVersion(int personId, string token);

        /// <summary>
        /// Link TDoc to Version
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId">The specification identifier</param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <param name="relatedTdoc">Related Tdoc</param>
        /// <param name="releaseId"></param>
        /// <returns>Success/Failure status</returns>
        ServiceResponse<bool> AllocateOrAssociateDraftVersion(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc);

        /// <summary>
        /// Checks the draft creation or association.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specId">The spec identifier.</param>
        /// <param name="releaseId">The release identifier.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <param name="technicalVersion">The technical version.</param>
        /// <param name="editorialVersion">The editorial version.</param>
        /// <returns>Draft creation or association status along with validation failures</returns>
        ServiceResponse<bool> CheckDraftCreationOrAssociation(int personId, int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion);

        /// <summary>
        /// See implementation
        /// </summary>
        /// <param name="version"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        ServiceResponse<SpecVersion> UpdateVersion(SpecVersion version, int personId);

        /// <summary>
        /// Unlink tdoc from related version
        /// </summary>
        /// <param name="uid">Tdoc uid</param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        ServiceResponse<bool> UnlinkTdocFromVersion(string uid, int personId);

        /// <summary>
        /// Check if user is allowed to edit version numbers
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="personId"></param>
        /// <returns>True for success case</returns>
        ServiceResponse<bool> CheckVersionNumbersEditAllowed(SpecVersion version, int personId);

        /// <summary>
        /// Create & Fill version Lastest Folder
        /// </summary>
        /// <returns>Service response bool</returns>
        ServiceResponse<bool> CreateAndFillVersionLatestFolder(string folderName, int personId);

        /// <summary>
        /// Get Ftp Folders Manager Status (from cache)
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        FtpFoldersManagerStatus GetFtpFoldersManagerStatus();

        /// <summary>
        /// return true if a copy is in progress
        /// </summary>
        /// <returns>True or False</returns>
        bool IsCopyLatestFolderInProgress();

        /// <summary>
        /// Clear cache of Ftp Folders Manager Status
        /// </summary>
        void ClearFtpFoldersManagerStatus();

        
        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        string GetFTPLatestFolderName();

        /// <summary>
        /// Create version for pCR tdoc if necessary (if doesn't already exist)
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <returns></returns>
        ServiceResponse<bool> CreatepCrDraftVersionIfNecessary(int personId, int specId, int releaseId,
            int meetingId, int majorVersion, int technicalVersion, int editorialVersion);
    }
}

