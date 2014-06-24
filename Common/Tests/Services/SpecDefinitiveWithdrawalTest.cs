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
    class SpecDefinitiveWithdrawalTest : BaseTest
    {
        const int USER_WITHDRAWN_RIGHT = 2;

        const int RELEASE_OPEN_ID = 2;
        const int RELEASE_WITHDRAWN_ID = 4;
        const int RELEASE_OPENED_VERSION_TO_TRANSPOSE = 6;

        const int SPEC_ID = 1;

        #region Tests
        [Test]
        public void DefinitiveWithdrawal_NominalCase()
        {
            SetMocks();

            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            var spec = specMgr.GetSpecificationById(USER_WITHDRAWN_RIGHT, SPEC_ID).Key;
            DateTime lastMOD = spec.MOD_TS.GetValueOrDefault();
            int SpecReleasetoEditCount = spec.Specification_Release.Where(e => !e.isWithdrawn.GetValueOrDefault()).ToList().Count;

            var specSvc = new SpecificationService();
            DateTime actionDate = DateTime.Now;
            Assert.IsTrue(specSvc.DefinitivelyWithdrawSpecification(USER_WITHDRAWN_RIGHT, SPEC_ID, 25));
            
            var specRes = specMgr.GetSpecificationById(USER_WITHDRAWN_RIGHT, SPEC_ID);
            Assert.AreEqual(false, specRes.Key.IsActive);

            int editCounter = 0;
            foreach (Specification_Release e in specRes.Key.Specification_Release)
            {
                Assert.AreEqual(true, e.isWithdrawn);
                if (actionDate <= e.UpdateDate)
                {
                    Assert.AreEqual(25, e.WithdrawMeetingId);
                    editCounter++;
                }
            }
            Assert.AreEqual(editCounter, SpecReleasetoEditCount);
            Assert.GreaterOrEqual(specRes.Key.MOD_TS, lastMOD);            
        }

        [Test]
        public void DefinitiveWithdrawal_Exception()
        {
            SetMocks();            

            var specSvc = new SpecificationService();
            //Invalid specification id
            Assert.False(specSvc.DefinitivelyWithdrawSpecification(USER_WITHDRAWN_RIGHT, 100, 25));
          
        }
        #endregion

        #region private methods
        private void SetMocks()
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
                Pk_Specification_ReleaseId = 2,
                Fk_ReleaseId = RELEASE_WITHDRAWN_ID,
                isWithdrawn = true,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };


            var sp6 = new Specification_Release()
            {
                Pk_Specification_ReleaseId = 3,
                Fk_ReleaseId = RELEASE_OPENED_VERSION_TO_TRANSPOSE,
                Release = new Release()
                {
                    Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open }
                }
            };

            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_OPEN_ID, true)).Return(sp2);
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_WITHDRAWN_ID, true)).Return(sp4);
            specRepo.Stub(s => s.GetSpecificationRelease(SPEC_ID, RELEASE_OPENED_VERSION_TO_TRANSPOSE, true)).Return(sp6);


            RepositoryFactory.Container.RegisterInstance<ISpecificationRepository>(specRepo);            

            //History Repository
            var historyRepository = MockRepository.GenerateMock<IHistoryRepository>();
            historyRepository.Stub(x => x.InsertOrUpdate(Arg<History>.Is.Anything));
            RepositoryFactory.Container.RegisterInstance<IHistoryRepository>(historyRepository);

            // Release manager
            var specMgr = MockRepository.GenerateMock<ISpecificationManager>();

            

            var spec = new Specification()
            {
                Pk_SpecificationId = SPEC_ID,
                Specification_Release = new List<Specification_Release>() { sp2, sp4, sp6 },
            };            
            specMgr.Stub(r => r.GetSpecificationById(Arg<int>.Is.Anything, Arg<int>.Matches(i => i == SPEC_ID))).Return(new KeyValuePair<Specification, UserRightsContainer>(spec, new UserRightsContainer()));
            ManagerFactory.Container.RegisterInstance<ISpecificationManager>(specMgr);
        }
        #endregion
    }
}
