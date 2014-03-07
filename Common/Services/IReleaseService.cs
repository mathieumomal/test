using System.Collections.Generic;
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

        string GetPreviousReleaseCode(int personID, int releaseId);
        
        void FreezeRelease(int releaseId, DateTime endDate);
    }
}
