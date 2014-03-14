using Etsi.Ultimate.Business;
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
    }
}
