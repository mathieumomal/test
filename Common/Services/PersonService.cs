using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
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

        /// <summary>
        /// Get Rights for the user
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>User Rights</returns>
        public UserRightsContainer GetRights(int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var rightManager = ManagerFactory.Resolve<IRightsManager>();
                rightManager.UoW = uoW;
                return rightManager.GetRights(personId);
            }
        }
    }
}
