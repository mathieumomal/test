using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    /// <summary>
    /// This class is the implementation in charge of all the operations concerning the releases.
    /// </summary>
    public class ReleaseService : IReleaseService
    {
        /// <summary>
        /// Get a pair of all releases and user's rights
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<List<DomainClasses.Release>,UserRightsContainer> GetAllReleases(int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                //Get list of releases
                return releaseManager.GetAllReleases(personId);
            }
        }

        /// <summary>
        /// Get a pair of all releases and user's rights filtered by status
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<List<DomainClasses.Release>, UserRightsContainer> GetAllReleasesByStatus(int personId, string releaseStatus)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                //Get list of releases filtered by status
                return releaseManager.GetAllReleasesByStatus(personId, releaseStatus);
            }
        } 

        public KeyValuePair<Release, UserRightsContainer> GetReleaseById(int personId, int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                return releaseManager.GetReleaseById(personId,releaseId); 
            }
        }

        public KeyValuePair<int, string> GetPreviousReleaseCode(int releaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                return releaseManager.GetPreviousReleaseCode(releaseId);
            }
        }

        public ServiceResponse<bool> FreezeRelease(int releaseId, DateTime? endDate, int personId, int? FreezeMtgId, string FreezeMtgRef)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                var response = releaseManager.FreezeRelease(releaseId, endDate, personId, FreezeMtgId, FreezeMtgRef);
                uoW.Save();
                return response;
            }
        }

        /// <summary>
        /// Close Release
        /// </summary>
        /// <param name="releaseId">Release ID</param>
        /// <param name="closureDate">Closure Date</param>
        /// <param name="closureMtgRef">Closure Meeting Reference</param>
        /// <param name="closureMtgId">Closure Meeting Reference ID</param>
        /// <param name="personID">Person ID</param>
        public ServiceResponse<bool> CloseRelease(int releaseId, DateTime? closureDate, string closureMtgRef, int? closureMtgId, int personID)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                var response = releaseManager.CloseRelease(releaseId, closureDate, closureMtgRef, closureMtgId, personID);
                uoW.Save();
                return response;
            }
        }

        /// <summary>
        /// Return the list of all releases' codes except the one with the identifier passed as input
        /// </summary>
        /// <param name="releaseId">The identifier of the release to exclude form the returned list</param>
        /// <returns></returns>
        public Dictionary<int, string> GetAllReleasesCodes(int releaseId)
        {            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                return releaseManager.GetAllReleasesCodes(releaseId);
            }
        }

        /// <summary>
        /// Edit an existing Release
        /// </summary>
        /// <param name="release">The edited Release</param>
        /// <param name="previousReleaseId">the identifier of the selected previous Release</param>
        /// <param name="personId">The person identifier</param>
        public void EditRelease(Release release, int previousReleaseId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                releaseManager.EditRelease(release, previousReleaseId, personId);
                uoW.Save();
            }
        }

        /// <summary>
        /// Insert a new Release
        /// </summary>
        /// <param name="release">The Release object of creation</param>
        /// <param name="previousReleaseId">the identifier of the selected previous Release</param>
        /// <param name="personId">The person identifier</param>
        public int CreateRelease(Release release, int previousReleaseId, int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                var newRelease = releaseManager.CreateRelease(release, previousReleaseId, personId);
                uoW.Save();
                return newRelease.Pk_ReleaseId;
            }
        }

        /// <summary>
        /// Get releases linked to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <param name="personId">Person id</param>
        /// <returns>List of releases linked to spec provided</returns>
        public ServiceResponse<List<Release>> GetReleasesLinkedToASpec(int specId, int personId)
        {
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                    releaseManager.UoW = uow;
                    var response = releaseManager.GetReleasesLinkedToASpec(specId);
                    if(response != null)
                        return new ServiceResponse<List<Release>>{Result = response};
                    throw new Exception("System not able to get releases linked to this specification: " + specId);
                }
            }
            catch (Exception e)
            {
                LogManager.Error("GetReleasesLinkedToASpec : an unexpected error occured", e);
                return new ServiceResponse<List<Release>>{Result = null, Report = new Report{ErrorList = new List<string>{ Localization.GenericError }}};
            }
        }

        public Release GetHighestNonClosedReleaseLinkedToASpec(int specId)
        {
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                    releaseManager.UoW = uow;
                    return releaseManager.GetHighestNonClosedReleaseLinkedToASpec(specId);
                }
            }
            catch (Exception e)
            {
                LogManager.Error("GetHighestNonClosedReleaseLinkedToASpec : an unexpected error occured", e);
                return null;
            }
        }
    }
}
