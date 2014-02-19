﻿using System;
using DatabaseImport.ModuleImport;
using DatabaseImportTests.LegacyDBSets;
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

    public class TestReleaseImport
    {
        [Test]
        public void Test_Clean()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ReleaseFakeDBSet();
            newDbSet.Add(new Domain.Release() { Pk_ReleaseId = 25, Code = "Rel2" });
            var remarkSet = new RemarkFakeDbSet();
            remarkSet.Add(new Domain.Remark() { Pk_RemarkId = 27, RemarkText = "test", Fk_ReleaseId = 25 });
            remarkSet.Add(new Domain.Remark() { Pk_RemarkId = 29, RemarkText = "test2" });

            newContext.Stub(ctx => ctx.Releases).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(GetStatuses());
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);

            var import = new ReleaseImport() { LegacyContext = null, NewContext = newContext, Report = null };
            import.CleanDatabase();

            Assert.AreEqual(0, newDbSet.All().Count);
            Assert.AreEqual(1, remarkSet.All().Count);  // only one remark deleted, the other is not linked to a remark.
        }


        [Test]
        public void Test_FillDatabase_CommonFields()
        {
            string releaseCode = "R1";
            string releaseDescr = "Release 1";
            string releaseShortDescr = "Rel 1";
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime closureDate = DateTime.Now.AddMonths(1);

            // Set up the mocks

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ReleaseFakeDBSet();
            var remarkSet = new RemarkFakeDbSet();
            newContext.Stub(ctx => ctx.Releases).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(GetStatuses());
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new ReleaseLegacyFakeDBSet();
            legacyDbSet.Add(
                new Release()
                {
                    Row_id = 1,
                    defunct = true,
                    Release_code = releaseCode,
                    Release_description = releaseDescr,
                    Release_short_description = releaseShortDescr,
                    ITUR_code = "zzzz",
                    previousRelease = null,
                    rel_proj_start = startDate,
                    rel_proj_end = closureDate
                }
                    );
            legacyContext.Stub(ctx => ctx.Releases).Return(legacyDbSet);

            // Report
            var report = new ImportReport();

            // Execute
            var import = new ReleaseImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();


            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newRelease = newDbSet.All()[0];
            Assert.AreEqual(releaseCode, newRelease.Code);
            Assert.AreEqual(releaseDescr, newRelease.Name);
            Assert.AreEqual(releaseShortDescr, newRelease.ShortName);
            Assert.AreEqual(startDate, newRelease.StartDate);
            Assert.AreEqual(closureDate, newRelease.ClosureDate);

            Assert.AreEqual(0, remarkSet.All().Count);
        }

        [Test]
        public void Test_FillDatabase_FreezeDates()
        {
            var meetingDate = DateTime.Now.AddMonths(-1);
            var meetingRef = "SP-21";
            var meetingId = 21;
            // Set up the mocks

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ReleaseFakeDBSet();
            var remarkSet = new RemarkFakeDbSet();
            var meetingSet = new MeetingFakeDBSet();
            meetingSet.Add(new Domain.Meeting() { MTG_ID = meetingId, MTG_REF = "3GPPSA#21", END_DATE = meetingDate });

            newContext.Stub(ctx => ctx.Releases).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(GetStatuses());
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);
            newContext.Stub(ctx => ctx.Meetings).Return(meetingSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new ReleaseLegacyFakeDBSet();
            legacyDbSet.Add(
                new Release()
                {
                    Row_id = 1,
                    Release_code = "R1",
                    Release_description = "Release 1",
                    Release_short_description = "Rel 1",
                    freeze_meeting = meetingRef
                }
                    );
            legacyDbSet.Add(
                new Release()
                {
                    Row_id = 2,
                    Release_code = "R2",
                    Release_description = "Release 2",
                    Release_short_description = "Rel 2",
                    freeze_meeting = "SMG-10"
                }
                );
            legacyContext.Stub(ctx => ctx.Releases).Return(legacyDbSet);

            // Report
            var report = new ImportReport();

            // Execute
            var import = new ReleaseImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(2, newDbSet.All().Count);

            var newRelease1 = newDbSet.All().Find(r => r.Code == "R1");
            Assert.AreEqual(meetingRef, newRelease1.Stage3FreezeMtgRef);
            Assert.AreEqual(meetingId, newRelease1.Stage3FreezeMtgId);
            Assert.AreEqual(meetingDate, newRelease1.Stage3FreezeDate);

            var newRelease2 = newDbSet.All().Find(r => r.Code == "R2");
            Assert.AreEqual("SMG-10", newRelease2.Stage3FreezeMtgRef);
            Assert.IsNull(newRelease2.Stage3FreezeMtgId);
            Assert.IsNull(newRelease2.Stage3FreezeDate);

        }

        [Test]
        public void Test_FillDatabase_ReleaseStatus()
        {
            var pastMeetingRef = "SP-21";
            var nextMeetingRef = "SP-22";
            // Set up the mocks

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ReleaseFakeDBSet();
            var remarkSet = new RemarkFakeDbSet();
            var meetingSet = new MeetingFakeDBSet();
            meetingSet.Add(new Domain.Meeting() { MTG_ID = 21, MTG_REF = "3GPPSA#21", END_DATE = DateTime.Now.AddMonths(-1) });
            meetingSet.Add(new Domain.Meeting() { MTG_ID = 22, MTG_REF = "3GPPSA#22", END_DATE = DateTime.Now.AddMonths(1) });

            var statusList = GetStatuses();

            newContext.Stub(ctx => ctx.Releases).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_ReleaseStatus).Return(statusList);
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);
            newContext.Stub(ctx => ctx.Meetings).Return(meetingSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new ReleaseLegacyFakeDBSet();
            legacyDbSet.Add(
                new Release()       // This release has defunct = false ==> It's closed
                {
                    Row_id = 1,
                    Release_code = "R1",
                    Release_description = "Release 1",
                    Release_short_description = "Rel 1",
                    defunct = true
                }
                    );
            legacyDbSet.Add(
                new Release()    // This release has a freeze meeting that is passed, but no defunct ==> It's frozen
                {
                    Row_id = 2,
                    Release_code = "R2",
                    Release_description = "Release 2",
                    Release_short_description = "Rel 2",
                    defunct = false,
                    freeze_meeting = "SP-21"
                }
                );
            legacyDbSet.Add(
                new Release()      // This release has a freeze meeting that is not passed => It's open
                {
                    Row_id = 3,
                    Release_code = "R3",
                    Release_description = "Release 3",
                    Release_short_description = "Rel 3",
                    defunct = false,
                    freeze_meeting = "SP-22"
                }
                );
            legacyDbSet.Add(
                new Release()       // This release has no freeze date => It's open
                {
                    Row_id = 4,
                    Release_code = "R4",
                    Release_description = "Release 4",
                    Release_short_description = "Rel 4",
                    defunct = false,
                }
                );
            legacyContext.Stub(ctx => ctx.Releases).Return(legacyDbSet);

            // Report
            var report = new ImportReport();

            // Execute
            var import = new ReleaseImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(4, newDbSet.All().Count);

            var newRelease1 = newDbSet.All().Find(r => r.Code == "R1");
            Assert.AreEqual(3, newRelease1.Fk_ReleaseStatus);

            var newRelease2 = newDbSet.All().Find(r => r.Code == "R2");
            Assert.AreEqual(2, newRelease2.Fk_ReleaseStatus);

            var newRelease3 = newDbSet.All().Find(r => r.Code == "R3");
            Assert.AreEqual(1, newRelease3.Fk_ReleaseStatus);

            var newRelease4 = newDbSet.All().Find(r => r.Code == "R4");
            Assert.AreEqual(1, newRelease4.Fk_ReleaseStatus);

        }

        private IDbSet<Domain.Enum_ReleaseStatus> GetStatuses()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Domain.Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" });
            list.Add(new Domain.Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" });
            list.Add(new Domain.Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" });

            return list;
        }


    }
}
