using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class PersonService : IPersonService
    {
        public string GetPersonDisplayName(int personID)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var personManager = new PersonManager();
                    personManager.UoW = uoW;
                    return personManager.GetPersonDisplayName(personID);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personID }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        /// <summary>
        /// Get Rights for the user
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>User Rights</returns>
        public UserRightsContainer GetRights(int personId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var rightManager = ManagerFactory.Resolve<IRightsManager>();
                    rightManager.UoW = uoW;
                    return rightManager.GetRights(personId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<View_Persons> GetByIds(List<int> rapporteurId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var personManager = ManagerFactory.Resolve<IPersonManager>();
                    personManager.UoW = uoW;
                    return personManager.GetByIds(rapporteurId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { rapporteurId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<View_Persons> LookFor(string keywords)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var personManager = ManagerFactory.Resolve<IPersonManager>();
                    personManager.UoW = uoW;
                    return personManager.LookFor(keywords);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { keywords }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public View_Persons FindPerson(int id)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var personManager = ManagerFactory.Resolve<IPersonManager>();
                    personManager.UoW = uoW;
                    return personManager.FindPerson(id);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { id }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public int GetChairmanIdByCommityId(int primeResponsibleGroupId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var personManager = ManagerFactory.Resolve<IPersonManager>();
                    personManager.UoW = uoW;
                    return personManager.GetChairmanIdByCommityId(primeResponsibleGroupId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { primeResponsibleGroupId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

    }
}
