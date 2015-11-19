using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Abstracts the release manager. Provide a number of services.
    /// </summary>
    public interface IReleaseManager
    {
        /// <summary>
        /// List all the releases.
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>List of Releases</returns>
        KeyValuePair<List<Release>, UserRightsContainer> GetAllReleases(int personID);

        /// <summary>
        /// List all the releases filtered by status.
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <param name="releaseStatus">Person ID</param>
        /// <returns>List of Releases</returns>
        KeyValuePair<List<Release>, UserRightsContainer> GetAllReleasesByStatus(int personID,
            string releaseStatus);

        /// <summary>
        /// Return release details based on the release ID. Also computes the rights of the user 
        /// concerning this release, against the personId.
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="id">Release ID</param>
        /// <returns>Release Details</returns>
        KeyValuePair<Release, UserRightsContainer> GetReleaseById(int personId, int id);

        /// <summary>
        /// Closes the release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="closureDate">Closure Date</param>
        /// <param name="closureMtgRef">Closure Meeting Reference</param>
        /// <param name="closureMtgId">Closure Meeting ID</param>
        /// <param name="personID">Person ID</param>
        ServiceResponse<bool> CloseRelease(int releaseId, DateTime? closureDate, string closureMtgRef, int? closureMtgId, int personID);

        /// <summary>
        /// Creates the release
        /// </summary>
        /// <param name="release">Release</param>
        /// <param name="previousReleaseId">Previous Release ID</param>
        /// <param name="personId">Person ID</param>
        /// <returns>New Release</returns>
        Release CreateRelease(Release release, int previousReleaseId, int personId);

        /// <summary>
        /// Edits the release
        /// </summary>
        /// <param name="release">Release</param>
        /// <param name="previousReleaseId">Previous Release ID</param>
        /// <param name="personId">Person ID</param>
        void EditRelease(Etsi.Ultimate.DomainClasses.Release release, int previousReleaseId, int personId);

        /// <summary>
        /// Freezes the release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="endDate">End Date</param>
        /// <param name="personId">Person ID</param>
        /// <param name="FreezeMtgId">Freeze Meeting ID</param>
        /// <param name="FreezeMtgRef">Freeze Meeting Reference</param>
        ServiceResponse<bool> FreezeRelease(int releaseId, DateTime? endDate, int personId, int? FreezeMtgId, string FreezeMtgRef);

        /// <summary>
        /// Lists all the releases
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <returns>Release Codes</returns>
        Dictionary<int, string> GetAllReleasesCodes(int releaseId);

        /// <summary>
        /// Computes the release code for the previous release.
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <returns>Previous Release Code</returns>
        KeyValuePair<int, string> GetPreviousReleaseCode(int releaseId);

        /// <summary>
        /// The unit of work with which  the release manager can work
        /// </summary>
        Etsi.Ultimate.Repositories.IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get next release details for the given current release id.
        /// </summary>
        /// <param name="releaseId">Relese ID</param>
        /// <returns>Next Release</returns>
        Release GetNextRelease(int releaseId);

        /// <summary>
        /// Get previous release details for the given current release id.
        /// </summary>
        /// <param name="releaseId">Relese ID</param>
        /// <returns>Previous Release</returns>
        Release GetPreviousRelease(int releaseId);

        /// <summary>
        /// Get releases linked to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <returns>List of releases linked to spec provided</returns>
        List<Release> GetReleasesLinkedToASpec(int specId);
    }
}
