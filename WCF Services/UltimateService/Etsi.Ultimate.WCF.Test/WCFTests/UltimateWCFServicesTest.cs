using Etsi.Ultimate.WCF.Service;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.WCFTests
{
    public class UltimateWCFServicesTest : BaseEffortTest
    {
        [Test, Description("System should return the CR objects related to the contribution")]
        public void WCFTest_CRService_GetChangeRequestByContribUid()
        {
            const string contribUid = "Change request description6";
            const string tdocNumber = "A0144";
            const int tdocRevision = 1;
            //Act
            var svcCr = new UltimateService();
            var result = svcCr.GetChangeRequestByContributionUID(contribUid);
            //Asserts
            Assert.IsNotNull(result);
            Assert.AreEqual(contribUid, result.TSGTDoc);
            Assert.AreEqual(tdocNumber, result.CRNumber);
            Assert.AreEqual(tdocRevision, result.Revision);
        }   
    }
}
