using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Services
{
    public class PersonService : IPersonService
    {
        public string GetPersonDisplayName(int personID)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var personManager = new PersonManager();
                personManager.UoW = uoW;                
                return personManager.GetPersonDisplayName(personID);
            }
        }
    }
}
