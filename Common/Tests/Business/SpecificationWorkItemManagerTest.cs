using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using System.Data.Entity;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Business
{
    class SpecificationWorkItemManagerTest : BaseTest
    {
        [Test]
        public void GetSpecificationWorkItemsBySpecId()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            SpecificationWIFakeRepository fakeRepo = new SpecificationWIFakeRepository();
            mockDataContext.Stub(x => x.Specification_WorkItem).Return((IDbSet<Specification_WorkItem>)fakeRepo.All);
            
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specWiManager = new SpecificationWorkItemManager();
            specWiManager.UoW = uow;

            var workItems = specWiManager.GetSpecificationWorkItemsBySpecId(1);
            Assert.AreEqual(3, workItems.Count);
        }

        [Test]
        public void GetSpecificationWorkItemsLabels()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            SpecificationWIFakeRepository fakeRepo = new SpecificationWIFakeRepository();
            mockDataContext.Stub(x => x.Specification_WorkItem).Return((IDbSet<Specification_WorkItem>)fakeRepo.All);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specWiManager = new SpecificationWorkItemManager();
            specWiManager.UoW = uow;

            var workItems = specWiManager.GetSpecificationWorkItemsLabels(1);
            Assert.AreEqual(3, workItems.Count);
            Assert.AreEqual("<strong>#2 - B</strong>", workItems.First());
            Assert.AreEqual("#1 - A", workItems.ElementAt(1));
            Assert.AreEqual("#3 - C", workItems.Last());
        }

    }
}
