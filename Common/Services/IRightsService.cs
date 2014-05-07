using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// The IRightsService interface provides support to get the generic right of the user.
    /// 
    /// The rights returned here are computed without any regard to the context (example: current status of object being edited).
    /// </summary>
    public interface IRightsService
    {
        /// <summary>
        /// Returns the Generic (=contextless) rights for the given user
        /// </summary>
        /// <param name="personId"> The user person ID inside Directory Services application.</param>
        /// <returns></returns>
        UserRightsContainer GetGenericRightsForUser(int personId);

    }
}
