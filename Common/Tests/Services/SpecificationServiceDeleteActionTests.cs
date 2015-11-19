using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecificationServiceDeleteActionTests : BaseEffortTest
    {
        #region Init
        private ISpecificationService _svc;
        private ISpecificationRepository _mockSpecRepo;
        private const int UserWithDeleteRight = 1;
        private const int UserWithoutDeleteRight = 0;
        private const int SpecIdForNominalCase = 1;
        private const int SpecIdForErrorCase = 2;
        private const int SpecIdForUnexistingSpec = 9999;

        [SetUp]
        public void Setup()
        {
            _svc = ServicesFactory.Resolve<ISpecificationService>();

            //Mock user rights
            var deletRightContainer = new UserRightsContainer();
            deletRightContainer.AddRight(Enum_UserRights.Specification_Delete);
            var mockUserRights = MockRepository.GenerateMock<IRightsManager>();
            mockUserRights.Stub(x => x.GetRights(Arg<int>.Is.Equal(UserWithDeleteRight)))
                .Return(deletRightContainer);
            mockUserRights.Stub(x => x.GetRights(Arg<int>.Is.Equal(UserWithoutDeleteRight)))
                .Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance(typeof (IRightsManager), mockUserRights);

            //Mock spec repository
            _mockSpecRepo = MockRepository.GenerateMock<ISpecificationRepository>();
            _mockSpecRepo.Stub(x => x.SpecExists(Arg<int>.Is.Equal(SpecIdForNominalCase))).Return("ABC_UID");
            _mockSpecRepo.Stub(x => x.SpecExists(Arg<int>.Is.Equal(SpecIdForErrorCase))).Return("ABC_UID");
            _mockSpecRepo.Stub(x => x.SpecExists(Arg<int>.Is.Equal(SpecIdForUnexistingSpec))).Return(null);
            _mockSpecRepo.Stub(x => x.DeleteSpecification(Arg<int>.Is.Equal(SpecIdForNominalCase))).Return(true);
            _mockSpecRepo.Stub(x => x.DeleteSpecification(Arg<int>.Is.Equal(SpecIdForErrorCase))).Return(false);
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecificationRepository), _mockSpecRepo);

            //Mock version repo
            var mockVersionRepo = MockRepository.GenerateMock<ISpecVersionsRepository>();
            //Spec for nominal case
            mockVersionRepo.Stub(x => x.AlreadyUploadedVersionsForSpec(Arg<int>.Is.Equal(SpecIdForNominalCase)))
                .Return(new List<SpecVersion>());
            mockVersionRepo.Stub(x => x.VersionsLinkedToChangeRequestsForSpec(Arg<int>.Is.Equal(SpecIdForNominalCase)))
                .Return(new List<SpecVersion>());
            //Spec for error cases
            mockVersionRepo.Stub(x => x.AlreadyUploadedVersionsForSpec(Arg<int>.Is.Equal(SpecIdForErrorCase)))
                           .Return(new List<SpecVersion> { new SpecVersion { MajorVersion = 1, TechnicalVersion = 0, EditorialVersion = 0 }, new SpecVersion { MajorVersion = 2, TechnicalVersion = 0, EditorialVersion = 0 } });
            mockVersionRepo.Stub(x => x.VersionsLinkedToChangeRequestsForSpec(Arg<int>.Is.Equal(SpecIdForErrorCase)))
                .Return(new List<SpecVersion> { new SpecVersion { CurrentChangeRequests = new List<ChangeRequest> { new ChangeRequest { CRNumber = "A", Revision = null }, new ChangeRequest { CRNumber = "B", Revision = 1 } } } });
            RepositoryFactory.Container.RegisterInstance(typeof (ISpecVersionsRepository), mockVersionRepo);

            //Mock viewContributionWithAditionnalData repo
            var mockVwad = MockRepository.GenerateMock<IViewContributionsWithAditionnalDataRepository>();
            mockVwad.Stub(x => x.FindContributionsRelatedToASpec(Arg<int>.Is.Equal(SpecIdForNominalCase)))
                .Return(new List<View_ContributionsWithAditionnalData>());
            mockVwad.Stub(x => x.FindContributionsRelatedToASpec(Arg<int>.Is.Equal(SpecIdForErrorCase)))
                .Return(new List<View_ContributionsWithAditionnalData>
                {
                    new View_ContributionsWithAditionnalData {uid = "ABCX_UID"},
                    new View_ContributionsWithAditionnalData {uid = "ABCY_UID"}
                });
            RepositoryFactory.Container.RegisterInstance(typeof (IViewContributionsWithAditionnalDataRepository),
                mockVwad);
        }
        #endregion

        #region Check tests
        [Test(Description = "Nominal case")]
        public void CheckDeleteSpecificationAllowed_NominalCase()
        {
            var response = _svc.CheckDeleteSpecificationAllowed(SpecIdForNominalCase, UserWithDeleteRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
        }

        [Test(Description = "User doesn't have the right to delete a spec : method should return false with error message")]
        public void CheckDeleteSpecificationAllowed_WithoutDeleteRight()
        {
            var response = _svc.CheckDeleteSpecificationAllowed(SpecIdForNominalCase, UserWithoutDeleteRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(Localization.RightError, response.Report.ErrorList.First());
        }

        [Test(Description = "Spec doesn't exist : method should return false with error message")]
        public void CheckDeleteSpecificationAllowed_SpecDoesntExist()
        {
            var response = _svc.CheckDeleteSpecificationAllowed(9999, UserWithDeleteRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(Localization.Error_Spec_Does_Not_Exist, response.Report.ErrorList.First());
        }

        [Test(Description = "System should not allowed spec deletion because spec is linked to some versions already uploaded or linked to tdoc or CRs : method should return false with warning message")]
        public void CheckDeleteSpecificationAllowed_NotAllowed()
        {
            var response = _svc.CheckDeleteSpecificationAllowed(SpecIdForErrorCase, UserWithDeleteRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(4, response.Report.GetNumberOfWarnings());
            Assert.AreEqual("Specification cannot be deleted because:", response.Report.WarningList.ElementAt(0));
            Assert.AreEqual("- 2 version(s) are already uploaded", response.Report.WarningList.ElementAt(1));
            Assert.AreEqual("- Linked to 2 CR(s)", response.Report.WarningList.ElementAt(2));
            Assert.AreEqual("- Linked to 2 TDoc(s)", response.Report.WarningList.ElementAt(3));
        }
        #endregion

        #region Delete tests
        [Test(Description = "Nominal case")]
        public void DeleteSpecification_NominalCase()
        {
            var response = _svc.DeleteSpecification(SpecIdForNominalCase, UserWithDeleteRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            _mockSpecRepo.AssertWasCalled(x => x.DeleteSpecification(Arg<int>.Is.Equal(SpecIdForNominalCase)));
        }

        [Test(Description = "If right error occured")]
        public void DeleteSpecification_RightError()
        {
            var response = _svc.DeleteSpecification(SpecIdForErrorCase, UserWithDeleteRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.RightError, response.Report.ErrorList.First());
            _mockSpecRepo.AssertWasNotCalled(x => x.DeleteSpecification(Arg<int>.Is.Equal(SpecIdForNominalCase)));
        }
        #endregion
    }
}
