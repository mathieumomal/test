using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    /// <summary>
    ///  Class in charge of testing SpecificationService.GetRightsForSpecReleases() method
    /// </summary>
    class SpecReleaseRightsServiceTest : BaseTest
    {
        const int PERSON_HAS_RIGHT = 1;
        const int PERSON_HAS_NO_RIGHT = 2;

        const int REL_OPEN_ID = 1;
        const int REL_FROZEN_ID = 2;
        const int SPEC_REL_WITHDRAWN_ID = 3;
        const int SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID = 4;

        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, true, true)]              // Nominal case
        [TestCase(PERSON_HAS_NO_RIGHT, REL_OPEN_ID, false, true, false)]          // User has no right                ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, REL_FROZEN_ID, false, true, false)]           // Release is not opened            ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_WITHDRAWN_ID, false, true, false)]   // Spec-Release is withdrawn        ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true, true, false)]              // Spec is withdrawn                ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true, true, false)]              // Spec is withdrawn                ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, true, false)]  // Transp. forced ==> FAILURE
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, false, false)]        // Spec is in draft status          ==> FAILURE
        public void ForceTransposition(int personId, int relId, bool isSpecWithdrawn, bool isUnderCC, bool expectedResult)
        {
            // Register all the mocks that might be needed.
            SetUpMocks();

            // Test the service
            var spec = CreateSpec(isSpecWithdrawn, isUnderCC);
            var specSvc = new SpecificationService();
            var result = specSvc.GetRightsForSpecReleases(personId, spec);

            Assert.AreEqual(expectedResult, result.Where(r => r.Key.Fk_ReleaseId == relId).FirstOrDefault().Value.HasRight(Enum_UserRights.Specification_ForceTransposition));

        }

        [TestCase(PERSON_HAS_RIGHT,SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, true)]          // Nominal case     => Success
        [TestCase(PERSON_HAS_NO_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, false)]     // No right         => Failure
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, true, false)]         // Withdrawn spec   => Failure
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_WITHDRAWN_ID, false, false)]                           // Withdrawn Spec Rel => Failure
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, false)]                                     // Open release, but no transposition => Failure
        public void UnforceTransposition(int personId, int relId, bool isWithDrawn, bool expectedResult )
        {
            SetUpMocks();

            // Test the service
            var spec = CreateSpec(isWithDrawn, false);

            var specSvc = new SpecificationService();
            var result = specSvc.GetRightsForSpecReleases(personId, spec);

            Assert.AreEqual(expectedResult, result.Where(r => r.Key.Fk_ReleaseId == relId).FirstOrDefault().Value.HasRight(Enum_UserRights.Specification_UnforceTransposition));
        }

        
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, true)]                      // Nominal case             ==> True
        [TestCase(PERSON_HAS_NO_RIGHT, REL_OPEN_ID, false, false)]                  // User has no right        ==> False
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true, false)]                      // Spec is withdrawn        ==> False
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_WITHDRAWN_ID, false, false)]           // Spec-rel is withdrawn    ==> False
        public void WithdrawFromRelease(int personId, int relId, bool isWithdrawn, bool expectedResult)
        {
            SetUpMocks();
            var spec = CreateSpec(isWithdrawn, false);

            var specSvc = new SpecificationService();
            var result = specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(expectedResult, result.Where(r => r.Key.Fk_ReleaseId == relId).FirstOrDefault().Value.HasRight(Enum_UserRights.Specification_WithdrawFromRelease));
        }

        /// <summary>
        /// Creates a specification to test.
        /// </summary>
        /// <param name="isWithDrawn"></param>
        /// <param name="isUnderCC"></param>
        /// <returns></returns>
        private Specification CreateSpec(bool isWithDrawn, bool isUnderCC)
        {
            return new Specification()
            {
                Pk_SpecificationId = 25,
                Specification_Release = new List<Specification_Release>() {
                    new Specification_Release() { Fk_ReleaseId = REL_OPEN_ID, Fk_SpecificationId = 25 },
                    new Specification_Release() { Fk_ReleaseId = REL_FROZEN_ID, Fk_SpecificationId = 25 },
                    new Specification_Release() { Fk_ReleaseId = SPEC_REL_WITHDRAWN_ID, isWithdrawn = true, Fk_SpecificationId = 25 },
                    new Specification_Release() { Fk_ReleaseId = SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, isWithdrawn = false, isTranpositionForced = true, Fk_SpecificationId = 25  },
                },
                IsActive = !isWithDrawn,
                IsUnderChangeControl = isUnderCC
            };
        }

        private void SetUpMocks()
        {
            // Sets up the right manager
            var rightsMgr = MockRepository.GenerateMock<IRightsManager>();

            var noRightContainer = new UserRightsContainer();
            var allRightsContainer = new UserRightsContainer();
            allRightsContainer.AddRight(Enum_UserRights.Specification_ForceTransposition);
            allRightsContainer.AddRight(Enum_UserRights.Specification_UnforceTransposition);
            allRightsContainer.AddRight(Enum_UserRights.Specification_WithdrawFromRelease);

            rightsMgr.Stub(r => r.GetRights(PERSON_HAS_NO_RIGHT)).Return(noRightContainer);
            rightsMgr.Stub(r => r.GetRights(PERSON_HAS_RIGHT)).Return(allRightsContainer);

            ManagerFactory.Container.RegisterInstance<IRightsManager>(rightsMgr);

            // Set up the release Repository
            var releaseManager = MockRepository.GenerateMock<IReleaseManager>();

            var releaseList = new List<Release>()
            {
                new Release() { Pk_ReleaseId = REL_OPEN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open } },
                new Release() { Pk_ReleaseId = REL_FROZEN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Frozen} },
                new Release() { Pk_ReleaseId = SPEC_REL_WITHDRAWN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open} },
                new Release() { Pk_ReleaseId = SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus() { Code = Enum_ReleaseStatus.Open} },

            };

            releaseManager.Stub(r => r.GetAllReleases(Arg<int>.Is.Anything)).Return(new KeyValuePair<List<Release>,UserRightsContainer>(releaseList,new UserRightsContainer()));
            ManagerFactory.Container.RegisterInstance<IReleaseManager>(releaseManager);

        }


    }
}
