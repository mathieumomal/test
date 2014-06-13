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
    class SpecificationsMassivePromotionActionTest : BaseTest
    {
        #region Tests
        [TestCase(1, 1, 2)]
        public void DefinitivelywithdrawSpecification(int personId, int initialReleaseId, int targetReleaseId)
        {
            //Arrange
            //User Rights
            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_BulkPromote);
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(personId)).Return(userRights);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            //Release
            var releaseDBSet = GetReleases();            
            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)releaseDBSet);
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            SpecVersionsFakeRepository specVersionRep = new SpecVersionsFakeRepository();
            RepositoryFactory.Container.RegisterInstance(typeof(ISpecVersionsRepository), specVersionRep);

            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction(uow);
            List<Specification> result = specificationsMassivePromotionAction.GetSpecificationForMassivePromotion(personId, initialReleaseId, targetReleaseId).Key;

            //Asserts
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result.Where(s => s.IsNewVersionCreationEnabled).ToList().Count);
             
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Get Fake releases Details
        /// </summary>
        /// <returns>Release Fake DBSet</returns>
        private ReleaseFakeDBSet GetReleases()
        {
            var releaseDbSet = new ReleaseFakeDBSet();
            ReleaseFakeRepository releaseFakeRepository = new ReleaseFakeRepository();
            var specReleaseList = new List<Specification_Release>() 
            {
                new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 1, IsActive= true, IsUnderChangeControl = true}
                },
                new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 2, Fk_ReleaseId = 1, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 2, IsActive= true, IsUnderChangeControl = true}
                },
                new Specification_Release() { Pk_Specification_ReleaseId = 3, Fk_SpecificationId = 3, Fk_ReleaseId = 1, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 3, IsActive= true, IsUnderChangeControl = false}
                },                
                new Specification_Release() { Pk_Specification_ReleaseId = 4, Fk_SpecificationId = 4, Fk_ReleaseId = 1, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 4, IsActive= true, IsUnderChangeControl = true, promoteInhibited= true}
                },
                new Specification_Release() { Pk_Specification_ReleaseId = 5, Fk_SpecificationId = 5, Fk_ReleaseId = 1, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 5, IsActive= true, IsUnderChangeControl = true}
                }
            };
            var specReleaseList2 = new List<Specification_Release>() 
            {                
                new Specification_Release() { Pk_Specification_ReleaseId = 6, Fk_SpecificationId = 2, Fk_ReleaseId = 2, isWithdrawn = false, Specification = new Specification()
                {Pk_SpecificationId = 2, IsActive= true, IsUnderChangeControl = true}
                },
            };
            var release1 = new Release() { Pk_ReleaseId = 1, Specification_Release = specReleaseList, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = Enum_ReleaseStatus .Open} };
            var release2 = new Release() { Pk_ReleaseId = 2, Specification_Release = specReleaseList2, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, Code = Enum_ReleaseStatus .Open} };
            releaseDbSet.Add(release1);
            releaseDbSet.Add(release2);
            return releaseDbSet;
        }

        #endregion
    }
}
