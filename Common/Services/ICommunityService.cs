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

        /// <summary>
        /// Return short name of a community by id
        /// </summary>
        /// <param name="id">Identifier of the community</param>
        /// <returns>Short name of the community</returns>
        string GetCommmunityshortNameById(int id);
    }
}
