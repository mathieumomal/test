using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services.ChangeRequestTests
{
    [Category("CR Tests")]
    public class CrReissueTest : BaseEffortTest
    {
        #region Tests

        [Test]
        public void Service_ReIssueCr_Success()
        {
            var crKey = new CrKeyFacade { SpecNumber = "22.104", Revision = 3, CrNumber = "AZEE", TsgTdocNumber = "TSG_ABC" };
            var cr = UoW.Context.ChangeRequests.FirstOrDefault(x => x.Specification.Number == crKey.SpecNumber && x.CRNumber == crKey.CrNumber && ((x.Revision ?? 0) == crKey.Revision));
            var tsgDataBefore = UoW.Context.ChangeRequestTsgDatas.Where(x => x.Fk_ChangeRequest == cr.Pk_ChangeRequest).Count();
            //Act
            var crService = new ChangeRequestService();
            var response = crService.ReIssueCr(crKey, "XYZ", 1);

            //Assert
            var tsgDataAfter = UoW.Context.ChangeRequestTsgDatas.Where(x => x.Fk_ChangeRequest == cr.Pk_ChangeRequest).ToList();
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.ErrorList.Count);
            Assert.AreEqual(tsgDataBefore + 1, tsgDataAfter.Count());
            Assert.AreEqual("XYZ", tsgDataAfter.Last().TSGTdoc);
            Assert.AreEqual(1, tsgDataAfter.Last().TSGMeeting);
        }

        [Test]
        public void Service_ReIssueCr_Failure()
        {
            var incorrectCrKey = new CrKeyFacade { SpecNumber = "Test", Revision = 1, CrNumber = "Test", TsgTdocNumber = "TSG_ABC" };
            //Act
            var crService = new ChangeRequestService();
            var response = crService.ReIssueCr(incorrectCrKey, "XYZ", 1);

            //Assert
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.ErrorList.Count);
            Assert.AreEqual("Change request not found : Spec#: Test, Cr#: Test, Revision: 1", response.Report.ErrorList[0]);
        } 

        #endregion
    }
}
