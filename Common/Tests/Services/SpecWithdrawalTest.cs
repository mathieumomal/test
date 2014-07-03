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

namespace Etsi.Ultimate.Tests.Services
{
    class SpecWithdrawalTest : BaseTest
    {
        const int USER_WITHDRAWN_RIGHT = 2;

        const int RELEASE_OPEN_ID = 2;
        const int RELEASE_WITHDRAWN_ID = 4;
        const int RELEASE_OPENED_VERSION_TO_TRANSPOSE = 6;

        const int SPEC_ID = 1;


        [Test]
        public void WithdrawFromRelease_NominalCase()
        {
            SetMocks(true, false );

            var specSvc = new SpecificationService();
            Assert.IsTrue(specSvc.WithdrawForRelease(USER_WITHDRAWN_RIGHT, RELEASE_OPEN_ID, SPEC_ID, 25));
            
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            var specRes = specMgr.GetSpecificationById(USER_WITHDRAWN_RIGHT, SPEC_ID);
            Assert.AreEqual(true, specRes.Key.Specification_Release.Where(r => r.Fk_ReleaseId == RELEASE_OPEN_ID).FirstOrDefault().isWithdrawn);
            Assert.AreEqual(25, specRes.Key.Specification_Release.Where(r => r.Fk_ReleaseId == RELEASE_OPEN_ID).FirstOrDefault().WithdrawMeetingId);
        }

        [Test]
        public void WithdrawFromRelease_NoRight()
        {
            SetMocks(true, false);

            var specSvc = new SpecificationService();
            Assert.False(specSvc.WithdrawForRelease(USER_WITHDRAWN_RIGHT, RELEASE_WITHDRAWN_ID, SPEC_ID, 25));
        }

        private void SetMocks(bool hasBasicRight, bool isSpecWithdrawn)
        {
            // Registering spec release
            var specRepo = MockRepository.GenerateMock<ISpecificationRepository>();
            
            var sp2 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_OPEN_ID,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Closed }
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
           

            var sp6 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 1,
                Fk_ReleaseId = RELEASE_OPENED_VERSION_TO_TRANSPOSE,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_OPEN_ID, true)).Return(sp2);
            specRepo.Stub(s => s.GetSpecificationReleaseByReleaseIdAndSpecId(SPEC_ID, RELEASE_WITHDRAWN_ID, true)).Return(sp4);
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
                new SpecVersion() { MajorVersion = RELEASE_OPENED_VERSION_TO_TRANSPOSE, TechnicalVersion = 1, EditorialVersion = 0, DocumentUploaded = DateTime.Now },
            });

            RepositoryFactory.Container.RegisterInstance<ISpecVersionsRepository>(versionRep);

            //History Repository
            var historyRepository = MockRepository.GenerateMock<IHistoryRepository>();
            historyRepository.Stub(x => x.InsertOrUpdate(Arg<History>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance<IHistoryRepository>(historyRepository);

            // Release manager
            var specMgr = MockRepository.GenerateMock<ISpecificationManager>();

            var rightsWithdraw = new UserRightsContainer();
            rightsWithdraw.AddRight(Enum_UserRights.Specification_WithdrawFromRelease);
            var rightsKO = new UserRightsContainer();

            var rightsResult = new List<KeyValuePair<Specification_Release, UserRightsContainer>>()
            {
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp2, hasBasicRight? rightsWithdraw:rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp4, rightsKO),
                new KeyValuePair<Specification_Release,UserRightsContainer>(sp6, hasBasicRight? rightsWithdraw:rightsKO),
            };
            specMgr.Stub(r => r.GetRightsForSpecReleases(Arg<int>.Is.Anything, Arg<Specification>.Is.Anything)).Return(rightsResult);

            var spec = new Specification()
            {
                Pk_SpecificationId = SPEC_ID,
                Specification_Release = new List<Specification_Release>() { sp2, sp4, sp6 },
            };
            specMgr.Stub(r => r.GetSpecificationById(Arg<int>.Is.Anything, Arg<int>.Is.Anything)).Return(new KeyValuePair<Specification, UserRightsContainer>(spec, rightsKO));
            specMgr.Stub(r => r.GetSpecificationById(Arg<int>.Is.Anything, Arg<int>.Matches(i => i == SPEC_ID))).Return(new KeyValuePair<Specification, UserRightsContainer>(spec, new UserRightsContainer()));
            ManagerFactory.Container.RegisterInstance<ISpecificationManager>(specMgr);
        }

    }
}
