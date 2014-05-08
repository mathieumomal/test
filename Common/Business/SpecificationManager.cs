using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Business.Security;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business
{
    public class SpecificationManager : ISpecificationManager
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
            if (!specification.IsActive)
            {
                personRights.RemoveRight(Enum_UserRights.Specification_Withdraw, true);
            }

            //Set the initial release
            specification.SpecificationInitialRelease = (specification.Specification_Release != null && specification.Specification_Release.Count > 0) ?
                specification.Specification_Release.ToList().OrderBy(r => r.Pk_Specification_ReleaseId).FirstOrDefault().Release.Name : string.Empty;


            return new KeyValuePair<Specification, UserRightsContainer>(specification, personRights);
        }

        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;

            var specifications = specificationRepo.GetSpecificationBySearchCriteria(searchObj);

            if (!personRights.HasRight(Enum_UserRights.Specification_View_UnAllocated_Number))
                specifications.Key.RemoveAll(x => String.IsNullOrEmpty(x.Number));

            return new KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer>(specifications, personRights);
        }


        public List<Specification> GetSpecificationBySearchCriteria(int personId, String searchString)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;

            var specifications = specificationRepo.GetSpecificationBySearchCriteria(searchString);

            if (!personRights.HasRight(Enum_UserRights.Specification_View_UnAllocated_Number))
                specifications.RemoveAll(x => String.IsNullOrEmpty(x.Number));

            return specifications;
        }

        public List<Enum_Technology> GetTechnologyList()
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;
            return specificationRepo.GetTechnologyList();
        }

        public List<Enum_Serie> GetSeries()
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;
            return specificationRepo.GetSeries();
        }

        #region ISpecificationManager Membres


        public KeyValuePair<bool, List<string>> CheckNumber(string specNumber)
        {
            #region local variable
            var state = true;
            var errors = new List<string>();
            #endregion

            #region Format verification
            //Match match = Regex.Match(specNumber, @"^[0-9]{2}\.[a-zA-Z0-9]{1,3}(\-(p|s|P|S)){0,2}$");
            Match match = Regex.Match(specNumber, @"^[0-9]{2}\.(\w|\-)*$");
            if (!match.Success)
            {
                errors.Add(Localization.Specification_ERR002_Number_Invalid_Format);
            }
            #endregion

            #region Existence verification


            #endregion


            if (errors.Count() > 0)
                state = false;
            return new KeyValuePair<bool, List<string>>(state, errors);
        }

        public bool CheckInhibitedToPromote(string specNumber)
        {
            throw new NotImplementedException();
        }

        public List<Specification> LookForNumber(string specNumber)
        {
            ISpecificationRepository repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;

            return repo
                    .All
                    .Where(x => (x.IsActive && x.Number.Contains(specNumber)))
                    .ToList();
        }

        #endregion
    }
}
