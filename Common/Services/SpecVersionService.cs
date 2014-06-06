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

