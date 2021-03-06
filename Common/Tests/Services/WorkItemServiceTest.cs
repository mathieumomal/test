﻿using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.Utils.Core;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System;
using System.Configuration;

namespace Etsi.Ultimate.Tests.Services
{
    class WorkItemServiceTest : BaseEffortTest
    {
        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;
            List<int> releaseIds = new List<int>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(10);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(10);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var wiService = new WorkItemService();

            //No Release Ids
            var workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, true, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(0, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            //Show All Records
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(18, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Hide 100% records at level 1
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, true, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(17, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(20, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Name search
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, "Stage", new List<int>());
            Assert.AreEqual(12, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Acronym search
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, "UPCON", String.Empty, new List<int>());
            Assert.AreEqual(0, workItems.Key.Count);//Because searching now by EffectiveAcronym
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //-----------------------
            //Test overloaded method (search string)
            //-----------------------
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, "UPCON");
            Assert.AreEqual(3, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Exclude level= 0 records
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, "Rel-12 Stage 1");
            Assert.AreEqual(0, workItems.Key.Count);
            //Search by WorkItem id
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, "113");
            Assert.AreEqual(1, workItems.Key.Count);
            //Search WIs with Acronym (WIS come from "\\TestData\\WorkItems\\WorkItem.csv". Here should return only WI : 100 and not the 100000005 because second one don't have acronym)
            workItems = wiService.GetWorkItemsBySearchCriteria(personID, "ABCD", true);
            Assert.AreEqual(1, workItems.Key.Count);

            mockDataContext.VerifyAllExpectations();
        }

        [Test(Description = "System should find only one WI with effective acronym EMC1 for release 2882")]
        public void GetWorkItemsBySearchCriteria_SearchByEffectiveAcronym()
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(12)).Return(userRights).Repeat.Times(10);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var wiService = new WorkItemService();
            var result = wiService.GetWorkItemsBySearchCriteria(12, new List<int> { 2882 }, 1, false, "EMC1", string.Empty,
                new List<int>());

            Assert.AreEqual(1, result.Key.Count);
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

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemByIdExtend(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(1);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(1);

            var mockPersonsRepo = MockRepository.GenerateMock<IPersonRepository>();
            mockPersonsRepo.Stub(x => x.Find(42718)).Return(new View_Persons() { FIRSTNAME = "First", LASTNAME = "Last" }).Repeat.Times(1);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            RepositoryFactory.Container.RegisterInstance(typeof(IPersonRepository), mockPersonsRepo);

            var wiService = new WorkItemService();

            //No Release Ids
            var workItems = wiService.GetWorkItemByIdExtend(personID, 102);            
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            Assert.AreEqual("First Last", workItems.Key.RapporteurName);

            mockDataContext.VerifyAllExpectations();
        }
        
        [Test, TestCaseSource("WorkItemData")]
        public void GetAllWorkItems(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(1);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(1);            

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var wiService = new WorkItemService();

            //No Release Ids
            var workItems = wiService.GetAllWorkItems(personID).Key;
            Assert.AreEqual(26, workItems.Count);

            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsCountBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            List<int> releaseIds = new List<int>();

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(6);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var wiService = new WorkItemService();

            //No Release Ids
            Assert.AreEqual(0, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            //Show All Records
            Assert.AreEqual(18, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            //Hide 100% records at level 1
            Assert.AreEqual(17, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));

            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            Assert.AreEqual(20, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            //Name search
            Assert.AreEqual(12, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, "Stage", new List<int>()));
            //Acronym search
            Assert.AreEqual(0, wiService.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, "UPCON", String.Empty, new List<int>()));//Because searching now by EffectiveAcronym

            mockDataContext.VerifyAllExpectations();
        }

        [Test(Description = "System should find only one WI with effective acronym EMC1 for release 2882")]
        public void GetWorkItemsCountBySearchCriteria_SearchByEffectiveAcronym()
        {
            var wiService = new WorkItemService();
            var result = wiService.GetWorkItemsCountBySearchCriteria(new List<int> { 2882 }, 1, false, "EMC1", string.Empty,
                new List<int>());

            Assert.AreEqual(1, result);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsByKeywords(WorkItemFakeDBSet workItemData)
        {
            List<int> releaseIds = new List<int>();

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var wiService = new WorkItemService();
            var wiKeyWords = new List<string>{"101", "UPCON"};
            var workItems = wiService.GetWorkItemsByKeywords(0, wiKeyWords);
            Assert.AreEqual(3, workItems.Count);
            Assert.AreEqual(100, workItems[0].Pk_WorkItemUid);
            Assert.AreEqual("UPCON", workItems[0].Acronym);
            Assert.AreEqual(570029, workItems[1].Pk_WorkItemUid);
            Assert.AreEqual("UPCON", workItems[1].Acronym);
            Assert.AreEqual(101, workItems[2].Pk_WorkItemUid);            
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetAllAcronyms(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var wiService = new WorkItemService();

            Assert.AreEqual(6, wiService.GetAllAcronyms().Count);
            Assert.Contains("UPCON", wiService.GetAllAcronyms());
            Assert.Contains("SEES", wiService.GetAllAcronyms());
            Assert.Contains("MCPTT", wiService.GetAllAcronyms());
            Assert.Contains("eWebRTCi", wiService.GetAllAcronyms());
            Assert.Contains("IOPS", wiService.GetAllAcronyms());
            Assert.Contains("UPCON-DOT", wiService.GetAllAcronyms());
        }

        [TestCase("IMS-CCR", 1)]
        [TestCase("IMS", 1)]
        [TestCase("CCR", 0)]
        [TestCase("ETRAN", 1)]
        public void LookForAcronyms(string keyword, int expectedCount)
        {
            var wiService = new WorkItemService();
            var result = wiService.LookForAcronyms(keyword);

            Assert.AreEqual(expectedCount, result.Count);
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
        public void AnalyseWorkPlanForImport_ZipDoesNotExist()
        {
            string basePath = "..\\..\\TestData\\WorkItems\\";
            string zipName = "OneLine_Nominal_Zipped_Does_Not_Exist.zip";
            string fileName = "OneLine_Nominal_Zipped.csv";

            StubCsvParser(basePath + fileName);

            var wiService = new WorkItemService();
            var result = wiService.AnalyseWorkPlanForImport(basePath + zipName);
            Assert.AreEqual(1, result.Value.GetNumberOfErrors());
            Assert.AreEqual(Utils.Localization.WorkItem_Import_Error_Analysis, result.Value.ErrorList.First());
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
        public void ImportWorkPlan_Nominal_WithoutExport()
        {
            var wiList = new List<WorkItem> { new WorkItem() };
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            // Mock WIRepository
            var wiRepositoryMock = MockRepository.GenerateMock<IWorkItemRepository>();
            wiRepositoryMock.Expect(x => x.InsertOrUpdate(Arg<WorkItem>.Is.Anything));
            wiRepositoryMock.Stub(x => x.AllIncluding()).IgnoreArguments().Return(wiList.AsQueryable());
            RepositoryFactory.Container.RegisterInstance(typeof(IWorkItemRepository), wiRepositoryMock);

            // Mock Workplan exporter
            var wpExporterMock = MockRepository.GenerateMock<IWorkPlanExporter>();
            wpExporterMock.Stub(x => x.ExportWorkPlan(Arg<string>.Is.Anything)).Repeat.Never();
            ManagerFactory.Container.RegisterInstance(typeof(IWorkPlanExporter), wpExporterMock);

            // Place something in the cache
            CacheManager.Insert("WI_IMPORT_"+"az12", wiList);

            var wiService = new WorkItemService();
            Assert.IsTrue(wiService.ImportWorkPlan("az12", string.Empty));
            wpExporterMock.VerifyAllExpectations();
        }

        [Test]
        public void ImportWorkPlan_Nominal_WithExport()
        {
            //Activate export inside web.config
            ConfigurationManager.AppSettings["ActivateWorkPlanExportAfterImport"] = "true";

            var wiList = new List<WorkItem> { new WorkItem() };
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            // Mock WIRepository
            var wiRepositoryMock = MockRepository.GenerateMock<IWorkItemRepository>();
            wiRepositoryMock.Expect(x => x.InsertOrUpdate(Arg<WorkItem>.Is.Anything));
            wiRepositoryMock.Stub(x => x.AllIncluding()).IgnoreArguments().Return(wiList.AsQueryable());
            RepositoryFactory.Container.RegisterInstance(typeof(IWorkItemRepository), wiRepositoryMock);

            // Mock Workplan exporter
            var wpExporterMock = MockRepository.GenerateMock<IWorkPlanExporter>();
            wpExporterMock.Stub(x => x.ExportWorkPlan(Arg<string>.Is.Anything)).Repeat.Once().Return(true);
            ManagerFactory.Container.RegisterInstance(typeof(IWorkPlanExporter), wpExporterMock);

            // Place something in the cache
            CacheManager.Insert("WI_IMPORT_" + "az12", wiList);

            var wiService = new WorkItemService();
            Assert.IsTrue(wiService.ImportWorkPlan("az12", string.Empty));
            wpExporterMock.VerifyAllExpectations();

            //RESET Desactivate export inside web.config
            ConfigurationManager.AppSettings["ActivateWorkPlanExportAfterImport"] = "false";
        }

        [Test]
        public void ImportWorkPlan_TokenNotFound()
        {
            CacheManager.Clear("WI_IMPORT_" + "az12");

            var wiService = new WorkItemService();
            Assert.IsFalse(wiService.ImportWorkPlan("az12", string.Empty));
        }

        [Test]
        public void GetReleaseRelatedToOneOfWiWithTheLowerWiLevel()
        {
            var mockWiRepo = MockRepository.GenerateMock<IWorkItemRepository>();
            mockWiRepo.Stub(x => x.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(Arg<List<int>>.Is.Anything))
                .Return(new Release{Pk_ReleaseId = 1}).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(mockWiRepo);

            var wiService = new WorkItemService();
            var result = wiService.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(0, new List<int> {1, 2});

            Assert.AreEqual(1, result.Pk_ReleaseId);
            mockWiRepo.VerifyAllExpectations();
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
            csvImporterMock.Stub(x => x.ParseCsv(path)).Return(new KeyValuePair<List<WorkItem>, Report>(wiList, new Report()));
            ManagerFactory.Container.RegisterInstance(typeof(IWorkItemCsvParser), csvImporterMock);
        }





        [Test]
        public void GetPrimeWorkItemBySpecificationID_ServiceAndBusiness()
        {
            WorkItem workItemBySpecID = new WorkItem();
            var repo_mock = MockRepository.GenerateMock<IWorkItemRepository>();
            repo_mock.Stub(x => x.GetPrimeWorkItemBySpecificationID(Arg<int>.Is.Anything)).Return(workItemBySpecID);
            RepositoryFactory.Container.RegisterInstance(repo_mock);

            var mgr = ServicesFactory.Resolve<IWorkItemService>();
            WorkItem result = mgr.GetPrimeWorkItemBySpecificationID(160000);
 
            result = mgr.GetPrimeWorkItemBySpecificationID(160000);
            Assert.AreEqual(workItemBySpecID.Pk_WorkItemUid, result.Pk_WorkItemUid);

            result = mgr.GetPrimeWorkItemBySpecificationID(888888);
            Assert.AreEqual(0, result.Pk_WorkItemUid);
        }
    }
}
