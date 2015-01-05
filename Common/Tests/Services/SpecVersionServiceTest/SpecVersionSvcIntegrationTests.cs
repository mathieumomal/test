using System.Linq;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    public class SpecVersionSvcIntegrationTests: BaseEffortTest
    {
        [TestCase(1, "", "", true)]
        [TestCase(400018, "22.110", "15.0.1", false)]
        [TestCase(428930, "22.101", "13.2.0", false)]
        public void GetVersionNumberWithSpecNumberByVersionId(int versionId, string expectedSpecNumber, string expectedVersionNumber, bool errorExpected)
        {
            var svc = ServicesFactory.Resolve<ISpecVersionService>();
            var result = svc.GetVersionNumberWithSpecNumberByVersionId(0, versionId);
            if (errorExpected)
            {
                Assert.AreEqual(1, result.Report.GetNumberOfErrors());
                Assert.AreEqual(Localization.Version_Not_Found, result.Report.ErrorList.FirstOrDefault());
                Assert.AreEqual(null, result.Result);
            }
            else
            {
                Assert.AreEqual(0, result.Report.GetNumberOfErrors());
                Assert.IsNotNull(result.Result);
                Assert.AreEqual(expectedVersionNumber, result.Result.Version);
                Assert.AreEqual(expectedSpecNumber, result.Result.SpecNumber);
            }
            
        }
    }
}
