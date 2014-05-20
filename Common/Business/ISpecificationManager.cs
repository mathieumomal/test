using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public interface ISpecificationManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
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
        /// Find specifications by a keywords which match to their Number
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        List<Specification> LookForNumber(string specNumber);

        /// <summary>
        /// Returns the list of allowed actions regarding each specification-release of the provided specification.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec);
    }
}
