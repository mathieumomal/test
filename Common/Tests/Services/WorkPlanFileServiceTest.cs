using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeManagers;
using Etsi.Ultimate.DataAccess;
using Rhino.Mocks;
using System.Data.Entity;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Tests.Services
{
    class WorkPlanFileServiceTest : BaseTest
    {
        [Test]
        public void AddWorkPlanFile()
        {
            string newFilePath = "newPah";
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            WorkPlanFileFakeRepository fakeRepo = new WorkPlanFileFakeRepository();
            mockDataContext.Stub(x => x.WorkPlanFiles).Return((IDbSet<WorkPlanFile>)fakeRepo.All);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            WorkPlanFile wp = new WorkPlanFile() { WorkPlanFilePath = newFilePath, CreationDate = DateTime.Now, EntityStatus = Enum_EntityStatus.New };

            var workPlanFilesvc = ServicesFactory.Resolve<IWorkPlanFileService>();
            workPlanFilesvc.AddWorkPlanFile(wp);
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<WorkPlanFile>.Matches(y => y.CreationDate == wp.CreationDate && y.WorkPlanFilePath == wp.WorkPlanFilePath)));
            mockDataContext.AssertWasCalled(x => x.SaveChanges(), y => y.Repeat.Once());
            mockDataContext.VerifyAllExpectations();
                
        }

        [Test]
        public void GetLastWorkPlanFile()
        {
            
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            WorkPlanFileFakeRepository fakeRepo = new WorkPlanFileFakeRepository();
            mockDataContext.Stub(x => x.WorkPlanFiles).Return((IDbSet<WorkPlanFile>)fakeRepo.All);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var workPlanFilesvc = ServicesFactory.Resolve<IWorkPlanFileService>();
            WorkPlanFile wpf= workPlanFilesvc.GetLastWorkPlanFile();
            Assert.AreEqual("Path3", wpf.WorkPlanFilePath);
            
        }
    }
}
