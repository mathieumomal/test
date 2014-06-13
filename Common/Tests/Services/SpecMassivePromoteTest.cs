using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Tests.Services
{
    class SpecMassivePromoteTest : BaseTest
    {

        #region Tests
        [Test]
        public void GetSpecificationForMassivePromotion()
        {
            SetMocks();

            UserRightsContainer userRights = new UserRightsContainer();
            userRights.AddRight(Enum_UserRights.Specification_BulkPromote);

            //Mock Rights Manager
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(1)).Return(userRights);
            Assert.IsTrue(mockRightsManager.GetRights(1).HasRight(Enum_UserRights.Specification_BulkPromote));
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);


            var specSvc = new SpecificationService();
            List<Specification> result = specSvc.GetSpecificationForMassivePromotion(1,1,2).Key;

            //Asserts
            Assert.AreEqual(3, result.Count);
            
        }

        #endregion


        #region private methods
        private void SetMocks()
        {
            // Registering spec release
            var relManager = MockRepository.GenerateMock<IReleaseManager>();

            var sp2 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 1,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                },
                Specification = new Specification() { Pk_SpecificationId = 1, IsActive = true, IsUnderChangeControl = true }
            };

            var sp4 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 2,
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 2,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                },
                Specification = new Specification() { Pk_SpecificationId = 2, IsActive = true, IsUnderChangeControl = true }
            };


            var sp6 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 3,
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 3,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                },
                Specification = new Specification() { Pk_SpecificationId = 3, IsActive = true, IsUnderChangeControl = false }
            };

            var sp7 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 7,
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 4,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                },
                Specification = new Specification() { Pk_SpecificationId = 4, IsActive = true, IsUnderChangeControl = true, promoteInhibited=true }
            };

            var sp8 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 7,
                Fk_ReleaseId = 2,
                Fk_SpecificationId = 2,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                },
                Specification = new Specification() { Pk_SpecificationId = 2, IsActive = true, IsUnderChangeControl = true}
            };

            List<Specification_Release> l1 = new List<Specification_Release>() { sp2, sp4, sp6, sp7 };
            List<Specification_Release> l2 = new List<Specification_Release>() { sp8 };

            Release r1 = new Release() { Pk_ReleaseId = 1, Specification_Release = l1 };
            Release r2 = new Release() { Pk_ReleaseId = 2, Specification_Release = l2 };

            //Release manager
            relManager.Stub(s => s.GetReleaseById(1, 1)).Return(new KeyValuePair<Release, UserRightsContainer>(r1,null));
            relManager.Stub(s => s.GetReleaseById(1, 2)).Return(new KeyValuePair<Release, UserRightsContainer>(r2, null));

            ManagerFactory.Container.RegisterInstance(typeof(IReleaseManager), relManager);

            //Versions Repository
            var specVersionRepository = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionRepository.Stub(x => x.GetVersionsByReleaseId(1)).Return(new List<SpecVersion>() { new SpecVersion(){Pk_VersionId=1, Specification = sp4.Specification}});
            RepositoryFactory.Container.RegisterInstance<ISpecVersionsRepository>(specVersionRepository);            
        }
        #endregion
    }
}
