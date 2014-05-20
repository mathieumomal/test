using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Repositories;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    [Category("Specification")]
    class SpecTranspositionServiceTest: BaseTest
    {
        const int USER_TRANSPOSE_NO_RIGHT = 1;
        const int USER_TRANSPOSE_RIGHT = 2;

        const int RELEASE_FROZEN_ID = 1;
        const int RELEASE_OPEN_ID = 2;
        const int RELEASE_CLOSED_ID = 3;
        const int RELEASE_WITHDRAWN_ID = 4;
        const int RELEASE_FORCED_TRANSPOSITION_ID = 5;
        const int RELEASE_OPENED_VERSION_TO_TRANSPOSE = 6;

        const int SPEC_ID = 1;


        [Test]
        public void ForceTransposition_NominalCase()
        {
            SetMocks();

            var specSvc = new SpecificationService();
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPEN_ID, SPEC_ID));
        }

        /*[Test]
        public void ForceTransposition_SendsVersionForTranspositionIfUploaded()
        {
            SetMocks();

            var transpMock = MockRepository.GenerateMock<ITranspositionManager>();
            transpMock.Stub( m => m.Transpose( Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance<ITranspositionManager>(transpMock);

            var specSvc = new SpecificationService();
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPEN_ID, SPEC_ID));
            transpMock.AssertWasCalled( m => m.Transpose( Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything));
        }*/

        [Test]
        public void ForceTransposition_ReturnsFalseWhenUserHasNoRight()
        {
            SetMocks();
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_NO_RIGHT, 1, 1));
        }

        [Test]
        public void ForceTransposition_ReturnsFalseWhenReleaseIsFrozenOrClosed()
        {
            SetMocks();
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_FROZEN_ID, SPEC_ID));
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_CLOSED_ID, SPEC_ID));
        }
        [Test]
        public void ForceTransposition_ReturnsFalseWhenReleaseIsWithdrawn()
        {
            SetMocks();
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_WITHDRAWN_ID, SPEC_ID));
        }

        [Test]
        public void ForceTransposition_ReturnsFalseIfForceTranspositionFlagIsAlreadySet()
        {
            SetMocks();
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_FORCED_TRANSPOSITION_ID, SPEC_ID));
        }

        private void SetMocks()
        {
            // Registering rights
            var rightsMgr = MockRepository.GenerateMock<IRightsManager>();
            rightsMgr.Stub(r => r.GetRights(USER_TRANSPOSE_NO_RIGHT)).Return(new UserRightsContainer());

            var rights = new UserRightsContainer();
            rights.AddRight(Enum_UserRights.Specification_ForceUnforceTransposition);
            rightsMgr.Stub(r => r.GetRights(USER_TRANSPOSE_RIGHT)).Return(rights);

            ManagerFactory.Container.RegisterInstance<IRightsManager>(rightsMgr);

            // Registering spec release
            var specRepo = MockRepository.GenerateMock<ISpecificationRepository>();
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_FROZEN_ID, true)).Return(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Frozen }
                }
            });
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_CLOSED_ID, true)).Return(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Closed }
                }
            });
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_OPEN_ID, true)).Return(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                isWithdrawn = false,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            });
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_WITHDRAWN_ID, true)).Return(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                isWithdrawn = true,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            });
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_FORCED_TRANSPOSITION_ID, true)).Return(new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                isWithdrawn = true,
                isTranpositionForced = true,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            });
            
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(specRepo);

        }

    }
}
