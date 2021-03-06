﻿using System;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Services.SpecVersionServiceTest
{
    [Category("Version")]
    public class SpecVersionDraftLinkTest : BaseEffortTest
    {
        #region Variables

        private SpecVersionService _versionSvc;
        private const int NonExistSpecId = 130000;
        private const int SpecIdUcc22101 = 136080;
        private const int SpecIdDraft22103 = 136082;
        private const int NonExistReleaseId = 2870;
        private const int ReleaseIdRel13 = 2883;
        private const int ReleaseIdRel14 = 2884;
        private const int ReleaseIdRel15 = 2885;
        private const int ReleaseIdClosed = 2874;
        private const int MajorVersion = 2;
        private const int TechnicalVersion = 1;
        private const int EditorialVersion = 0;
        private const int UserHasNoRight = 1;
        private const int UserHasRight = 2;
        private const int MccUser = 4;
		private const int PrimeRapporteurHasNoRight = 3;		
        private const int ReleaseIdWithdrawn = 666;
        private const int SpecIdDraft160000 = 160000;
        #endregion

        #region Setups

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _versionSvc = new SpecVersionService();
            SetupMocks();
        } 

        #endregion

        #region Tests

        [Test]
        public void CheckDraftCreationOrAssociation_VersionShouldBeDraft_MajorInferiorFrom3()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, NonExistSpecId, ReleaseIdRel13, 13, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Error_Version_Major_Number_Should_Be_Draft, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_SpecShouldExist()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, NonExistSpecId, ReleaseIdRel13, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Error_Spec_Does_Not_Exist, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_ReleaseShouldExist()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdUcc22101, NonExistReleaseId, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Error_Release_Does_Not_Exist, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_SpecShouldBeInDraftMode()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdUcc22101, ReleaseIdRel13, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Error_Spec_Draft_Status, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_SpecReleaseShouldExist()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft22103, ReleaseIdRel15, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Allocate_Error_SpecRelease_Does_Not_Exist, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_SpecReleaseIsWithdrawn()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft160000, ReleaseIdWithdrawn, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Specification_reserve_withdrawn_error, svcResponse.Report.ErrorList.First());
        }

        [Test, Description("System should allow association if version is already allocated")]
        public void CheckDraftCreationOrAssociation_AllowsAllocatedVersionsToBeLinked()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft22103, ReleaseIdRel13,
                2, 0, 0);
            Assert.IsTrue(svcResponse.Result);
        }

        [Test, Description("System should not allow allocation of earlier versions.")]
        [TestCase(MajorVersion - 1, TechnicalVersion, EditorialVersion, Description = "Current Version: 2.1.0, Error when version: 1.1.0")]        
        public void CheckDraftCreationOrAssociation_VersionShouldBeLatest(int majorVersion, int technicalVersion, int editorialVersion)
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft22103, ReleaseIdRel13, majorVersion, technicalVersion, editorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Localization.Error_Lower_Version, MajorVersion, TechnicalVersion, EditorialVersion), svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_Rights()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasNoRight, SpecIdDraft22103, ReleaseIdRel13, MajorVersion, TechnicalVersion + 1, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Drafts_Rights_Error_Not_Reporteur, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_NoAllocateVersionRights_ButIsPrimeRapporteur_NoErrors()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(PrimeRapporteurHasNoRight, SpecIdDraft22103, ReleaseIdRel13, MajorVersion, TechnicalVersion + 1, EditorialVersion);
            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_NoErrors()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft22103, ReleaseIdRel13, MajorVersion, TechnicalVersion + 1, EditorialVersion);
            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_ExistingVersionButWrongRelease()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(UserHasRight, SpecIdDraft22103, ReleaseIdRel14, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Wrong_Release_Version, svcResponse.Report.ErrorList.First());
        }

        #endregion

        #region MCC tests
        [Test]
        public void CheckDraftCreationOrAssociation_MCC_SpecReleaseWithdrawn()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(MccUser, SpecIdDraft160000, ReleaseIdWithdrawn, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Specification_reserve_withdrawn_error, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_MCC_ReleaseClosed()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(MccUser, SpecIdDraft22103, ReleaseIdClosed, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsFalse(svcResponse.Result);
            Assert.AreEqual(1, svcResponse.Report.GetNumberOfErrors());
            Assert.AreEqual(Localization.Specification_reserve_withdrawn_error, svcResponse.Report.ErrorList.First());
        }

        [Test]
        public void CheckDraftCreationOrAssociation_MCC_SpecReleaseDoesntExist()
        {
            var svcResponse = _versionSvc.CheckDraftCreationOrAssociation(MccUser, SpecIdDraft160000, ReleaseIdRel13, MajorVersion, TechnicalVersion, EditorialVersion);
            Assert.IsTrue(svcResponse.Result);
            Assert.AreEqual(0, svcResponse.Report.GetNumberOfErrors());
        }
        #endregion

        #region Test Data

        private static void SetupMocks()
        {
            var noRights = new UserRightsContainer();
            var allocateRights = new UserRightsContainer();
            allocateRights.AddRight(Enum_UserRights.Versions_Allocate);
            var mccRights = new UserRightsContainer();
            mccRights.AddRight(Enum_UserRights.Versions_Allocate);
            mccRights.AddRight(Enum_UserRights.Contribution_DraftTsTrEnabledReleaseField);

            var rightsManager = MockRepository.GenerateMock<IRightsManager>();
            rightsManager.Stub(x => x.GetRights(UserHasNoRight)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(PrimeRapporteurHasNoRight)).Return(noRights);
            rightsManager.Stub(x => x.GetRights(UserHasRight)).Return(allocateRights);
            rightsManager.Stub(x => x.GetRights(MccUser)).Return(mccRights);

            ManagerFactory.Container.RegisterInstance(rightsManager);
        } 

        #endregion
    }
}
