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
    class SpecificationDefinitiveWithdrawalActionTest : BaseTest
    {
        #region Tests
        [TestCase(1, 1, 1)]
        public void DefinitivelywithdrawSpecification(int personId, int specificationId, int withdrawalMeetingId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_Withdraw);            
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Specification
            var specDBSet = GetSpecifications();
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)specDBSet);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specificationDefinitiveWithdrawalAction = new SpecificationDefinitiveWithdrawalAction(uow);

            //Initial Assert
            var spec = specDBSet.Find(specificationId);
            DateTime lastMOD = spec.MOD_TS.GetValueOrDefault();
            Assert.AreEqual(3, spec.Specification_Release.Count);

            //Act
            DateTime actionDate = DateTime.Now;
            specificationDefinitiveWithdrawalAction.WithdrawDefinivelySpecification(personId, specificationId, withdrawalMeetingId);

            //Assert
            Assert.AreEqual(false, spec.IsActive);
            foreach (Specification_Release e in spec.Specification_Release)
            {
                Assert.AreEqual(true, e.isWithdrawn);
                if (e.EntityStatus == Enum_EntityStatus.Modified)
                {
                    Assert.GreaterOrEqual(e.UpdateDate.GetValueOrDefault(), actionDate);
                    Assert.AreEqual(1, e.WithdrawMeetingId);
                }
            }
            Assert.GreaterOrEqual(spec.MOD_TS, lastMOD);
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
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var specReleaseList = new List<Specification_Release>() 
            {
                new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isWithdrawn = false, Release = releaseFakeRepository.All.FirstOrDefault()},
                new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isWithdrawn = false},
                new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 1, Fk_ReleaseId = 2, isWithdrawn = true}
            };
            var specification = new Specification() { Pk_SpecificationId = 1, Number = "00.01U", Title = "First specification", IsActive = true, Specification_Release = specReleaseList, MOD_TS = DateTime.Today.AddDays(-15) };
            specDbSet.Add(specification);
            return specDbSet;
        }

        #endregion


    }
}
