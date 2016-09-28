using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Specifications;
using NUnit.Framework;
namespace Etsi.Ultimate.Tests.Business
{
    public class SpecReleaseManagerTests : BaseEffortTest
    {
        [TestCase(1, 1, false)]
        [TestCase(136080, 2884, false)]
        [TestCase(136080, 2885, true)]
        public void CreateSpecRelease(int specId, int releaseId, bool expectedResult)
        {
            var specReleaseMgr = ManagerFactory.Resolve<ISpecReleaseManager>();
            specReleaseMgr.UoW = UoW;

            var result = specReleaseMgr.CreateSpecRelease(specId, releaseId);
            var count = UoW.Context.Specification_Release.Count();

            Assert.AreEqual(expectedResult, result);

            UoW.Save();
            var countAfter = UoW.Context.Specification_Release.Count();
            if (expectedResult)
            {
                Assert.AreEqual(count + 1, countAfter);
            }
            else
            {
                Assert.AreEqual(count, countAfter);
            }
        }
    }
}
