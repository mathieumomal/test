using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications.Interfaces
{
    public interface ISpecificationManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Get all versions of the spec associated to their foundation CRs
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        ServiceResponse<List<SpecVersionFoundationCrs>> GetSpecVersionsFoundationCrs(int personId, int specId); 
            
        /// <summary>
        ///  Returns the specification from its ID, as well as the list of rights of the user.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        KeyValuePair<DomainClasses.Specification, UserRightsContainer> GetSpecificationById(int personId, int id);

        /// <summary>
        /// Returns the list of specifications matching given criteria. Includes or not the Spec release details depending on
        /// whether the includeSpecRel flag is set to true or to false.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj"></param>
        /// <param name="includeRelations"></param>
        /// <returns></returns>
        KeyValuePair<KeyValuePair<List<DomainClasses.Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj, bool includeRelations);

        /// <summary>
        /// Returns TRUE and nothing if the specification number is valid and FALSE and the list of the errors if the specification is not valid :
        /// - correctly formatted (ERR-002)
        /// - not already exist in database (ERR-003)
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber);

        /// <summary>
        /// Return TRUE if "the number matches one of the inhibit promote patterns" or false
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        bool CheckInhibitedToPromote(string specNumber);

        /// <summary>
        /// Put the spec as inhibited to promote : 
        /// - promoteInhibited = TRUE
        /// - IsForPublication = FALSE
        /// </summary>
        /// <param name="spec"></param>
        DomainClasses.Specification PutSpecAsInhibitedToPromote(DomainClasses.Specification spec);

        /// <summary>
        /// Test specifications already exists :
        /// if foredit = true -> We allow one spec founded (edit mode case)
        /// if foredit = false -> we don't allow any spec founded
        /// </summary>
        /// <param name="specNumber">The spec number.</param>
        /// <returns></returns>
        KeyValuePair<bool, List<string>> LookForNumber(string specNumber);

        /// <summary>
        /// Returns the list of allowed actions regarding each specification-release of the provided specification.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="shouldProvideRemoveRight"></param>
        /// <returns></returns>
        List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec, bool shouldProvideRemoveRight = false);

        /// <summary>
        /// Returns the list of allowed actions regarding a providied specification-release.
        /// </summary>
        /// <param name="userRights">Current user rights</param>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="releaseId"></param>
        /// <param name="releases">List of all releases</param>
        /// <param name="version"></param>
        /// <returns></returns>
        KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases, SpecVersion version = null);

        /// <summary>
        /// Returns the list of allowed actions regarding a providied specification-release with potentially right to remove the spec release.
        /// </summary>
        /// <param name="userRights">Current user rights</param>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="releaseId"></param>
        /// <param name="releases">List of all releases</param>
        /// <param name="releaseIdsRelatedToThisSpec">List of release ids linked to the current spec</param>
        /// <param name="specVersionsOfSpec">List of versions which belongs to this spec</param>
        /// <returns></returns>
        KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases, List<int> releaseIdsRelatedToThisSpec, List<SpecVersion> specVersionsOfSpec);

        /// <summary>
        /// Get a specRelease by specId and ReleaseId
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        Specification_Release GetSpecReleaseBySpecIdAndReleaseId(int specId, int releaseId);

        /// <summary>
        /// Get all the specs related to a release, by a release id
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        List<DomainClasses.Specification> GetSpecsRelatedToARelease(int releaseId);

        /// <summary>
        /// Get specifications corresponding to a list of ids
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        KeyValuePair<List<DomainClasses.Specification>, UserRightsContainer> GetSpecifications(int personId, List<int> ids);

        /// <summary>
        /// Gets the specifications by numbers.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specNumbers">The specification numbers.</param>
        /// <returns>List of specifications</returns>
        List<Specification> GetSpecificationsByNumbers(int personId, List<string> specNumbers);

        /// <summary>
        /// See implementation
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        ServiceResponse<bool> RemoveSpecRelease(int specId, int releaseId, int personId);

        /// <summary>
        /// Delete spec
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        ServiceResponse<bool> DeleteSpecification(int specId, int personId);

        /// <summary>
        /// Check if spec deletion is allowed
        /// </summary>
        /// <param name="specId">Spec id</param>
        /// <param name="personId">Person id</param>
        /// <returns>True if it's allowed and false with the list of error for the other case</returns>
        ServiceResponse<bool> CheckDeleteSpecificationAllowed(int specId, int personId);
    }
}
