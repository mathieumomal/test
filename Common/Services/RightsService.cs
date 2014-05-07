using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// Default implementation of the IRightsService interface.
    /// </summary>
    public class RightsService: IRightsService
    {
        #region IRightsService Members

        /// <summary>
        /// See Interface description.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public DomainClasses.UserRightsContainer GetGenericRightsForUser(int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var rightsManager = ManagerFactory.Resolve<IRightsManager>();
                rightsManager.UoW = uoW;
                return rightsManager.GetRights(personId);
            }
        }

        #endregion
    }
}
