using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface ISpecVersionService
    {
        /// <summary>
        /// Returns a list of specVersion of a specification
        /// </summary>
        /// <param name="specificationId">Specification Id</param>
        /// <returns>List of SpecVersions including related releases</returns>
        List<SpecVersion> GetVersionsBySpecId(int specificationId);

         /// <summary>
        /// Returns the list of versions of a specification release
        /// </summary>
        /// <param name="specificationId">The specification identifier</param>
        /// <param name="releaseId">The identifier of the specification's release</param>
        /// <returns>List of versions objects</returns>
        List<SpecVersion> GetVersionsForSpecRelease(int specificationId, int releaseId);

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <returns>A couple (version,userrights)</returns>
        KeyValuePair<SpecVersion, UserRightsContainer> GetVersionsById(int VersionId, int personId);
    }
}
