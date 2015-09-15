using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services.ChangeRequestTests
{
    [Category("CR Tests")]
    public class CrRevisionTest : BaseEffortTest
    {
        #region Tests

        [Test]
        public void Service_ReviseCr_Success()
        {
            var crKey = new CrKeyFacade { SpecNumber = "22.104", Revision = 3, CrNumber = "AZEE", TsgTdocNumber = "TSG_ABC" };
            var cr = UoW.Context.ChangeRequests.FirstOrDefault(x => x.Specification.Number == crKey.SpecNumber && x.CRNumber == crKey.CrNumber && ((x.Revision ?? 0) == crKey.Revision));
            var crDataBefore = UoW.Context.ChangeRequests.Count();

            //Act
            var crService = new ChangeRequestService();
            var response = crService.ReviseCr(crKey, "XYZ", 1, "source");

            //Assert
            var crDataAfter = UoW.Context.ChangeRequests.ToList();
            var newCrRecord = UoW.Context.ChangeRequests.ToList().Last();

            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.ErrorList.Count);
            Assert.AreEqual(crDataBefore + 1, crDataAfter.Count());

            Assert.AreEqual(cr.CRNumber, newCrRecord.CRNumber);
            Assert.AreEqual(cr.Revision + 1, newCrRecord.Revision);
            Assert.AreEqual(cr.Subject, newCrRecord.Subject);
            Assert.AreEqual(cr.Fk_WGStatus, newCrRecord.Fk_WGStatus);
            Assert.AreEqual(cr.WGSourceOrganizations, newCrRecord.WGSourceOrganizations);
            Assert.AreEqual(cr.WGSourceForTSG, newCrRecord.WGSourceForTSG);
            Assert.AreEqual(cr.WGMeeting, newCrRecord.WGMeeting);
            Assert.AreEqual(cr.WGTarget, newCrRecord.WGTarget);
            Assert.AreEqual("", newCrRecord.WGTDoc);
            Assert.AreEqual(cr.Fk_Enum_CRCategory, newCrRecord.Fk_Enum_CRCategory);
            Assert.AreEqual(cr.Fk_Specification, newCrRecord.Fk_Specification);
            Assert.AreEqual(cr.Fk_Release, newCrRecord.Fk_Release);
            Assert.AreEqual(cr.Fk_CurrentVersion, newCrRecord.Fk_CurrentVersion);
            Assert.AreEqual(cr.Fk_NewVersion, newCrRecord.Fk_NewVersion);
            Assert.AreEqual(cr.Fk_Impact, newCrRecord.Fk_Impact);
            Assert.AreEqual("XYZ", newCrRecord.ChangeRequestTsgDatas.First().TSGTdoc);
            Assert.AreEqual(1, newCrRecord.ChangeRequestTsgDatas.First().TSGMeeting);
            Assert.AreEqual("source", newCrRecord.ChangeRequestTsgDatas.First().TSGSourceOrganizations);
        }

        [Test]
        public void Service_ReviseCr_Failure()
        {
            var incorrectCrKey = new CrKeyFacade { SpecNumber = "Test", Revision = 1, CrNumber = "Test", TsgTdocNumber = "TSG_ABC" };
            //Act
            var crService = new ChangeRequestService();
            var response = crService.ReviseCr(incorrectCrKey, "XYZ", 1, "source");

            //Assert
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.ErrorList.Count);
            Assert.AreEqual("Change request not found : Spec#: Test, Cr#: Test, Revision: 1", response.Report.ErrorList[0]);
        } 

        #endregion
    }
}
