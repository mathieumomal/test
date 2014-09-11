using Etsi.Ultimate.WCF.Interface;
using Etsi.Ultimate.WCF.Interface.Entities;
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
        /// <returns>List of Releases</returns>
        public List<Release> GetReleases(int personID)
        {
            ServiceHelper serviceHelper = new ServiceHelper();
            return serviceHelper.GetReleases(personID);
        }
    }
}