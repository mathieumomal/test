using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    [TestFixture]
    class SpecToItuConverterTests : BaseEffortTest
    {
        private SpecToItuRecordConverter _converter;
        private List<KeyValuePair<string, string>> _clausesAndSpecs;

        private const string UccSpecWithUploadedVersionForRel12 = "22.105";
        private const int Release12Id = 2882;
        private const int Release13Id = 2883;
        private const int EndReleaseId = 2883;
        private const int LastMeetingId = 22941;    // This means meeting 22942 is out of scope

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            InitializeUserRightsMock();
            _converter = new SpecToItuRecordConverter() { UoW = UoW };
            _clausesAndSpecs = new List<KeyValuePair<string, string>>();
        }

        #region Feature: select right versions

        [Test, Description("If no specification is provided in input, system returns an empty list.")]
        public void ConvertToItuRecords_ReturnsEmptyForEmptyList()
        {
            var response = _converter.BuildItuRecordsForSpec(_clausesAndSpecs, Release12Id, EndReleaseId, LastMeetingId);

            Assert.IsNotNull(response.Result);
            Assert.AreEqual(0, response.Result.Count);
        }

        [Test, Description("System returns latest version from the release," +
                           " discarding the ones that have been allocated after the requested meeting")]
        public void ConvertToItuRecords_ReturnsLatestVersionFromRelease()
        {
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.2", "22.101"));
            var response = _converter.BuildItuRecordsForSpec(_clausesAndSpecs, Release13Id, Release13Id, LastMeetingId);

            Assert.IsNotNull(response.Result);
            Assert.AreEqual(1, response.Result.Count);
            Assert.AreEqual("13.1.1", response.Result.First().SpecVersionNumber);
        }

        [Test,
         Description("If a set of releases is provided, system should return one record per meeting, whenever possible")
        ]
        public void ConvertToItuRecords_ReturnsOneRecordPerRelease()
        {
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.2", UccSpecWithUploadedVersionForRel12));
            var response = _converter.BuildItuRecordsForSpec(_clausesAndSpecs, Release12Id, Release13Id, LastMeetingId).Result;

            Assert.AreEqual(2, response.Count);
            Assert.AreEqual("12.0.0", response.First().SpecVersionNumber);
            Assert.AreEqual("13.0.0", response.Last().SpecVersionNumber);
        }

        [Test, Description("System returns specifications ordered by clause")]
        public void ConvertToItuRecords_OrdersOutputByClause()
        {
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.3", "22.101"));
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.2", UccSpecWithUploadedVersionForRel12));

            var response =
                _converter.BuildItuRecordsForSpec(_clausesAndSpecs, Release13Id, Release13Id, LastMeetingId).Result;
            Assert.AreEqual(2, response.Count);
            Assert.AreEqual("5.1.2", response.First().ClauseNumber);
            Assert.AreEqual("5.1.3", response.Last().ClauseNumber);
        }


        #endregion

        #region Error cases

        [Test, Description("System must run checks, and return error if plenary meeting does not exist")]
        public void ConvertToItuRecords_ReturnsErrorIfPlenaryDoesNotExist()
        {
            CheckError(Release13Id, Release13Id, 897456, Localization.ItuConversion_Error_InvalidMeetingId);
        }

        [Test, Description("System must run checks, and return error if start release can't be found")]
        public void ConvertToItuRecords_ReturnsErrorIfStartReleaseDoesNotExist()
        {
            CheckError(897564, Release13Id, LastMeetingId, Localization.ItuConversion_Error_InvalidStartRelease);
        }

        [Test, Description("System must run checks, and return error if end release cannot be found")]
        public void ConvertToItuRecords_ReturnsErrorIfEndReleaseDoesNotExist()
        {
            CheckError(Release13Id, 894565, LastMeetingId, Localization.ItuConversion_Error_InvalidEndRelease);
        }

        [Test, Description("System must run checks, and return error if no release is candidate (start > end)")]
        public void ConvertToItuRecords_ReturnsErrorIfNoReleaseFoundInBetween()
        {
            CheckError(Release13Id, Release12Id, LastMeetingId, Localization.ItuConversion_Error_InvalidReleaseOrder);
        }

        /// <summary>
        /// Function to check error. Used because the Localization error messages cannot fit into TestCases.
        /// </summary>
        private void CheckError(int startRelease, int endRelease, int meetingId, string errorMessage)
        {
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.2", UccSpecWithUploadedVersionForRel12));
            var response = _converter.BuildItuRecordsForSpec(_clausesAndSpecs, startRelease, endRelease, meetingId);

            Assert.AreEqual(0, response.Result.Count);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(errorMessage, response.Report.ErrorList.First());
        }
        #endregion

        #region Feature: fill in ITU Records

        [Test, Description("For each candidate version, system must fill in clause number")]
        public void ConvertToItuRecords_FillsInClauseNumber()
        {
            var response = GetGenericCase();

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Count);
            Assert.AreEqual("5.1.2", response.First().ClauseNumber);
        }

        [Test, Description("For each candidate version, system must fill specification number")]
        public void ConvertToItuRecords_FillsInSpecNumber()
        {
            var response = GetGenericCase();
            Assert.AreEqual(UccSpecWithUploadedVersionForRel12, response.First().SpecificationNumber);
        }

        [Test, Description("For each candidate version, system must fill version number")]
        public void ConvertToItuRecords_FillsInVersionNumber()
        {
            var response = GetGenericCase();
            Assert.AreEqual("12.0.0", response.First().SpecVersionNumber);
        }

        [Test, Description("For each candidate version, system must fill in Title")]
        public void ConvertToItuRecord_FillsInTitle()
        {
            var response = GetGenericCase();
            Assert.AreEqual(Context.Specifications.SingleOrDefault(s => s.Number==UccSpecWithUploadedVersionForRel12).Title,
                response.First().Title);
        }

        [Test, Description("For each candidate version, system must fill in Release")]
        public void ConvertToItuRecord_FillsInReleaseShortName()
        {
            var response = GetGenericCase();
            Assert.AreEqual("Release 12", response.First().SdoVersionReleaase);
        }

        [Test, Description("For each candidate version, system must fill 'ETSI' as SDO")]
        public void ConvertToItuRecords_FillSdo()
        {
            var response = GetGenericCase();
            Assert.AreEqual("ETSI", response.First().Sdo);
        }

        [Test, Description("For each candidate version, system should write 'Not found in WPMDB' in export ")]
        public void ConvertToItuRecords_SetsNotFoundOnWpmRelatedFieldsIfNoWkiIdInVersion()
        {
            var response = GetGenericCase();
            Assert.AreEqual("Not found in WPM DB", response.First().SdoReference);
            Assert.AreEqual("Not found in WPM DB", response.First().VersionPublicationStatus);
            Assert.AreEqual("Not found in WPM DB", response.First().Hyperlink);
            Assert.AreEqual("Not found in WPM DB", response.First().PublicationDate);
        }

        #endregion

        private List<ItuRecord> GetGenericCase()
        {
            _clausesAndSpecs.Add(new KeyValuePair<string, string>("5.1.2", UccSpecWithUploadedVersionForRel12));
            var response =
                _converter.BuildItuRecordsForSpec(_clausesAndSpecs, Release12Id, Release12Id, LastMeetingId).Result;
            return response;
        }
    }
}
