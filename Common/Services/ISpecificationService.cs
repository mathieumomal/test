using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface ISpecificationService
    {
        /// <summary>
        /// Returns A specification details including related remarks, history, Parent/Child specification, and releases
        /// </summary>
        /// <param name="specificationId">The identifier of the requested specification</param>
        KeyValuePair<Specification, UserRightsContainer> GetSpecificationDetailsById(int personId, int specificationId);

        /// <summary>
        /// Returns list of specifications including RGs
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj">Object with all search parameters</param>
        /// <returns></returns>
        KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj);

        /// <summary>
        /// Returns list of specifications including RGs
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchString">Search string for searching Spec Title/Number</param>
        /// <returns></returns>
        List<Specification> GetSpecificationBySearchCriteria(int personId, String searchString);

        /// <summary>
        /// Returns list of specifications that matches the search text except the excluded ones, including RGs. 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchString">Search string for searching Spec Title/Number</param>
        /// <returns></returns>
        List<Specification> GetSpecificationBySearchCriteriaWithExclusion(int personId, String searchString, List<string> toExclude);

        /// <summary>
        /// Creates a specification. 
        /// 
        /// Checks the rights of the user and the specification data, then push it to the database.
        ///
        /// In particular, the creation will fail if the specification: <ul>
        /// <li>has a primary key already defined. Call EditSpecification instead.</li>
        /// <li>has no release attached as initial release.</li>
        /// </summary>
        /// <param name="personId">The ID of the person requesting the specification.</param>
        /// <param name="spec">The details of the specification that has been created.</param>
        /// <returns>Key value pair containing:
        /// - as Key: The Primary key of the specification. -1 if creation failed.
        /// - as Value: The list of errors and warnings to take into account.</returns>
        KeyValuePair<int, Report> CreateSpecification(int personId, Specification spec, string baseurl);

        /// <summary>
        /// Updates the data concerning a specification.
        /// 
        /// Checks the rights of the user and the specification data, then push the data to the database.
        /// 
        /// In particular, method will check that primary key of the specification is already defined. Else, consider using CreateSpecification instead.
        /// </summary>
        /// <param name="personId">The ID of the person requesting the specification.</param>
        /// <param name="spec">The details of the specification that has been created.</param>
        /// <returns>True if update went well, else false. Additionally, the value of the Pair is a report of the </returns>
        KeyValuePair<int, Report> EditSpecification(int personId, Specification spec);

        /// <summary>
        /// Returns list of technologies
        /// </summary>
        /// <returns></returns>
        List<Enum_Technology> GetTechnologyList();

        /// <summary>
        /// Returns list of series
        /// </summary>
        /// <returns></returns>
        List<Enum_Serie> GetSeries();

        /// <summary>
        /// Return the list of all possible specification's technologies
        /// </summary>
        /// <returns>List of Enum_Technology</returns>
        List<Enum_Technology> GetAllSpecificationTechnologies();

        /// Returns TRUE and nothing if the specification number is valid and FALSE and the list of the errors if the specification is not valid :
        /// - correctly formatted (ERR-002)
        /// - not already exist in database (ERR-003)
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber);

        /// <summary>
        /// Test specifications already exists
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        KeyValuePair<bool, List<string>> LookForNumber(string specNumber);

        /// <summary>
        /// Create an Excel file corresponding to the query that has been performed.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj"></param>
        /// <param name="baseurl"></param>
        /// <returns></returns>
        string ExportSpecification(int personId, SpecificationSearch searchObj, string baseurl);

        /// <summary>
        /// Sends the specification for transposition. This means:
        /// - set the flag in the Spec_Release record
        /// - if a version is uploaded and no version is allocated, sends the latest version for transposition.
        /// </summary>
        /// <param name="personId">The Id of the person doing the action.</param>
        /// <param name="releaseId">The Id of the release for which transposition should be done.</param>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        bool ForceTranspositionForRelease(int personId, int releaseId, int specificationId);

        /// <summary>
        /// Prevent the specification from going directly to transposition upon upload for the given open release. This means:
        /// - set the flag in the Spec_Release record to False
        /// </summary>
        /// <param name="personId">Id of the person doing the action</param>
        /// <param name="releaseId">Id of the release for which transposition should be done</param>
        /// <param name="specificationId">If of the specification to unforce transposition on.</param>
        /// <returns></returns>
        bool UnforceTranspositionForRelease(int personId, int releaseId, int specificationId);

        /// <summary>
        /// Prevent Specification from promoting to the next release
        /// </summary>
        /// <param name="personId">personId</param>
        /// <param name="specificationId">specificationId</param>
        /// <returns></returns>
        bool SpecificationInhibitPromote(int personId, int specificationId);

        /// <summary>
        /// Allow Specification from promoting to the next release
        /// </summary>
        /// <param name="personId">personId</param>
        /// <param name="specificationId">specificationId</param>
        /// <returns></returns>
        bool SpecificationRemoveInhibitPromote(int personId, int specificationId);

        /// <summary>
        /// Promote Specification to next release
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        /// <returns>True/False</returns>
        bool PromoteSpecification(int personId, int specificationId, int currentReleaseId);

        /// <summary>
        /// Withdraws a specification from a given release.
        /// </summary>
        /// <param name="personId">ID of the person requesting the withdrawal</param>
        /// <param name="releaseId">ID of release for which spec should be withdrawn</param>
        /// <param name="specificationId">ID of spec to be withdrawn</param>
        /// <param name="withdrawalMtgId">ID of meeting for which spec should be withdrawn.</param>
        /// <returns></returns>
        bool WithdrawForRelease(int personId, int releaseId, int specificationId, int withdrawalMtgId);

        /// <summary>
        /// Withdraws definively a specification.
        /// </summary>
        /// <param name="personId">ID of the person requesting the withdrawal</param>
        /// <param name="specificationId">ID of spec to be definitively withdrawn</param>
        /// <param name="withdrawalMtgId">ID of meeting for which spec should be withdrawn.</param>
        /// <returns></returns>
        bool DefinitivelyWithdrawSpecification(int personId, int specificationId, int withdrawalMeetingId);

        /// <summary>
        /// For a given specification, computes the authorized actions on each release.
        /// </summary>
        /// <param name="spec">The specification on which we want to compute rights</param>
        /// <returns>A list of Specification_Release, in the order they were provided in the spec.Specification_Release field, with the associated rights</returns>
        List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec);

        /// <summary>
        /// Get the list of specification candidates to a massive promote for a release
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="initialReleaseId"></param>
        /// <returns></returns>
        KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId);

        /// <summary>
        /// Promote massively a set of specification
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specificationIds">Specifications concerned by promotion</param>
        /// <param name="targetReleaseId">The source release of the promotion</param>
        /// <returns></returns>
        bool PerformMassivePromotion(int personId, List<Specification> specificationIds, int initialReleaseId);
    }
}
