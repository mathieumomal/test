using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Security
{

    /// <summary>
    /// Provides a system to manage the rights of the users. The system must by default
    /// be able to return the list of actions the user can perform according to the user
    /// ID.
    /// </summary>
    public interface IRightsManager
    {
        /// <summary>
        /// Context to be provided in case the database needs to be targeted.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Fetches the rights of the user, given his user ID.
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        UserRightsContainer GetRights(int personID);

        /// <summary>
        /// Check whether the user is MCC member or not
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>true/false</returns>
        bool IsUserMCCMember(int personID);
    }
}
