using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface IPersonService
    {
        /// <summary>
        /// Returns the name to dislay for a User
        /// </summary>
        /// <param name="personID">Identifier of the user</param>
        /// <returns></returns>
        string GetPersonDisplayName(int personID);

      
        /// <summary>
        /// Get Rights for the user
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>User Rights</returns>
        UserRightsContainer GetRights(int personId);
    }
}
