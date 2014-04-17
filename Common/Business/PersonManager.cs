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

        #region IPersonManager Membres

        public List<View_Persons> GetByIds(List<int> rapporteurId)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;
            List<View_Persons> listPersonsFound = new List<View_Persons>();

            foreach (int id in rapporteurId)
            {
                try
                {
                    View_Persons person = repo.Find(id);
                    if (person != null)
                    {
                        listPersonsFound.Add(person);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error("Finding person using id failed", ex);
                }
            }
            listPersonsFound.Reverse();
            return listPersonsFound;  
        }

        public List<View_Persons> LookFor(string keywords)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;

            return repo
                    .All
                    .Where(x => (x.FIRSTNAME != null && x.FIRSTNAME.ToLower().Contains(keywords.ToLower()) && x.DELETED_FLG.Equals("N")) ||
                            (x.LASTNAME != null && x.LASTNAME.ToLower().Contains(keywords.ToLower()) && x.DELETED_FLG.Equals("N")) ||
                            (x.Email != null && x.Email.ToLower().Contains(keywords.ToLower())) && x.DELETED_FLG.Equals("N"))
                    .ToList();
        }

        public View_Persons FindPerson(int id)
        {
            IPersonRepository repo = RepositoryFactory.Resolve<IPersonRepository>();
            repo.UoW = UoW;

            return repo
                    .Find(id);
        }

        #endregion
    }
}
