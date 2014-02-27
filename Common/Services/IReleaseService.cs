using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    
    /// <summary>
    /// Interface description all operations that are allowed regarding releases.
    /// </summary>
    public interface IReleaseService
    {
        KeyValuePair<List<DomainClasses.Release>, UserRightsContainer> GetAllReleases(int personID);
    }
}
