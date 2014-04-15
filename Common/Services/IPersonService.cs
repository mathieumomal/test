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


        #region Rapporteur control methods

        /// <summary>
        /// Get th rapporteurs by their id and their primary status (Only one rapporteur is Primary)
        /// </summary>
        /// <param name="rapporteurIdAndIsPrimary"></param>
        /// <returns></returns>
        List<View_Persons> GetByIds(List<KeyValuePair<int, bool>> rapporteurIdAndIsPrimary);

        /// <summary>
        /// Find rapporteurs by a keywords which match to their firstname/lastname/email
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        List<View_Persons> LookFor(string keywords);

        #endregion
    }
}
