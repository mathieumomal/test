﻿using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;


namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class WorkItemRepositoryTest : BaseEffortTest
    {
        [Test]
        public void WorkItem_GetAll()
        {
            var repo = new WorkItemRepository() { UoW = GetUnitOfWork() };
            var results = repo.All.ToList();

            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void WorkItem_GetAllIncluding()
        {
            var repo = new WorkItemRepository() { UoW = GetUnitOfWork() };
            var results = repo.AllIncluding(w => w.Remarks).ToList();

            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void WorkItem_Find()
        {
            var repo = new WorkItemRepository() { UoW = GetUnitOfWork() };
            Assert.AreEqual("Name", repo.Find(1).Name);
        }

        [Test]
        public void WorkItem_InsertOrUpdate()
        {
            var repo = new WorkItemRepository() { UoW = GetUnitOfWork() };
            var newWi = new WorkItem();
            newWi.IsNew = true;
            newWi.Name = "release number 4";
            repo.InsertOrUpdate(newWi);
            Assert.AreEqual(2, repo.All.ToList().Count);
        }

        [Test]
        public void WorkItem_Delete()
        {
            var repo = new WorkItemRepository() { UoW = GetUnitOfWork() };
            repo.Delete(1);
            Assert.AreEqual(0, repo.All.ToList().Count);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItems(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(2);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
         
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiRepository = new WorkItemRepository() { UoW = uow };
            var response = wiRepository.GetWorkItemsByIds(new List<int>() { 100, 101 });

            Assert.AreEqual(2, response.Count);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(5);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();

            List<int> releaseIds = new List<int>();

            //No Release Ids
            var repo = new WorkItemRepository() { UoW = uow };
            var workItems = repo.GetWorkItemsBySearchCriteria(releaseIds, 5, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(0, workItems.Count);

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            //Show All Records
            workItems = repo.GetWorkItemsBySearchCriteria(releaseIds, 5, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(18, workItems.Count);

            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            workItems = repo.GetWorkItemsBySearchCriteria(releaseIds, 5, String.Empty, String.Empty, new List<int>());
            Assert.AreEqual(20, workItems.Count);
            //Name search
            workItems = repo.GetWorkItemsBySearchCriteria(releaseIds, 5, String.Empty, "Stage", new List<int>());
            Assert.AreEqual(12, workItems.Count);
            //Acronym search
            workItems = repo.GetWorkItemsBySearchCriteria(releaseIds, 5, "UPCON", String.Empty, new List<int>());
            Assert.AreEqual(0, workItems.Count);//Because searching now by EffectiveAcronym

            mockDataContext.VerifyAllExpectations();
        }

        [Test(Description = "System should find only one WI with effective acronym EMC1 for release 2882")]
        public void GetWorkItemsBySearchCriteria_SearchByEffectiveAcronym()
        {
            var wiRepository = new WorkItemRepository { UoW = UoW };
            var result = wiRepository.GetWorkItemsBySearchCriteria(new List<int> { 2882 }, 1, "EMC1", string.Empty,
                new List<int>());

            Assert.AreEqual(1, result.Count);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsCountBySearchCriteria(WorkItemFakeDBSet workItemData)
        {
            List<int> releaseIds = new List<int>();

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData).Repeat.Times(6);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiRepository = new WorkItemRepository() { UoW = uow };

            //No Release Ids
            Assert.AreEqual(0, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));

            //----------------------
            //Test with 1 Release Id
            //----------------------
            releaseIds.Add(527);
            Assert.AreEqual(18, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            //Hide 100% records at level 1
            Assert.AreEqual(17, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, true, String.Empty, String.Empty, new List<int>()));
            //-----------------------
            //Test with 2 Release Ids
            //-----------------------
            releaseIds.Add(526);
            Assert.AreEqual(20, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, String.Empty, new List<int>()));
            Assert.AreEqual(12, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, String.Empty, "Stage", new List<int>()));
            Assert.AreEqual(0, wiRepository.GetWorkItemsCountBySearchCriteria(releaseIds, 5, false, "UPCON", String.Empty, new List<int>()));//Because searching now by EffectiveAcronym

            mockDataContext.VerifyAllExpectations();
        }

        [Test(Description = "System should find only one WI with effective acronym EMC1 for release 2882")]
        public void GetWorkItemsCountBySearchCriteria_SearchByEffectiveAcronym()
        {
            var wiRepository = new WorkItemRepository { UoW = UoW };
            var result = wiRepository.GetWorkItemsCountBySearchCriteria(new List<int> { 2882 }, 1, false, "EMC1", string.Empty,
                new List<int>());

            Assert.AreEqual(1, result);
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetAllAcronyms(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiRepository = new WorkItemRepository() { UoW = uow };

            Assert.AreEqual(6, wiRepository.GetAllAcronyms().Count);
            Assert.Contains("UPCON", wiRepository.GetAllAcronyms());
            Assert.Contains("SEES", wiRepository.GetAllAcronyms());
            Assert.Contains("MCPTT", wiRepository.GetAllAcronyms());
            Assert.Contains("eWebRTCi", wiRepository.GetAllAcronyms());
            Assert.Contains("IOPS", wiRepository.GetAllAcronyms());
            Assert.Contains("UPCON-DOT", wiRepository.GetAllAcronyms());
        }

        [Test, TestCaseSource("WorkItemData")]
        public void GetWorkItemsByReleaseId(WorkItemFakeDBSet workItemData)
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.WorkItems).Return((IDbSet<WorkItem>)workItemData);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wiRepository = new WorkItemRepository() { UoW = uow };

            Assert.AreEqual(18, wiRepository.GetAllWorkItemsForReleases(new List<int>() { 527 }).Count);
        }

        [TestCase(2, 1274, 2882, Description = "The WI with lower WiLevel is the 2 with level 1. The related Release should be 2882")]
        [TestCase(2, 3, 2882, Description = "The WI with lower WiLevel are both WI but 2 has the lower Pk_WIUid. The related Release should be 2882")]
        [TestCase(1274, 0, 2883, Description = "The WI with lower WiLevel could only be 1274 because the other doesn't exist. The related Release should be 2883")]
        public void GetReleaseRelatedToTheFirstLevelWorkplans(int wi1, int wi2, int expectedReleaseId)
        {
            var wiRepository = new WorkItemRepository { UoW = UoW };
            var releaseFound = wiRepository.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(new List<int> {wi1, wi2});
            
            Assert.AreEqual(expectedReleaseId, releaseFound.Pk_ReleaseId);
        }

        [Test]
        public void GetReleaseRelatedToTheFirstLevelWorkplans_NoWiFound()
        {
            var wiRepository = new WorkItemRepository { UoW = UoW };
            var releaseFound = wiRepository.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(new List<int> { 0 });

            Assert.IsNull(releaseFound);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            //var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var wiDbSet = new WorkItemFakeDBSet();
            wiDbSet.Add(new WorkItem()
            {
                Pk_WorkItemUid = 1,
                WorkplanId = 2,
                Acronym = "TEST",
                Name = "Name",
                Completion = 12,
                CreationDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now.AddMonths(3),
                Fk_ParentWiId = null,
                Fk_ReleaseId = 1,
                LastModification = DateTime.Now
            });

            iUltimateContext.WorkItems = wiDbSet;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
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

        [TestCase(160000, 1274)]
        [TestCase(999999, 0)]
        public void GetPrimeWorkItemBySpecificationID_Test(int specificationID, int expectedWiId)
        {
            var repo = RepositoryFactory.Resolve<IWorkItemRepository>();
            repo.UoW = UoW;

            var workItemBySpecID = repo.GetPrimeWorkItemBySpecificationID(specificationID);
            if (expectedWiId != 0)
            {
                Assert.AreEqual(expectedWiId, workItemBySpecID.Pk_WorkItemUid);
            }
            else
            {
                Assert.IsNull(workItemBySpecID);
            }
        }
    }
}
