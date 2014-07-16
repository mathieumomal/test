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

        /// <summary>
        /// Returns the list of users corresponding to given Ids.
        /// The system automatically eliminates all the unknown IDs, and send back the 
        /// list of person in same order as the IDs.
        /// </summary>
        /// <param name="personIds"></param>
        /// <returns></returns>
        List<View_Persons> GetByIds(List<int> personIds);

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

        

        /// <summary>
        /// Find the list of Prime responsible group secretaries Emails
        /// </summary>
        /// <param name="primeResponsibleGroupId"></param>
        /// <returns></returns>
        List<string> GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(int primeResponsibleGroupId);
    }
}
