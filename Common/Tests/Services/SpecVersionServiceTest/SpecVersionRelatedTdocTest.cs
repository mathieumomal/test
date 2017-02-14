using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Versions;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionRelatedTdocTest : BaseEffortTest
    {
        #region constantes
        const int UserHasRight = 1;
        #endregion

        #region setup
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SetupMocks();
        }

        private void SetupMocks()
        {
            var allocateRights = new UserRightsContainer();
            allocateRights.AddRight(Enum_UserRights.Versions_Allocate);

            var rightsManager = MockRepository.GenerateMock<IRightsManager>();
            rightsManager.Stub(x => x.GetRights(UserHasRight)).Return(allocateRights);

            var specMgr = MockRepository.GenerateMock<ISpecificationManager>();
            specMgr.Stub(
                x =>
                    x.GetRightsForSpecRelease(Arg<UserRightsContainer>.Is.Anything, Arg<int>.Is.Anything,
                        Arg<Specification>.Is.Anything, Arg<int>.Is.Anything, Arg<List<Release>>.Is.Anything, Arg<SpecVersion>.Is.Anything)).Return(new KeyValuePair<Specification_Release, UserRightsContainer>(null, allocateRights));
            specMgr.Stub(x => x.GetSpecificationById(UserHasRight, 136080))
                .Return(new KeyValuePair<Specification, UserRightsContainer>(UoW.Context.Specifications.FirstOrDefault(x => x.Pk_SpecificationId == 136080), allocateRights));
            ManagerFactory.Container.RegisterInstance(rightsManager);
            ManagerFactory.Container.RegisterInstance(specMgr);
        }
        #endregion

        #region integration tests

        [TestCase(0, 136080, 2883, 0, 13, 0, 1, "R2-001")]
        public void AllocateOrAssociateDraftVersion_TryToCreateSpecRelease(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc)
        {
            var mock = MockRepository.GenerateMock<ISpecReleaseManager>();
            mock.Stub(x => x.CreateSpecRelease(Arg<int>.Is.Anything, Arg<int>.Is.Anything)).Repeat.Once();
            ManagerFactory.Container.RegisterInstance(mock);

            var specVersionSvc = new SpecVersionService();
            specVersionSvc.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion,
                editorialVersion, relatedTdoc);

            mock.VerifyAllExpectations();
        }

        /// <summary>
        /// Version already exist : ASSOCIATION
        /// Please note that to define that a version already exist just the specId and the versions data are necessary
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <param name="relatedTdoc"></param>
        /// <param name="isSuccess"></param>
        /// <param name="infoMessage"></param>
        [TestCase(0, 136080, 2883, 0, 13, 0, 1, "R2-001", true, "")]
        public void UpdateVersionRelatedTdoc_Test_VersionAlreadyExist(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc, bool isSuccess, string infoMessage)
        {
            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>(); repo.UoW = UoW;
            var versionBefore = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            Assert.AreEqual(null, versionBefore.RelatedTDoc);

            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion,
                editorialVersion, relatedTdoc);

            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
            var versionAfter = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            Assert.AreEqual(relatedTdoc, versionAfter.RelatedTDoc);
        }

        /// <summary>
        /// Version doesn't already exist : ALLOCATION
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="meetingId"></param>
        /// <param name="majorVersion"></param>
        /// <param name="technicalVersion"></param>
        /// <param name="editorialVersion"></param>
        /// <param name="relatedTdoc"></param>
        /// <param name="isSuccess"></param>
        /// <param name="infoMessage"></param>
        [TestCase(UserHasRight, 136080, 2874, 0, 13, 0, 2, "R2-001", true, "Version does not exist. So, Tdoc cannot be linked.")]
        public void UpdateVersionRelatedTdoc_Test_VersionDoesntAlreadyExist(int personId, int specId, int releaseId, int meetingId, int majorVersion, int technicalVersion,
            int editorialVersion, string relatedTdoc, bool isSuccess, string infoMessage)
        {
            //Mock version number validator
            var mockISpecVersionNumberValidator = MockRepository.GenerateMock<ISpecVersionNumberValidator>();
            mockISpecVersionNumberValidator.Stub(
                x =>
                    x.CheckSpecVersionNumber(Arg<SpecVersion>.Is.Anything, Arg<SpecVersion>.Is.Anything,
                        Arg<SpecNumberValidatorMode>.Is.Anything, Arg<int>.Is.Anything)).Return(new ServiceResponse<bool> { Result = true });
            ManagerFactory.Container.RegisterInstance(typeof(ISpecVersionNumberValidator),
                mockISpecVersionNumberValidator);

            var repo = RepositoryFactory.Resolve<ISpecVersionsRepository>(); repo.UoW = UoW;
            var versionBefore = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            Assert.AreEqual(null, versionBefore);

            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.AllocateOrAssociateDraftVersion(personId, specId, releaseId, meetingId, majorVersion, technicalVersion,
                editorialVersion, relatedTdoc);

            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
            var versionAfter = repo.GetVersion(specId, majorVersion, technicalVersion, editorialVersion);
            Assert.IsNotNull(versionAfter);
            Assert.AreEqual(relatedTdoc, versionAfter.RelatedTDoc);
        }

        [Test, Description("Error while linking TDoc to Version")]
        public void UpdateVersionRelatedTdoc_Failure_Test()
        {
            const string errorMessage = "Test Exception Raised";
            var mockVersionManager = MockRepository.GenerateMock<ISpecVersionManager>();
            mockVersionManager.Stub(x => x.AllocateOrAssociateDraftVersion(Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything)).Throw(new Exception(errorMessage));
            ManagerFactory.Container.RegisterInstance(typeof(ISpecVersionManager), mockVersionManager);

            var specVersionSvc = new SpecVersionService();
            var svcResponse = specVersionSvc.AllocateOrAssociateDraftVersion(0, 1, 0, 0, 0, 1, 2, "0001");

            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(errorMessage, svcResponse.Report.ErrorList.First());
        }

        [Test, Description("System should remove TDoc reference from old draft")]
        public void UpdateVersionRelatedTDoc_RemovesDraft()
        {
            const int versionId = 428931;
            const string relatedTdoc = "R4-869451";

            var version = UoW.Context.SpecVersions.First(v => v.Pk_VersionId == versionId);
            version.RelatedTDoc = relatedTdoc;
            UoW.Save();

            // Call the specVersion service
            var specVersionSvc = new SpecVersionService();
            specVersionSvc.AllocateOrAssociateDraftVersion(0, 136080, 0, 0, 13, 0, 1, relatedTdoc);

            // Check that TDoc is no longer linked to previous version
            var versionAgain = UoW.Context.SpecVersions.First(v => v.Pk_VersionId == versionId);
            Assert.IsNullOrEmpty(versionAgain.RelatedTDoc);

        }

        [TestCase(136080, 2883,13,0,1,true)]
        [TestCase(136080, 2883, 13, 0, 2, false)]
        [TestCase(150000, 2883, 14, 0, 0, false)]
        public void CheckIfVersionExists(int specId, int releaseId, int majorVersion, int technicalVersion,
            int editorialVersion, bool expectedResult)
        {
            var specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = UoW;

            var result = specVersionRepo.CheckIfVersionExists(specId, releaseId, majorVersion, technicalVersion,
                editorialVersion);

            Assert.AreEqual(expectedResult, result);
        }
        #endregion
    }
}
