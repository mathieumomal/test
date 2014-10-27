using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DatabaseImport.ModuleImport.NGPPDB.Contribution;
using DatabaseImportTests.FakeDBSets;
using Etsi.Ngppdb.DataAccess;
using Etsi.Ngppdb.DomainClasses;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using DatabaseImport.ModuleImport;

namespace DatabaseImportTests
{
    public class TestContributionImport
    {
        #region Contribution
        [TestCaseSource("GetContributionForTests")]
        public void FillDatabase_TDoc(C2006_03_17_tdocs legacyObject, Contribution expectedObject)
        {
            //New ultimate context modk
            var new3GppDbContext = MockRepository.GenerateMock<IUltimateContext>();
            new3GppDbContext.Stub(x => x.Meetings).Return(GetMeetings());

            // New ngppdb context mock
            var newNgppDbContext = MockRepository.GenerateMock<INGPPDBContext>();
            var newDbSet = new ContributionFakeDbSet();
            newNgppDbContext.Stub(ctx => ctx.Contribution).Return(newDbSet);
            newNgppDbContext.Stub(ctx => ctx.Enum_ContributionStatus).Return(GetContributionStatus());
            newNgppDbContext.Stub(ctx => ctx.Enum_ContributionType).Return(GetContributionTypes());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new TdocLegacyFakeDbSet {legacyObject};
            legacyContext.Stub(ctx => ctx.C2006_03_17_tdocs).Return(legacyDbSet);
            var legacyCrSet = new ListOfGSM3GCRsFakeDbSet { new List_of_GSM___3G_CRs{ Doc_1st_Level="C1-131002"}};
            legacyContext.Stub(ctx => ctx.List_of_GSM___3G_CRs).Return(legacyCrSet);
            legacyContext.Stub(ctx => ctx.plenary_meetings_with_end_dates).Return(GetLegaycMeetings());

            // Report
            var report = new Report();

            // Execute
            var import = new ContributionImport()
            {
                LegacyContext = legacyContext, 
                Report = report, 
                NgppdbContext = newNgppDbContext, 
                UltimateContext = new3GppDbContext,
                MtgHelper = new MeetingHelper(legacyContext, new3GppDbContext)
            };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.Count());

            var newTDoc = newDbSet.FirstOrDefault();
            Assert.IsNotNull(newTDoc);
            Assert.AreEqual(expectedObject.uid, newTDoc.uid);
            Assert.AreEqual(expectedObject.title, newTDoc.title);
            Assert.AreEqual(expectedObject.fk_Enum_ContributionStatus, newTDoc.fk_Enum_ContributionStatus);
            Assert.AreEqual(expectedObject.MainContact, newTDoc.MainContact);
            Assert.AreEqual(expectedObject.fk_Owner, newTDoc.fk_Owner);
            Assert.AreEqual(expectedObject.fk_Enum_ContributionType, newTDoc.fk_Enum_ContributionType);
            if (expectedObject.ContribAllocation.FirstOrDefault() != null)
            {
                Assert.IsNotEmpty(newTDoc.ContribAllocation);
                Assert.AreEqual(expectedObject.ContribAllocation.FirstOrDefault().fk_Meeting,
                    newTDoc.ContribAllocation.FirstOrDefault().fk_Meeting);
                Assert.AreEqual(expectedObject.ContribAllocation.FirstOrDefault().lastModificationAuthor,
                    newTDoc.ContribAllocation.FirstOrDefault().lastModificationAuthor);
            }
            else
            {
                Assert.IsEmpty(newTDoc.ContribAllocation);
            }
        }
        #endregion
        

        #region data

        private IEnumerable<TestCaseData> GetContributionForTests
        {
            get
            {
                yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 1,
                        doc_mtgId = 30288,
                        doc_mtg = "C1-82b",
                        doc_tdoc = "C1-131002",
                        doc_title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        doc_source = "Vodafone",
                        doc_remarks = "ReServeD",
                        doc_hlink = "http://www.3gpp.org/ftp/tsg_ct/WG1_mm-cc-sm_ex-CN1/TSGC1_82bis_San-Diego/Docs/C1-131003.zip",
                        record_added = new DateTime(2010, 1, 18),
                        url_checked = new DateTime(2010, 1, 18),
                        crHarvested = null,
                        crFlag = true
                    },
                    new Contribution()
                    {
                        title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        name = null,
                        uid = "C1-131002",
                        MainContact = "Import from MS Access",
                        fk_Enum_ContributionStatus = 17,
                        fk_Owner = 0,
                        fk_Enum_ContributionType = 2,
                        ContribAllocation = new List<ContribAllocation>
                        {
                            new ContribAllocation
                            {
                                fk_Meeting = 1,
                                lastModificationAuthor = "Import from MS Access",
                                lastModificationDate = DateTime.Now,
                                ContribAllocation_Date = DateTime.Now,
                                ContribAllocation_Number = 0
                            }
                        },
                        fk_Enum_For = 1,

                        meetingNotes = null,
                        @abstract = "",
                        dateAvailable = new DateTime(2010, 1, 18),
                        isPostponed = false,
                        fk_ContributionSource = null,
                        fk_Commitee = null,
                        fk_AssignedTo = null,
                        fk_SubmittedAsRole = null,
                        fk_RevisionOf = 2,
                        fk_ContributionFile = null,
                        fk_Enum_Vote = null,
                        lastModificationDate = new DateTime(2010, 1, 18),
                        lastModificationAuthor = "MEREDITH",
                        Contact = null,
                        fk_SubmittedBy = null,
                        revisionRequested = null,
                        revisionPreApproved = null,
                        isTreated = null,
                        Denorm_Source = "ETSI",
                        Status_Comment = null,
                        CurrentStatusDate = null,
                        isLocked = false,
                        statusInRC = null,
                        commentsInRC = null,
                        ActiveRemoteConsensus = null,
                        LastStatusChangeAuthor = null,
                        fk_RevisedIn = null,
                        LongName = null,
                        Fk_LatestAllocation = null,
                        LastStatusChangeAuthor_old = null
                    }
                );

                yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 2,
                        doc_mtgId = 30288,
                        doc_mtg = "C1-82X",
                        doc_tdoc = "C1-131003",
                        doc_title = "title",
                        doc_source = "Vodafone",
                        doc_remarks = "default ---",
                        doc_hlink = "http://www.3gpp.org/ftp/tsg_ct/WG1_mm-cc-sm_ex-CN1/TSGC1_82bis_San-Diego/Docs/C1-131003.zip",
                        record_added = new DateTime(2010, 1, 18),
                        url_checked = new DateTime(2010, 1, 18),
                        crHarvested = null,
                        crFlag = false
                    },
                    new Contribution()
                    {
                        title = "title",
                        name = null,
                        uid = "C1-131003",
                        MainContact = "Import from MS Access",
                        fk_Enum_ContributionStatus = 17,
                        fk_Owner = 0,
                        fk_Enum_ContributionType = 1,
                        fk_Enum_For = 1,

                        ContribAllocation = new List<ContribAllocation>
                        {
                            new ContribAllocation
                            {
                                fk_Meeting = 3,
                                lastModificationAuthor = "Import from MS Access",
                                lastModificationDate = DateTime.Now,
                                ContribAllocation_Date = DateTime.Now,
                                ContribAllocation_Number = 0
                            }
                        },

                        meetingNotes = null,
                        @abstract = "",
                        dateAvailable = new DateTime(2010, 1, 18),
                        isPostponed = false,
                        fk_ContributionSource = null,
                        fk_Commitee = null,
                        fk_AssignedTo = null,
                        fk_SubmittedAsRole = null,
                        fk_RevisionOf = 2,
                        fk_ContributionFile = null,
                        fk_Enum_Vote = null,
                        lastModificationDate = new DateTime(2010, 1, 18),
                        lastModificationAuthor = "MEREDITH",
                        Contact = null,
                        fk_SubmittedBy = null,
                        revisionRequested = null,
                        revisionPreApproved = null,
                        isTreated = null,
                        Denorm_Source = "ETSI",
                        Status_Comment = null,
                        CurrentStatusDate = null,
                        isLocked = false,
                        statusInRC = null,
                        commentsInRC = null,
                        ActiveRemoteConsensus = null,

                        LastStatusChangeAuthor = null,
                        fk_RevisedIn = null,
                        LongName = null,
                        Fk_LatestAllocation = null,
                        LastStatusChangeAuthor_old = null
                    }
                );
            }
        }

        private EnumContributionStatusFakeDbSet GetContributionStatus()
        {
            var dbSet = new EnumContributionStatusFakeDbSet()
            {
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 8, Enum_Value = "Reserved", Enum_Code = "Reserved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 9, Enum_Value = "Available", Enum_Code = "Available"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 5, Enum_Value = "Withdrawn", Enum_Code = "Withdrawn"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 6, Enum_Value = "Revised", Enum_Code = "Revised"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 10, Enum_Value = "Approved", Enum_Code = "Approved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 11, Enum_Value = "Agreed", Enum_Code = "Agreed"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 12, Enum_Value = "Partially approved", Enum_Code = "PartApproved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 13, Enum_Value = "Technically endorsed", Enum_Code = "TechEndorsed"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 14, Enum_Value = "Merged", Enum_Code = "Merged"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 15, Enum_Value = "Reissued", Enum_Code = "Reissued"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 16, Enum_Value = "Treated", Enum_Code = "Treated"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 17, Enum_Value = "-", Enum_Code = "Unknown"}
            };
            return dbSet;
        }

        private EnumContributionTypeFakeDbSet GetContributionTypes()
        {
            var dbSet = new EnumContributionTypeFakeDbSet
            {
                new Enum_ContributionType() {pk_Enum_ContributionType = 1, Enum_Code="Other", Enum_Value = "Other Contribution"},
                new Enum_ContributionType() {pk_Enum_ContributionType = 2, Enum_Code = "CR", Enum_Value="Change Request"}, 
            };
            return dbSet;
        }

        private MeetingFakeDBSet GetMeetings()
        {
            var list = new MeetingFakeDBSet
            {
                new Meeting {MTG_ID = 1, MtgShortRef = "C1-82b"},
                new Meeting {MTG_ID = 2, MtgShortRef = "C1-82c"}
            };
            return list;
        }

        private MeetingsWithEndDatesFakeDbSet GetLegaycMeetings()
        {
            var list = new MeetingsWithEndDatesFakeDbSet
            {
                new plenary_meetings_with_end_dates{ meeting= "C1-82X", mtg_id=3 }
            };
            return list;
        }
        #endregion
    }
}
