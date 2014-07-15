using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services
{
    public class UtilsServiceTest
    {
        [TestCase(0, "00")]
        [TestCase(12, "0c")]
        [TestCase(1245, "yl")]
        [TestCase(12464385, "7f5kx")]
        public void EncodeToBase36Digits2(long value, string resultExpected)
        {
            var UtilsMgr = new UtilsService();
            var result = UtilsMgr.EncodeToBase36Digits2(value);

            Assert.AreEqual(resultExpected, result);
        }

        [TestCase("00", 0)]
        [TestCase("0c", 12)]
        [TestCase("yl", 1245)]
        [TestCase("7f5kx", 12464385)]
        public void DecodeBase36ToDecimal(string value, long resultExpected)
        {
            var UtilsMgr = new UtilsService();
            var result = UtilsMgr.DecodeBase36ToDecimal(value);

            Assert.AreEqual(resultExpected, result);
        }
    }
}
