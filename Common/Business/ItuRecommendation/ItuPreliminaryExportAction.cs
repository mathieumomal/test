using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Business class to export Itu preliminary data
    /// </summary>
    public class ItuPreliminaryExportAction
    {
        #region Public Methods

        /// <summary>
        /// Ultimate Unit of work, with access to U3GPPDB
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

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
            var response = new ServiceResponse<string>();

            // Check user permissions
            if (!HasRight(personId))
            {
                response.Report.LogError(Localization.RightError);
                return response;
            }

            // Get the required data to export
            var ituPreliminaryDataExtractor = ManagerFactory.Resolve<IItuPreliminaryDataExtractor>();
            ituPreliminaryDataExtractor.UoW = UoW;
            var ituPreliminaryRecords = ituPreliminaryDataExtractor.GetItuPreliminaryRecords(startReleaseId, endReleaseId, saPlenaryMeetingId);

            if (ituPreliminaryRecords.Report.GetNumberOfErrors() > 0)
            {
                response.Report.ErrorList = ituPreliminaryRecords.Report.ErrorList;
                return response;
            }

            // Export to excel
            var fileName = ituRecommendationName + "_Preliminary_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH\\hmm") + ".xlsx";

            var excelExporter = ManagerFactory.Resolve<IItuPreliminaryExporter>();
            if (excelExporter.CreateItuPreliminaryFile(ConfigVariables.DefaultPublicTmpPath + fileName, ituPreliminaryRecords.Result))
            {
                response.Result = ConfigVariables.DefaultPublicTmpAddress + fileName;
            }

            return response;
        } 

        #endregion

        #region Private Methods

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

        #endregion
    }
}
