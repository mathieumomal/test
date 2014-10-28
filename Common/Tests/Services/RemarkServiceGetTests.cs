using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    public class RemarkServiceGetTests: BaseEffortTest
    {
        public const int MccUserId = 80;
        public const int AnonymousUserId = 0;

        RemarkService service;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            // Set up rights
            var userRights = MockRepository.GenerateMock<IRightsManager>();
            var rightsContainer = new UserRightsContainer();
            rightsContainer.AddRight(Enum_UserRights.Remarks_ViewPrivate);
            userRights.Stub(r => r.GetRights(MccUserId)).Return(rightsContainer);
            userRights.Stub(r => r.GetRights(AnonymousUserId)).Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);

            service = new RemarkService();

        }

        [Test, Description("System should return an empty list in case no remark can be found")]
        public void RemarksService_GetRemarks_ReturnsEmptyListIfNoRemarks()
        {
            
            var response = service.GetRemarks("version", 4521, 0);

            Assert.IsNotNull(response.Result);
            Assert.IsEmpty(response.Result);
        }

        [Test, Description("System should return user rights")]
        public void RemarksService_GetRemarks_ReturnsUserRights()
        {
            var response = service.GetRemarks("version", 4521, MccUserId);

            Assert.IsNotNull(response.Rights);
            Assert.IsTrue(response.Rights.HasRight(Enum_UserRights.Remarks_ViewPrivate));
        }

        [Test, Description("System should return remarks for versions, if any")]
        public void RemarksService_GetRemarks_ReturnsRemarksForVersion()
        {
            var response = service.GetRemarks("version", 428931, MccUserId);

            Assert.IsNotNull(response.Result);
            Assert.AreEqual(2,response.Result.Count);
        }

        [Test, Description("System should return remarks for spec release object, if any")]
        public void RemarksService_GetRemarks_ReturnsRemarksForSpecReleases()
        {
            var response = service.GetRemarks("specRelease", 7, MccUserId);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(1, response.Result.Count);
            Assert.AreEqual(217992, response.Result.First().Pk_RemarkId);
        }

        [Test, Description("System should discard all the remarks that user should not be able to view")]
        public void RemarksService_GetRemarks_RemovesNonAllowedRemarks()
        {
            var response = service.GetRemarks("version", 428931, AnonymousUserId);
            Assert.AreEqual(1, response.Result.Count);
            Assert.AreEqual(217990, response.Result.First().Pk_RemarkId);

        }



    }
}
