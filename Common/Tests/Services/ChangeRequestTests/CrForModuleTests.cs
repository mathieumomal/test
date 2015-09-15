using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Services.ChangeRequestTests
{
    public class CrForModuleTests : BaseEffortTest
    {
        private const int PersonId = 0;

        [Test]
        public void GetChangeRequests_NominalCase()
        {
            //Get db object
            var crTotalCount = UoW.Context.ChangeRequests.Count();
            var concernedCr = UoW.Context.ChangeRequests.Find(5);

            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = 1, SkipRecords = 0 };

            //Call method
            var svc = new ChangeRequestService();
            var response = svc.GetChangeRequests(PersonId, searchObj);

            //Tests
            var crListFacade = response.Result.Key[0];
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.IsNotEmpty(response.Result.Key);
            Assert.AreEqual(crTotalCount, response.Result.Value);

            Assert.AreEqual(crListFacade.ChangeRequestId, concernedCr.Pk_ChangeRequest);
            Assert.AreEqual(crListFacade.SpecNumber, concernedCr.Specification.Number);
            Assert.AreEqual(crListFacade.ChangeRequestNumber, concernedCr.CRNumber);
            Assert.AreEqual(crListFacade.Revision, (concernedCr.Revision ?? 0) == 0 ? "-" : concernedCr.Revision.ToString());
            Assert.AreEqual(crListFacade.ImpactedVersion, string.Format("{0}.{1}.{2}", concernedCr.CurrentVersion.MajorVersion, concernedCr.CurrentVersion.TechnicalVersion,
                        concernedCr.CurrentVersion.EditorialVersion));
            Assert.AreEqual(crListFacade.TargetRelease, concernedCr.Release.ShortName);
            Assert.AreEqual(crListFacade.Title, concernedCr.Subject);
            Assert.AreEqual(crListFacade.WgTdocNumber, concernedCr.WGTDoc);
            Assert.AreEqual(crListFacade.TsgTdocNumber, concernedCr.ChangeRequestTsgDatas.First().TSGTdoc);
            Assert.AreEqual(crListFacade.WgStatus, concernedCr.WgStatus.Description);
            Assert.AreEqual(crListFacade.TsgStatus, concernedCr.ChangeRequestTsgDatas.First().TsgStatus.Description);
            Assert.AreEqual(crListFacade.NewVersion, string.Format("{0}.{1}.{2}", concernedCr.NewVersion.MajorVersion, concernedCr.NewVersion.TechnicalVersion,
                        concernedCr.NewVersion.EditorialVersion));
            Assert.AreEqual(crListFacade.SpecId, concernedCr.Fk_Specification ?? 0);
            Assert.AreEqual(crListFacade.TargetReleaseId, concernedCr.Release.Pk_ReleaseId);
            Assert.AreEqual(crListFacade.NewVersionPath, concernedCr.NewVersion.Location);
        }

        [Test]
        public void GetChangeRequests_ShouldBeLinkToACrPack()
        {
            //Get db object
            var crTotalCount = UoW.Context.ChangeRequests.Count();

            //Search object build
            var searchObj = new ChangeRequestsSearch { PageSize = 100, SkipRecords = 0 };

            //Call method
            var svc = new ChangeRequestService();
            var response = svc.GetChangeRequests(PersonId, searchObj);

            //Tests
            var crListFacade = response.Result.Key[0];
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.IsNotEmpty(response.Result.Key);
            Assert.AreEqual(crTotalCount, response.Result.Value);

            //CR not linked to TDoc of type CR Pack or in TSG level
            Assert.IsTrue(response.Result.Key.FirstOrDefault(x => x.ChangeRequestId == 24).ShouldBeLinkToACrPack);
            //CR linked to TDoc of type CR Pack or in TSG level <=> contains a TSG additionnal data entry
            Assert.IsFalse(response.Result.Key.FirstOrDefault(x => x.ChangeRequestId == 5).ShouldBeLinkToACrPack);
        }
    }
}
