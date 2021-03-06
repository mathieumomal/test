﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Services
{
    [TestFixture]
    class FinalizeCrsTests: BaseEffortTest
    {

        ChangeRequestService crService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            crService = new ChangeRequestService();

            InitializeUserRightsMock();
        }

        [Test, Description("System should link the CR to an already existing version")]
        public void FinalizeCr_LinkToExistingPendingUploadVersion()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0004" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(428934, UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0004")).Fk_NewVersion);
        }

        [Test, Description("System should allocate a new version if there is no version pending upload.")]
        public void FinalizeCr_AllocatesNewVersionWhenThereIsNoPendingUpload()
        {
            var versionCount = UoW.Context.SpecVersions.Count();
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0003" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(versionCount+1, UoW.Context.SpecVersions.Count());

            var newVersionId = UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0003")).Fk_NewVersion;
            Assert.IsNotNull(newVersionId);
            var newVersion = UoW.Context.SpecVersions.Find(newVersionId.Value);
            Assert.AreEqual(12, newVersion.MajorVersion.GetValueOrDefault());
            Assert.AreEqual(1, newVersion.TechnicalVersion.GetValueOrDefault());
            Assert.AreEqual(0, newVersion.EditorialVersion.GetValueOrDefault());
        }

        [Test, Description("System should create a new SpecRelease if spec release does not exist yet.")]
        public void FinalizeCr_CreateSpecReleaseIfSpecReleaseDoesNotExist()
        {
            var specReleaseCount = UoW.Context.Specification_Release.Count();
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0005" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(specReleaseCount + 1, UoW.Context.Specification_Release.Count());   // New spec release
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
        }

        [Test, Description("System should raise an error if spec release does not exist yet and the inhibitPromote flag is set to true.")]
        public void FinalizeCr_RaisedAnErrorIfSpecReleaseDoesNotExistAndInhibitPromoteTrue()
        {
            //Set promoteInhibited to true
            var spec = UoW.Context.Specifications.First(x => x.Pk_SpecificationId == 136084);
            spec.promoteInhibited = true;
            UoW.Context.SaveChanges();

            var specReleaseCount = UoW.Context.Specification_Release.Count();
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0005" });
            Assert.IsFalse(response.Result);
            Assert.AreEqual(specReleaseCount, UoW.Context.Specification_Release.Count());   // No new spec release
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(1, response.Report.GetNumberOfErrors());
            Assert.AreEqual("Could not allocate version for Specification 22.105: Release Rel-14 is not defined and cannot be created for this specification because the specification has inhibit promote flag set to true.", response.Report.ErrorList.FirstOrDefault());
        }

        [Test, Description("System should log warning if spec release is withdrawn")]
        public void FinalizeCr_LogsWarningIfSpecReleaseIsWithdrawn()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0009" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(1, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Localization.FinalizeCrs_Warn_SpecReleaseWithdrawn, "22.101", "Rel-14"), response.Report.WarningList.First());
        }

        [Test, Description("System should not treat CRs that are not TSG approved.")]
        public void FinalizeCr_IgnoreNonTsgApprovedCrs()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0001" });
            Assert.IsTrue(response.Result);
            Assert.IsFalse(UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0001")).Fk_NewVersion.HasValue);
        }

        [Test, Description("System should not treat CRs that are for closed releases")]
        public void FinalizeCr_IgnoreCrTargetingClosedReleases()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0002" });
            Assert.IsTrue(response.Result);
            Assert.IsFalse(UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0002")).Fk_NewVersion.HasValue);
        }

        [Test, Description("System should ignore CRs that are already treated")]
        public void FinalizeCr_IgnoresCrsWhichAlreadyHaveAVersion()
        {
            var oldVersion = UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0001")).Fk_NewVersion.GetValueOrDefault();
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0006" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(oldVersion, UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0001")).Fk_NewVersion.GetValueOrDefault());
        }

        [Test, Description("System should ignore CRs that belong to specifications that are withdrawn and log a warning")]
        public void FinalizeCr_IgnoresCrTargetWithdrawnSpec()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0007" });
            Assert.IsTrue(response.Result);
            Assert.IsFalse(UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0007")).Fk_NewVersion.HasValue);
            Assert.AreEqual(1, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Localization.FinalizeCrs_Warn_WithDrawnSpec, "22.102"), response.Report.WarningList.First());
        }

        [Test, Description("System should ignore CRs that belong to specifications not under change control and log a warning")]
        public void FinalizeCr_IgnoresCrTargetSpecsNotUcc()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0008" });
            Assert.IsTrue(response.Result);
            Assert.IsFalse(UoW.Context.ChangeRequests.FirstOrDefault(x => x.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0008")).Fk_NewVersion.HasValue);
            Assert.AreEqual(1, response.Report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Localization.FinalizeCrs_Warn_DraftSpec, "22.104"), response.Report.WarningList.First());
        }

        [Test, Description("If two CRs are for same spec, system should allocate only one version.")]
        public void FinalizeCr_CreatesOneSingleVersionPerSpec()
        {
            var versionCount = UoW.Context.SpecVersions.Count();
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0003", "RP-CR0010" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(versionCount + 1, UoW.Context.SpecVersions.Count());
            var versionId = UoW.Context.SpecVersions.ToList().Last().Pk_VersionId;

            Assert.AreEqual(versionId, UoW.Context.ChangeRequests.First(cr => cr.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0003")).Fk_NewVersion);
            Assert.AreEqual(versionId, UoW.Context.ChangeRequests.First(cr => cr.ChangeRequestTsgDatas.Any(t => t.TSGTdoc == "RP-CR0010")).Fk_NewVersion);
        }

        [Test, Description("System should not create a new SpecRelease if spec release already created.")]
        public void FinalizeCr_DoNotCreateSpecReleaseIfSpecReleaseAlreadyExists()
        {
            //Remove spec release
            var specRelToRemove = UoW.Context.Specification_Release.FirstOrDefault(
                x => x.Fk_ReleaseId == 2882 && x.Fk_SpecificationId == 136084);
            UoW.Context.Specification_Release.Remove(specRelToRemove);
            UoW.Save();
            var specReleaseCount = UoW.Context.Specification_Release.Count();

            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.SPECMGR_ID, new List<string> { "RP-CR0003", "RP-CR0010" });
            Assert.IsTrue(response.Result);
            Assert.AreEqual(specReleaseCount + 1, UoW.Context.Specification_Release.Count());   // Only one new spec release created
            Assert.AreEqual(0, response.Report.GetNumberOfWarnings());
        }

        [Test]
        public void FinalizeCr_ReturnsFalseInCaseOfError()
        {
            var response = crService.SetCrsAsFinal(UserRolesFakeRepository.ANONYMOUS_ID, new List<string> { "RP-CR0003" });
            Assert.IsFalse(response.Result);
        }
    }
}
