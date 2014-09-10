using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.WCF.Interface;
using System.Collections.Generic;

namespace Etsi.Ultimate.WCF.Service
{
    /// <summary>
    /// Provide the information which is related to ultimate database
    /// </summary>
    public class UltimateService : IUltimateService
    {
        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <returns>List of releases</returns>
        public List<Release> GetReleases(int personID)
        {
            return new List<Release>();
        }
    }
}