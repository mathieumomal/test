using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Retrieving all the data necessary to build Itu preliminary records
    /// </summary>
    public class ItuPreliminaryDataExtractor : IItuPreliminaryDataExtractor
    {
        #region IItuPreliminaryDataExtractor Members

        /// <summary>
        /// Ultimate Unit of work, with access to U3GPPDB
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Gets the itu preliminary records based on the following criteria.
        /// ·	Specification number must be greater than 20.xxx and less than 40.xxx, but excludes the following series:
        ///     ·	25.xxx 
        ///     ·	34.xxx
        ///     ·	36.xxx
        ///     ·	37.xxx
        ///     ·	xx.8xx
        /// ·	Specification must be Under Change Control
        /// ·	The spec-Release must correspond to the selected range of Releases, i.e between Start and End Release.
        /// ·	The latest version in the corresponding Releases must have been made available on a meeting that took place on or before the selected SA plenary meeting.
        /// ·	This latest version must have been be transposed to ETSI and its ETSI transposition must be published
        /// ·	The specification should not be a test specification, meaning that its title does not contain any of the following strings:
        ///     ·	test specification
        ///     ·	test data
        ///     ·	test sequence 
        ///     ·	test method 
        ///     ·	subjective test 
        ///     ·	test suite 
        ///     ·	conform
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

            var validReleaseIds = RetrieveValidReleaseIds(startReleaseId, endReleaseId, response.Report);
            if (response.Report.GetNumberOfErrors() > 0)
                return response;

            // Retrieve all specifications that are matching the criteria.
            var specRepository = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepository.UoW = UoW;
           
            //[Filter 1] : Get UCC specs without prohibited titles + Filter valid spec-releases
            var prohibittedTitles = new List<string> { "test specification", "test data", "test sequence", "test method", "subjective test", "test suite", "conform" };
            var specListLevel1Filter = specRepository.AllIncluding().Where(x => ((x.IsActive && (x.IsUnderChangeControl ?? false)) &&
                                                                                 (!prohibittedTitles.Any(y => x.Title.ToLower().Contains(y))) &&
                                                                                 (!String.IsNullOrEmpty(x.Number)) &&
                                                                                 (x.Specification_Release.Any(z => validReleaseIds.Contains(z.Fk_ReleaseId))))).ToList();

            //[Filter 2] : Filter specs based on the number format & prohibited series
            var regexProhibitedSeries = new Regex(@"^((25[.].*)|(34[.].*)|(36[.].*)|(37[.].*)|(\w+[.]8.*))$");
            var specListLevel2Filter = specListLevel1Filter.Where(x => ((Convert.ToInt32(x.Number.Split('.')[0]) > 20) &&
                                                                        (Convert.ToInt32(x.Number.Split('.')[0]) < 40) &&
                                                                        (!regexProhibitedSeries.IsMatch(x.Number)))).ToList();

            //Get latest available versions in the corresponding Releases (On a meeting that took place on or before the selected SA plenary meeting).
            var specIds = specListLevel2Filter.Select(x => x.Pk_SpecificationId).ToList();
            var latestAvailableVersions = RetrieveLatestAvailableVersions(specIds, validReleaseIds, forbiddenMeetingIds);

            //Get the published transposed versions to ETSI
            var availableTransposedPublishedVersions = FilterPublishedTransposedVersions(latestAvailableVersions);

            //[Filter 3] : Filter specs which are having latest available versions & those are transposed to etsi and published.
            var validVersionIds = availableTransposedPublishedVersions.Select(x => x.Pk_VersionId).ToList();
            var specListLevel3Filter = specListLevel2Filter.Where(x => x.Versions.Any(y => validVersionIds.Contains(y.Pk_VersionId))).ToList();

            response.Result = specListLevel3Filter.Select(x => new ItuPreliminaryRecord() { SpecificationId = x.Pk_SpecificationId, Type = x.SpecificationTypeShortName, SpecificationNumber = x.Number, Title = x.Title }).ToList();

            return response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Filters the published transposed versions.
        /// </summary>
        /// <param name="latestAvailableVersions">The latest available versions.</param>
        /// <returns>List of versions</returns>
        private List<SpecVersion> FilterPublishedTransposedVersions(List<SpecVersion> latestAvailableVersions)
        {
            var tranposedVersions = latestAvailableVersions.Where(v => v.ETSI_WKI_ID != null).ToList();

            // Retrieve all workitems
            var wiRepository = RepositoryFactory.Resolve<IEtsiWorkItemRepository>();
            wiRepository.UoW = UoW;
            var allWis = wiRepository.GetWorkItemsByIds(tranposedVersions.Select(v => v.ETSI_WKI_ID ?? 0).ToList());
            var publishedWids = allWis.Where(x => x.published == 1).Select(y => y.WKI_ID).ToList();

            var transposedPublishedVersions = tranposedVersions.Where(x => publishedWids.Contains(x.ETSI_WKI_ID ?? 0)).ToList();

            return transposedPublishedVersions;
        }

        /// <summary>
        /// Retrieves the latest available versions.
        /// </summary>
        /// <param name="specIds">The spec ids.</param>
        /// <param name="validReleaseIds">The valid release ids.</param>
        /// <param name="forbiddenMeetingIds">The forbidden meeting ids.</param>
        /// <returns>List of versions</returns>
        private List<SpecVersion> RetrieveLatestAvailableVersions(List<int> specIds, List<int> validReleaseIds, List<int> forbiddenMeetingIds)
        {
            var specVersionsRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionsRepository.UoW = UoW;

            var latestVersions = specVersionsRepository.AllIncluding().ToList().GroupBy(x => new { x.Fk_SpecificationId, x.Fk_ReleaseId })
                                                                      .Select(y => y.OrderByDescending(major => major.MajorVersion)
                                                                                    .ThenByDescending(technical => technical.TechnicalVersion)
                                                                                    .ThenByDescending(editorial => editorial.EditorialVersion).First())
                                                                      .Where(z => specIds.Contains(z.Fk_SpecificationId ?? 0) && 
                                                                                  validReleaseIds.Contains(z.Fk_ReleaseId ?? 0)).ToList();

            var latestAvailableVersions = latestVersions.Where(x => ((x.DocumentUploaded != null) &&
                                                               x.Source.HasValue && 
                                                               !forbiddenMeetingIds.Contains(x.Source.Value))).ToList();
            return latestAvailableVersions;

        }

        /// <summary>
        /// Retrieves the valid release ids.
        /// </summary>
        /// <param name="startReleaseId">The start release identifier.</param>
        /// <param name="endReleaseId">The end release identifier.</param>
        /// <param name="report">The report.</param>
        /// <returns>List of valid release ids</returns>
        private List<int> RetrieveValidReleaseIds(int startReleaseId, int endReleaseId, Report report)
        {
            var releaseManager = new ReleaseManager { UoW = UoW };
            var allReleases = releaseManager.GetAllReleases(0).Key.OrderBy(r => r.SortOrder).ToList();

            // Retrieve initial release
            var initialRelease = allReleases.Find(r => r.Pk_ReleaseId == startReleaseId);
            if (initialRelease == null)
                report.LogError(Localization.ItuConversion_Error_InvalidStartRelease);

            // Retrieve final release
            var endRelease = allReleases.Find(r => r.Pk_ReleaseId == endReleaseId);
            if (endRelease == null)
                report.LogError(Localization.ItuConversion_Error_InvalidEndRelease);

            if (report.GetNumberOfErrors() > 0)
                return null;

            return allReleases.Where(x => x.SortOrder >= initialRelease.SortOrder && x.SortOrder <= endRelease.SortOrder).Select(y => y.Pk_ReleaseId).ToList();
        } 

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
