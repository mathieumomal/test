using Etsi.UserRights.Interface;
using System;

namespace Etsi.UserRights.Service
{
    /// <summary>
    /// Service to provide User Rights to the requested client
    /// </summary>
    public class UserRightsService : IUserRightsService
    {
        /// <summary>
        /// Get Rights for User
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <param name="portal">Portal Name (ETSI / Ultimate)</param>
        /// <returns>User Rights object</returns>
        public PersonRights GetRights(int personID, string portal)
        {
            PersonRights userRights = null;
            IRights rights = null;

            if (!String.IsNullOrEmpty(portal))
            {
                if (portal.Equals("ETSI", StringComparison.InvariantCultureIgnoreCase))
                    rights = new ETSIRights();
                else if (portal.Equals("Ultimate", StringComparison.InvariantCultureIgnoreCase))
                    rights = new UltimateRights();
            }

            if (rights != null)
                userRights = rights.GetRights(personID);

            return userRights;    
        }       
    }
}
