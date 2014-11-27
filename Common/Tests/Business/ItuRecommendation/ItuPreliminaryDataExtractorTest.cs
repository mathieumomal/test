using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    [TestFixture]
    public class ItuPreliminaryDataExtractorTest : BaseEffortTest
    {
        #region Constants

        private const int releaseR00 = 2874;
        private const int release12Id = 2882;
        private const int release15Id = 2885;
        private const int unPublishedWiId = 18815;
        private const int saPlenaryMeetingId = 22905;
        private const int validSpecIdForItuPreliminary = 150000; 

        #endregion

        #region Setup

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            InitializeUserRightsMock();
        }

        #endregion

        #region Tests

        [Test, Description("System must return the valid spec details for Itu preliminary export")]
        public void GetItuPreliminaryRecords_ValidSpec()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(1, ituPreliminaryData.Result.Count);
            Assert.AreEqual(validSpecIdForItuPreliminary, ituPreliminaryData.Result.FirstOrDefault().SpecificationId);
        }

        [TestCase("20.000", Description = "Specification should not return if specification number less than or equal to 20.xxx")]
        [TestCase("19.000", Description = "Specification should not return if specification number less than or equal to 20.xxx")]
        [TestCase("40.000", Description = "Specification should not return if specification number greater than or equal to 40.xxx")]
        [TestCase("41.000", Description = "Specification should not return if specification number greater than or equal to 40.xxx")]
        [TestCase("25.000", Description = "Specification should not return if specification number having the 25.xxx series")]
        [TestCase("34.000", Description = "Specification should not return if specification number having the 34.xxx series")]
        [TestCase("36.000", Description = "Specification should not return if specification number having the 36.xxx series")]
        [TestCase("37.000", Description = "Specification should not return if specification number having the 37.xxx series")]
        [TestCase("22.810", Description = "Specification should not return if specification number having the xx.8xx series")]
        public void GetItuPreliminaryRecords_CheckNumber(string number)
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Number = number;
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [TestCase("test specification for priliminary export", Description = "Specification should not return if title contains 'test specification'")]
        [TestCase("export test data", Description = "Specification should not return if title contains 'test data'")]
        [TestCase("spec test sequence for priliminary export", Description = "Specification should not return if title contains 'test sequence'")]
        [TestCase("test method", Description = "Specification should not return if title contains 'test method'")]
        [TestCase("subjective test", Description = "Specification should not return if title contains 'subjective test'")]
        [TestCase("test suite", Description = "Specification should not return if title contains 'test suite'")]
        [TestCase("conform spec", Description = "Specification should not return if title contains 'conform'")]
        public void GetItuPreliminaryRecords_CheckTitle(string title)
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Title = title;
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [TestCase(true, false, Description = "Draft specification should not return")]
        [TestCase(true, null, Description = "Draft specification should not return")]
        [TestCase(false, true, Description = "Withdrawn under change control specs should not return")]
        [TestCase(false, false, Description = "Withdrawn before change control specs should not return")]
        [TestCase(false, null, Description = "Withdrawn before change control specs should not return")]
        public void GetItuPreliminaryRecords_CheckStatus(bool isActive, bool? isUnderChangeControl)
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.IsActive = isActive;
            specToTest.IsUnderChangeControl = isUnderChangeControl;
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [Test, Description("The spec-Release must correspond to the selected range of Releases, i.e between Start and End Release.")]
        public void GetItuPreliminaryRecords_CheckSpecReleaseInRange()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Specification_Release.ToList().ForEach(x => x.Fk_ReleaseId = releaseR00);
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [Test, Description("Specification should not return if latest version has not made available on a meeting that took place on or before the selected SA plenary meeting.")]
        public void GetItuPreliminaryRecords_CheckLatestVersionAvailability()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Versions.ToList().ForEach(x => x.Location = null);
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [Test, Description("Specification should not return if latest version not transposed to ETSI")]
        public void GetItuPreliminaryRecords_CheckLatestVersionTranspositionStatus()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Versions.ToList().ForEach(x => x.ETSI_WKI_ID = null);
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        [Test, Description("Specification should not return if latest version transposed to ETSI, but not published")]
        public void GetItuPreliminaryRecords_CheckLatestVersionTranspositionPublishedStatus()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var specToTest = UoW.Context.Specifications.Find(validSpecIdForItuPreliminary);
            specToTest.Versions.ToList().ForEach(x => x.ETSI_WKI_ID = unPublishedWiId);
            UoW.Context.SaveChanges();
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(0, ituPreliminaryData.Result.Count);
        }

        #endregion
    }
}
