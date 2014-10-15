using System.Collections.Generic;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services
{
    public class CrPackServiceTest : BaseEffortTest
    {
        #region constants

        private const string TsgTdoc = "TSG_ABC";
        #endregion

        #region integration tests
        [Test]
        public void UpdateChangeRequestPackRelatedCrs_NominalCase()
        {
            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(TsgTDocDecisions(),TsgTdoc);
            Assert.IsTrue(response.Result);

            var crWg1 = UoW.Context.ChangeRequests.Find(4);
            var crWg2 = UoW.Context.ChangeRequests.Find(5);
            var crWg3 = UoW.Context.ChangeRequests.Find(7);

            //Check status
            Assert.AreEqual(1, crWg1.Fk_TSGStatus);
            Assert.AreEqual(2, crWg2.Fk_TSGStatus);
            Assert.IsNull(crWg3.Fk_TSGStatus);
            //Check TsgTdocNumber
            Assert.AreEqual(TsgTdoc, crWg1.TSGTDoc);
            Assert.AreEqual(TsgTdoc, crWg2.TSGTDoc);
            Assert.AreEqual(TsgTdoc, crWg3.TSGTDoc);
        }
        #endregion 

        #region data
        /// <summary>
        /// TSGs the t document decisions.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> TsgTDocDecisions()
        {
            var tsgTDocDecisions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string,string>("WG1", "Agreed"),
                new KeyValuePair<string,string>("WG2", "Approved"),
                new KeyValuePair<string,string>("WG3", "")
            };
            return tsgTDocDecisions;
        }
        #endregion
    }
}
