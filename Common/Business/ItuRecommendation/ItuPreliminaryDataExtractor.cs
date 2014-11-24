using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    public class ItuPreliminaryDataExtractor : IItuPreliminaryDataExtractor
    {
        #region IItuPreliminaryDataExtractor Members

        /// <summary>
        /// Ultimate Unit of work, with access to U3GPPDB
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Gets the itu preliminary records.
        /// </summary>
        /// <param name="startReleaseId">The start release identifier.</param>
        /// <param name="endReleaseId">The end release identifier.</param>
        /// <param name="saPlenaryMeetingId">The sa plenary meeting identifier.</param>
        /// <returns>Itu preliminary data</returns>
        public ServiceResponse<List<ItuPreliminaryRecord>> GetItuPreliminaryRecords(int startReleaseId, int endReleaseId, int saPlenaryMeetingId)
        {
            var response = new ServiceResponse<List<ItuPreliminaryRecord>> { Result = new List<ItuPreliminaryRecord>() };

            // Retrieve all necessary information concerning meetings and releases and perform checks
            var forbiddenMeetingIds = RetrieveForbiddenMeetingIds(saPlenaryMeetingId, response.Report);
            if (response.Report.GetNumberOfErrors() > 0)
                return response;

            // Retrieve all specifications that are matching the criteria.
            var specRepository = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepository.UoW = UoW;
            var specList = specRepository.AllIncluding().Where(x => (x.IsActive && (x.IsUnderChangeControl ?? false))).ToList();

            response.Result = specList.Select(x => new ItuPreliminaryRecord() { SpecificationId = x.Pk_SpecificationId, Type = x.SpecificationTypeShortName, SpecificationNumber = x.Number, Title = x.Title }).ToList();

            return response;
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieve all the meetings for which we should discard versions.
        /// It corresponds to meetings that ends after the provided meeting.
        /// </summary>
        /// <param name="lastMeetingId">The last meeting identifier.</param>
        /// <param name="report">The report.</param>
        /// <returns>List of forbidden meeting ids</returns>
        private List<int> RetrieveForbiddenMeetingIds(int lastMeetingId, Report report)
        {
            var mtgRepository = RepositoryFactory.Resolve<IMeetingRepository>();
            mtgRepository.UoW = UoW;
            var meeting = mtgRepository.Find(lastMeetingId);

            if (meeting == null)
            {
                report.ErrorList.Add(Localization.ItuConversion_Error_InvalidMeetingId);
                return null;
            }

            var forbiddenMeetingIds = mtgRepository.All.Where(m => m.END_DATE > meeting.END_DATE).Select(m => m.MTG_ID).ToList();
            return forbiddenMeetingIds;
        } 

        #endregion
    }

    /// <summary>
    /// Interface in charge of retrieving all the data necessary to build Itu preliminary records
    /// </summary>
    public interface IItuPreliminaryDataExtractor
    {
        /// <summary>
        /// Unit of work from where data can be fetched.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Gets the itu preliminary records.
        /// </summary>
        /// <param name="startReleaseId">The start release identifier.</param>
        /// <param name="endReleaseId">The end release identifier.</param>
        /// <param name="saPlenaryMeetingId">The sa plenary meeting identifier.</param>
        /// <returns>Itu preliminary data</returns>
        ServiceResponse<List<ItuPreliminaryRecord>> GetItuPreliminaryRecords(int startReleaseId, int endReleaseId, int saPlenaryMeetingId);
    }
}
