using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecVersionsManager
    {
        private IUltimateUnitOfWork _uoW;

        public SpecVersionsManager(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        /// <summary>
        /// Returns a list of specVersion of a specification
        /// </summary>
        /// <param name="specificationId">Specification Id</param>
        /// <returns>List of SpecVersions including related releases</returns>
        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = _uoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        /// <summary>
        /// Returns the list of versions of a specification release
        /// </summary>
        /// <param name="specificationId">The specification identifier</param>
        /// <param name="releaseId">The identifier of the specification's release</param>
        /// <returns>List of versions objects</returns>
        public List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId)
        {
            List<SpecVersion> result = new List<SpecVersion>(); 
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            result = repo.GetVersionsForSpecRelease(specificationId, releaseId);
            return result;
        }

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <returns>A couple (version,userrights)</returns>
        public KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            SpecVersion version = repo.Find(versionId); ;

            if (version == null)
                return new KeyValuePair<SpecVersion, UserRightsContainer>(null, null);

            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            var personRights = rightManager.GetRights(personId);

            // Get information about the releases, in particular the status.
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = _uoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            var specificationManager = new SpecificationManager();
            specificationManager.UoW = _uoW;
            //Get calculated rights
            KeyValuePair<Specification_Release, UserRightsContainer> specRelease_Rights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases);
            //check for modifications
            //GetRightsForVersion(personRights, version, personId, Enum_UserRights.Versions_Upload, isSpecreleaseWithrawn);
            //GetRightsForVersion(personRights, version, personId, Enum_UserRights.Versions_Allocate, isSpecreleaseWithrawn);
            
            //UserRightsContainer rights = specificationManager.GetRightsForSpecReleases(personId, verison.Specification).Where(e => e.Key.Release.Pk_ReleaseId == verison.Release.Pk_ReleaseId).FirstOrDefault().Value;

            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specRelease_Rights.Value);
        }

        /// <summary>
        /// Enable to Allocate a version or to to upload it from scratch
        /// </summary>
        /// <param name="version">The new version to allocate or upload</param>
        public void UploadOrAllocateVersion(SpecVersion version)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            repo.InsertOrUpdate(version);
        }

        /// <summary>
        /// Edit userRights ( based on role) to take into account those based on specification-release data
        /// </summary>
        /// /// <param name="currentRights">Role base user rights</param>
        /// <param name="version">The current version</param>
        /// <param name="personId">The user id</param>
        /// <param name="rightToCheck">The right to check</param>
        /// <param name="isSpecReleaseWithdrawn">Is version's specfication release withdrawn </param>
        /// <returns></returns>
        /*private UserRightsContainer GetRightsForVersion(UserRightsContainer currentRights, SpecVersion version, int personId, Enum_UserRights rightToCheck, bool isSpecReleaseWithdrawn)
        {            
            // User have the right => Check if we need to remove it
            if (currentRights.HasRight(rightToCheck))
            {
                if (!(version.Specification.IsActive && version.Release.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed && !isSpecReleaseWithdrawn)) 
                {
                    //Remove right
                    currentRights.RemoveRight(rightToCheck);
                }
            }
            // User does not have the right => Check if we need to add it
            else
            {
                //User id rapporteur of the specification
                // for allocation, being rapporteur does not enable to get the right
                if (version.Specification.PrimeSpecificationRapporteurIds.Contains(personId) && rightToCheck != Enum_UserRights.Versions_Allocate)
                {
                    if (version.Specification.IsActive && version.Release.Enum_ReleaseStatus.Code != Enum_ReleaseStatus.Closed && isSpecReleaseWithdrawn)
                    {
                        //Still a draft
                        if(!version.Specification.IsUnderChangeControl)
                            //Add right
                            currentRights.AddRight(rightToCheck);
                    }
                }
            }

            return currentRights;
        }*/

    }
}
