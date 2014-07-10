using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Tests.FakeRepositories;

namespace Etsi.Ultimate.Tests.Business
{
    class WorkItemCsvParserTest: BaseTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            RegisterRepositories();
        }

        [Test]
        public void ImportCsv_LogsErrorOnNonCsvFile()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/empty.txt");

            var report = result.Value;
            Assert.AreEqual(1, report.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.WorkItem_Import_Invalid_File_Format, report.ErrorList.First());
        }

        [Test]
        public void ImportCsv_EmptyFile()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/empty.csv");

            Assert.IsNotNull(result.Key);
            Assert.AreEqual(0, result.Key.Count);
        }

        [Test]
        public void ImportCsv_ShowsUnexpectedExceptionError()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser();

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/empty.csv");

            var report = result.Value;
            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Unknown_Exception, "None"), report.ErrorList.First());

        }

        [Test]
        public void ImportCsv_NotWellFormedWorkPlanLogsError()
        {

            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser();
            wiImporter.UoW = new UltimateUnitOfWork();

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/BadFormat.csv");

            var report = result.Value;
            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Bad_Format, "An inconsistent number of columns has been detected."), report.ErrorList.First());
        }

        [Test]
        public void ImportCsv_FileDoesNotExist()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/nowhere.csv");
            var report = result.Value;

            Assert.AreEqual(1, report.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_FileNotFound, "../../TestData/WorkItems/nowhere.csv"),
                report.ErrorList.First());
        }

        [Test]
        public void ImportCsv_NominalCase()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/OneLine_Nominal.csv");

            Assert.IsNotNull(result.Key);
            Assert.AreEqual(1, result.Key.Count);
            Assert.AreEqual(0, result.Value.GetNumberOfIssues());

            var wi = result.Key.First();

            Assert.AreEqual(17, wi.WorkplanId);
            Assert.AreEqual(630000, wi.Pk_WorkItemUid);
            Assert.AreEqual("Isolated E-UTRAN Operation for Public Safety", wi.Name);
            Assert.AreEqual("IOPS", wi.Acronym);
            Assert.AreEqual(0, wi.WiLevel);
            Assert.AreEqual(ReleaseFakeRepository.OPENED_RELEASE_ID, wi.Fk_ReleaseId);
            Assert.AreEqual(new DateTime(2014, 3, 7), wi.StartDate);
            Assert.AreEqual(new DateTime(2014, 9, 17), wi.EndDate);
            Assert.AreEqual(0, wi.Completion);
            Assert.AreEqual("SP-140069", wi.Wid);
            Assert.AreEqual("SP-140070", wi.StatusReport);
            Assert.AreEqual("General Dynamics Broadband", wi.RapporteurCompany);
            Assert.AreEqual(27904, wi.RapporteurId);
            Assert.AreEqual("Mathieu Mangion(mathieu.mangion@etsi.org)", wi.RapporteurStr);
            Assert.AreEqual(1, wi.Remarks.Count);
            Assert.AreEqual("Triggered by Rel-13 TR 22.897 Study on Isolated E-UTRAN Operation for Public Safety (FS_IOPS)", wi.Remarks.First().RemarkText);
            Assert.AreEqual("Stage 1", wi.TssAndTrs);

            Assert.AreEqual("SP-63", wi.TsgApprovalMtgRef);
            Assert.AreEqual("PCG-32", wi.PcgApprovalMtgRef);
            Assert.AreEqual(MeetingsFakeRepository.SA63_MTG_ID, wi.TsgApprovalMtgId);
            Assert.AreEqual(MeetingsFakeRepository.PCG32_MTG_ID, wi.PcgApprovalMtgId);

            Assert.AreEqual("SP-64", wi.TsgStoppedMtgRef);
            Assert.AreEqual("PCG-33", wi.PcgStoppedMtgRef);
            Assert.AreEqual(MeetingsFakeRepository.SA64_MTG_ID, wi.TsgStoppedMtgId);
            Assert.AreEqual(MeetingsFakeRepository.PCG33_MTG_ID, wi.PcgStoppedMtgId);

            Assert.AreEqual(new DateTime(2014, 2, 5), wi.CreationDate);
            Assert.AreEqual(new DateTime(2014, 2, 6), wi.LastModification);

            Assert.AreEqual(2, wi.WorkItems_ResponsibleGroups.Count);
            Assert.AreEqual("S1", wi.WorkItems_ResponsibleGroups.ElementAt(0).ResponsibleGroup);
            Assert.AreEqual(2, wi.WorkItems_ResponsibleGroups.ElementAt(0).Fk_TbId);
            Assert.IsTrue(wi.WorkItems_ResponsibleGroups.ElementAt(0).IsPrimeResponsible.GetValueOrDefault());

            Assert.IsTrue(wi.IsNew);
        }

        [Test]
        public void ImportCsv_TestWrongWpOrderShouldLogError()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/Wrong_WpOrder.csv");

            var report = result.Value;
            Assert.AreEqual(1, report.ErrorList.Count);

            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Wrong_Order, "16", "17"), report.ErrorList.First());
        }

        [Test]
        public void ImportCsv_Level0WorkItemUidGeneration()
        {
            RegisterRepositories();
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemOneLineLevel0FakeRepository>(new TransientLifetimeManager());

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/Level0_WorkItems.csv");

            var wiList = result.Key;
            var report = result.Value;
            Assert.AreEqual(2, wiList.Count);
            Assert.AreEqual(100000005, wiList.First().Pk_WorkItemUid);
            Assert.AreEqual(100000006, wiList.ElementAt(1).Pk_WorkItemUid);
        }

        [Test]
        public void ImportCsv_Level0Wi()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/Nominal_WithLevel0Wi.csv");

            // Having Uid 0 must not be a problem.
            Assert.AreEqual(0, result.Value.GetNumberOfIssues());
            Assert.AreEqual(1, result.Key.Count);

            // However, it means system should compute a UI. 
            var aWi = result.Key.First();
            Assert.AreEqual(100000001, aWi.Pk_WorkItemUid);

        }

        [Test]
        public void ImportCsv_ManagesHierarchy()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/HierarchyMgt.csv");

            var wiList = result.Key;

            Assert.AreEqual(6, wiList.Count);

            var id6 = wiList.Where(w => w.WorkplanId == 6).FirstOrDefault();
            Assert.IsNull(id6.Fk_ParentWiId);

            var id17 = wiList.Where(w => w.WorkplanId == 17).FirstOrDefault();
            Assert.AreEqual(100000001, id17.Fk_ParentWiId);

            var id20 = wiList.Where(w => w.WorkplanId == 20).FirstOrDefault();
            Assert.AreEqual(630002, id20.Fk_ParentWiId);

            var id21 = wiList.Where(w => w.WorkplanId == 21).FirstOrDefault();
            Assert.AreEqual(100000001, id17.Fk_ParentWiId);
        }

        [Test]
        public void ImportCsv_BadHierarchy()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/BadHierarchy.csv");

            var report = result.Value;
            Assert.AreEqual(1, report.ErrorList.Count);
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Hierarchy, 24, 630000, 2, 0), report.ErrorList.First());
        }


        [Test]
        public void ImportCsv_WorkItemNotModified()
        {
            RegisterRepositories();
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemOneLineFakeRepository>(new TransientLifetimeManager());

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/OneLine_Nominal.csv");

            Assert.AreEqual(0, result.Key.Count);
        }

        [Test]
        public void ImportCsv_WpIdFieldShouldBeUnique()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/WpId_NonUnique.csv");

            Assert.AreEqual(1, result.Value.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_DuplicateWorkplanId, "17"), result.Value.WarningList.First());
        }

       

        [Test]
        public void ImportCsv_WpUidMustBeUnique()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/UID_NonUnique.csv");

            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_DuplicateUID, "630000", "17", "18"), result.Value.ErrorList.First());
        }

        [Test]
        public void ImportCsv_DuplicateAcronymMustLogWarningInHierarchy()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            // A warning happens if one child and his parent have same acronym.
            // System should exclude from this the "-" field.
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/DuplicateAcronymWarning.csv");

            Assert.AreEqual(1, result.Value.GetNumberOfWarnings());
            Assert.AreEqual(0, result.Value.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSubLevel, "25", "630001", "IOPS"), result.Value.WarningList.First());
        }

        [Test]
        public void ImportCsv_DuplicateAcronymMustLogErrorOutOfHierarchy()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/DuplicateAcronymError.csv");

            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_DuplicateAcronymSameLevel, "26", "630002", "IOPS2", "630001"), result.Value.ErrorList.First());
        }

        [Test]
        public void ImportCsv_IncorrectRelease()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/IncorrectReleases.csv");

            var report = result.Value;
            Assert.AreEqual(1, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Release, "24", "630000", "Rel-21231"), report.WarningList.First());

            var wiList = result.Key;
            Assert.AreEqual(2, wiList.Count);
            Assert.IsNull(wiList.ElementAt(0).Fk_ReleaseId);
            Assert.IsNull(wiList.ElementAt(1).Fk_ReleaseId);
            
        }

        [Test]
        public void ImportCsv_InvalidDateLogsWarning()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidDates.csv");

            var report = result.Value;
            Assert.AreEqual(4, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Incorrect_StartDate, "24", "630000", "0/03/2014"), report.WarningList.First());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Incorrect_EndDate, "25", "630001", "29/2/2014"), report.WarningList[1]);
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Incorrect_CreationDate, "25", "630001", "test"), report.WarningList[2]);
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Incorrect_LastModifiedDate, "25", "630001", "30/02/2014"), report.WarningList[3]);

        }


        [Test]
        public void ImportCsv_InvalidCompletionLogsWarning()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidCompletion.csv");

            var wiList = result.Key;
            var report = result.Value;
            Assert.AreEqual(4, wiList.Count);
            Assert.IsNull(wiList.First().Completion);


            Assert.AreEqual(3, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Incorrect_Completion, "25", "630001", "0%2"), report.WarningList.First());

        }

        [Test]
        public void ImportCsv_InvalidHyperLinkAndStatusReport()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidWiDAndStatus.csv");

            var wiList = result.Key;
            var report = result.Value;

            Assert.AreEqual(2, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_WiD, "25", "630001", "Not applicable"), report.WarningList.First());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_StatusReport, "25", "630001", "RP-070037, RP-070531"), report.WarningList[1]);
            
        }

       
        [Test]
        public void ImportCsv_CannotFindRapporteur()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/UnknownRapporteur.csv");

            var wiList = result.Key;
            var report = result.Value;

            Assert.AreEqual(3, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Rapporteur, "17", "630000", "mathieu.mang@etsi.org"), report.WarningList.First());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Rapporteur, "18", "630001", "sdsq"), report.WarningList[1]);
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Rapporteur, "20", "107", "xyz"), report.WarningList[2]);

        }

        [Test]
        public void ImportCsv_AddNewRemark()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemOneLineFakeRepository>(new TransientLifetimeManager());
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/NewRemarks.csv");

            var wiList = result.Key;
            var report = result.Value;

            Assert.AreEqual(1, wiList.Count);

            var wi = wiList.First();
            Assert.AreEqual(2, wi.Remarks.Count);
            Assert.AreEqual("New remark", wi.Remarks.ElementAt(1).RemarkText);

        }

        [Test]
        public void ImportCsv_InvalidTsgPcgDates()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemOneLineFakeRepository>(new TransientLifetimeManager());
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidTsgPcgDates.csv");

            var wiList = result.Key;
            var report = result.Value;

            Assert.AreEqual(1, wiList.Count);
            Assert.AreEqual(4, report.GetNumberOfWarnings());

            int wpId = 17;
            int uid = 630000;

            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_TsgApprovedMtg, wpId, uid, "SP-1"), 
                report.WarningList.First());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_PcgApprovedMtg, wpId, uid, "PCG-1"),
                report.WarningList.ElementAt(1));
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_TsgStoppedMtg, wpId, uid, "brr"),
                report.WarningList.ElementAt(2));
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_PcgStoppedMtg, wpId, uid, "ATC-12"),
                report.WarningList.ElementAt(3));


        }

        [Test]
        public void ImportCsv_InvalidCommittees()
        {
            RegisterRepositories();
            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemOneLineFakeRepository>(new TransientLifetimeManager());
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidCommittees.csv");

            var wiList = result.Key;
            var report = result.Value;

            Assert.AreEqual(2, wiList.Count);
            Assert.AreEqual(2, report.GetNumberOfWarnings());


            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Resource, "18", "630001", "A3"),
                report.WarningList.First());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Resource, "18", "630001", "B5"),
                report.WarningList.ElementAt(1));

        }

        [Test]
        public void ImportCsv_NoLevelMeansLevel0()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/InvalidLevel.csv");

            var wiList = result.Key;
            var report = result.Value;
            Assert.AreEqual(2, wiList.Count);
            Assert.AreEqual(0, wiList.First().WiLevel);
            Assert.AreEqual(1, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Invalid_Level, "18", "630001", "A"), report.WarningList.First());
        }

        [Test]
        public void ImportCsv_TrimsAcronyms()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/AcronymTooLong.csv");

            var wiList = result.Key;
            var report = result.Value;
            Assert.AreEqual(1, wiList.Count);
            Assert.AreEqual(50, wiList.First().Acronym.Length);
            Assert.AreEqual(1, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Acronym_Too_Long, "17", "630000", "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJ"), report.WarningList.First());
        }

        [Test]
        public void ImportCsv_TrimsTsAndTrs()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };

            var result = wiImporter.ParseCsv("../../TestData/WorkItems/TSsAndTRsTooLong.csv");

            var wiList = result.Key;
            var report = result.Value;
            Assert.AreEqual(1, wiList.Count);
            Assert.AreEqual(50, wiList.First().TssAndTrs.Length);
            Assert.AreEqual(1, report.GetNumberOfWarnings());
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_TsTr_Too_Long, "17", "630000"), report.WarningList.First());
        }

        [Test]
        public void ImportCsv_DifferentReleaseCase()
        {
            RegisterRepositories();

            var wiImporter = new WorkItemCsvParser() { UoW = new UltimateUnitOfWork() };
            
            var result = wiImporter.ParseCsv("../../TestData/WorkItems/ReleaseDifferentFromParent.csv").Value;
            Assert.AreEqual(String.Format(Utils.Localization.WorkItem_Import_Parent_Release_Different_With_Child, "10", "580099", "Rel-2", "Rel-1"), result.WarningList.First());
        }

        private void RegisterRepositories()
        {
            RepositoryFactory.Container.RegisterType<IWorkItemRepository, WorkItemEmptyFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IReleaseRepository, ReleaseFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IPersonRepository, PersonFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<IMeetingRepository, MeetingsFakeRepository>(new TransientLifetimeManager());
            RepositoryFactory.Container.RegisterType<ICommunityRepository, CommunityFakeRepository>(new TransientLifetimeManager());
        }

    }
}
