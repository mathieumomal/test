﻿using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Tests.Services
{
    class WorkItemServiceTest : BaseTest
    {
        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsByRelease(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;
            List<int> releaseIds = new List<int>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(3);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(3);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var wiService = new WorkItemService();

            //No Release Ids
            var workItems = wiService.GetWorkItemsByRelease(personID, releaseIds);
            Assert.AreEqual(0, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //One Release Id
            releaseIds.Add(527);
            workItems = wiService.GetWorkItemsByRelease(personID, releaseIds);
            Assert.AreEqual(18, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //Two Release Ids
            releaseIds.Add(526);
            workItems = wiService.GetWorkItemsByRelease(personID, releaseIds);
            Assert.AreEqual(20, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemById(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(3);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(3);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var wiService = new WorkItemService();

            //No Release Ids
            var workItems = wiService.GetWorkItemById(personID, 105);
            Assert.AreEqual("Stage 1 for RAN Sharing Enhancements", workItems.Key.Name);
            Assert.AreEqual(100, workItems.Key.Completion);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            workItems = wiService.GetWorkItemById(personID, 100000002);
            Assert.AreEqual("Rel-12 Stage 1 frozen 03/2013", workItems.Key.Name);
            Assert.AreEqual(2, workItems.Key.WorkplanId);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            workItems = wiService.GetWorkItemById(personID, 630000);
            Assert.AreEqual("BB1: RAN Downlink Traffic Differentiation Congestion Detection and Reporting", workItems.Key.Name);
            Assert.AreEqual("UPCON-DOT", workItems.Key.Acronym);
            Assert.AreEqual("Alla Goldner (agoldner@allot.com)", workItems.Key.RapporteurStr);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void AnalyseWorkPlanForImport_Nominal()
        {
            StubCsvParser("../../TestData/WorkItems/OneLine_Nominal.csv");

            var wiService = new WorkItemService();

            var result = wiService.AnalyseWorkPlanForImport("../../TestData/WorkItems/OneLine_Nominal.csv");
            Assert.IsNotNullOrEmpty(result.Key);
            Assert.IsNotNull(CacheManager.Get("WI_IMPORT_" + result.Key));
        }

        [Test]
        public void AnalyseWorkPlanForImport_NominalWithZip()
        {
            string basePath = "..\\..\\TestData\\WorkItems\\";
            string zipName = "OneLine_Nominal_Zipped.zip";
            string fileName = "OneLine_Nominal_Zipped.csv";

            // Clean csvFile
            if (File.Exists(basePath + fileName))
                File.Delete(basePath + fileName);

            StubCsvParser(basePath + fileName);

            var wiService = new WorkItemService();

            var result = wiService.AnalyseWorkPlanForImport(basePath + zipName);
            Assert.IsNotNullOrEmpty(result.Key);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void AnalyseWorkPlanForImport_ZipDoesNotExist()
        {
            string basePath = "..\\..\\TestData\\WorkItems\\";
            string zipName = "OneLine_Nominal_Zipped_Does_Not_Exist.zip";
            string fileName = "OneLine_Nominal_Zipped.csv";

            StubCsvParser(basePath + fileName);

            var wiService = new WorkItemService();
            var result = wiService.AnalyseWorkPlanForImport(basePath + zipName);
        }

       
        [Test]
        public void AnalyseWorkPlanForImport_ZipDoesNotContainCsv()
        {
            string basePath = "..\\..\\TestData\\WorkItems\\";
            string zipName = "OneLine_Nominal_ZippedNoCsv.zip";
            string fileName = "OneLine_Nominal_Zipped.csv";

            // Clean csvFile
            if (File.Exists(basePath + fileName))
                File.Delete(basePath + fileName);

            StubCsvParser(basePath + fileName);

            var wiService = new WorkItemService();

            var result = wiService.AnalyseWorkPlanForImport(basePath + zipName);
            Assert.IsNullOrEmpty(result.Key);
            Assert.AreEqual(1,result.Value.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.WorkItem_Import_Bad_Zip_File, result.Value.ErrorList.First());
        }

        [Test]
        public void ImportWorkPlan_Nominal()
        {
            // Mock WIRepository
            var wiRepositoryMock = MockRepository.GenerateMock<IWorkItemRepository>();
            wiRepositoryMock.Expect(x => x.InsertOrUpdate(Arg<WorkItem>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance(typeof(IWorkItemRepository), wiRepositoryMock);

            // Place something in the cache
            var wiList = new List<WorkItem>() { new WorkItem() };
            CacheManager.Insert("WI_IMPORT_"+"az12", wiList);

            var wiService = new WorkItemService();
            Assert.IsTrue(wiService.ImportWorkPlan("az12"));
        }

        [Test]
        public void ImportWorkPlan_TokenNotFound()
        {
            CacheManager.Clear("WI_IMPORT_" + "az12");

            var wiService = new WorkItemService();
            Assert.IsFalse(wiService.ImportWorkPlan("az12"));
        }

        /// <summary>
        /// Get the WorkItem Data from csv
        /// </summary>
        public IEnumerable<WorkItemFakeDBSet> WorkItemData
        {
            get
            {
                var workItemList = GetAllTestRecords<WorkItem>(Directory.GetCurrentDirectory() + "\\TestData\\WorkItems\\WorkItem.csv");
                WorkItemFakeDBSet workItemFakeDBSet = new WorkItemFakeDBSet();
                workItemList.ForEach(x => workItemFakeDBSet.Add(x));

                yield return workItemFakeDBSet;
            }
        }

        private void StubCsvParser(string path)
        {
            var wiList = new List<WorkItem>() { new WorkItem() };
            var csvImporterMock = MockRepository.GenerateMock<IWorkItemCsvParser>();
            csvImporterMock.Stub(x => x.ParseCsv(path)).Return(new KeyValuePair<List<WorkItem>, ImportReport>(wiList, new ImportReport()));
            ManagerFactory.Container.RegisterInstance(typeof(IWorkItemCsvParser), csvImporterMock);
        }
    }
}