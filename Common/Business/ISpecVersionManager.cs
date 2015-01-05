using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public interface ISpecVersionManager
    {
        IUltimateUnitOfWork UoW { get; set; }

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
        List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId);

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <param name="personId"></param>
        /// <returns>A couple (version,userrights)</returns>
        KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId);

        /// <summary>
        /// Get version 'number' and spec number related to a version by a versionId
        /// </summary>
        /// <returns></returns>
        ServiceResponse<VersionForCrListFacade> GetVersionNumberWithSpecNumberByVersionId(int personId, int versionId);

        /// <summary>
        /// Allocate version for a set of promoted specification for a release
        /// </summary>
        /// <param name="specifications"></param>
        /// <param name="release">Target release of promotion</param>
        /// <param name="personId"></param>
        /// <returns></returns>
        List<Report> AllocateVersionFromMassivePromote(List<Specification> specifications, Release release, int personId);

        /// <summary>
        /// Insert SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <param name="terminalName">Terminal Name</param>
        /// <returns>Success/Failure</returns>
        bool InsertEntity(SpecVersion entity, string terminalName);

        /// <summary>
        /// Update SpecVersion entity
        /// </summary>
        /// <param name="entity">SpecVersion</param>
        /// <returns>Success/Failure</returns>
        bool UpdateEntity(SpecVersion entity);
        
        /// <summary>
        /// Delete SpecVersion entity
        /// </summary>
        /// <param name="primaryKey">Primary Key</param>
        /// <returns>Success/Failure</returns>
        bool DeleteEntity(int primaryKey);

        /// <summary>
        /// Count the number of (latest) versions which pending upload
        /// </summary>
        /// <returns></returns>
        int CountVersionsPendingUploadByReleaseId(int releaseId);

    }
}
