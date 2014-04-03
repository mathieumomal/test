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
        /// Check whether the user is MCC member or not
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>true/false</returns>
        bool IsUserMCCMember(int personID);
    }
}
