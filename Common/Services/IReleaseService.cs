﻿using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using System;

namespace Etsi.Ultimate.Services
{
    
    /// <summary>
    /// Interface description all operations that are allowed regarding releases.
    /// </summary>
    public interface IReleaseService
    {
        /// <summary>
        /// Abstract method to get a pair of all releases and user's rights
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        KeyValuePair<List<DomainClasses.Release>, UserRightsContainer> GetAllReleases(int personID);

        /// <summary>
        /// Returns the details of a release, and the associated rights that user can perform on the release.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KeyValuePair<DomainClasses.Release, UserRightsContainer> GetReleaseById(int personID, int releaseId);

        KeyValuePair<int, string> GetPreviousReleaseCode(int releaseId);

        void FreezeRelease(int releaseId, DateTime endDate, int personId, int FreezeMtgId, string FreezeMtgRef);

        /// <summary>
        /// Close Release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="closureDate">Closure Date</param>
        /// <param name="closureMtgRef">Closure Meeting Reference</param>
        /// <param name="closureMtgId">Closure Meeting Reference ID</param>
        /// <param name="personID">Person ID</param>
        void CloseRelease(int releaseId, DateTime closureDate, string closureMtgRef, int closureMtgId, int personID);

        /// <summary>
        /// Return the list of all releases' codes except the one with the identifier passed as input
        /// </summary>
        /// <param name="releaseId">The identifier of the release to exclude form the returned list</param>
        /// <returns></returns>
        Dictionary<int, string> GetAllReleasesCodes(int releaseId);

        void EditRelease(Release release, int previousReleaseId, int personId);

        int CreateRelease(Release release, int previousReleaseId, int personId);

    }
}
