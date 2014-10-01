using System;
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
using Etsi.Ultimate.DomainClasses;


namespace DatabaseImportTests
{
    public class TestCRImport
    {
        /// <summary>
        /// CR Category insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRCategory()
        {
            var categoryExample = "A";
            var meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRCategoryFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_CRCategory).Return(newDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new CRCategoryLegacyFakeDbSet
            {
                new CR_categories()
                {
                    Row_id = 1,
                    CR_category = "A",
                    meaning = meaningExample
                },
                new CR_categories()
                {
                    Row_id = 2,
                    CR_category = "B",
                    meaning = meaningExample
                }
            };
            legacyContext.Stub(ctx => ctx.CR_categories).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new Enum_CRCategoryImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(2, newDbSet.All().Count);

            var newCrCategory = newDbSet.All()[0];
            Assert.AreEqual(meaningExample, newCrCategory.Description);
            Assert.AreEqual(categoryExample, newCrCategory.Code);
        }

        /// <summary>
        /// TDoc Status insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_TDocStatus()
        {
            var validStatus = "tech endorsed";
            var statusToRemove = " - ";
            var meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_TDocStatusFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_TDocStatus).Return(newDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new CRStatusValuesLegacyFakeDbSet();
            legacyDbSet.Add(
                new CR_status_values()
                {
                    Row_id = 1,
                    CR_status_value = statusToRemove,
                    sort_order = 43,
                    use = meaningExample,
                    WG = true,
                    TSG = false
                }
            );
            legacyDbSet.Add(
                new CR_status_values()
                {
                    Row_id = 2,
                    CR_status_value = validStatus,
                    sort_order = 60,
                    use = meaningExample,
                    WG = true,
                    TSG = true
                }
            );
            legacyContext.Stub(ctx => ctx.CR_status_values).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new Enum_TDocStatusImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newTDocStatus = newDbSet.All()[0];
            Assert.AreEqual(validStatus + " - " + meaningExample, newTDocStatus.Description);
            Assert.AreEqual(validStatus, newTDocStatus.Code);
            Assert.AreEqual(true, newTDocStatus.WGUsable);
            Assert.AreEqual(true, newTDocStatus.TSGUsable);
            Assert.AreEqual(60, newTDocStatus.SortOrder);
        }

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
            var report = new Domain.Report();

            // Execute
            var import = new Enum_CRImpactImport() { LegacyContext = null, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(4, newDbSet.All().Count);

            var newTDocStatus = newDbSet.All()[0];
            Assert.AreEqual("UICS Apps", newTDocStatus.Code);
        }

        /*[TestCaseSource("GetTDocObjects")]
        public void Test_FillDatabase_TDoc(C2006_03_17_tdocs legacyObject, TDoc expectedObject)
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new TDocFakeDbSet();
            newContext.Stub(ctx => ctx.TDocs).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_TDocStatus).Return(GetTDocStatus());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new TdocLegacyFakeDbSet();
            legacyDbSet.Add(legacyObject);
            legacyContext.Stub(ctx => ctx.C2006_03_17_tdocs).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new TDocImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newTDoc = newDbSet.All()[0];
            Assert.AreEqual(expectedObject.UID, newTDoc.UID);
            Assert.AreEqual(expectedObject.Title, newTDoc.Title);
            Assert.AreEqual(expectedObject.Source, newTDoc.Source);
            Assert.AreEqual(expectedObject.Fk_TDocStatus, newTDoc.Fk_TDocStatus);
        }*/
        
        [TestCaseSource("GetCRObjects")]
        public void test_FillDatabase_CR(List_of_GSM___3G_CRs legacyObject, ChangeRequest expectedObject)
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new ChangeRequestFakeDbSet();
            var newWIDbSet = new CRWIFakeDbSet();
            var newRemarksDbSet = new RemarkFakeDbSet();
            newContext.Stub(ctx => ctx.ChangeRequests).Return(newDbSet);
            newContext.Stub(ctx => ctx.Remarks).Return(newRemarksDbSet);
            newContext.Stub(ctx => ctx.CR_WorkItems).Return(newWIDbSet);
            newContext.Stub(ctx => ctx.Enum_CRCategory).Return(GetCRCategory());
            newContext.Stub(ctx => ctx.Enum_TDocStatus).Return(GetTDocStatus());
            newContext.Stub(ctx => ctx.Specifications).Return(GetSpecs());
            newContext.Stub(ctx => ctx.Releases).Return(GetReleases());
            newContext.Stub(ctx => ctx.Specification_Release).Return(GetSpecRelease());
            newContext.Stub(ctx => ctx.SpecVersions).Return(GetVersions());
            newContext.Stub(ctx => ctx.Meetings).Return(GetMeetings());
            newContext.Stub(ctx => ctx.WorkItems).Return(GetWIs());
            newContext.Stub(ctx => ctx.Communities).Return(GetCommunities());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new ListOfGSM3GCRsFakeDbSet();
            legacyDbSet.Add(legacyObject);
            legacyContext.Stub(ctx => ctx.List_of_GSM___3G_CRs).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new CRImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newCR = newDbSet.All()[0];
            Assert.AreEqual(expectedObject.CRNumber, newCR.CRNumber);
            Assert.AreEqual(expectedObject.Revision, newCR.Revision);
            Assert.AreEqual(expectedObject.CreationDate, newCR.CreationDate);
            Assert.AreEqual(expectedObject.Subject, newCR.Subject);
            Assert.AreEqual(expectedObject.Fk_Enum_CRCategory, newCR.Fk_Enum_CRCategory);
            Assert.AreEqual(expectedObject.Fk_WGStatus, newCR.Fk_WGStatus);
            Assert.AreEqual(expectedObject.Fk_TSGStatus,newCR.Fk_TSGStatus);
            Assert.AreEqual(expectedObject.Fk_Release, newCR.Fk_Release);
            Assert.AreEqual(expectedObject.Fk_Specification, newCR.Fk_Specification);
            Assert.AreEqual(expectedObject.TSGSourceOrganizations, newCR.TSGSourceOrganizations);
            Assert.AreEqual(expectedObject.WGSourceOrganizations, newCR.WGSourceOrganizations);
            Assert.AreEqual(expectedObject.Fk_CurrentVersion, newCR.Fk_CurrentVersion);
            Assert.AreEqual(expectedObject.Fk_NewVersion, newCR.Fk_NewVersion);
            Assert.AreEqual(expectedObject.TSGMeeting, newCR.TSGMeeting);
            Assert.AreEqual(expectedObject.WGMeeting, newCR.WGMeeting);
            Assert.AreEqual(expectedObject.TSGTDoc, newCR.TSGTDoc);
            Assert.AreEqual(expectedObject.WGTDoc, newCR.WGTDoc);
            Assert.AreEqual(expectedObject.WGTarget, newCR.WGTarget);
            Assert.AreEqual(expectedObject.TSGTarget, newCR.TSGTarget);
            Assert.AreEqual(expectedObject.WGSourceForTSG, newCR.WGSourceForTSG);
            Assert.AreEqual(1, newCR.Remarks.Count);
            Assert.AreEqual(2, newCR.CR_WorkItems.Count);
        }

        /// <summary>
        /// Send to the test method the legacy CR object and the new expected CR object
        /// </summary>
        public IEnumerable<TestCaseData> GetCRObjects
        {
            get
            {
                yield return new TestCaseData(
                    new List_of_GSM___3G_CRs()
                    {
                        Row_id = 1,
                        CR = "0013",
                        created = new DateTime(2010, 1, 18),
                        Rev = "-",
                        Subject = "test",
                        Cat = "-",
                        Status_1st_Level = "agreed",
                        Status_2nd_Level = "approved",
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
                    new ChangeRequest()
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

                yield return new TestCaseData(
                    new List_of_GSM___3G_CRs()
                    {
                        Row_id = 2,
                        CR = "A018",
                        created = new DateTime(2010, 1, 18),
                        Rev = "2",
                        Subject = "test2",
                        Cat = "B",
                        Status_1st_Level = "posTponed",
                        Status_2nd_Level = "postPoned",
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
                    new ChangeRequest()
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
        /// <summary>
        /// Send to the test method the legacy TDoc object and the new expected TDoc object
        /// </summary>
        /*public IEnumerable<TestCaseData> GetTDocObjects
        {
            get
            {
                yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 1,
                        doc_tdoc = "C1-131002",
                        doc_title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        doc_source = "Vodafone",
                        doc_remarks = "aGreed"
                    },
                    new TDoc()
                    {
                        UID = "C1-131002",
                        Title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        Source = "Vodafone",
                        Fk_TDocStatus = 1,
                    }
                );

                yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 2,
                        doc_tdoc = "C1-131004",
                        doc_title = "Pseudo-CR on iFire Firewall Detection and Traversal",
                        doc_source = "Vodafone/Acme Packet",
                        doc_remarks = "postponed"
                    },
                    new TDoc()
                    {
                        UID = "C1-131004",
                        Title = "Pseudo-CR on iFire Firewall Detection and Traversal",
                        Source = "Vodafone/Acme Packet",
                        Fk_TDocStatus = 3,
                    }
                );


            }
        }*/


        private IDbSet<Domain.Enum_CRCategory> GetCRCategory()
        {
            var list = new Enum_CRCategoryFakeDbSet();
            list.Add(new Domain.Enum_CRCategory() { Pk_EnumCRCategory = 1, Code = "A", Description = "test1" });
            list.Add(new Domain.Enum_CRCategory() { Pk_EnumCRCategory = 2, Code = "B", Description = "test2" });
            list.Add(new Domain.Enum_CRCategory() { Pk_EnumCRCategory = 3, Code = "C", Description = "test3" });

            return list;
        }

        private IDbSet<Domain.Enum_TDocStatus> GetTDocStatus()
        {
            var list = new Enum_TDocStatusFakeDbSet();
            list.Add(new Domain.Enum_TDocStatus() { Pk_EnumTDocStatus = 1, SortOrder = 1, TSGUsable = true, WGUsable = true, Description = "test", Code = "agreed" });
            list.Add(new Domain.Enum_TDocStatus() { Pk_EnumTDocStatus = 2, SortOrder = 2, TSGUsable = false, WGUsable = true, Description = "test", Code = "approved" });
            list.Add(new Domain.Enum_TDocStatus() { Pk_EnumTDocStatus = 3, SortOrder = 3, TSGUsable = true, WGUsable = false, Description = "test", Code = "postponed" });

            return list;
        }

        private IDbSet<Domain.Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Domain.Specification() { Pk_SpecificationId = 1, Number = "1" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 2, Number = "2" });
            return list;
        }

        private IDbSet<Domain.WorkItem> GetWIs()
        {
            var list = new WorkItemFakeDBSet();
            list.Add(new Domain.WorkItem() { Pk_WorkItemUid = 1, Acronym = "AZE" });
            list.Add(new Domain.WorkItem() { Pk_WorkItemUid = 2, Acronym = "RTY" });
            return list;
        }

        private IDbSet<Domain.Community> GetCommunities()
        {
            var list = new CommunityFakeDBSet();
            list.Add(new Domain.Community() { TbId = 1, ShortName = "C3", ParentTbId = 2 });
            list.Add(new Domain.Community() { TbId = 2, ShortName = "C9", ParentTbId = 3 });
            return list;
        }

        private IDbSet<Domain.Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Domain.Release() { Pk_ReleaseId = 1, Code = "Ph1" });
            list.Add(new Domain.Release() { Pk_ReleaseId = 2, Code = "Ph2" });
            return list;
        }
        private IDbSet<Domain.Specification_Release> GetSpecRelease()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Domain.Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2 });
            list.Add(new Domain.Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 1, Fk_ReleaseId = 5 });
            return list;
        }

        private IDbSet<Domain.SpecVersion> GetVersions()
        {
            var list = new SpecVersionFakeDBSet();
            list.Add(new Domain.SpecVersion() { Pk_VersionId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 2, MajorVersion = 1, TechnicalVersion = 1, EditorialVersion = 0 });
            list.Add(new Domain.SpecVersion() { Pk_VersionId = 2, Fk_SpecificationId = 2, Fk_ReleaseId = 2, MajorVersion = 2, TechnicalVersion = 1, EditorialVersion = 5 });
            return list;
        }
        private IDbSet<Domain.Meeting> GetMeetings()
        {
            var list = new MeetingFakeDBSet();
            list.Add(new Domain.Meeting() { MTG_ID = 1, MtgShortRef = "S3-48" });
            list.Add(new Domain.Meeting() { MTG_ID = 2, MtgShortRef = "S3-49" });
            return list;
        }

    }
}
