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
                LogManager.Error("Exception while exporting ITU recommendation: " + e.Message);
                LogManager.Error(e.StackTrace);

                var errorResponse = new ServiceResponse<string>{ Result = null };
                errorResponse.Report.LogError(Localization.GenericError);
                return errorResponse;
            }
        }
    }
}
