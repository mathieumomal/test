using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// This class is in charge of all the business logic concerning the creation of a specification.
    /// </summary>
    public class SpecificationCreateAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Verifies all the fields of a specification, then send it to database.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public int Create(int personId, Specification spec)
        {
            // Specification must not already have a primary key.
            if (spec.Pk_SpecificationId != default(int))
            {
                throw new InvalidOperationException("Cannot create specification that already has a primary key");
            }

            // Check user rights. User must have rights to create specifications
            var userRights = ManagerFactory.Resolve<IRightsManager>().GetRights(personId);
            if (!userRights.HasRight(Enum_UserRights.Specification_Create))
            {
                throw new InvalidOperationException("User " + personId + " is not allowed to create a specification");
            }
           

            return 1;
        }
    }
}
