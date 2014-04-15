using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public interface IPersonManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Returns the name to dislay for a User
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        string GetPersonDisplayName(int personID);


        #region Rapporteur control methods

        /// <summary>
        /// Get th rapporteurs by their id and their primary status (Only one rapporteur is Primary)
        /// </summary>
        /// <param name="rapporteurIdAndIsPrimary"></param>
        /// <returns></returns>
        List<View_Persons> GetByIds(List<int> rapporteurId);

        /// <summary>
        /// Find rapporteurs by a keywords which match to their firstname/lastname/email
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        List<View_Persons> LookFor(string keywords);

        #endregion
    }
}
