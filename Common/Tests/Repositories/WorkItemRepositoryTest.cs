using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using Etsi.Ultimate.Utils;


namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class WorkItemRepositoryTest : BaseTest
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
            var results = repo.AllIncluding( w => w.Remarks).ToList();

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
                       

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = new FakeContext();
            //var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var wiDbSet = new WorkItemFakeDBSet();
            wiDbSet.Add(new WorkItem() { Pk_WorkItemUid = 1, WorkplanId = 2, Acronym = "TEST", Name = "Name", 
                Completion = 12, CreationDate = DateTime.Now.AddMonths(-1), EndDate = DateTime.Now.AddMonths(3), 
                Fk_ParentWiId = null, Fk_ReleaseId = 1, LastModification = DateTime.Now }); 
            
            iUltimateContext.WorkItems = wiDbSet;

            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);
            return iUnitOfWork;
        }

    }
}
