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
        KeyValuePair<Specification, UserRightsContainer>  GetSpecificationDetailsById(int personId, int specificationId);

        /// <summary>
        /// Returns list of specifications including related remarks, history, Parent/Child specification, and releases
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchObj">Object with all search parameters</param>
        /// <returns></returns>
        KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObj);

        /// <summary>
        /// Returns list of specifications including related remarks, history, Parent/Child specification, and releases
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="searchString">Search string for searching Spec Title/Number</param>
        /// <returns></returns>
        List<Specification> GetSpecificationBySearchCriteria(int personId, String searchString);

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
        KeyValuePair<int, ImportReport> CreateSpecification(int personId, Specification spec);

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
        KeyValuePair<bool,ImportReport> EditSpecification(int personId, Specification spec);

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
        KeyValuePair<bool, List<string>> CheckNumber(string specNumber);

        /// <summary>
        /// Return TRUE if "the number matches one of the inhibit promote patterns" or false
        /// </summary>
        /// <param name="specNumber"></param>
        /// <returns></returns>
        bool CheckInhibitedToPromote(string specNumber);

        /// <summary>
        /// Create an Excel file corresponding to the query that has been performed.
        /// </summary>
        /// <param name="exportPath"></param>
        /// <returns></returns>
        string ExportSpecification(int personId, SpecificationSearch searchObj);

    }
}
