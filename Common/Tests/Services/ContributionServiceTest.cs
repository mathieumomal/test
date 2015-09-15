using System.Linq;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    public class ContributionServiceTest : BaseEffortTest
    {
        [TestCase(1, 0, 0, "nothing", 0, Description = "No CR-Packs found")]
        [TestCase(1, 1, 1, "16", 1, Description = "Find by uid")]
        [TestCase(1, 1, 2, "16", 0, Description = "Find by uid with incorrect tbid")]
        [TestCase(1, 2, 2, "CRPACK2", 1, Description = "Find by title")]
        [TestCase(1, 2, 1, "CRPACK2", 0, Description = "Find by title with incorrect tbid")]
        public void GetCrPacksByKeywords(int personId, int crPackIdExpected, int tbId, string keywords, int noOfRecordsExpected)
        {
            var service = ServicesFactory.Resolve<IContributionService>();
            var response = service.GetCrPacksByTbIdAndKeywords(personId, tbId, keywords);

            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(noOfRecordsExpected, response.Result.Count);
            if (noOfRecordsExpected > 0)
                Assert.AreEqual(crPackIdExpected, response.Result.First().pk_Contribution);
        }
    }
}
