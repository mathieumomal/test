using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using Domain = Etsi.Ultimate.DomainClasses;
using NUnit.Framework;

namespace DatabaseImportTests
{
    
    public class TestUtils
    {
        [TestCase(null, 0 ,0, 0)]
        [TestCase("", 0, 0, 0)]
        [TestCase("test", 0, 0, 1)]
        [TestCase("1", 0, 1, 0)]
        [TestCase("1 43", 0, 0, 1)]
        [TestCase("1987", 0, 1987, 0)]
        [TestCase("test", null, null, 1)]
        [TestCase(null, null, null, 0)]
        public void CheckStringToIntTest(string str, int? defaultValue, int? expectedResult, int expectedNumberOfWarning)
        {
            var report = new Domain.Report();
            var result = Utils.CheckStringToInt(str, defaultValue, "message", "id", report);

            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedNumberOfWarning, report.WarningList.Count());
        }
    }
}
