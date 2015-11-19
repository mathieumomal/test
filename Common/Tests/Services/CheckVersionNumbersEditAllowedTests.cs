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
    public class CheckVersionNumbersEditAllowedTests : BaseEffortTest
    {
        #region Init
        private ISpecVersionService _svc;
        private ISpecVersionsRepository _mockVersionRepo;
        private const int UserWithVersionEditRight = 1;
        private const int UserWithoutVersionEditRight = 0;
        private const int VersionIdForNominalCase = 1;
        private const int VersionIdForErrorCase = 2;
        private const int SpecIdForNominalCase = 1;
        private const int SpecIdForErrorCase = 2;

        [SetUp]
        public void Setup()
        {
            _svc = ServicesFactory.Resolve<ISpecVersionService>();

            //Mock user rights
            var versionEditRightContainer = new UserRightsContainer();
            versionEditRightContainer.AddRight(Enum_UserRights.Versions_Edit);
            var mockUserRights = MockRepository.GenerateMock<IRightsManager>();
            mockUserRights.Stub(x => x.GetRights(Arg<int>.Is.Equal(UserWithVersionEditRight)))
                .Return(versionEditRightContainer);
            mockUserRights.Stub(x => x.GetRights(Arg<int>.Is.Equal(UserWithoutVersionEditRight)))
                .Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance(typeof (IRightsManager), mockUserRights);

            //Mock version repository
            _mockVersionRepo = MockRepository.GenerateMock<ISpecVersionsRepository>();
            _mockVersionRepo.Stub(x => x.FindCrsLinkedToAVersion(Arg<int>.Is.Equal(VersionIdForNominalCase))).Return(new SpecVersion());
            _mockVersionRepo.Stub(x => x.FindCrsLinkedToAVersion(Arg<int>.Is.Equal(VersionIdForErrorCase))).Return(new SpecVersion { CurrentChangeRequests = new List<ChangeRequest> { new ChangeRequest { CRNumber = "A" } } });
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), _mockVersionRepo);

            //Mock viewContributionWithAditionnalData repo
            var mockVwad = MockRepository.GenerateMock<IViewContributionsWithAditionnalDataRepository>();
            mockVwad.Stub(x => x.FindContributionsRelatedToASpecAndVersionNumber(Arg<int>.Is.Equal(SpecIdForNominalCase), Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Return(new List<View_ContributionsWithAditionnalData>());
            mockVwad.Stub(x => x.FindContributionsRelatedToASpecAndVersionNumber(Arg<int>.Is.Equal(SpecIdForErrorCase), Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything))
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
        public void CheckVersionNumbersEditAllowed_NominalCase()
        {
            var response = _svc.CheckVersionNumbersEditAllowed(GetBaseVersion(), UserWithVersionEditRight);
            Assert.IsTrue(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
        }

        [Test(Description = "User doesn't have the right to edit version : method should return false with error message")]
        public void CheckVersionNumbersEditAllowed_WithoutDeleteRight()
        {
            var response = _svc.CheckVersionNumbersEditAllowed(GetBaseVersion(), UserWithoutVersionEditRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(Localization.RightError, response.Report.ErrorList.First());
        }

        [Test(Description = "Version is not linked to a spec : method should return false with error message")]
        public void CheckVersionNumbersEditAllowed_SpecVersionNotExist()
        {
            var version = GetBaseVersion();
            version.Fk_SpecificationId = null;
            var response = _svc.CheckVersionNumbersEditAllowed(version, UserWithVersionEditRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(Localization.GenericError, response.Report.ErrorList.First());
        }

        [Test(Description = "System should not allowed version edition because version is already uploaded or linked to tdoc and/or CRs : method should return false with warning message")]
        public void CheckVersionNumbersEditAllowed_NotAllowed()
        {
            var version = GetBaseVersion();
            version.Pk_VersionId = VersionIdForErrorCase;
            version.Location = "A";
            version.Fk_SpecificationId = SpecIdForErrorCase;
            var response = _svc.CheckVersionNumbersEditAllowed(version, UserWithVersionEditRight);
            Assert.IsFalse(response.Result);
            Assert.AreEqual(0, response.Report.GetNumberOfErrors());
            Assert.AreEqual(4, response.Report.GetNumberOfWarnings());
            Assert.AreEqual("Version cannot be edited because:", response.Report.WarningList.ElementAt(0));
            Assert.AreEqual("Version is already uploaded", response.Report.WarningList.ElementAt(1));
            Assert.AreEqual("Version is linked to 1 CR(s)", response.Report.WarningList.ElementAt(2));
            Assert.AreEqual("Version is linked to 2 TDoc(s)", response.Report.WarningList.ElementAt(3));
        }
        #endregion

        #region data

        private SpecVersion GetBaseVersion()
        {
            return new SpecVersion
            {
                Pk_VersionId = VersionIdForNominalCase,
                Fk_SpecificationId = SpecIdForNominalCase,
                Location = "",
                DocumentUploaded = null
            };
        }
        #endregion
    }
}
