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
        ///  Returns the specification from its ID, as well as the list of rights of the user.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        KeyValuePair<Specification, UserRightsContainer> GetSpecificationById(int personId, int id);

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
        /// Test specifications already exists :
        /// if foredit = true -> We allow one spec founded (edit mode case)
        /// if foredit = false -> we don't allow any spec founded
        /// </summary>
        /// <param name="specNumber">The spec number.</param>
        /// <param name="forEdit"></param>
        /// <returns></returns>
        KeyValuePair<bool, List<string>> LookForNumber(string specNumber);

        /// <summary>
        /// Returns the list of allowed actions regarding each specification-release of the provided specification.
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec);        

        /// <summary>
        /// Returns the list of allowed actions regarding a providied specification-release.
        /// </summary>
        /// <param name="userRights">Current user rights</param>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="releaseId"></param>
        /// <param name="releases">List of all releases</param>
        /// <returns></returns>
        KeyValuePair<Specification_Release, UserRightsContainer> GetRightsForSpecRelease(UserRightsContainer userRights, int personId, Specification spec, int releaseId, List<Release> releases);
        
    }
}
