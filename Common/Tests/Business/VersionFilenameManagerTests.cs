using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business
{
    public class VersionFilenameManagerTests : BaseEffortTest
    {
        [TestCase("22.001", 1,1,1, "22001-111")]
        [TestCase("22001", 1,1,1, "22001-111")]
        [TestCase("22.001", 1,1,10, "22001-11a")]
        [TestCase("22.001", 1,10,1, "22001-1a1")]
        [TestCase("22.001", 10,1,1, "22001-a11")]
        [TestCase("22.001", 11,11,11, "22001-bbb")]
        [TestCase("22.001", 35,35,35, "22001-zzz")]
        [TestCase("22.001", 36,11,11, "22001-361111")]
        [TestCase("22.001", 36,99,36, "22001-369936")]
        public void GenerateValidFilename(string specNumber, int major, int technical, int editorial, string expectedResult)
        {
            var mgr = ManagerFactory.Resolve<IVersionFilenameManager>();
            var result = mgr.GenerateValidFilename(specNumber, major, technical, editorial);

            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(1, 1, 1, "111")]
        [TestCase(1, 1, 10, "11a")]
        [TestCase(1, 10, 1, "1a1")]
        [TestCase(10, 1, 1, "a11")]
        [TestCase(11, 11, 11, "bbb")]
        [TestCase(35, 35, 35, "zzz")]
        [TestCase(36, 11, 11, "361111")]
        [TestCase(36, 99, 36, "369936")]
        public void GenerateVersionString(int major, int technical, int editorial, string expectedResult)
        {
            var mgr = ManagerFactory.Resolve<IVersionFilenameManager>();
            var result = mgr.GenerateVersionString(major, technical, editorial);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
