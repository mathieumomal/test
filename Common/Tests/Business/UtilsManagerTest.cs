using Etsi.Ultimate.Business;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business
{
    public class UtilsManagerTest
    {
        [TestCase(0, 12, 1, "0c1")]
        [TestCase(0, null, 1, "001")]
        [TestCase(null, null, null, "000")]
        [TestCase(12, 12, 1245, "0c0cyl")]
        [TestCase(35, 35, 35, "zzz")]
        [TestCase(12, 35, 12464385, "0c0z7f5kx")]
        public void EncodeVersionToBase36(int? majorVersion, int? technicalVersion, int? editorialVersion, string resultExpected)
        {
            var result = UtilsManager.EncodeVersionToBase36(majorVersion, technicalVersion, editorialVersion);

            Assert.AreEqual(resultExpected, result);
        }
    }
}
