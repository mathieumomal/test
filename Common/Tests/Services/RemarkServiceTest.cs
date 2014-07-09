using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services
{
    public class RemarkServiceTest : BaseTest
    {
        #region Tests

        [Test]
        public void RemarkService_InsertEntity()
        {
            //Arrange
            int online_PrimaryKey = 0;
            string terminalName = "T1";
            Remark remark1 = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };
            Remark remark2 = new Remark() { Pk_RemarkId = 2, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 2" };
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 1, Fk_RemarkId = 3 };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SyncInfoes).Return((IDbSet<SyncInfo>)new SyncInfoFakeDBSet() { syncInfo });
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new RemarkService();

            //Assert
            //[1] Fault call
            online_PrimaryKey = service.InsertEntity(null, terminalName);
            Assert.AreEqual(0, online_PrimaryKey);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<Remark>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            //[2] Call for already processed record
            online_PrimaryKey = service.InsertEntity(remark1, terminalName);
            Assert.AreEqual(3, online_PrimaryKey);
            mockDataContext.AssertWasNotCalled(x => x.SetAdded(Arg<Remark>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            //[3] Call for new insert
            online_PrimaryKey = service.InsertEntity(remark2, terminalName);
            mockDataContext.AssertWasCalled(x => x.SetAdded(Arg<Remark>.Matches(y =>
                   ((y.Pk_RemarkId == remark2.Pk_RemarkId)
                && (y.Fk_PersonId == remark2.Fk_PersonId)
                && (y.IsPublic == remark2.IsPublic)
                && (y.RemarkText == remark2.RemarkText)
                && (y.SyncInfoes.Count == 1)
                && (y.SyncInfoes.FirstOrDefault().TerminalName == terminalName)
                && (y.SyncInfoes.FirstOrDefault().Offline_PK_Id == remark2.Pk_RemarkId)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void RemarkService_UpdateEntity()
        {
            //Arrange
            Remark remark1 = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };
            Remark remark2 = new Remark() { Pk_RemarkId = 2, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 2" };
            Remark remarkDB = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = false, RemarkText = "Remark DB" };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return(((IDbSet<Remark>)new RemarkFakeDbSet() { remarkDB }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new RemarkService();

            //Assert
            bool isSuccess = service.UpdateEntity(null);
            Assert.IsFalse(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<Remark>.Is.Anything));
            mockDataContext.AssertWasNotCalled(x => x.SaveChanges());

            isSuccess = service.UpdateEntity(remark2);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetModified(Arg<Remark>.Is.Anything));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());

            isSuccess = service.UpdateEntity(remark1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetModified(Arg<Remark>.Matches(y =>
                   ((y.Pk_RemarkId == remark1.Pk_RemarkId)
                && (y.Fk_PersonId == remark1.Fk_PersonId)
                && (y.IsPublic == remark1.IsPublic)
                && (y.RemarkText == remark1.RemarkText)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        [Test]
        public void RemarkService_DeleteEntity()
        {
            //Arrange
            Remark remark1 = new Remark() { Pk_RemarkId = 1, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 1" };
            Remark remark2 = new Remark() { Pk_RemarkId = 2, Fk_PersonId = 121, IsPublic = true, RemarkText = "Remark 2" };

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Remarks).Return(((IDbSet<Remark>)new RemarkFakeDbSet() { remark1, remark2 }));
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var service = new RemarkService();

            //Assert
            bool isSuccess = service.DeleteEntity(0);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasNotCalled(x => x.SetDeleted(Arg<Remark>.Is.Anything));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());

            isSuccess = service.DeleteEntity(1);
            Assert.IsTrue(isSuccess);
            mockDataContext.AssertWasCalled(x => x.SetDeleted(Arg<Remark>.Matches(y =>
                   ((y.Pk_RemarkId == remark1.Pk_RemarkId)
                && (y.Fk_PersonId == remark1.Fk_PersonId)
                && (y.IsPublic == remark1.IsPublic)
                && (y.RemarkText == remark1.RemarkText)))));
            mockDataContext.AssertWasCalled(x => x.SaveChanges());
        }

        #endregion
    }
}
