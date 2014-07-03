using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
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
        /// Allocate/Upload a version
        /// </summary>
        /// <param name="version">Version to allocate/upload</param>
        /// <returns>Result of the operation</returns>
        Report UploadOrAllocateVersion(SpecVersion version, bool isDraft, int personId);

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
    }
}
