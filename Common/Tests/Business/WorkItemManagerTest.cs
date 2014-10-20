using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;

namespace Etsi.Ultimate.Tests.Business
{
    class WorkItemManagerTest : BaseTest
    {
        int personID = 12;
        int releaseId = 1;

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            List<int> releaseIds = new List<int>();
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(6);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(6);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();


            var wiManager = new WorkItemManager(uow);

            //No Release Ids
            var workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, true, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(0, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            //Show All Records
            workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(18, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Hide 100% records at level 1
            workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, true, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(17, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(20, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Name search
            workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, String.Empty, "Stage", new List<int>());
            Assert.AreEqual(12, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));
            //Acronym search
            workItems = wiManager.GetWorkItemsBySearchCriteria(personID, releaseIds, 5, false, "UPCON", String.Empty, new List<int>());
            Assert.AreEqual(3, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemById(WorkItemFakeDBSet workItemData)
        {

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(3);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(3);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            //No Release Ids
            var wiManager = new WorkItemManager(uow);

            var workItems = wiManager.GetWorkItemById(personID, 105);
            Assert.AreEqual("Stage 1 for RAN Sharing Enhancements", workItems.Key.Name);
            Assert.AreEqual(100, workItems.Key.Completion);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            workItems = wiManager.GetWorkItemById(personID, 100000002);
            Assert.AreEqual("Rel-12 Stage 1 frozen 03/2013", workItems.Key.Name);
            Assert.AreEqual(2, workItems.Key.WorkplanId);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            workItems = wiManager.GetWorkItemById(personID, 630000);
            Assert.AreEqual("BB1: RAN Downlink Traffic Differentiation Congestion Detection and Reporting", workItems.Key.Name);
            Assert.AreEqual("UPCON-DOT", workItems.Key.Acronym);
            Assert.AreEqual("Alla Goldner (agoldner@allot.com)", workItems.Key.RapporteurStr);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemIds(WorkItemFakeDBSet workItemData)
        {
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(1);


            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Times(2);


            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            //No Release Ids
            var wiManager = new WorkItemManager(uow);
            var workItemIds = new List<int>()
            {
                110,
                111
            };
            var response = wiManager.GetWorkItemByIds(personID, workItemIds);
            Assert.AreEqual(2, response.Key.Count);
            Assert.AreEqual("Stage 1 for Mission Critical Push To Talk over LTE", response.Key[0].Name);
            Assert.AreEqual("eWebRTCi", response.Key[1].Acronym);
             mockDataContext.VerifyAllExpectations();
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsCountBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            List<int> releaseIds = new List<int>();

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(6);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiManager = new WorkItemManager(uow);

            //No Release Ids
            Assert.AreEqual(0, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            //Show All Records
            Assert.AreEqual(18, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            //Hide 100% records at level 1
            Assert.AreEqual(17, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));

            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            Assert.AreEqual(20, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            //Name search
            Assert.AreEqual(12, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, "Stage", new List<int>()));
            //Acronym search
            Assert.AreEqual(3, wiManager.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, "UPCON", String.Empty, new List<int>()));

            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void GetWorkItemsBySearchCriteria_TakesTBsIntoAccount()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)WorkItemDataWithTbs);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(mockRightsManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiManager = new WorkItemManager(uow);

            var releases = new List<int>() { releaseId };
            var tbs = new List<int>() { 1 };
            var result = wiManager.GetWorkItemsBySearchCriteria(personID, releases, 1, false, String.Empty, String.Empty, tbs).Key;
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetWorkItemsBySearchCriteria_CleansUpTopHierarchy()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)WorkItemDataWithTbs);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(mockRightsManager);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiManager = new WorkItemManager(uow);

            var releases = new List<int>() { releaseId };
            var tbs = new List<int>() { 2 };
            var result = wiManager.GetWorkItemsBySearchCriteria(personID, releases, 2, false, String.Empty, String.Empty, tbs).Key;
            Assert.AreEqual(2, result.Count);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetAllAcronyms(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiManager = new WorkItemManager(uow);

            Assert.AreEqual(6, wiManager.GetAllAcronyms().Count);
            Assert.Contains("UPCON", wiManager.GetAllAcronyms());
            Assert.Contains("SEES", wiManager.GetAllAcronyms());
            Assert.Contains("MCPTT", wiManager.GetAllAcronyms());
            Assert.Contains("eWebRTCi", wiManager.GetAllAcronyms());
            Assert.Contains("IOPS", wiManager.GetAllAcronyms());
            Assert.Contains("UPCON-DOT", wiManager.GetAllAcronyms());
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetAllWorkItems(WorkItemFakeDBSet workItemData)
        {
            int personID = 12;

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.WorkItem_ImportWorkplan);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Once();

            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personID)).Return(userRights).Repeat.Once();

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            var wiManager = new WorkItemManager(uow);

            var workItems = wiManager.GetAllWorkItems(personID);
            Assert.AreEqual(26, workItems.Key.Count);
            Assert.IsTrue(workItems.Value.HasRight(Enum_UserRights.WorkItem_ImportWorkplan));

            mockDataContext.VerifyAllExpectations();
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

        public WorkItemFakeDBSet WorkItemDataWithTbs
        {
            get
            {
                var sa1Responsible = new List<WorkItems_ResponsibleGroups>(){
                    new WorkItems_ResponsibleGroups() { Fk_TbId = 1, IsPrimeResponsible = true }
                };
                var sa2Responsible = new List<WorkItems_ResponsibleGroups>(){
                    new WorkItems_ResponsibleGroups() { Fk_TbId = 2, IsPrimeResponsible = true },
                };
                var sa3Responsible = new List<WorkItems_ResponsibleGroups>(){
                    new WorkItems_ResponsibleGroups() { Fk_TbId = 3, IsPrimeResponsible = true },
                };

                var wi1 = new WorkItem() { Pk_WorkItemUid = 1, Acronym = "Test", Name = "Test", Fk_ReleaseId = 1, WiLevel = 1, WorkItems_ResponsibleGroups = sa1Responsible };
                var wi11 = new WorkItem() { Pk_WorkItemUid = 2, Acronym = "Test1.1", Name = "Test1.1", Fk_ReleaseId = 1, WiLevel = 2, Fk_ParentWiId = 1, WorkItems_ResponsibleGroups = sa1Responsible };
                var wi12 = new WorkItem() { Pk_WorkItemUid = 3, Acronym = "Test1.2", Name = "Test1.2", Fk_ReleaseId = 1, WiLevel = 2, Fk_ParentWiId = 1, WorkItems_ResponsibleGroups = sa2Responsible };
                wi1.ChildWis.Add(wi11);
                wi1.ChildWis.Add(wi12);

                var wi2 = new WorkItem() { Pk_WorkItemUid = 4, Acronym = "Test2", Name = "Test2", Fk_ReleaseId = 1, WiLevel = 1, WorkItems_ResponsibleGroups = sa3Responsible };
                var wi21 = new WorkItem() { Pk_WorkItemUid = 5, Acronym = "Test2.1", Name = "Test2.1", Fk_ReleaseId = 1, WiLevel = 2, Fk_ParentWiId = 4, WorkItems_ResponsibleGroups = sa1Responsible };
                var wi22 = new WorkItem() { Pk_WorkItemUid = 6, Acronym = "Test2.2", Name = "Test2.2", Fk_ReleaseId = 1, WiLevel = 2, Fk_ParentWiId = 4, WorkItems_ResponsibleGroups = sa3Responsible };
                wi2.ChildWis.Add(wi21);
                wi2.ChildWis.Add(wi22);

                var wis = new WorkItemFakeDBSet();
                wis.Add(wi1);
                wis.Add(wi11);
                wis.Add(wi12);
                wis.Add(wi2);
                wis.Add(wi21);
                wis.Add(wi22);


                return wis;
            }
        }
    }
}

