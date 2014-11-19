using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Implementation of ISpecToItuRecordConverter
    /// </summary>
    public class SpecToItuRecordConverter : ISpecToItuRecordConverter
    {
        public const string StaticSdo = "ETSI";
        public const string MissingInformationInWpmdb = "Not found in WPM DB";

        private List<Release> _allReleases;

        /// <summary>
        /// Ultimate Unit of work, with access to U3GPPDB
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Builds the ITU record by fetching data from tables and views of U3GPPDB.
        /// </summary>
        /// <param name="clausesAndSpecNumbers"></param>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <param name="lastMeetingId"></param>
        /// <returns></returns>
        public ServiceResponse<List<ItuRecord>> BuildItuRecordsForSpec(List<KeyValuePair<string, string>> clausesAndSpecNumbers, int startReleaseId, int endReleaseId, int lastMeetingId)
        {
            var response = new ServiceResponse<List<ItuRecord>> { Result = new List<ItuRecord>() };


            // Retrieve all necessary information concerning meetings and releases and perform checks
            var forbiddenMeetingIds = RetrieveForbiddenMeetingIds(lastMeetingId, response.Report);
            var allowedMajorVersions = RetrieveAllowedMajorVersions(startReleaseId, endReleaseId, response.Report);
            if (response.Report.GetNumberOfErrors() > 0)
                return response;

            // Retrieve all specifications that are matching.
            var specificationRepository = RepositoryFactory.Resolve<ISpecificationRepository>();
            specificationRepository.UoW = UoW;
            var specNumberToClause = clausesAndSpecNumbers.ToDictionary(x => x.Value, y => y.Key);
            var specifications =
                specificationRepository.GetSpecificationListByNumber(clausesAndSpecNumbers.Select(x => x.Value).ToList());

            // Retrieve all versions 
            var versionRepository = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepository.UoW = UoW;
            var allVersions =
                versionRepository.GetVersionsBySpecIds(specifications.Select(s => s.Pk_SpecificationId).ToList(),
                    allowedMajorVersions);

            // Start the loop
            var ituList = new List<ItuRecord>();
            foreach (var spec in specifications)
            {
                int specId = spec.Pk_SpecificationId;
                var versions =
                    allVersions.Where(v => v.Fk_SpecificationId.GetValueOrDefault() == specId &&
                             v.Source.HasValue && !forbiddenMeetingIds.Contains(v.Source.Value))
                        .OrderByDescending(v => v.MajorVersion)
                        .ThenByDescending(v => v.TechnicalVersion)
                        .ThenByDescending(v => v.EditorialVersion).ToList();

                foreach (var majorVersion in allowedMajorVersions)
                {
                    var version = versions.FirstOrDefault(v => v.MajorVersion == majorVersion);

                    if (version != null)
                    {
                        var newRecord = CreateNewItuRecord(specNumberToClause[spec.Number], version, spec);
                        ituList.Add(newRecord);
                    }
                }
            }

            response.Result = ituList.OrderBy(itu => itu.ClauseNumber).ToList();


            return response;
        }

        private ItuRecord CreateNewItuRecord(string clause, SpecVersion version, Specification spec)
        {
            return new ItuRecord
            {
                ClauseNumber = clause,
                SpecificationNumber = spec.Number,
                SpecVersionNumber =
                    version.MajorVersion + "." + version.TechnicalVersion + "." + version.EditorialVersion,
                Title = spec.Title,
                Sdo = StaticSdo,
                SdoVersionReleaase = _allReleases.Find(r => r.Pk_ReleaseId == version.Fk_ReleaseId).IturCode,
                SdoReference = MissingInformationInWpmdb,
                Hyperlink = MissingInformationInWpmdb,
                PublicationDate = MissingInformationInWpmdb,
                VersionPublicationStatus = MissingInformationInWpmdb

            };
        }

        /// <summary>
        /// Retrieves from the releases all the allowed major versions.
        /// </summary>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <returns></returns>
        private List<int> RetrieveAllowedMajorVersions(int startReleaseId, int endReleaseId, Report report)
        {
            var releaseManager = new ReleaseManager { UoW = UoW };
            _allReleases = releaseManager.GetAllReleases(0).Key;

            // retrieve initial release
            var initialRelease = _allReleases.Find(r => r.Pk_ReleaseId == startReleaseId);
            if (initialRelease == null)
                report.LogError(Localization.ItuConversion_Error_InvalidStartRelease);

            // retrieve final release
            var endRelease = _allReleases.Find(r => r.Pk_ReleaseId == endReleaseId);
            if (endRelease == null)
                report.LogError(Localization.ItuConversion_Error_InvalidEndRelease);
            
            if (report.GetNumberOfErrors() > 0)
                return null;

            var allowedMajorVersions =
                _allReleases.Where(r => r.SortOrder >= initialRelease.SortOrder && r.SortOrder <= endRelease.SortOrder
                    && r.Version3g.HasValue)
                    .OrderBy(r => r.SortOrder)
                    .Select(r => r.Version3g.GetValueOrDefault()).ToList();

            if (allowedMajorVersions.Count == 0)
            {
                report.LogError(Localization.ItuConversion_Error_InvalidReleaseOrder);
                return null;
            }

            return allowedMajorVersions;
        }

        /// <summary>
        /// Retrieve all the meetings for which we should discard versions.
        /// It corresponds to meetings that ends after the provided meeting.
        /// </summary>
        /// <param name="lastMeetingId"></param>
        /// <returns></returns>
        private List<int> RetrieveForbiddenMeetingIds(int lastMeetingId, Report report)
        {
            var mtgRepository = RepositoryFactory.Resolve<IMeetingRepository>();
            mtgRepository.UoW = UoW;
            var plenaryMeeting = mtgRepository.Find(lastMeetingId);

            if (plenaryMeeting == null)
            {
                report.ErrorList.Add(Localization.ItuConversion_Error_InvalidMeetingId);
                return null;
            }

            var forbiddenMeetingIds =
                mtgRepository.All.Where(m => m.END_DATE > plenaryMeeting.END_DATE).Select(m => m.MTG_ID).ToList();
            return forbiddenMeetingIds;
        }
    }

    /// <summary>
    /// Interface in charge of retrieving all the data necessary to build Itu records
    /// </summary>
    public interface ISpecToItuRecordConverter
    {
        /// <summary>
        /// Unit of work from where data can be fetched.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Functions that takes in parameter a list of spec numbers associated to their clauses and output a list
        /// of corresponding ITU records
        /// </summary>
        /// <param name="clausesAndSpecNumbers"></param>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <param name="lastMeetingId"></param>
        /// <returns></returns>
        ServiceResponse<List<ItuRecord>> BuildItuRecordsForSpec(List<KeyValuePair<string, string>> clausesAndSpecNumbers, int startReleaseId, int endReleaseId, int lastMeetingId);
    }
}
