using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public class SpecVersionService : ISpecVersionService, IOfflineService<SpecVersion>
    {
        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetVersionsBySpecId(specificationId);
            }
        }

        public List<SpecVersion> GetVersionsForSpecRelease(int specificationId, int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetVersionsForASpecRelease(specificationId, releaseId);
            }
        }

        public KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int VersionId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specVersionManager = new SpecVersionsManager(uoW);
                return specVersionManager.GetSpecVersionById(VersionId, personId);
            }
        }

        /// <summary>
        /// Allocate/Upload a version
        /// </summary>
        /// <param name="version">Version to allocate/upload</param>
        /// <returns>Result of the operation</returns>
        public Report UploadOrAllocateVersion(SpecVersion version, bool isDraft)
        {
            Report result = new Report();
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager(uoW);
                    result = specVersionManager.UploadOrAllocateVersion(version, isDraft);

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
                    var specVersionManager = new SpecVersionsManager(uoW);
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

        #region IOfflineService Members

        /// <summary>
        /// Insert SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <returns>Success/Failure</returns>
        public int InsertEntity(SpecVersion entity)
        {
            int primaryKeyID = 0;

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.InsertEntity(entity))
                    {
                        uoW.Save();
                        primaryKeyID = entity.Pk_VersionId;
                    }
                }
                catch (Exception)
                {
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
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.UpdateEntity(entity))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception)
                {
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
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.DeleteEntity(primaryKey))
                    {
                        uoW.Save();
                        isSuccess = true;
                    }
                }
                catch (Exception)
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        #endregion
    }
}

