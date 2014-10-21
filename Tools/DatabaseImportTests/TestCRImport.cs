using System;
using System.Linq;
using System.Text.RegularExpressions;
using DatabaseImport.ModuleImport.U3GPPDB.CR;
using DatabaseImportTests.FakeDBSets;
using Etsi.Ultimate.DataAccess;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Tests.FakeSets;
using System.Collections.Generic;
using System.Data.Entity;
using Etsi.Ultimate.DomainClasses;


namespace DatabaseImportTests
{
    public class TestCrImport
    {
        #region CR categories
        /// <summary>
        /// CR Category insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRCategory()
        {
            const string categoryExample = "A";
            const string meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRCategoryFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_CRCategory).Return(newDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new CRCategoryLegacyFakeDbSet
            {
                new CR_categories
                {
                    Row_id = 1,
                    CR_category = "A",
                    meaning = meaningExample
                },
                new CR_categories
                {
                    Row_id = 2,
                    CR_category = "B",
                    meaning = meaningExample
                }
            };
            legacyContext.Stub(ctx => ctx.CR_categories).Return(legacyDbSet);

            // Report
            var report = new Report();

            // Execute
            var import = new EnumCrCategoryImport { LegacyContext = legacyContext, UltimateContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(2, newDbSet.All().Count);

            var newCrCategory = newDbSet.All()[0];
            Assert.AreEqual(meaningExample, newCrCategory.Description);
            Assert.AreEqual(categoryExample, newCrCategory.Code);
        }
        #endregion

        #region CR impact
        /// <summary>
        /// TDoc Status insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRImpact()
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRImpactFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_CRImpact).Return(newDbSet);

            // Report
            var report = new Report();

            // Execute
            var import = new EnumCrImpactImport { LegacyContext = null, UltimateContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(4, newDbSet.All().Count);

            var newTDocStatus = newDbSet.All()[0];
            Assert.AreEqual("UICS Apps", newTDocStatus.Code);
        }
        #endregion

        #region CR
        [TestCaseSource("GetCRObjects")]
        public void FillDatabase_CR_NominalCase(List_of_GSM___3G_CRs legacyObject, ChangeRequest expectedObject)
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ChangeRequestFakeDbSet();
            var newWiDbSet = new CRWIFakeDbSet();
            var newRemarksDbSet = new RemarkFakeDbSet();
            newContext.Stub(ctx => ctx.ChangeRequests).Return(newDbSet);
            newContext.Stub(ctx => ctx.Remarks).Return(newRemarksDbSet);
            newContext.Stub(ctx => ctx.CR_WorkItems).Return(newWiDbSet);
            newContext.Stub(ctx => ctx.Enum_CRCategory).Return(GetCRCategory());
            newContext.Stub(ctx => ctx.Enum_ChangeRequestStatus).Return(getChangeRequestStatus());
            newContext.Stub(ctx => ctx.Specifications).Return(GetSpecs());
            newContext.Stub(ctx => ctx.Releases).Return(GetReleases());
            newContext.Stub(ctx => ctx.Specification_Release).Return(GetSpecRelease());
            newContext.Stub(ctx => ctx.SpecVersions).Return(GetVersions());
            newContext.Stub(ctx => ctx.Meetings).Return(GetMeetings());
            newContext.Stub(ctx => ctx.WorkItems).Return(GetWIs());
            newContext.Stub(ctx => ctx.Communities).Return(GetCommunities());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new ListOfGSM3GCRsFakeDbSet { legacyObject };
            legacyContext.Stub(ctx => ctx.List_of_GSM___3G_CRs).Return(legacyDbSet);

            // Report
            var report = new Report();

            // Execute
            var import = new CrImport { LegacyContext = legacyContext, UltimateContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newCr = newDbSet.All()[0];
            Assert.AreEqual(expectedObject.CRNumber, newCr.CRNumber);
            Assert.AreEqual(expectedObject.Revision, newCr.Revision);
            Assert.AreEqual(expectedObject.CreationDate, newCr.CreationDate);
            Assert.AreEqual(expectedObject.Subject, newCr.Subject);
            Assert.AreEqual(expectedObject.Fk_Enum_CRCategory, newCr.Fk_Enum_CRCategory);
            Assert.AreEqual(expectedObject.Fk_WGStatus, newCr.Fk_WGStatus);
            Assert.AreEqual(expectedObject.Fk_TSGStatus, newCr.Fk_TSGStatus);
            Assert.AreEqual(expectedObject.Fk_Release, newCr.Fk_Release);
            Assert.AreEqual(expectedObject.Fk_Specification, newCr.Fk_Specification);
            Assert.AreEqual(expectedObject.TSGSourceOrganizations, newCr.TSGSourceOrganizations);
            Assert.AreEqual(expectedObject.WGSourceOrganizations, newCr.WGSourceOrganizations);
            Assert.AreEqual(expectedObject.Fk_CurrentVersion, newCr.Fk_CurrentVersion);
            Assert.AreEqual(expectedObject.Fk_NewVersion, newCr.Fk_NewVersion);
            Assert.AreEqual(expectedObject.TSGMeeting, newCr.TSGMeeting);
            Assert.AreEqual(expectedObject.WGMeeting, newCr.WGMeeting);
            Assert.AreEqual(expectedObject.TSGTDoc, newCr.TSGTDoc);
            Assert.AreEqual(expectedObject.WGTDoc, newCr.WGTDoc);
            Assert.AreEqual(expectedObject.WGTarget, newCr.WGTarget);
            Assert.AreEqual(expectedObject.TSGTarget, newCr.TSGTarget);
            Assert.AreEqual(expectedObject.WGSourceForTSG, newCr.WGSourceForTSG);
            Assert.AreEqual(1, newCr.Remarks.Count);
            Assert.AreEqual(2, newCr.CR_WorkItems.Count);
        }
        #endregion

        #region scratch test
        [TestCase("agreed", true)]
        [TestCase("Agreed", true)]
        [TestCase("BadStatus", false)]
        [TestCase("agreed - test", true)]
        [TestCase("test - agreed - test", true)]
        public void StringContain_CaseInsensitive(string stringToFound, bool expectedResult)
        {
            var statuses = new List<string> {"Approved", "Agreed"};
            var result = statuses.FirstOrDefault(x => stringToFound.ToLower().Contains(x.ToLower()));

            if (expectedResult)
            {
                Assert.AreEqual("Agreed", result);
            }
            else
            {
                Assert.IsNull(result);
            }
        }
        #endregion

        #region data
        /// <summary>
        /// Send to the test method the legacy CR object and the new expected CR object
        /// </summary>
        private IEnumerable<TestCaseData> GetCRObjects
        {
            get
            {
                //Test 1
                yield return new TestCaseData(
                    new List_of_GSM___3G_CRs//Legacy data
                    {
                        Row_id = 1,
                        CR = "0013",
                        created = new DateTime(2010, 1, 18),
                        Rev = "-",
                        Subject = "test",
                        Cat = "-",
                        Status_1st_Level = "agreed",//TSG level
                        Status_2nd_Level = "approved",//WG level
                        Spec = "1",
                        Phase = "Ph2",
                        Version_Current = "1.1.0",
                        Version_New = "2.2.18",
                        Source_1st_Level = "Vodafone ",
                        Source_2nd_Level = " FT ",
                        Meeting_1st_Level = "S3-48",
                        Meeting_2nd_Level = "JZAYEZ",
                        Doc_1st_Level = "AZER",
                        Doc_2nd_Level = "TYUI",
                        Remarks = "TEST REMARQUES",
                        Workitem = "AZE/RTY",
                        WG_Responsible = "C3"
                    },
                    new ChangeRequest//New data
                    {
                        CRNumber = "0013",
                        CreationDate = new DateTime(2010, 1, 18),
                        Revision = 0,
                        Subject = "test",
                        Fk_Enum_CRCategory = null,
                        Fk_WGStatus = 2,
                        Fk_TSGStatus = 1,
                        Fk_Specification = 1,
                        Fk_Release = 2,
                        TSGSourceOrganizations = "Vodafone",
                        WGSourceOrganizations = "FT",
                        TSGMeeting = 1,
                        WGMeeting = null,
                        Fk_NewVersion = null,
                        Fk_CurrentVersion = 1,
                        TSGTDoc = "AZER",
                        WGTDoc = "TYUI",
                        WGTarget = 1,
                        TSGTarget = 2,
                        WGSourceForTSG = 1
                    }
                );

                //Test 2
                yield return new TestCaseData(
                    new List_of_GSM___3G_CRs//Legacy data
                    {
                        Row_id = 2,
                        CR = "A018",
                        created = new DateTime(2010, 1, 18),
                        Rev = "2",
                        Subject = "test2",
                        Cat = "B",
                        Status_1st_Level = "--- RejEcted - -",//TSG level
                        Status_2nd_Level = "postponed",//WG level
                        Spec = "2",
                        Phase = "Ph2",
                        Version_Current = "1.2.0",
                        Version_New = "2.1.5",
                        Source_1st_Level = "GSM1 ",
                        Source_2nd_Level = "PT12",
                        Meeting_1st_Level = "S3-4",
                        Meeting_2nd_Level = "S3-49",
                        Doc_1st_Level = "AZER2",
                        Doc_2nd_Level = "TYUI2",
                        Remarks = "TEST REMARQUES",
                        Workitem = "RTY/AZE",
                        WG_Responsible = "C9"
                    },
                    new ChangeRequest//New data
                    {
                        CRNumber = "A018",
                        CreationDate = new DateTime(2010, 1, 18),
                        Revision = 2,
                        Subject = "test2",
                        Fk_Enum_CRCategory = 2,
                        Fk_WGStatus = null,
                        Fk_TSGStatus = 3,
                        Fk_Specification = 2,
                        Fk_Release = 2,
                        TSGSourceOrganizations = "GSM1",
                        WGSourceOrganizations = "PT12",
                        TSGMeeting = null,
                        WGMeeting = 2,
                        Fk_NewVersion = 2,
                        Fk_CurrentVersion = null,
                        TSGTDoc = "AZER2",
                        WGTDoc = "TYUI2",
                        WGTarget = 2,
                        TSGTarget = 3,
                        WGSourceForTSG = 2
                    }
                );
            }
        }

        private IDbSet<Enum_CRCategory> GetCRCategory()
        {
            var list = new Enum_CRCategoryFakeDbSet
            {
                new Enum_CRCategory {Pk_EnumCRCategory = 1, Code = "A", Description = "test1"},
                new Enum_CRCategory {Pk_EnumCRCategory = 2, Code = "B", Description = "test2"},
                new Enum_CRCategory {Pk_EnumCRCategory = 3, Code = "C", Description = "test3"}
            };

            return list;
        }

        private IDbSet<Enum_ChangeRequestStatus> getChangeRequestStatus()
        {
            var list = new Enum_ChangeRequestStatusFakeDbSet
            {
                new Enum_ChangeRequestStatus
                {
                    Pk_EnumChangeRequestStatus = 1,
                    Description = "Agreed",
                    Code = "Agreed"
                },
                new Enum_ChangeRequestStatus
                {
                    Pk_EnumChangeRequestStatus = 2,
                    Description = "Approved",
                    Code = "Approved"
                },
                new Enum_ChangeRequestStatus
                {
                    Pk_EnumChangeRequestStatus = 3,
                    Description = "Rejected",
                    Code = "Rejected"
                }
            };

            return list;
        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet
            {
                new Specification {Pk_SpecificationId = 1, Number = "1"},
                new Specification {Pk_SpecificationId = 2, Number = "2"}
            };
            return list;
        }

        private IDbSet<WorkItem> GetWIs()
        {
            var list = new WorkItemFakeDBSet
            {
                new WorkItem {Pk_WorkItemUid = 1, Acronym = "AZE"},
                new WorkItem {Pk_WorkItemUid = 2, Acronym = "RTY"}
            };
            return list;
        }

        private IDbSet<Community> GetCommunities()
        {
            var list = new CommunityFakeDBSet
            {
                new Community {TbId = 1, ShortName = "C3", ParentTbId = 2},
                new Community {TbId = 2, ShortName = "C9", ParentTbId = 3}
            };
            return list;
        }

        private IDbSet<Domain.Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet
            {
                new Domain.Release {Pk_ReleaseId = 1, Code = "Ph1"},
                new Domain.Release {Pk_ReleaseId = 2, Code = "Ph2"}
            };
            return list;
        }
        private IDbSet<Specification_Release> GetSpecRelease()
        {
            var list = new SpecificationReleaseFakeDBSet
            {
                new Specification_Release {Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2},
                new Specification_Release {Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 1, Fk_ReleaseId = 5}
            };
            return list;
        }

        private IDbSet<SpecVersion> GetVersions()
        {
            var list = new SpecVersionFakeDBSet
            {
                new SpecVersion
                {
                    Pk_VersionId = 1,
                    Fk_SpecificationId = 1,
                    Fk_ReleaseId = 2,
                    MajorVersion = 1,
                    TechnicalVersion = 1,
                    EditorialVersion = 0
                },
                new SpecVersion
                {
                    Pk_VersionId = 2,
                    Fk_SpecificationId = 2,
                    Fk_ReleaseId = 2,
                    MajorVersion = 2,
                    TechnicalVersion = 1,
                    EditorialVersion = 5
                }
            };
            return list;
        }
        private IDbSet<Meeting> GetMeetings()
        {
            var list = new MeetingFakeDBSet
            {
                new Meeting {MTG_ID = 1, MtgShortRef = "S3-48"},
                new Meeting {MTG_ID = 2, MtgShortRef = "S3-49"}
            };
            return list;
        }
        #endregion
    }
}
