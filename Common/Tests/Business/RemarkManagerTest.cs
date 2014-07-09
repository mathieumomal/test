using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
    public class RemarkManagerTest : BaseTest
    {
        #region Tests

        [Test]
        public void RemarkManager_InsertEntity()
        {
            //Arrange
            Remark remark = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };
            string terminalName = "T1";

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new RemarkManager(uow);
            
            //Assert
            bool isSuccess = manager.InsertEntity(null, terminalName);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<Remark>.Is.Anything));

            isSuccess = manager.InsertEntity(remark, terminalName);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<Remark>.Matches(y => 
                   ((y.Pk_RemarkId == remark.Pk_RemarkId)
                && (y.Fk_PersonId == remark.Fk_PersonId)
                && (y.IsPublic == remark.IsPublic)
                && (y.RemarkText == remark.RemarkText)
                && (y.SyncInfoes.Count == 1)
                && (y.SyncInfoes.FirstOrDefault().TerminalName == terminalName)
                && (y.SyncInfoes.FirstOrDefault().Offline_PK_Id == remark.Pk_RemarkId)))));
        }

        [Test]
        public void RemarkManager_UpdateEntity()
        {
            //Arrange
            Remark remark1 = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = false, RemarkText = "Remark 1 - Offline" };
            Remark remark2 = new Remark() { Pk_RemarkId = 2, Fk_PersonId = 122, IsPublic = true, RemarkText = "Remark 2" };
            Remark remarkDB = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return(((IDbSet<Remark>)new RemarkFakeDbSet() { remarkDB }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new RemarkManager(uow);

            //Assert
            bool isSuccess = manager.UpdateEntity(null);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<Remark>.Is.Anything));

            isSuccess = manager.UpdateEntity(remark2);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<Remark>.Is.Anything));

            isSuccess = manager.UpdateEntity(remark1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<Remark>.Matches(y =>
                   ((y.Pk_RemarkId == remark1.Pk_RemarkId)
                && (y.Fk_PersonId == remark1.Fk_PersonId)
                && (y.IsPublic == remark1.IsPublic)
                && (y.RemarkText == remark1.RemarkText)))));
        }

        [Test]
        public void RemarkManager_DeleteEntity()
        {
            //Arrange
            Remark remark1 = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = false, RemarkText = "Remark 1" };
            Remark remark2 = new Remark() { Pk_RemarkId = 2, Fk_PersonId = 122, IsPublic = true, RemarkText = "Remark 2" };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return(((IDbSet<Remark>)new RemarkFakeDbSet() { remark1, remark2 }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var manager = new RemarkManager(uow);

            //Assert
            bool isSuccess = manager.DeleteEntity(0);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetDeleted(Arg<Remark>.Is.Anything));

            isSuccess = manager.DeleteEntity(1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetDeleted(Arg<Remark>.Matches(y =>
                   ((y.Pk_RemarkId == remark1.Pk_RemarkId)
                && (y.Fk_PersonId == remark1.Fk_PersonId)
                && (y.IsPublic == remark1.IsPublic)
                && (y.RemarkText == remark1.RemarkText)))));
        }

        #endregion
    }
}
