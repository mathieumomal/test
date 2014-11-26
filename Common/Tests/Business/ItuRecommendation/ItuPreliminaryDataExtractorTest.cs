using Etsi.Ultimate.Business.ItuRecommendation;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business.ItuRecommendation
{
    [TestFixture]
    public class ItuPreliminaryDataExtractorTest : BaseEffortTest
    {
        private const int release12Id = 2882;
        private const int release13Id = 2883;
        private const int release14Id = 2884;
        private const int release15Id = 2885;

        private const int publishedWiId = 17240;
        private const int unPublishedWiId = 18815;

        private const int saPlenaryMeetingId = 22905;
        private const int validSpecIdForItuPreliminary = 150000;

        #region Setup

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            InitializeUserRightsMock();
        }

        #endregion

        #region Tests

        [Test]
        public void GetItuPreliminaryRecords_ValidSpec()
        {
            var dataExtractor = new ItuPreliminaryDataExtractor() { UoW = UoW };
            var ituPreliminaryData = dataExtractor.GetItuPreliminaryRecords(release12Id, release15Id, saPlenaryMeetingId);
            Assert.IsNotNull(ituPreliminaryData.Result);
            Assert.AreEqual(validSpecIdForItuPreliminary, ituPreliminaryData.Result.FirstOrDefault().SpecificationId);
        } 

        #endregion
    }
}
