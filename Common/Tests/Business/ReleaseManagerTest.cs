using Etsi.Ultimate.Business;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;

namespace Etsi.Ultimate.Tests.Business
{
    class ReleaseManagerTest : BaseTest
    {
        #region Constants

        private static readonly string RELEASE_CACHE_KEY = "ULT_BIZ_RELEASES_ALL";

        #endregion

        #region Tests

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        [TestCase(-1, 1)]
        [TestCase(20, 1)]
        public void GetNextReleaseWhenExists(int currentReleaseID, int nextReleaseID)
        {
            //Arrange
            CacheManager.Clear(RELEASE_CACHE_KEY);
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var releases = releaseFakeRepository.All;
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var releaseManager = new ReleaseManager();
            releaseManager.UoW = uow;

            //Act
            var nextRelease = releaseManager.GetNextRelease(currentReleaseID);

            //Assert
            Assert.AreEqual(nextReleaseID, nextRelease.Pk_ReleaseId);
            mockDataContext.VerifyAllExpectations();
        }

        [Test]
        public void GetNextReleaseWhenItIsNotExists()
        {
            //Arrange
            CacheManager.Clear(RELEASE_CACHE_KEY);
            int currentReleaseID = 4; //Latest Release
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var releases = releaseFakeRepository.All;
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releases).Repeat.Once();
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var releaseManager = new ReleaseManager();
            releaseManager.UoW = uow;

            //Act
            var nextRelease = releaseManager.GetNextRelease(currentReleaseID);

            //Assert
            Assert.IsNull(nextRelease);
            mockDataContext.VerifyAllExpectations();
        }

        #endregion
    }
}
