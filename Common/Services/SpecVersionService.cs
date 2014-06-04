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
    public class SpecVersionService : ISpecVersionService
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

        public bool AllocateVersion(SpecVersion version, int oldVersionId)
        {
            bool operationResult = true;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {                    
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.CheckIfVersionAlloawed(version, specVersionManager.GetSpecVersionById(oldVersionId, 0).Key, false))
                    {
                        specVersionManager.UploadOrAllocateVersion(version);
                        uoW.Save();
                    }
                    else
                    {
                        operationResult = false;
                    }
                    
                }
                catch (Exception ex)
                {
                    operationResult = false;
                }
            }
            return operationResult;
        }

        public bool UploadVersion(SpecVersion version, int oldVersionId)
        {
            bool operationResult = true;            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager(uoW);
                    if (specVersionManager.CheckIfVersionAlloawed(version, specVersionManager.GetSpecVersionById(oldVersionId, 0).Key, true))
                    {
                        specVersionManager.UploadOrAllocateVersion(version);
                        uoW.Save();
                    }
                    else
                    {
                        operationResult = false;
                    }

                }
                catch (Exception ex)
                {
                    operationResult = false;
                }
            }
            return operationResult;
        }

        /// <summary>
        /// Validate Uploaded version document & provide validation summary
        /// </summary>
        /// <param name="fileExtension">File Extension (.doc/.docx)</param>
        /// <param name="memoryStream">Memory Stream</param>
        /// <param name="version">Specification Version</param>
        /// <param name="title">Specification Title</param>
        /// <param name="release">Specification Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>Validation Summary</returns>
        public Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string version, string title, string release, DateTime meetingDate)
        {
            Report validationReport;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specVersionManager = new SpecVersionsManager(uoW);
                    validationReport = specVersionManager.ValidateVersionDocument(fileExtension, memoryStream, version, title, release, meetingDate);
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
    }
}

