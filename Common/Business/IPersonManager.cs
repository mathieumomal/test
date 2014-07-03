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

        /// <summary>
        /// Find one person by his own id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        View_Persons FindPerson(int id);

        /// <summary>
        /// Find the commity's chairman
        /// </summary>
        /// <param name="primeResponsibleGroupId"></param>
        /// <returns></returns>
        int GetChairmanIdByCommityId(int primeResponsibleGroupId);

        #endregion

        /// <summary>
        /// Find the list of Prime responsible group secretaries Emails
        /// </summary>
        /// <param name="primeResponsibleGroupId"></param>
        /// <returns></returns>
        List<string> GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(int primeResponsibleGroupId);
    }
}
