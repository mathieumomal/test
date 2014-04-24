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

    }
}
