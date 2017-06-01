using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.ItuRecommendation;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public class ItuRecommendationService: IItuRecommendationService
    {
        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="ituRecommendationName"></param>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <param name="saPlenaryMeetingId"></param>
        /// <param name="seedFilePath"></param>
        /// <returns></returns>
        public ServiceResponse<string> ExportItuRecommendation(int personId, string ituRecommendationName, int startReleaseId, int endReleaseId, int saPlenaryMeetingId, string seedFilePath)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {

                    var exportItuAction = new ItuRecommendationExportAction() { UoW = uoW };
                    return exportItuAction.ExportItuRecommendation(personId, ituRecommendationName, startReleaseId, endReleaseId, saPlenaryMeetingId, seedFilePath);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, ituRecommendationName, startReleaseId, endReleaseId, saPlenaryMeetingId, seedFilePath }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                var errorResponse = new ServiceResponse<string>{ Result = null };
                errorResponse.Report.LogError(Localization.GenericError);
                return errorResponse;
            }
        }


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
        public ServiceResponse<string> ExportItuPreliminary(int personId, string ituRecommendationName, int startReleaseId, int endReleaseId, int saPlenaryMeetingId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var exportItuPreliminaryAction = new ItuPreliminaryExportAction() { UoW = uoW };
                    return exportItuPreliminaryAction.ExportItuPreliminary(personId, ituRecommendationName, startReleaseId, endReleaseId, saPlenaryMeetingId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, ituRecommendationName, startReleaseId, endReleaseId, saPlenaryMeetingId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                var errorResponse = new ServiceResponse<string> { Result = null };
                errorResponse.Report.LogError(Localization.GenericError);
                return errorResponse;
            }
        }
    }
}
