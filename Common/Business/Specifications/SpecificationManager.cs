﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationManager : ISpecificationManager
    {
        #region properties
        private ISpecificationRepository _specificationRepo;

        public IUltimateUnitOfWork UoW { get; set; }
        #endregion

        #region Get specifications
        /// <summary>
        /// Get all versions of the spec associated to their foundation CRs
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        public ServiceResponse<List<SpecVersionFoundationCrs>> GetSpecVersionsFoundationCrs(int personId, int specId)
        {
            var response = new ServiceResponse<List<SpecVersionFoundationCrs>>
            {
                Result = new List<SpecVersionFoundationCrs>()
            };
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            specVersionRepo.GetVersionsWithFoundationsCrsBySpecId(specId).ForEach(x => response.Result.Add(new SpecVersionFoundationCrs
            {
                VersionId = x.Pk_VersionId,
                FoundationCrs = x.FoundationsChangeRequests.Select(cr => new FoundationCrsInfo
                {
                    CrNumber = cr.CRNumber,
                    Revision = cr.Revision.GetValueOrDefault()
                }).ToList()
            }));
            
            return response;
        }


        /// <summary>
        /// See interface
        /// No rights consideration : we get only the basic spec information
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecifications(int personId, List<int> ids)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var specifications = repo.GetSpecifications(ids);
            return new KeyValuePair<List<Specification>, UserRightsContainer>(specifications, null);
        }

        /// <summary>
        /// Gets the specifications by numbers.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specNumbers">The specification numbers.</param>
        /// <returns>List of specifications</returns>
        public List<Specification> GetSpecificationsByNumbers(int personId, List<string> specNumbers)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var specifications = repo.GetSpecificationListByNumber(specNumbers);
            return specifications;
        }

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

            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;

            var specification = _specificationRepo.Find(id);

            if (specification == null)
                return new KeyValuePair<Specification, UserRightsContainer>(null, null);

            // remove some rights depending on release status:
            // - a withdrawn specification can be withdrawn
            if (!specification.IsActive)
            {
                personRights.RemoveRight(Enum_UserRights.Specification_Withdraw, true);
            }

            //Set the initial release
            specification.SpecificationInitialRelease = string.Empty;
            if (specification.Specification_Release != null && specification.Specification_Release.Count > 0)
            {
                var relMgr = ManagerFactory.Resolve<IReleaseManager>();
                relMgr.UoW = UoW;
                var allReleases = relMgr.GetAllReleases(personId).Key.OrderBy(r => r.SortOrder);

                var initialRelease = allReleases.FirstOrDefault(x => specification.Specification_Release.Any( y => y.Fk_ReleaseId == x.Pk_ReleaseId));
                if (initialRelease != null)
                    specification.SpecificationInitialRelease = initialRelease.Name;
            }

            // CLean up unwanted remarks.
            if (specification.Versions != null && !personRights.HasRight(Enum_UserRights.Remarks_ViewPrivate))
            {
                foreach (var v in specification.Versions)
                {
                    var rem = v.Remarks.ToList();
                    rem.RemoveAll(x => !x.IsPublic.GetValueOrDefault());
                    v.Remarks = rem;
                }
            }

            //Remove delete right which should be delivered thanks to another dedicated business part
            if (personRights.HasRight(Enum_UserRights.Specification_Delete))
                personRights.RemoveRight(Enum_UserRights.Specification_Delete, true);

            return new KeyValuePair<Specification, UserRightsContainer>(specification, personRights);
        }

        public List<Specification> GetAllSpecifications(int personId)
        {
            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;

            var specification = _specificationRepo.All;

            if (specification == null)
                return new List<Specification>();

            return specification.ToList();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj"></param>
        /// <param name="includeRelations"></param>
        /// <returns></returns>
        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj, bool includeRelations)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;

            var specifications = _specificationRepo.GetSpecificationBySearchCriteria(searchObj,includeRelations);

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

            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;

            var specifications = _specificationRepo.GetSpecificationBySearchCriteria(searchString);

            if (!personRights.HasRight(Enum_UserRights.Specification_View_UnAllocated_Number))
                specifications.RemoveAll(x => String.IsNullOrEmpty(x.Number));

            return specifications;
        }

        /// <summary>
        /// Get Specification details by using Number
        /// </summary>
        /// <param name="number">Specification Number</param>
        /// <returns>Specification Details</returns>
        public Specification GetSpecificationByNumber(string number)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            return repo.GetSpecificationByNumber(number);
        }
        #endregion

        #region Related enums
        /// <summary>
        /// Returns the list of all technologies.
        /// </summary>
        /// <returns></returns>
        public List<Enum_Technology> GetTechnologyList()
        {
            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;
            return _specificationRepo.GetTechnologyList();
        }

        /// <summary>
        /// Returns the list of all specification series.
        /// </summary>
        /// <returns></returns>
        public List<Enum_Serie> GetSeries()
        {
            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;
            return _specificationRepo.GetSeries();
        }
        #endregion

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

            if (errors.Any())
                state = false;
            return new KeyValuePair<bool, List<string>>(state, errors);
        }

        /// <summary>
        /// Test specifications already exists :
        /// if foredit = true -> We allow one spec founded (edit mode case)
        /// if foredit = false -> we don't allow any spec founded
        /// </summary>
        /// <param name="specNumber">The spec number.</param>
        /// <returns></returns>
        public KeyValuePair<bool, List<string>> LookForNumber(string specNumber)
        {
            var state = true;
            var errors = new List<string>();

            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var result = repo
                    .All
                    .Where(x => x.Number.Equals(specNumber))
                    .ToList();
            if (result.Any())
                errors.Add(String.Format(Localization.Specification_ERR003_Number_Already_Use,result.First().Title));

            if (errors.Any())
                state = false;
            return new KeyValuePair<bool, List<string>>(state, errors);
        }

        /// <summary>
        /// Return TRUE if "the number matches one of the inhibit promote patterns" or false
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        public bool CheckInhibitedToPromote(string specNumber)
        {
            var listInhibitPromotePatterns = new List<string>
            {
                @"^30\.(\w|\-)*$",
                @"^50\.(\w|\-)*$",
                @"^[0-9]{2}\.8(\w|\-)*$"
            };
            foreach (var inihibitPromotePattern in listInhibitPromotePatterns)
            {
                Match match = Regex.Match(specNumber, inihibitPromotePattern);
                if (match.Success)
                {
                    return true;
                }
            }
            return false;
        }

        public Specification PutSpecAsInhibitedToPromote(Specification spec)
        {
            if (spec != null && spec.Number != null)
            {
                if (CheckInhibitedToPromote(spec.Number))
                {
                    spec.IsForPublication = false;
                    spec.promoteInhibited = true;
                }
                else
                {
                    spec.IsForPublication = true;
                    spec.promoteInhibited = false;
                }
                return spec;
            }
            return null;
        }

        #endregion

        #region Spec Release rights

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="shouldProvideRemoveRight">Should provide remove right (disable by default for performance reasons)</param>
        /// <returns></returns>
        public List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec, bool shouldProvideRemoveRight = false)
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

            if (shouldProvideRemoveRight)
            {
                //Get list of release ids linked to this spec
                var releaseIdsOfSpec = spec.Specification_Release.Select(x => x.Fk_ReleaseId).ToList();

                //Get list of versions linked to this spec
                var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
                specVersionRepo.UoW = UoW;
                var versionsOfSpec = specVersionRepo.GetVersionsBySpecId(spec.Pk_SpecificationId);

                result.AddRange(spec.Specification_Release.Select(sRel => GetRightsForSpecRelease(userRights, personId, spec, sRel.Fk_ReleaseId, releases, releaseIdsOfSpec, versionsOfSpec)));
            }
            else
            {
                result.AddRange(spec.Specification_Release.Select(sRel => GetRightsForSpecRelease(userRights, personId, spec, sRel.Fk_ReleaseId, releases)));
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
        /// <param name="version">Give version to give associated rights (default value : null)</param>
        /// <returns></returns>
        public KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases, SpecVersion version = null)
        {
            var rights = new UserRightsContainer();
            var specRelease = spec.Specification_Release.FirstOrDefault(r => r.Fk_ReleaseId == releaseId && r.Fk_SpecificationId == spec.Pk_SpecificationId);
            if (specRelease == null)
                return new KeyValuePair<Specification_Release, UserRightsContainer>(null, null);

            //Get spec rapporteur(s)
            var specRapporteurRepo = RepositoryFactory.Resolve<ISpecificationRapporteurRepository>();
            specRapporteurRepo.UoW = UoW;
            var specRapporteurs = specRapporteurRepo.FindBySpecId(spec.Pk_SpecificationId) ?? new List<SpecificationRapporteur>();

            // This right is common to any action.
            if (spec.IsActive && !specRelease.isWithdrawn.GetValueOrDefault())
            {
                var rel = releases.First(r => r.Pk_ReleaseId == specRelease.Fk_ReleaseId);

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
                // Conditions :
                // - Release shoudl exists
                // - Release should not be closed OR user should have the specific class : Versions_AllocateUpload_For_ReleaseClosed_Allowed
                if (rel != null && 
                    (rel.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed || 
                    userRights.HasRight(Enum_UserRights.Versions_AllocateUpload_For_ReleaseClosed_Allowed)))
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

                    //Person should have right to allocate new version if :
                    //1) Have Versions_Allocate right
                    //2) Or is prime rapporteur of the related spec
                    if (userRights.HasRight(Enum_UserRights.Versions_Allocate) || specRapporteurs.Any(x => x.Fk_RapporteurId == personId && x.IsPrime))
                    {
                        rights.AddRight(Enum_UserRights.Versions_Allocate);
                    }
                }

                    //Major version field is always enabled
                    if(userRights.HasRight(Enum_UserRights.Versions_Modify_MajorVersion))
                        rights.AddRight(Enum_UserRights.Versions_Modify_MajorVersion);
                }
            else if(spec.IsActive && specRelease.isWithdrawn.GetValueOrDefault())
            {
                if (userRights.HasRight(Enum_UserRights.Specification_UnWithdrawFromRelease))
                {
                    rights.AddRight(Enum_UserRights.Specification_UnWithdrawFromRelease);
                }
            }

            //Get the latest spec_release
            var latestSpecRelease = spec.Specification_Release.ToList().OrderByDescending(x => ((x.Release == null) ? 0 : (x.Release.SortOrder ?? 0))).First();
            //Get the latest release
            var latestRelease = releases.OrderByDescending(x => x.SortOrder ?? 0).First();
            
            //Promote button should display only on latest spec_release, But, it should not display if it is already promoted to latest release
            if (userRights.HasRight(Enum_UserRights.Specification_Promote)
                && (latestSpecRelease.Fk_ReleaseId == releaseId)
                && (latestRelease.Pk_ReleaseId != releaseId)
                && (!spec.promoteInhibited.GetValueOrDefault())
                && (spec.IsActive))
            {
                rights.AddRight(Enum_UserRights.Specification_Promote);
            }

            //Get the first spec_release
            var firstSpecRelease = spec.Specification_Release.ToList().OrderBy(x => ((x.Release == null) ? 0 : (x.Release.SortOrder ?? 0))).First();
            //Get the first release
            var firstRelease = releases.OrderBy(x => x.SortOrder ?? 0).First();

            /* Demote button should display only on first spec_release, 
            But, it should not display if it is the initial release */
            if (userRights.HasRight(Enum_UserRights.Specification_Demote)
                && (firstSpecRelease.Fk_ReleaseId == releaseId)
                && (firstRelease.Pk_ReleaseId != releaseId)
                && (!spec.promoteInhibited.GetValueOrDefault())
                && (spec.IsActive))
            {
                rights.AddRight(Enum_UserRights.Specification_Demote);
            }

            //InhibitPromote button should display only on latest spec_release
            // Test the right of the user to InhibitPromote
            if (userRights.HasRight(Enum_UserRights.Specification_InhibitPromote)
                && (latestSpecRelease.Fk_ReleaseId == releaseId)
                && (!spec.promoteInhibited.GetValueOrDefault())
                && (spec.IsActive))
            {
                rights.AddRight(Enum_UserRights.Specification_InhibitPromote);
            }

            //RemoveInhibitPromote button should display only on latest spec_release
            // Test the right of the user to remove InhibitPromote
            if (userRights.HasRight(Enum_UserRights.Specification_RemoveInhibitPromote)
                && (latestSpecRelease.Fk_ReleaseId == releaseId)
                && (spec.promoteInhibited.GetValueOrDefault())
                && (spec.IsActive))
            {
                rights.AddRight(Enum_UserRights.Specification_RemoveInhibitPromote);
            }

            if (userRights.HasRight(Enum_UserRights.Remarks_AddPrivateByDefault))
                rights.AddRight(Enum_UserRights.Remarks_AddPrivateByDefault);

            if (userRights.HasRight(Enum_UserRights.Remarks_ViewPrivate))
                rights.AddRight(Enum_UserRights.Remarks_ViewPrivate);

            //Version edit right
            if (userRights.HasRight(Enum_UserRights.Versions_Edit))
                rights.AddRight(Enum_UserRights.Versions_Edit);

            //Version avoid quality checks
            if (userRights.HasRight(Enum_UserRights.Versions_Avoid_Quality_Checks))
                rights.AddRight(Enum_UserRights.Versions_Avoid_Quality_Checks);

            //If version provided 
            //AND user have rights to delete draft version 
            //AND version is draft (MajorVersion < 3) 
            // -> give Version_Draft_Delete right
            if(userRights.HasRight(Enum_UserRights.Version_Draft_Delete)
                && version != null 
                && version.MajorVersion != null 
                && version.MajorVersion < 3)
                rights.AddRight(Enum_UserRights.Version_Draft_Delete);

            return new KeyValuePair<Specification_Release, UserRightsContainer>(specRelease, rights);
        }

        /// <summary>
        /// Returns the list of user rights for a given release of a specification.
        /// For each release, this method determines if user has right to:
        /// - force transposition
        /// - unforce transposition
        /// - Withdraw from release
        /// - Upload a version
        /// - Allocate a version
        /// - Remove the spec version
        /// </summary>
        /// <param name="userRights">Basic rights of the user</param>
        /// <param name="personId">ID</param>
        /// <param name="spec">Specification</param>
        /// <param name="releaseId">Corresponding release</param>
        /// <param name="releases">List of all releases, in order to fetch some fields (such as status).</param>
        /// <param name="releaseIdsRelatedToThisSpec">List of release ids linked to the current spec</param>
        /// <param name="specVersionsOfSpec">List of versions which belongs to this spec</param>
        /// <returns></returns>
        public KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases, List<int> releaseIdsRelatedToThisSpec, List<SpecVersion> specVersionsOfSpec)
        {
            var globalRights = GetRightsForSpecRelease(userRights, personId, spec, releaseId, releases);

            //Attribute remove right
            var releaseOfSpecOrdered = releases.Where(x => releaseIdsRelatedToThisSpec.Contains(x.Pk_ReleaseId)).OrderByDescending(x => x.SortOrder).ToList();
            var firstReleaseIdOfSpec = releaseOfSpecOrdered.First().Pk_ReleaseId;
            var lastReleaseIdOfSpec = releaseOfSpecOrdered.Last().Pk_ReleaseId;

            if (userRights.HasRight(Enum_UserRights.SpecificationRelease_Remove) &&
                (releaseId == firstReleaseIdOfSpec || releaseId == lastReleaseIdOfSpec))
            {
                if (!specVersionsOfSpec.Any(
                        x => x.Fk_ReleaseId == releaseId && x.Fk_SpecificationId == spec.Pk_SpecificationId))
                    globalRights.Value.AddRight(Enum_UserRights.SpecificationRelease_Remove);
                else
                    globalRights.Value.AddRight(Enum_UserRights.SpecificationRelease_Remove_Disabled);
            }

            return new KeyValuePair<Specification_Release, UserRightsContainer>(globalRights.Key, globalRights.Value);
        }

        #endregion

        #region Spec Release
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public Specification_Release GetSpecReleaseBySpecIdAndReleaseId(int specId, int releaseId)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var specRelease = repo.GetSpecificationReleaseByReleaseIdAndSpecId(specId, releaseId, true);
            return specRelease;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public List<Specification> GetSpecsRelatedToARelease(int releaseId)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var specs = repo.GetAllRelatedSpecificationsByReleaseId(releaseId);
            return specs;
        }

        /// <summary>
        /// Remove spec release
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> RemoveSpecRelease(int specId, int releaseId, int personId)
        {
            var response = new ServiceResponse<bool>{Result = true};

            //Get specification
            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;
            var specification = _specificationRepo.Find(specId);
            if (specification == null)
            {
                response.Result = false;
                response.Report.LogError(Localization.Error_Spec_Does_Not_Exist);
                return response;
            }

            // Get releases
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            // Get user rights
            var rightsManager = ManagerFactory.Resolve<IRightsManager>();
            rightsManager.UoW = UoW;
            var userRights = rightsManager.GetRights(personId);

            //Get list of release ids linked to this spec
            var releaseIdsOfSpec = specification.Specification_Release.Select(x => x.Fk_ReleaseId).ToList();
            //Get list of versions linked to this spec
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;
            var versionsOfSpec = specVersionRepo.GetVersionsBySpecId(specification.Pk_SpecificationId);

            //Get user rights for spec release
            var specReleaseRights = GetRightsForSpecRelease(userRights, personId, specification, releaseId, releases, releaseIdsOfSpec, versionsOfSpec);

            //Check right to remove
            if (!specReleaseRights.Value.HasRight(Enum_UserRights.SpecificationRelease_Remove))
            {
                response.Result = false;
                response.Report.LogError(Localization.RightError);
            }

            //Remove Spec-Release
            if(response.Result)
                _specificationRepo.DeleteSpecRelease(specId, releaseId);

            return response;
        }

        #endregion

        #region Delete specifications

        /// <summary>
        /// Check if spec deletion is allowed
        /// </summary>
        /// <param name="specId">Spec id</param>
        /// <param name="personId">Person id</param>
        /// <returns>True if it's allowed and false with the list of info for the other case</returns>
        public ServiceResponse<bool> CheckDeleteSpecificationAllowed(int specId, int personId)
        {
            var response = new ServiceResponse<bool>{Result = true};

            //[0] User should have spec delete right
            var rightsManager = ManagerFactory.Resolve<IRightsManager>();
            rightsManager.UoW = UoW;
            var userRights = rightsManager.GetRights(personId);
            if (!userRights.HasRight(Enum_UserRights.Specification_Delete))
            {
                response.Result = false;
                response.Report.LogError(Localization.RightError);
                return response;
            }

            //[1] Spec should exist
            var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            var uid = specRepo.SpecExists(specId);
            if (uid == null)
            {
                response.Result = false;
                response.Report.LogError(Localization.Error_Spec_Does_Not_Exist);
                return response;
            }

            //(Required elements)
            var warnings = new List<string>();
            var specVersionsRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionsRepo.UoW = UoW;


            //[2] Do not allowed deletion if related versions already uploaded
            var versionsAlreadyUploaded = specVersionsRepo.AlreadyUploadedVersionsForSpec(specId);
            if (versionsAlreadyUploaded.Count > 0)
            {
                warnings.Add(string.Format(Localization.Specification_Delete_Versions_Already_Uploaded, versionsAlreadyUploaded.Count));
            }

            //[3] Do not allowed deletion if related versions are linked to CR(s)
            var versionsLinkedToCrs = specVersionsRepo.VersionsLinkedToChangeRequestsForSpec(specId);
            if (versionsLinkedToCrs.Count > 0)
            {
                var crsCount = versionsLinkedToCrs.Sum(version => version.CurrentChangeRequests.Count);
                warnings.Add(string.Format(Localization.Specification_Delete_Linked_Crs, crsCount));
            }
 
            //[4] Do not allowed deletion if related versions are linked to TDoc(s)
            var viewContributionWithAddData =
                RepositoryFactory.Resolve<IViewContributionsWithAditionnalDataRepository>();
            viewContributionWithAddData.UoW = UoW;
            var tdocsRelatedToSomeVersion = viewContributionWithAddData.FindContributionsRelatedToASpec(specId);
            if (tdocsRelatedToSomeVersion.Count > 0)
            {
                warnings.Add(string.Format(Localization.Specification_Delete_Linked_Tdocs, tdocsRelatedToSomeVersion.Count));
            }


            //Check for warnings : if at least one warning detected -> do not allowed deletion and precise reason why
            if (warnings.Count > 0)
            {
                response.Result = false;
                response.Report.WarningList.Add(Localization.Specification_Delete_Warnings_Title);
                warnings.ForEach(x => response.Report.WarningList.Add(x));
            }

            return response;
        }

        /// <summary>
        /// Delete spec with all its related records
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> DeleteSpecification(int specId, int personId)
        {
            var response = new ServiceResponse<bool> { Result = true };

            //Check right
            var rightResponse = CheckDeleteSpecificationAllowed(specId, personId);
            if (!rightResponse.Result)
                return new ServiceResponse<bool>{Result = false, Report = new Report{ErrorList = new List<string>{Localization.RightError}}};
            
            //Delete spec
            _specificationRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            _specificationRepo.UoW = UoW;
            var deleteResult = _specificationRepo.DeleteSpecification(specId);
            return !deleteResult ? 
                new ServiceResponse<bool> { Result = false, Report = new Report { ErrorList = new List<string> { Localization.Error_Spec_Does_Not_Exist } } } : 
                response;
        } 

        #endregion

        #region Prime and Secondary Responsible groups

        public Specification SetPrimeAndSecondaryResponsibleGroupsData(Specification spec)
        {
            if (spec == null || spec.SpecificationResponsibleGroups == null || spec.SpecificationResponsibleGroups.Count == 0)
                return spec;

            //Get community data
            var communityManager = ManagerFactory.Resolve<ICommunityManager>();
            communityManager.UoW = UoW;
            var communityIds = spec.SpecificationResponsibleGroups.Select(x => x.Fk_commityId).ToList();
            var communities = communityManager.GetCommmunityByIds(communityIds);
            if (communities == null)
                return spec;

            //Filled specification's "extend properties" accordingly
            //1) Prime responsible
            var primeResponsibleGroup =
                spec.SpecificationResponsibleGroups.FirstOrDefault(x => x.IsPrime);
            if (primeResponsibleGroup != null)
            {
                var primeResponsibleGroupId = primeResponsibleGroup.Fk_commityId;
                var primeResponsibleGroupCommunity =
                    communities.FirstOrDefault(x => x.TbId == primeResponsibleGroupId);
                if (primeResponsibleGroupCommunity != null)
                {
                    spec.PrimeResponsibleGroupFullName = primeResponsibleGroupCommunity.TbName;
                    spec.PrimeResponsibleGroupShortName = primeResponsibleGroupCommunity.ShortName;
                }
            }

            //2) Secondary responsible groups
            var secondaryResponsibleGroups =
                spec.SpecificationResponsibleGroups.Where(x => !x.IsPrime).ToList();
            if (secondaryResponsibleGroups.Count > 0)
            {
                var secondaryResponsibleGroupsIds =
                    secondaryResponsibleGroups.Select(x => x.Fk_commityId).Distinct().ToList();
                spec.SecondaryResponsibleGroupsFullNames = string.Join(", ",
                    communities.Where(x => secondaryResponsibleGroupsIds.Contains(x.TbId))
                        .Select(y => y.TbName));
            }
            return spec;
        }

        public Specification SetParentAndChildrenPrimeResponsibleGroupsData(Specification spec)
        {
            if (spec == null)
                return null;

            var communityManager = ManagerFactory.Resolve<ICommunityManager>();
            communityManager.UoW = UoW;

            if (spec.SpecificationParents != null && spec.SpecificationParents.Count > 0)
            {
                var communityIds = spec.SpecificationParents.Where(s => s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null).Select(x => x.PrimeResponsibleGroup.Fk_commityId).ToList();
                var communities = communityManager.GetCommmunityByIds(communityIds);
                if (communities == null)
                    return spec;

                foreach (Specification s in spec.SpecificationParents)
                {
                    if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                    {
                        var currentCommunity =
                            communities.FirstOrDefault(x => x.TbId == s.PrimeResponsibleGroup.Fk_commityId);
                        if (currentCommunity != null)
                        {
                            s.PrimeResponsibleGroupShortName = currentCommunity.ShortName;
                        }
                    }
                }
            }

            if (spec.SpecificationChilds != null && spec.SpecificationChilds.Count > 0)
            {
                var communityIds = spec.SpecificationChilds.Where(s => s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null).Select(x => x.PrimeResponsibleGroup.Fk_commityId).ToList();
                var communities = communityManager.GetCommmunityByIds(communityIds);

                foreach (Specification s in spec.SpecificationChilds)
                {
                    if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                    {
                        var currentCommunity =
                            communities.FirstOrDefault(x => x.TbId == s.PrimeResponsibleGroup.Fk_commityId);
                        if (currentCommunity != null)
                        {
                            s.PrimeResponsibleGroupShortName = currentCommunity.ShortName;
                        }
                    }
                }
            }
            return spec;
        }
        #endregion
    }
}
