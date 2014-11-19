using DocumentFormat.OpenXml.ExtendedProperties;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    public class ItuRecommendationExportAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Verifies the right of user.
        /// Then parse the Seed file to extract all the specifications.
        /// Then queries the database to fetch all the data concerning these specifications.
        /// Finally outputs the external file.
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
            var response = new ServiceResponse<string>();
            if (!HasRight(personId))
            {
                response.Report.LogError(Localization.RightError);
                return response;
            }

            // Else parse the document
            var seedFileParser = ManagerFactory.Resolve<ISeedFileParser>();
            var parserResponse = seedFileParser.ExtractSpecificationNumbersFromSeedFile(seedFilePath);
            if (parserResponse.Report.GetNumberOfErrors() > 0)
            {
                response.Report.ErrorList = parserResponse.Report.ErrorList;
                return response;
            }
            var specificationsToFillIn = parserResponse.Result;
            
            // Then convert the specification.
            var converter = ManagerFactory.Resolve<ISpecToItuRecordConverter>();
            var convertedSpecificationRecords = converter.BuildItuRecordsForSpec(specificationsToFillIn, startReleaseId, endReleaseId, saPlenaryMeetingId);

            var fileName = ituRecommendationName + "_" + DateTime.UtcNow.ToString("yyyy-MM-dd_hh\\hmm") + ".xlsx";

            // Still to fix: the path is to be computed against Application parameters.
            var excelExporter = ManagerFactory.Resolve<IItuRecommendationExporter>();
            if (excelExporter.CreateItuFile(ConfigVariables.DefaultPublicTmpPath + fileName,
                convertedSpecificationRecords.Result))
            {
                response.Result = ConfigVariables.DefaultPublicTmpAddress + fileName;
            }
            return response; 
        }
    
        /// <summary>
        /// Checks whether user has right to manage itu recommendations.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        private bool HasRight(int personId)
        {
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;

            var rights = rightsMgr.GetRights(personId);
            return rights.HasRight(Enum_UserRights.Specification_ManageITURecommendations);
        }
    }
}
