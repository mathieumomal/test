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
    class WorkPlanFileManagerTest : BaseTest
    {
        

        [Test]
        public void GetLastWorkPlanFile()
        {
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            WorkPlanFileFakeRepository fakeRepo = new WorkPlanFileFakeRepository();
            mockDataContext.Stub(x => x.WorkPlanFiles).Return((IDbSet<WorkPlanFile>)fakeRepo.All);

            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var wpFileManager = new WorkPlanFileManager();
            wpFileManager.UoW = uow;

            var wpFile = wpFileManager.GetLastWorkPlanFile();
            Assert.AreEqual("Path3", wpFile.WorkPlanFilePath);
        }
    }
}
