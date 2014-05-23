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

        /// <summary>
        /// Default implementation of the Interface. See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
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

        public List<Specification> GetAllSpecifications(int personId)
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;

            var specification = specificationRepo.All;

            if (specification == null)
                return new List<Specification>();

            return specification.ToList();
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

        /// <summary>
        /// Returns the list of all specification which number or title matches the string provided.
        /// </summary>
        /// <param name="personId">The ID of the user doing the search</param>
        /// <param name="searchString">The string to search on.</param>
        /// <returns></returns>
        public List<Specification> GetSpecificationByNumberAndTitle(int personId, String searchString)
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

        /// <summary>
        /// Returns the list of all technologies.
        /// </summary>
        /// <returns></returns>
        public List<Enum_Technology> GetTechnologyList()
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;
            return specificationRepo.GetTechnologyList();
        }

        /// <summary>
        /// Returns the list of all specification series.
        /// </summary>
        /// <returns></returns>
        public List<Enum_Serie> GetSeries()
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepo.UoW = UoW;
            return specificationRepo.GetSeries();
        }

        #region ISpecificationManager Membres

        /// <summary>
        /// Returns TRUE and nothing if the specification number is valid and FALSE and the list of the errors if the specification is not valid :
        /// - correctly formatted (ERR-002)
        /// - not already exist in database (ERR-003)
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        public KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber)
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

            if (errors.Count() > 0)
                state = false;
            return new KeyValuePair<bool, List<string>>(state, errors);
        }

        /// <summary>
        /// Test specifications already exists :
        /// if foredit = true -> We allow one spec founded (edit mode case)
        /// if foredit = false -> we don't allow any spec founded
        /// </summary>
        /// <param name="specNumber">The spec number.</param>
        /// <param name="forEdit"></param>
        /// <returns></returns>
        public KeyValuePair<bool, List<string>> LookForNumber(string specNumber)
        {
            var state = true;
            var errors = new List<string>();

            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var result = repo
                    .All
                    .Where(x => (x.IsActive && x.Number.Equals(specNumber)))
                    .ToList();
            if (result.Count() > 0)
                errors.Add(Localization.Specification_ERR003_Number_Already_Use);
            
            if (errors.Count() > 0)
                state = false;
            return new KeyValuePair<bool, List<string>>(state, errors);
        }

        /// <summary>
        /// Return TRUE if "the number matches one of the inhibit promote patterns" or false
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CheckInhibitedToPromote(string specNumber)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec)
        {
            var result = new List<KeyValuePair<Specification_Release, UserRightsContainer>>();

            // Get user rights
            var rightsManager = ManagerFactory.Resolve<IRightsManager>();
            rightsManager.UoW = UoW;
            var userRights = rightsManager.GetRights(personId);

            // Get information about the releases, in particular the status.
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            foreach (var sRel in spec.Specification_Release)
            {
                result.Add(GetRightsForSpecRelease(userRights, personId,spec,sRel.Fk_ReleaseId, releases));
            }
            return result;
        }

        /// <summary>
        /// Returns the list of user rights for a given release of a specification.
        /// For each release, this method determines if user has right to:
        /// - force transposition
        /// - unforce transposition
        /// - Withdraw from release
        /// - Upload a version
        /// - Allocate a version
        /// </summary>
        /// <param name="userRights">Basic rights of the user</param>
        /// <param name="personId">ID</param>
        /// <param name="spec">Specification</param>
        /// <param name="releaseId">Corresponding release</param>
        /// <param name="releases">List of all releases, in order to fetch some fields (such as status).</param>
        /// <returns></returns>
        public KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases)
        {
            var rights = new UserRightsContainer();
            Specification_Release specRelease = spec.Specification_Release.Where(r => r.Fk_ReleaseId == releaseId && r.Fk_SpecificationId == spec.Pk_SpecificationId).FirstOrDefault();
            if (specRelease == null)
                return new KeyValuePair<Specification_Release, UserRightsContainer>(null, null);
            // This right is common to any action.
            if (spec.IsActive && !specRelease.isWithdrawn.GetValueOrDefault())
            {
                var rel = releases.Where(r => r.Pk_ReleaseId == specRelease.Fk_ReleaseId).FirstOrDefault();

                // Test the right of the user to force transposition.
                if (userRights.HasRight(Enum_UserRights.Specification_ForceTransposition)
                    && rel != null && rel.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open
                    && !specRelease.isTranpositionForced.GetValueOrDefault()
                    && spec.IsUnderChangeControl.GetValueOrDefault())
                {
                    rights.AddRight(Enum_UserRights.Specification_ForceTransposition);
                }

                // Test the right of the user to unforce transposition.
                if (userRights.HasRight(Enum_UserRights.Specification_UnforceTransposition)
                    && specRelease.isTranpositionForced.GetValueOrDefault())
                {
                    rights.AddRight(Enum_UserRights.Specification_UnforceTransposition);
                }

                // Test the right of the user to withdraw transposition from 
                if (userRights.HasRight(Enum_UserRights.Specification_WithdrawFromRelease))
                {
                    rights.AddRight(Enum_UserRights.Specification_WithdrawFromRelease);
                }

                // Test the right of the user to upload/Allocate a version
                // User have the right => Check if we need to remove it
                if (rel.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed)
                {

                    if (userRights.HasRight(Enum_UserRights.Versions_Upload))
                    {
                        rights.AddRight(Enum_UserRights.Versions_Upload);
                    }
                    // User does not have the right by default => Check if we need to add it
                    else
                    {
                        //User id rapporteur of the specification
                        // for allocation, being rapporteur does not enable to get the right
                        if (spec.PrimeSpecificationRapporteurIds.Contains(personId))
                        {
                            //Still a draft
                            if (!spec.IsUnderChangeControl.GetValueOrDefault())
                                rights.AddRight(Enum_UserRights.Versions_Upload);
                        }
                    }

                    if (userRights.HasRight(Enum_UserRights.Versions_Allocate))
                    {
                        rights.AddRight(Enum_UserRights.Versions_Allocate);
                    }
                }
            }
            return new KeyValuePair<Specification_Release, UserRightsContainer>(specRelease, rights);
        }

        /// <summary>
        /// Return workitems label list of a spec
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        public List<string> GetWorkItemLabel(int specId)
        {
            specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            var workItemRepo = RepositoryFactory.Resolve<IWorkItemRepository>();
            specificationRepo.UoW = UoW;
            workItemRepo.UoW = UoW;

            var spec = specificationRepo.Find(specId);
            var wisLink = spec.Specification_WorkItem.ToList();
;

            var workItemLabels = new List<string>();
            foreach (var wiLink in wisLink)
            {
                var wi = workItemRepo.Find(wiLink.Fk_WorkItemId);
                var label = new StringBuilder()
                    .Append("#")
                    .Append(wi.UID)
                    .Append(" - ")
                    .Append(wi.Name)
                    .ToString();
                workItemLabels.Add(label);
            }
            return workItemLabels;
        }
    }
}
