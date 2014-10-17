using Etsi.UserRights.Interface;

namespace Etsi.UserRights.Service
{
    /// <summary>
    /// Interface to fetch user rights
    /// </summary>
    public interface IRights
    {
        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <returns>User Rights object</returns>
        PersonRights GetRights(int personId);
    }
}
