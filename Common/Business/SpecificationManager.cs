using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Business
{
    public class SpecificationManager
    {

        private ISpecificationRepository specificationRepo;

        public IUltimateUnitOfWork UoW { get; set; }

        public SpecificationManager() { }


        private int personId;

        public KeyValuePair<Specification, UserRightsContainer> GetSpecificationById(int personId, int id)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;

            var specification = specificationRepo.Find(id);



            if (specification == null)
                return new KeyValuePair<Specification, UserRightsContainer>(null, null);

            // remove some rights depending on release status:
            // - a withdrawn specification can be withdrawn
            if (specification.DefinitivelyWithdrawn != null &&  specification.DefinitivelyWithdrawn.Value)
            {
                personRights.RemoveRight(Enum_UserRights.Specification_Withdraw, true);
            }
           

            return new KeyValuePair<Specification, UserRightsContainer>(specification, personRights);
        }
    }
}
