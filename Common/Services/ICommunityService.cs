using Etsi.Ultimate.DomainClasses;
using System.Collections.Generic;

namespace Etsi.Ultimate.Services
{
    public interface ICommunityService
    {
        /// <summary>
        /// Get All Communities
        /// </summary>
        /// <returns>List of Communities</returns>
        List<Community> GetCommunities();
    }
}
