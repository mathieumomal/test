using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DomainClasses;
using Rhino.Mocks;
using NUnit.Framework;
using System.Web;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    class SpecificationWorkItemRepositoryTest : BaseTest
    {
        [Test]
        public void SpecificationWorkItemRepository_GetAll()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            Assert.AreEqual(3, repo.All.ToList().Count);

        }

        [Test]
        public void SpecificationWorkItemRepository_AllIncluding()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            Assert.AreEqual(repo.AllIncluding(t => t.WorkItem).ToList().Count, repo.All.ToList().Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void SpecificationWorkItemRepository_Find()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            repo.Find(0);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void SpecificationWorkItemRepository_InsertOrUpdate()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            repo.InsertOrUpdate(new Specification_WorkItem());
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void SpecificationWorkItemRepository_Delete()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            repo.Delete(0);
        }

        [Test]
        public void SpecificationWorkItemRepository_GetSpecsForWI()
        {
            IUltimateUnitOfWork uow = GetUnitOfWork();
            var repo = new SpecificationWorkItemRepository();
            repo.UoW = uow;
            var result = repo.GetWorkItemsForSpec(1);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.First().IsPrimary);
            Assert.IsFalse(result.Last().IsPrimary);
        }

        /// <summary>
        /// Create Mocks to simulate DB with objects
        /// </summary>
        private IUltimateUnitOfWork GetUnitOfWork()
        {
            var iUnitOfWork = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            var iUltimateContext = MockRepository.GenerateMock<IUltimateContext>();

            var wi1 = new WorkItem()
            {
                Pk_WorkItemUid = 1,
                Acronym = "acro1",
                WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                        new WorkItems_ResponsibleGroups(){
                            Pk_WorkItemResponsibleGroups =1,
                            ResponsibleGroup = "S1",
                            IsPrimeResponsible = true
                        },
                        new WorkItems_ResponsibleGroups(){
                            Pk_WorkItemResponsibleGroups =2,
                            ResponsibleGroup = "S3"
                        }
                    }
            };
            var wi2 = new WorkItem(){
                Pk_WorkItemUid = 2,
                Acronym = "acro2",
                WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                    new WorkItems_ResponsibleGroups(){
                        Pk_WorkItemResponsibleGroups =1,
                        ResponsibleGroup = "S3"
                    },
                    new WorkItems_ResponsibleGroups(){
                        Pk_WorkItemResponsibleGroups =2,
                        ResponsibleGroup = "S2",
                        IsPrimeResponsible = true
                    }
                }
            };
            var wi3 = new WorkItem(){
                    Pk_WorkItemUid = 3,
                    Acronym = "acro3",
                    WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                        new WorkItems_ResponsibleGroups(){
                            Pk_WorkItemResponsibleGroups =1,
                            ResponsibleGroup = "S3"
                        },
                        new WorkItems_ResponsibleGroups(){
                            Pk_WorkItemResponsibleGroups =2,
                            ResponsibleGroup = "S2",
                            IsPrimeResponsible = true
                        }
                    }
                };

            var wiDbSet = new WorkItemFakeDBSet();
            List<WorkItem> tmpWIList = new List<WorkItem>(){ wi1, wi2, wi3 };
            tmpWIList.ForEach(e => wiDbSet.Add(e));

            var tmpList = new List<Specification_WorkItem>()
            {
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 1,
                    isPrime = true,
                    WorkItem = tmpWIList[0],
                    Fk_SpecificationId = 1
                },
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 2,
                    isPrime = false,
                    WorkItem =  tmpWIList[1],
                    Fk_SpecificationId = 1 
                },
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 3,
                    isPrime = false,
                    WorkItem = tmpWIList[2],
                    Fk_SpecificationId = 1  
                }
            };
            var dbSet = new SpecificationWorkItemFakeDBSet();
            tmpList.ForEach(e => dbSet.Add(e));

            iUltimateContext.Stub(ctx => ctx.WorkItems).Return(wiDbSet);
            iUltimateContext.Stub(ctx => ctx.Specification_WorkItem).Return(dbSet);
            iUnitOfWork.Stub(uow => uow.Context).Return(iUltimateContext);

            return iUnitOfWork;
        }
    }
}
