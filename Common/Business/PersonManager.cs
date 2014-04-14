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
    public class PersonManager : IPersonManager
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public PersonManager()
        {
        }

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


        #region Rapporteur control methods

        public List<View_Persons> GetByIds(List<KeyValuePair<int, Boolean>> rapporteurIdAndIsPrimary)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;
            List<View_Persons> listPersonsFound = new List<View_Persons>();
            View_Persons personPrimary = null;

            foreach (KeyValuePair<int, Boolean> idsWithPrimary in rapporteurIdAndIsPrimary)
            {
                var id = idsWithPrimary.Key;
                var primaryState = idsWithPrimary.Value;
                
                try
                {
                    View_Persons person = repo.Find(id);
                    if (person != null && !primaryState)
                    {
                        listPersonsFound.Add(person);
                    }
                    else if (person != null && primaryState)
                    {
                        personPrimary = person;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error("Finding person using id failed", ex);
                }
            }
            //Add primary person to the end of the list
            if (personPrimary != null)
            {
                listPersonsFound.Add(personPrimary);
            }
            //We inverse the list to have the primary person (if he exists) to the top of the list
            listPersonsFound.Reverse();
            return listPersonsFound;  
        }

        public List<View_Persons> LookFor(string keywords)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;

            return repo
                    .All
                    .Where(x => (x.FIRSTNAME != null && x.FIRSTNAME.ToLower().Contains(keywords.ToLower())) ||
                            (x.LASTNAME != null && x.LASTNAME.ToLower().Contains(keywords.ToLower())) ||
                            (x.Email != null && x.Email.ToLower().Contains(keywords.ToLower())))
                    .ToList();
        }

        #endregion
    }
}
