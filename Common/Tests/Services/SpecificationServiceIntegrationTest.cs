using System.Linq;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecificationServiceIntegrationTest : BaseEffortTest
    {
        [Test]
        public void GetSpecVersionsFundationCrs()
        {
            var specSvc = new SpecificationService();
            var result = specSvc.GetSpecVersionsFundationCrs(0, 140000);

            Assert.AreEqual(1, result.Result.Count);
            Assert.AreEqual(400000, result.Result.FirstOrDefault().VersionId);
            Assert.AreEqual("3568", result.Result.FirstOrDefault().FoundationCrs.FirstOrDefault().CRNumber);
            Assert.AreEqual(16, result.Result.FirstOrDefault().FoundationCrs.FirstOrDefault().Revision);
        }
    }
}
