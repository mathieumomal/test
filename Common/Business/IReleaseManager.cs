using System;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;


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
        /// <param name="personID"></param>
        /// <returns></returns>
        KeyValuePair<List<Release>, UserRightsContainer> GetAllReleases(int personID);

        /// <summary>
        /// Return release details based on the release ID. Also computes the rights of the user 
        /// concerning this release, against the personId.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        KeyValuePair<Release, UserRightsContainer> GetReleaseById(int personId, int id);

        /// <summary>
        /// Closes the release
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="closureDate"></param>
        /// <param name="closureMtgRef"></param>
        /// <param name="closureMtgId"></param>
        /// <param name="personID"></param>
        void CloseRelease(int releaseId, DateTime? closureDate, string closureMtgRef, int? closureMtgId, int personID);

        /// <summary>
        /// Creates the release
        /// </summary>
        /// <param name="release"></param>
        /// <param name="previousReleaseId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        Release CreateRelease(Release release, int previousReleaseId, int personId);

        /// <summary>
        /// Edits the release
        /// </summary>
        /// <param name="release"></param>
        /// <param name="previousReleaseId"></param>
        /// <param name="personId"></param>
        void EditRelease(Etsi.Ultimate.DomainClasses.Release release, int previousReleaseId, int personId);

        /// <summary>
        /// Freezes the release
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="endDate"></param>
        /// <param name="personId"></param>
        /// <param name="FreezeMtgId"></param>
        /// <param name="FreezeMtgRef"></param>
        void FreezeRelease(int releaseId, DateTime? endDate, int personId, int? FreezeMtgId, string FreezeMtgRef);

        /// <summary>
        /// Lists all the releases
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        Dictionary<int, string> GetAllReleasesCodes(int releaseId);

        /// <summary>
        /// Computes the release code for the previous release.
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        KeyValuePair<int, string> GetPreviousReleaseCode(int releaseId);

        /// <summary>
        /// The unit of work with which  the release manager can work
        /// </summary>
        Etsi.Ultimate.Repositories.IUltimateUnitOfWork UoW { get; set; }
    }
}
