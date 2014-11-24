using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public interface IItuRecommendationService
    {
        
        /// <summary>
        /// Exports requested ITU recommendation, given recommendation name, start and end release, SA plenary meeting Id and seedFilePath
        /// </summary>
        /// <param name="personId">Id of the person performing the action</param>
        /// <param name="ituRecommendationName">Name of the ITU Recommendation</param>
        /// <param name="startReleaseId">Initial release Id</param>
        /// <param name="endReleaseId">Final release id. Release must be younger that initial release</param>
        /// <param name="saPlenaryMeetingId">Id of the SA plenary. Used to discard all versions that are posterior to the meeting.</param>
        /// <param name="seedFilePath">Path where the seed file can be found.</param>
        /// <returns>If everything went well, string contains the URL where ITU recommendation can be fetched.
        /// Else, Report should contain errors.</returns>
        ServiceResponse<string> ExportItuRecommendation(int personId, string ituRecommendationName, int startReleaseId, int endReleaseId, int saPlenaryMeetingId, string seedFilePath);

        /// <summary>
        /// Exports the itu preliminary report based on the given recommendation name, start and end release & SA plenary meeting
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="ituRecommendationName">Name of the itu recommendation.</param>
        /// <param name="startReleaseId">The start release identifier.</param>
        /// <param name="endReleaseId">The end release identifier.</param>
        /// <param name="saPlenaryMeetingId">The sa plenary meeting identifier.</param>
        /// <returns>If everything went well, string contains the URL where ITU preliminary can be fetched.
        /// Else, Report should contain errors.</returns>
        ServiceResponse<string> ExportItuPreliminary(int personId, string ituRecommendationName, int startReleaseId, int endReleaseId, int saPlenaryMeetingId);
    }
}
