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
using Etsi.Ultimate.DataAccess;
using System.Data.Entity;
using Etsi.Ultimate.Tests.FakeSets;

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
        const int RELEASE_FROZEN_SPEC_ALREADY_TRANSPOSED = 7;

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

            var transposeMgr = MockRepository.GenerateMock<ITranspositionManager>();
            transposeMgr.Stub(r => r.TransposeAllowed(Arg<SpecVersion>.Is.Anything)).Return(true);
            transposeMgr.Stub(r => r.Transpose(Arg<Specification>.Is.Anything, Arg<SpecVersion>.Is.Anything)).Return(true);
            ManagerFactory.Container.RegisterInstance<ITranspositionManager>(transposeMgr);
           
            var specSvc = new SpecificationService();
            DateTime previous = DateTime.Now;
            Assert.AreEqual(true, specSvc.ForceTranspositionForRelease(USER_TRANSPOSE_RIGHT, RELEASE_OPENED_VERSION_TO_TRANSPOSE, SPEC_ID));
            var versionSvc = new SpecVersionService();
            var lastVersion = versionSvc.GetVersionsForSpecRelease(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE).OrderByDescending(s => s.MajorVersion).ThenByDescending(s => s.TechnicalVersion)
                .ThenByDescending(s => s.EditorialVersion).FirstOrDefault();
            Assert.AreEqual(true, lastVersion.ForcePublication);
            Assert.GreaterOrEqual(DateTime.Now, lastVersion.DocumentPassedToPub);
            Assert.LessOrEqual(previous, lastVersion.DocumentPassedToPub);
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

        /// <summary>
        /// Transposition of a version
        /// Conditions to be transposed
        /// U- : no under change control
        /// F- : release not frozen
        /// TF- : specRelease not transposed by force
        /// SP- : specForPublication
        /// WI- : Already transposed.
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="resultExpected"></param>
        [TestCase(1,false)]//U-
        [TestCase(2, false)]//FP- SP-
        [TestCase(3, false)]//TF- F- SP-
        [TestCase(4, true)]//U TF
        [TestCase(5, true)]//U SP F
        [TestCase(7, false)] //U SP F WI-
        public void TransposeAllowed(int versionId, bool resultExpected)
        {
            //User Right
            var mockRightsManager = MockRepository.GenerateMock<IRightsManager>();
            mockRightsManager.Stub(x => x.GetRights(0)).Return(new UserRightsContainer());
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), mockRightsManager);

            var mockDataContext = MockRepository.GenerateMock<IUltimateContext>();
            mockDataContext.Stub(x => x.SpecVersions).Return((IDbSet<SpecVersion>)GetSpecVersions());
            mockDataContext.Stub(x => x.Releases).Return((IDbSet<Release>)GetReleases());
            mockDataContext.Stub(x => x.Enum_ReleaseStatus).Return((IDbSet<Enum_ReleaseStatus>)GetReleaseStatus());
            mockDataContext.Stub(x => x.Specifications).Return((IDbSet<Specification>)GetSpecs());
            mockDataContext.Stub(x => x.Specification_Release).Return((IDbSet<Specification_Release>)GetSpecReleases());
            RepositoryFactory.Container.RegisterInstance(typeof(IUltimateContext), mockDataContext);

            //METHOD
            var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>();
            ITranspositionManager transposeMgr = ManagerFactory.Resolve<ITranspositionManager>();
            transposeMgr.UoW = uow;
            var result = transposeMgr.TransposeAllowed(GetSpecVersions().Where(x => x.Pk_VersionId == versionId).FirstOrDefault());

            Assert.AreEqual(resultExpected, result);
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

            var sp7 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_FROZEN_ID,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Frozen }
                },
                isWithdrawn = false,
                isTranpositionForced = false,
            };

            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_FROZEN_ID, true)).Return(sp1);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_CLOSED_ID, true)).Return(sp2);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPEN_ID, true)).Return(sp3);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_WITHDRAWN_ID, true)).Return(sp4);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_FORCED_TRANSPOSITION_ID, true)).Return(sp5);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE, true)).Return(sp6);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE, true)).Return(sp6);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_FROZEN_SPEC_ALREADY_TRANSPOSED, true)).Return(sp7);

            
            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(specRepo);


            // Version repository
            var versionRep = MockRepository.GenerateMock<ISpecVersionsRepository>();
            versionRep.Stub(v => v.GetVersionsForSpecRelease(SPEC_ID, RELEASE_OPEN_ID)).Return(new List<SpecVersion>() {
                    new SpecVersion() { Pk_VersionId=1, MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 0, EditorialVersion = 2, DocumentUploaded = DateTime.Now.AddDays(-1), DocumentPassedToPub = DateTime.Now }
                });
            versionRep.Stub(v => v.GetVersionsForSpecRelease(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE)).Return(new List<SpecVersion>()
            {
                new SpecVersion() {Pk_VersionId=2, MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 0, EditorialVersion = 2, DocumentUploaded = DateTime.Now.AddDays(-1) },
                new SpecVersion() {Pk_VersionId=3, MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 1, EditorialVersion = 0, DocumentUploaded = DateTime.Now, Location="http://www.3gpp.org/ftp/Specs/archive/22_series/22.368/22368-c20.zip", Fk_ReleaseId = 1, Fk_SpecificationId = 1 },
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

            specMgr.Stub(r => r.GetSpecReleaseBySpecIdAndReleaseId(Arg<int>.Is.Anything, Arg<int>.Is.Anything)).Return(sp6);

            ManagerFactory.Container.RegisterInstance<ISpecificationManager>(specMgr);


        }

        private IDbSet<Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Specification() { Pk_SpecificationId = 1, Number = "1", IsUnderChangeControl = true, IsForPublication = false });
            list.Add(new Specification() { Pk_SpecificationId = 2, Number = "2", IsUnderChangeControl = false, IsForPublication = true });
            list.Add(new Specification() { Pk_SpecificationId = 3, Number = "3", IsUnderChangeControl = true, IsForPublication = false });
            list.Add(new Specification() { Pk_SpecificationId = 4, Number = "3", IsUnderChangeControl = true, IsForPublication = true });
            return list;
        }

        private IDbSet<Specification_Release> GetSpecReleases()
        {
            var list = new SpecificationReleaseFakeDBSet();
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 1, Fk_SpecificationId = 1, Fk_ReleaseId = 1, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 3, Fk_ReleaseId = 2, isTranpositionForced = false });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 3, Fk_ReleaseId = 3, isTranpositionForced = true });
            list.Add(new Specification_Release() { Pk_Specification_ReleaseId = 2, Fk_SpecificationId = 4, Fk_ReleaseId = 1, isTranpositionForced = false });
            return list;
        }

        private IDbSet<SpecVersion> GetVersions()
        {
            var list = new SpecVersionFakeDBSet();
            list.Add(new SpecVersion() { Pk_VersionId = 6, Fk_SpecificationId = 1, Fk_ReleaseId = 1 });
            return list;
        }

        private IDbSet<Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Release() { Pk_ReleaseId = 1, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            list.Add(new Release() { Pk_ReleaseId = 2, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 3, Fk_ReleaseStatus = 1, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Open", Enum_ReleaseStatusId = 1, Description = "Open" } });
            list.Add(new Release() { Pk_ReleaseId = 4, Fk_ReleaseStatus = 2, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = "Frozen", Enum_ReleaseStatusId = 2, Description = "Frozen" } });
            return list;
        }

        private IDbSet<Enum_ReleaseStatus> GetReleaseStatus()
        {
            var list = new Enum_ReleaseStatusFakeDBSet();
            list.Add(new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, Code = "Frozen", Description = "Frozen" });
            return list;
        }

        private SpecVersionFakeDBSet GetSpecVersions()
        {
            var versionDbSet = new SpecVersionFakeDBSet();

            var version = new SpecVersion()
            {
                Pk_VersionId = 1,
                Location = "L1",
                MajorVersion = 10,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 9, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 1, Fk_VersionId = 1, RemarkText = "R1" } },
                Fk_SpecificationId = 2,
                Fk_ReleaseId = 1

            };
            var version2 = new SpecVersion()
            {
                Pk_VersionId = 2,
                Location = null,
                MajorVersion = 20,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 10, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 2, Fk_VersionId = 2, RemarkText = "R22" } },
                Fk_SpecificationId = 1,
                Fk_ReleaseId = 1
            };
            var version3 = new SpecVersion()
            {
                Pk_VersionId = 3,
                Location = null,
                MajorVersion = 30,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R333" } },
                Fk_SpecificationId = 3,
                Fk_ReleaseId = 2
            };
            var version4 = new SpecVersion()
            {
                Pk_VersionId = 4,
                Location = null,
                MajorVersion = 30,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R33" } },
                Fk_SpecificationId = 3,
                Fk_ReleaseId = 3
            };
            var version5 = new SpecVersion()
            {
                Pk_VersionId = 5,
                Location = null,
                MajorVersion = 30,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R1998" } },
                Fk_SpecificationId = 4,
                Fk_ReleaseId = 1
            };
            var version7 = new SpecVersion()
            {
                Pk_VersionId = 7,
                Location = null,
                MajorVersion = 30,
                TechnicalVersion = 2,
                EditorialVersion = 1,
                Source = 1,
                DocumentUploaded = new DateTime(2013, 11, 18),
                ProvidedBy = 1,
                Remarks = new List<Remark>() { new Remark() { Pk_RemarkId = 3, Fk_VersionId = 3, RemarkText = "R1998" } },
                Fk_SpecificationId = 4,
                Fk_ReleaseId = 1,
                ETSI_WKI_ID = 12
            };
            versionDbSet.Add(version);
            versionDbSet.Add(version2);
            versionDbSet.Add(version3);
            versionDbSet.Add(version4);
            versionDbSet.Add(version5);
            versionDbSet.Add(version7);

            return versionDbSet;
        }
    }
}
