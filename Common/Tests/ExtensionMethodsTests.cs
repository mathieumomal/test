using Etsi.Ultimate.Utils;
using NUnit.Framework;
namespace Etsi.Ultimate.Tests
{
    public class ExtensionMethodsTests
    {
        [TestCase("3GPP GERAN", "GERAN")]
        [TestCase(" 3GPP  GERAN", "GERAN")]
        [TestCase("3GPP GERAN 2", "GERAN 2")]
        [TestCase("3gpp geran 2", "geran 2")]
        [TestCase("3gpP GERAN 2", "GERAN 2")]
        [TestCase(null, "")]
        [TestCase("3GPP", "3GPP")]
        [TestCase(" 3GPP ", "3GPP")]
        [TestCase("3GPP ", "3GPP")]
        public void Without3GppAtTheBeginning(string initial, string expected)
        {
            var result = initial.Remove3GppAtTheBeginningOfAString();

            Assert.AreEqual(expected, result);
        }

        [TestCase("3GPP GERAN 1, 3GPP GERAN 2", "GERAN 1, GERAN 2")]
        [TestCase(" 3GPP GERAN 1, 3GPP  GERAN 2", "GERAN 1, GERAN 2")]
        [TestCase(",,", "")]
        [TestCase(null, "")]
        [TestCase("3GPP, 3GPP GERAN 1, 3GPP  GERAN 2", "3GPP, GERAN 1, GERAN 2")]
        public void Remove3GppFromListofSecondaryResponsibleGroups(string initial, string expected)
        {
            var result = initial.Remove3GppInsideListOfElementsComaSeparated();

            Assert.AreEqual(expected, result);
        }
    }
}
