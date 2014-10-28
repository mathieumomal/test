using System;
using DatabaseImport.ModuleImport;
using DatabaseImport.ModuleImport.U3GPPDB.Version;
using DatabaseImportTests.FakeDBSets;
using Etsi.Ultimate.DataAccess;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Tests.FakeSets;
using System.Collections.Generic;
using System.Data.Entity;

namespace DatabaseImportTests
{
    public class TestVersionImport
    {
        #region test datas
        public DateTime AchievedDate = DateTime.Now.AddDays(-1);
        public DateTime ExpertProvided = DateTime.Now.AddDays(10);
        public DateTime DocumentUploaded = DateTime.Now.AddDays(10);
        public DateTime DocumentPassedToPub = DateTime.Now.AddDays(15);
        public String LegacyLocation = "get it#http://www.3gpp.org/ftp/Specs/archive/24_series/24.008/24008-420.zip#";
        public String Location = "http://www.3gpp.org/ftp/Specs/archive/24_series/24.008/24008-420.zip";
        #endregion

        [Test]
        public void Version_Test_FillDatabase()
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new VersionFakeDBSet();
            //var releaseDbSet = new ReleaseFakeDBSet();
            newContext.Stub(ctx => ctx.SpecVersions).Return(newDbSet);
            newContext.Stub(ctx => ctx.Releases).Return(GetReleases());
            newContext.Stub(ctx => ctx.Specifications).Return(GetSpecs());
            newContext.Stub(ctx => ctx.Meetings).Return(GetMeetings());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.C2001_04_25_schedule).Return(GetSchedule_Legacy());
            legacyContext.Stub(ctx => ctx.plenary_meetings_with_end_dates).Return(new MeetingsWithEndDatesFakeDbSet());

            // Report
            var report = new Domain.Report();
            // Execute
            var import = new VersionImport() { LegacyContext = legacyContext, UltimateContext = newContext, MtgHelper = new MeetingHelper(legacyContext,newContext) };
            import.FillDatabase();


            // Tests globals
            Assert.AreEqual(2, newDbSet.All().Count);

            var newVersion = newDbSet.All()[0];
            //Global version
            Assert.AreEqual(5, newVersion.MajorVersion);
            Assert.AreEqual(4, newVersion.TechnicalVersion);
            Assert.AreEqual(99, newVersion.EditorialVersion);
            Assert.AreEqual(AchievedDate, newVersion.AchievedDate);
            Assert.AreEqual(ExpertProvided, newVersion.ExpertProvided);
            Assert.AreEqual(Location, newVersion.Location);

            Assert.AreEqual(true, newVersion.SupressFromSDO_Pub);
            Assert.AreEqual(true, newVersion.ForcePublication);
            Assert.AreEqual(DocumentUploaded, newVersion.DocumentUploaded);
            Assert.AreEqual(false, newVersion.Multifile);
            Assert.AreEqual(23, newVersion.ETSI_WKI_ID);
            Assert.AreEqual(3, newVersion.Fk_SpecificationId);
            Assert.AreEqual(45, newVersion.Fk_ReleaseId);
            Assert.AreEqual(DocumentPassedToPub, newVersion.DocumentPassedToPub);
            Assert.AreEqual(983, newVersion.Source);

        }
        #region new datas
        private IDbSet<Domain.Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Domain.Release() { Pk_ReleaseId = 1, Code = "Ph2" });
            list.Add(new Domain.Release() { Pk_ReleaseId = 2, Code = "Ph1" });
            list.Add(new Domain.Release() { Pk_ReleaseId = 45, Code = "R98" });
            return list;
        }

        private IDbSet<Domain.Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Domain.Specification() { Pk_SpecificationId = 1, Number = "01.01" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 2, Number = "01.02" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 3, Number = "01.03" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 4, Number = "02.72" });
            return list;
        }
        private IDbSet<Domain.Meeting> GetMeetings()
        {
            var list = new MeetingFakeDBSet();
            list.Add(new Domain.Meeting() { MTG_ID = 98, MTG_REF = "3GPPSA#21", MtgShortRef = "SP-21"});
            list.Add(new Domain.Meeting() { MTG_ID = 983, MTG_REF = "3GPPPCG#10", MtgShortRef = "PCG-10" });
            return list;
        }
        #endregion

        #region legacy datas
        private IDbSet<C2001_04_25_schedule> GetSchedule_Legacy()
        {
            var list = new ScheduleLegacyFakeDBSet();
            list.Add(
                new C2001_04_25_schedule()
                {
                    spec = "01.03",
                    MAJOR_VERSION_NB = 5,
                    TECHNICAL_VERSION_NB = 4,
                    EDITORIAL_VERSION_NB = 99,
                    ACHIEVED_DATE = AchievedDate,
                    expert_provided = ExpertProvided,
                    location = LegacyLocation,
                    suppress_SDO_publication = true,
                    force_publication = true,
                    uploaded = DocumentUploaded,
                    multifile = null,
                    WKI_ID = 23,
                    release = "R98",
                    toETSI = DocumentPassedToPub,
                    meeting = "PCG-10"
                }
            );
            list.Add(
                new C2001_04_25_schedule()
                {
                    spec = "02.72",
                    MAJOR_VERSION_NB = 1460,
                    TECHNICAL_VERSION_NB = 2,
                    EDITORIAL_VERSION_NB = 999,
                    ACHIEVED_DATE = AchievedDate,
                    expert_provided = ExpertProvided,
                    location = LegacyLocation,
                    suppress_SDO_publication = null,
                    force_publication = null,
                    uploaded = DocumentUploaded,
                    multifile = null,
                    WKI_ID = 34,
                    release = "Ph1",
                    meeting = "PCG-10"
                }
            );
            return list;
        }
        #endregion
    }
}
