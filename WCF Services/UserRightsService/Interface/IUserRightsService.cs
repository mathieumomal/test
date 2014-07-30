using System.ServiceModel;

namespace Etsi.UserRights.Interface
{    
    /// <summary>
    /// Fetch rights for the ETSI / Ultimate users
    /// </summary>
    [ServiceContract]
    public interface IUserRightsService
    {
        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <param name="portal">Portal Name (ETSI / Ultimate)</param>
        /// <returns>User Rights object</returns>
        [OperationContract]
        PersonRights GetRights(int personID, string portal);
    }
}
