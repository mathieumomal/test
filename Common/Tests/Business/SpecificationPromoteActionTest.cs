using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeRepositories;
using Etsi.Ultimate.Tests.FakeSets;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
    class SpecificationPromoteActionTest : BaseTest
    {
        #region Tests

        [TestCase(1, 1, 1)]
        public void PromoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            userRights.AddRight(Enum_UserRights.Specification_InhibitPromote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specificationPromoteAction = new SpecificationPromoteAction(uow);
            
            //Initial Assert
            var spec = specDBSet.Find(specificationId);
            Assert.AreEqual(1, spec.Specification_Release.Count);

            //Act
            specificationPromoteAction.PromoteSpecification(personId, specificationId, currentReleaseId);

            //Assert
            Assert.AreEqual(2, spec.Specification_Release.Count);
            var newSpecRelease = spec.Specification_Release.ToList().Where(x => x.Pk_Specification_ReleaseId == default(int)).FirstOrDefault();
            Assert.AreEqual(false, newSpecRelease.isWithdrawn);
            Assert.AreEqual(2, newSpecRelease.Fk_ReleaseId);
            Assert.IsNotNull(newSpecRelease.CreationDate);
            Assert.IsNotNull(newSpecRelease.UpdateDate);
        }

        [TestCase(1, 1, 1)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "You don't have right to promote specification")]
        public void PromoteSpecificationWithoutRights(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specificationPromoteAction = new SpecificationPromoteAction(uow);

            //Act
            specificationPromoteAction.PromoteSpecification(personId, specificationId, currentReleaseId);
        }

        [TestCase(1, 1, 4)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "There is no next release found in the system. Hence, you cannot promote specification")]
        public void PromoteSpecificationForLatestRelease(int personId, int specificationId, int currentReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Create);
            userRights.AddRight(Enum_UserRights.Specification_EditLimitted);
            userRights.AddRight(Enum_UserRights.Specification_InhibitPromote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)Releases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specificationPromoteAction = new SpecificationPromoteAction(uow);

            //Act
            specificationPromoteAction.PromoteSpecification(personId, specificationId, currentReleaseId);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Fake Specification Details
        /// </summary>
        /// <returns>Specification Fake DBSet</returns>
        private SpecificationFakeDBSet GetSpecifications()
        {
            var specDbSet = new SpecificationFakeDBSet();
            var release = Releases().FirstOrDefault();
            var specRelease = new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = release.Pk_ReleaseId, isWithdrawn = false, Release = release };
            var specReleaseList = new List<Specification_Release>() { specRelease };
            var specification = new Specification() { Pk_SpecificationId = 1, Number = "00.01U", Title = "First specification", IsActive=true, Specification_Release = specReleaseList };
            specDbSet.Add(specification);
            return specDbSet;
        }

        /// <summary>
        /// Get Fake Releases
        /// </summary>
        /// <returns>Queryable Release list</returns>
        private IQueryable<Release> Releases()
        {
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            return releaseFakeRepository.All;
        }

        #endregion
    }
}
