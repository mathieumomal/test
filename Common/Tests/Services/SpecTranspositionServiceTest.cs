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
            SetMocks(true);

            var transpMock = MockRepository.GenerateMock<ITranspositionManager>();
            transpMock.Stub(m => m.Transpose(Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance<ITranspositionManager>(transpMock);

            var specSvc = new SpecificationService();
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPEN_ID, SPEC_ID));

            // System should not transpose something that is already transposed.
            transpMock.AssertWasNotCalled(m => m.Transpose(Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything));
        }

        [Test]
        public void ForceTransposition_SendsVersionForTranspositionIfUploaded()
        {
            SetMocks(true);

            var transpMock = MockRepository.GenerateMock<ITranspositionManager>();
            transpMock.Stub( m => m.Transpose( Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance<ITranspositionManager>(transpMock);

            var specSvc = new SpecificationService();
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPENED_VERSION_TO_TRANSPOSE, SPEC_ID));
            transpMock.AssertWasCalled( m => m.Transpose( Arg<Specification>.Is.Anything, Arg<SpecVersion>.Matches(v => v.TechnicalVersion == 1)));
        }

        [Test]
        public void ForceTransposition_SendsVersionForTransposition()
        {
            SetMocks(true);            
            var specSvc = new SpecificationService();
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPENED_VERSION_TO_TRANSPOSE, SPEC_ID));            
        }

        [Test]
        public void ForceTransposition_ReturnsFalseWhenUserHasNoRight()
        {
            SetMocks(false);
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_NO_RIGHT, 1, 1));
        }

        [Test]
        public void ForceTransposition_ReturnsFalseWhenReleaseIsFrozenOrClosed()
        {
            SetMocks(true);
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_FROZEN_ID, SPEC_ID));
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_CLOSED_ID, SPEC_ID));
        }
        [Test]
        public void ForceTransposition_ReturnsFalseWhenReleaseIsWithdrawn()
        {
            SetMocks(true);
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_WITHDRAWN_ID, SPEC_ID));
        }

        [Test]
        public void ForceTransposition_ReturnsFalseIfForceTranspositionFlagIsAlreadySet()
        {
            SetMocks(true);
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_FORCED_TRANSPOSITION_ID, SPEC_ID));
        }

        /// <summary>
        /// The rights are already tested in SpecREleaseRightsServiceTest, so only Nominal case is needed
        /// </summary>
        [Test]
        public void UnforceTransposition_NominalCase()
        {
            SetMocks(true);
            var specSvc = new SpecificationService();
            Assert.IsTrue(specSvc.UnforceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_FORCED_TRANSPOSITION_ID, SPEC_ID));
        }

        [Test]
        public void UnforceTransposition_NoRight()
        {
            SetMocks(true);
            var specSvc = new SpecificationService();
            Assert.IsFalse(specSvc.UnforceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_CLOSED_ID, SPEC_ID));
        }

        private void SetMocks(bool hasBasicRight)
        {
            // Registering spec release
            var specRepo = MockRepository.GenerateMock<ISpecificationRepository>();
            var sp1 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_FROZEN_ID,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Frozen }
                }
            };

            var sp2 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_OPEN_ID,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Closed }
                }
            };

            var sp3 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_CLOSED_ID,
                isWithdrawn = false,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            var sp4 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_WITHDRAWN_ID,
                isWithdrawn = true,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            var sp5 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_FORCED_TRANSPOSITION_ID,
                isWithdrawn = false,
                isTranpositionForced = true,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            var sp6 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_OPENED_VERSION_TO_TRANSPOSE,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_FROZEN_ID, true)).Return(sp1);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_CLOSED_ID, true)).Return(sp2);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPEN_ID, true)).Return(sp3);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_WITHDRAWN_ID, true)).Return(sp4);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_FORCED_TRANSPOSITION_ID, true)).Return(sp5);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE, true)).Return(sp6);

            
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(specRepo);


            // Version repository
            var versionRep = MockRepository.GenerateMock<ISpecVersionsRepository>();
            versionRep.Stub(v => v.GetVersionsForSpecRelease(SPEC_ID, RELEASE_OPEN_ID)).Return(new List<SpecVersion>() {
                    new SpecVersion() { MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 0, EditorialVersion = 2, DocumentUploaded = DateTime.Now.AddDays(-1), DocumentPassedToPub = DateTime.Now }
                });
            versionRep.Stub(v => v.GetVersionsForSpecRelease(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE)).Return(new List<SpecVersion>()
            {
                new SpecVersion() { MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 0, EditorialVersion = 2, DocumentUploaded = DateTime.Now.AddDays(-1) },
                new SpecVersion() { MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 1, EditorialVersion = 0, DocumentUploaded = DateTime.Now, Location="http://www.3gpp.org/ftp/Specs/archive/22_series/22.368/22368-c20.zip" },
            });

            RepositoryFactory.Container.RegisterInstance<ISpecVersionsRepository>(versionRep);

            // Release manager
            var specMgr = MockRepository.GenerateMock<ISpecificationManager>();

            var rightsForce = new UserRightsContainer();
            rightsForce.AddRight(Enum_UserRights.Specification_ForceTransposition);
            var rightsUnforce = new UserRightsContainer();
            rightsUnforce.AddRight(Enum_UserRights.Specification_UnforceTransposition);
            var rightsKO = new UserRightsContainer();

            var rightsResult = new List<KeyValuePair<Specification_Release, UserRightsContainer>>()
            {
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp1, rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp2, hasBasicRight? rightsForce:rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp3, rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp4, rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp5, hasBasicRight? rightsUnforce:rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp6, hasBasicRight? rightsForce:rightsKO),
            };
            specMgr.Stub(r => r.GetRightsForSpecReleases(Arg<int>.Is.Anything, Arg<Specification>.Is.Anything)).Return(rightsResult);

            var spec = new Specification()
            {
                Pk_SpecificationId = SPEC_ID,
                Specification_Release = new List<Specification_Release>() { sp1, sp2, sp3, sp4, sp5, sp6 },
            };
            specMgr.Stub(r => r.GetSpecificationById(Arg<int>.Is.Anything, Arg<int>.Is.Anything)).Return(new KeyValuePair<Specification, UserRightsContainer>(spec, rightsKO));

            ManagerFactory.Container.RegisterInstance<ISpecificationManager>(specMgr);
        }

    }
}
