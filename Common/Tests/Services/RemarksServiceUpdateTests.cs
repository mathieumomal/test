using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Services;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    class RemarksServiceUpdateTests: BaseEffortTest
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

        [Test]
        public void RemarksService_Update_AddNewEntriesForVersions()
        {
            var newRemarks = new List<Remark>{
                new Remark { IsPublic= true, Fk_VersionId=428931, RemarkText="New Remark" }
            };

            var response = service.UpdateRemarks(newRemarks, MccUserId);

            Assert.IsTrue(response.Result);
            Assert.AreEqual(3, service.GetRemarks("version", 428931, MccUserId).Result.Count);
        }

        [Test, Description("When updating a remark, only the RemarkText can be updated")]
        public void RemarksService_Update_UpdatesExistingEntriesForVersions()
        {
            var newRemarks = new List<Remark>{
                new Remark { Pk_RemarkId=217990,  IsPublic= false, Fk_VersionId=428931, RemarkText="New Remark" }
            };

            var response = service.UpdateRemarks(newRemarks, MccUserId);

            Assert.IsTrue(response.Result);
            Assert.IsFalse(Context.Remarks.Find(217990).IsPublic.GetValueOrDefault());
            Assert.AreNotEqual("New Remark", Context.Remarks.Find(217990).RemarkText);

        }

        [Test, Description("When updating a remark, only the RemarkText can be updated")]
        public void RemarksService_Update_AddsNewEntriesForSpecRelease()
        {
            var newRemarks = new List<Remark>{
                new Remark { IsPublic= false, Fk_SpecificationReleaseId=7, RemarkText="New Remark" }
            };

            var response = service.UpdateRemarks(newRemarks, MccUserId);

            Assert.IsTrue(response.Result);
            Assert.AreEqual(2, service.GetRemarks("specRelease",7, MccUserId).Result.Count);

        }
    }
}
