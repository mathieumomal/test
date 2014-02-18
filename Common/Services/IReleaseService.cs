using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    
    /// <summary>
    /// Interface description all operations that are allowed regarding releases.
    /// </summary>
    public interface IReleaseService
    {
        List<DomainClasses.Release> GetAllReleases();
    }
}
