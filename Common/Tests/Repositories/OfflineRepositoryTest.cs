using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Repositories
{
    [TestFixture]
    public class OfflineRepositoryTest : BaseTest
    {
        #region Tests

        [Test]
        public void OfflineRepository_InsertOfflineEntity()
        {
            //Arrange
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 386, Fk_VersionId = 396 };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new OfflineRepository();
            repo.UoW = uow;
            repo.InsertOfflineEntity(syncInfo);

            //Assert
            mockDataContext.AssertWasCalled(x => x.SetAdded(syncInfo));
        }

        [Test]
        public void OfflineRepository_UpdateOfflineEntity()
        {
            //Arrange
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 386, Fk_VersionId = 396 };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new OfflineRepository();
            repo.UoW = uow;
            repo.UpdateOfflineEntity(syncInfo);

            //Assert
            mockDataContext.AssertWasCalled(x => x.SetModified(syncInfo));
        }

        [Test]
        public void OfflineRepository_DeleteOfflineEntity()
        {
            //Arrange
            SyncInfo syncInfo = new SyncInfo() { Pk_SyncId = 1, TerminalName = "T1", Offline_PK_Id = 386, Fk_VersionId = 396 };
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //Action
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var repo = new OfflineRepository();
            repo.UoW = uow;
            repo.DeleteOfflineEntity(syncInfo);

            //Assert
            mockDataContext.AssertWasCalled(x => x.SetDeleted(syncInfo));
        } 

        #endregion
    }
}
