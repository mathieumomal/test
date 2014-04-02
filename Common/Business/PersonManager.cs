using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class PersonManager
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public PersonManager()
        {
        }

        /// <summary>
        /// Returns the name to dislay for a User
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public string GetPersonDisplayName(int personID)
        {
            IPersonRepository personRepo = RepositoryFactory.Resolve<IPersonRepository>();
            personRepo.UoW = UoW;
            try
            {
                View_Persons person = personRepo.Find(personID);

                if (person != null)
                    return person.FIRSTNAME + ' ' + person.LASTNAME;
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                LogManager.Error("Finding person using id failed", ex);
                return string.Empty;
            }
        }
    }
}
