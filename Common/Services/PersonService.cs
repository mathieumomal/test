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
        /// Check whether the user is MCC member or not
        /// </summary>
        /// <param name="personID">Person ID</param>
        /// <returns>true/false</returns>
        public bool IsUserMCCMember(int personID)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var rightsManager = new RightsManager();
                rightsManager.UoW = uoW;
                return rightsManager.IsUserMCCMember(personID);
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
