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
        [TestCase(null, 0 ,0)]
        [TestCase("", 0, 0)]
        [TestCase("test", 0, 0)]
        [TestCase("1", 0, 1)]
        [TestCase("1 43", 0, 0)]
        [TestCase("1987", 0, 1987)]
        [TestCase("test", null, null)]
        [TestCase(null, null, null)]
        public void CheckStringToIntTest(string str, int? defaultValue, int? expectedResult)
        {
            var result = Utils.CheckStringToInt(str, defaultValue, "message", "id");

            Assert.AreEqual(expectedResult, result);
        }
    }
}
