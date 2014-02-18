using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public interface IReleaseService
    {
        List<DomainClasses.Release> GetAllReleases();
    }
}
