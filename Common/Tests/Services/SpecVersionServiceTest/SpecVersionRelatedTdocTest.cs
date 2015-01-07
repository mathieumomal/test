using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionRelatedTdocTest : BaseEffortTest
    {
        [TestCase(136080, 2883, 13, 0, 1, "R2-001", true, "")]
        [TestCase(136080, 2883, 13, 0, 2, "R2-001", false, "Version not found.")]
        public void UpdateVersionRelatedTdoc_Test(int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion, string relatedTdoc, bool isSuccess, string errorMessage)
        {
            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.UpdateVersionRelatedTdoc(specId, releaseId, majorVersion, technicalVersion, editorialVersion, relatedTdoc);

            if (isSuccess)
            {
                Assert.IsTrue(svcResponse.Result);
                Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
            }
            else
            {
                Assert.IsFalse(svcResponse.Result);
                Assert.AreEqual(errorMessage, svcResponse.Report.ErrorList.First());
            }
            
        }
    }
}
