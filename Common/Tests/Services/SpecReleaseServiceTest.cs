using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Practices.Unity;

namespace Etsi.Ultimate.Tests.Services
{
    /// <summary>
    ///  Class in charge of testing SpecificationService concerning spec-release related methods
    /// </summary>
    public class SpecReleaseServiceTest : BaseEffortTest
    {
        #region properties
        const int SpecId = 25;
        const int PERSON_HAS_RIGHT = 1;
        const int PERSON_HAS_NO_RIGHT = 2;
        const int PERSON_HAS_BASIC_RIGHT = 3;

        const int REL_OPEN_ID = 1;
        const int REL_FROZEN_ID = 2;
        const int REL_CLOSED_ID = 10;
        const int SPEC_REL_WITHDRAWN_ID = 3;
        const int SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID = 4;

        const int FIRST_RELEASES_LIST_MOCK = 1;
        const int SECOND_RELEASES_LIST_MOCK = 2;

        const int INITIAL_REL_ID = 1;
        const int SECOND_REL_ID = 2;
        const int THIRD_REL_ID = 3;

        private SpecificationService _specSvc;
        #endregion

        #region init
        [SetUp]
        public void SetUp()
        {
            // Register all the mocks that might be needed.
            SetUpRightsMocks();
            _specSvc = new SpecificationService();
        }
        #endregion

        #region GetRightsForSpecReleases
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
            SetUpGetSpecReleaseRightsMocks();

            // Test the service
            var spec = CreateSpec(isSpecWithdrawn, isUnderCC);

            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(expectedResult, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.Specification_ForceTransposition));

        }

        [TestCase(PERSON_HAS_RIGHT,SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, true)]          // Nominal case     => Success
        [TestCase(PERSON_HAS_NO_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, false)]     // No right         => Failure
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, true, false)]         // Withdrawn spec   => Failure
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_WITHDRAWN_ID, false, false)]                           // Withdrawn Spec Rel => Failure
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, false)]                                     // Open release, but no transposition => Failure
        public void UnforceTransposition(int personId, int relId, bool isWithDrawn, bool expectedResult )
        {
            SetUpGetSpecReleaseRightsMocks();

            // Test the service
            var spec = CreateSpec(isWithDrawn, false);

            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(expectedResult, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.Specification_UnforceTransposition));
        }

        
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, false, true)]                      // Nominal case             ==> True
        [TestCase(PERSON_HAS_NO_RIGHT, REL_OPEN_ID, false, false)]                  // User has no right        ==> False
        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true, false)]                      // Spec is withdrawn        ==> False
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_WITHDRAWN_ID, false, false)]           // Spec-rel is withdrawn    ==> False
        public void WithdrawFromRelease(int personId, int relId, bool isWithdrawn, bool expectedResult)
        {
            SetUpGetSpecReleaseRightsMocks();

            var spec = CreateSpec(isWithdrawn, false);

            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(expectedResult, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.Specification_WithdrawFromRelease));
        }

        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true,          Description = "Has all rights for open release          ==> true")]
        [TestCase(PERSON_HAS_RIGHT, REL_CLOSED_ID, true,        Description = "Has all rights for closed release        ==> true")]
        [TestCase(PERSON_HAS_BASIC_RIGHT, REL_OPEN_ID, true,    Description = "Has basic rights for open release        ==> true")]
        [TestCase(PERSON_HAS_BASIC_RIGHT, REL_CLOSED_ID, false, Description = "Has basic rights for closed release      ==> false")]
        [TestCase(PERSON_HAS_NO_RIGHT, REL_OPEN_ID, false,      Description = "Has no right for open release            ==> false")]
        [TestCase(PERSON_HAS_NO_RIGHT, REL_CLOSED_ID, false,    Description = "Has no right for closed release          ==> false")] 
        public void ShouldBeAbleToAllocateOrUpdate(int personId, int relId, bool expectedResult)
        {
            SetUpGetSpecReleaseRightsMocks();

            var spec = CreateSpec(false, false);

            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(expectedResult, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.Versions_Allocate));
            Assert.AreEqual(expectedResult, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.Versions_Upload));
        }

        [TestCase(PERSON_HAS_RIGHT, REL_OPEN_ID, true, false,                                 Description = "User have the right to remove spec release (is first one)                                                                    ==> true")]
        [TestCase(PERSON_HAS_NO_RIGHT, REL_CLOSED_ID, false, false,                           Description = "User have not right to delete spec release because not first or last one                                                     ==> false")]
        [TestCase(PERSON_HAS_RIGHT, SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, false, true,    Description = "User have not right to delete spec release because contains versions BUT delete possible if remove all versions before        ==> false")]
        public void ShouldBeAbleToRemoveSpecRelease(int personId, int relId, bool shouldContainRemoveRight, bool shouldContainRemoveDisabledRight)
        {
            SetUpGetSpecReleaseRightsMocks();

            var spec = CreateSpec(false, false);
            
            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(shouldContainRemoveRight, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.SpecificationRelease_Remove));
            Assert.AreEqual(shouldContainRemoveDisabledRight, result.First(r => r.Key.Fk_ReleaseId == relId).Value.HasRight(Enum_UserRights.SpecificationRelease_Remove_Disabled));
        }

        [TestCase(PERSON_HAS_RIGHT, SECOND_REL_ID, true, Description = "User have the right to demote spec release ==> true")]
        [TestCase(PERSON_HAS_RIGHT, THIRD_REL_ID, false, Description = "User have not right to demote spec but the release is not the first ==> false")]
        [TestCase(PERSON_HAS_NO_RIGHT, SECOND_REL_ID, false, Description = "user have not right to demote spec ==> false")]
        public void ShouldBeAbleToDemoteSpecRelease(int personId, int relId, bool shouldContainDemoteRight)
        {
            SetUpGetSpecReleaseRightsMocks(SECOND_RELEASES_LIST_MOCK);

            var spec = CreateAlternativeSpec();

            var result = _specSvc.GetRightsForSpecReleases(personId, spec);
            Assert.AreEqual(shouldContainDemoteRight, result.First(r => r.Key.Fk_ReleaseId == relId).Value
                .HasRight(Enum_UserRights.Specification_Demote));
        }
        #endregion

        #region Remove SpecRelease

        [Test]
        public void RemoveSpecReleaseTests_NominalCase()
        {
            //Test preparation : Delete versions linked to this spec release
            UoW.Context.SpecVersions.Where(x => x.Fk_ReleaseId == 2885 && x.Fk_SpecificationId == 150000).ToList().ForEach(x => UoW.Context.SetDeleted(x));
            UoW.Save();
            Assert.IsNotNull(
                UoW.Context.Specification_Release.FirstOrDefault(
                    x => x.Fk_ReleaseId == 2885 && x.Fk_SpecificationId == 150000));
            Assert.IsNotEmpty(UoW.Context.Remarks.Where(x => x.Fk_SpecificationReleaseId == 15).ToList());

            _specSvc.RemoveSpecRelease(150000, 2885, PERSON_HAS_RIGHT);

            Assert.IsNull(
                UoW.Context.Specification_Release.FirstOrDefault(
                    x => x.Fk_ReleaseId == 2885 && x.Fk_SpecificationId == 150000));
            Assert.IsEmpty(UoW.Context.Remarks.Where(x => x.Fk_SpecificationReleaseId == 15).ToList());
        }

        [Test]
        public void RemoveSpecReleaseTests_ShouldFailedBecauseContainsVersions()
        {
            var result = _specSvc.RemoveSpecRelease(150000, 2885, PERSON_HAS_RIGHT);

            Assert.AreEqual(1, result.Report.GetNumberOfErrors());
            Assert.Contains("You do not have right to perform this action", result.Report.ErrorList);
        }

        #endregion

        #region data
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
                Pk_SpecificationId = SpecId,
                Specification_Release = new List<Specification_Release>() {
                    new Specification_Release() { Fk_ReleaseId = REL_OPEN_ID, Fk_SpecificationId = SpecId },
                    new Specification_Release() { Fk_ReleaseId = REL_FROZEN_ID, Fk_SpecificationId = SpecId },
                    new Specification_Release() { Fk_ReleaseId = REL_CLOSED_ID, Fk_SpecificationId = SpecId },
                    new Specification_Release() { Fk_ReleaseId = SPEC_REL_WITHDRAWN_ID, isWithdrawn = true, Fk_SpecificationId = SpecId },
                    new Specification_Release() { Fk_ReleaseId = SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, isWithdrawn = false, isTranpositionForced = true, Fk_SpecificationId = SpecId  },
                },
                IsActive = !isWithDrawn,
                IsUnderChangeControl = isUnderCC
            };
        }

        /// <summary>
        /// Creates a specification to test.
        /// </summary>
        /// <returns></returns>
        private Specification CreateAlternativeSpec()
        {
            return new Specification()
            {
                Pk_SpecificationId = SpecId,
                Specification_Release = new List<Specification_Release>() {
                    new Specification_Release() { Fk_ReleaseId = SECOND_REL_ID, Fk_SpecificationId = SpecId },
                    new Specification_Release() { Fk_ReleaseId = THIRD_REL_ID, Fk_SpecificationId = SpecId }
                },
                IsActive = true,
                IsUnderChangeControl = true
            };
        }

        private void SetUpRightsMocks()
        {
            // Sets up the right manager
            var rightsMgr = MockRepository.GenerateMock<IRightsManager>();

            var noRightContainer = new UserRightsContainer();
            var basicRightsContainer = new UserRightsContainer();
            basicRightsContainer.AddRight(Enum_UserRights.Specification_ForceTransposition);
            basicRightsContainer.AddRight(Enum_UserRights.Specification_UnforceTransposition);
            basicRightsContainer.AddRight(Enum_UserRights.Specification_WithdrawFromRelease);
            basicRightsContainer.AddRight(Enum_UserRights.Versions_Allocate);
            basicRightsContainer.AddRight(Enum_UserRights.Versions_Upload);
            var allRightsContainer = new UserRightsContainer();
            allRightsContainer.AddRight(Enum_UserRights.Specification_ForceTransposition);
            allRightsContainer.AddRight(Enum_UserRights.Specification_UnforceTransposition);
            allRightsContainer.AddRight(Enum_UserRights.Specification_WithdrawFromRelease);
            allRightsContainer.AddRight(Enum_UserRights.Versions_Allocate);
            allRightsContainer.AddRight(Enum_UserRights.Versions_Upload);
            allRightsContainer.AddRight(Enum_UserRights.Versions_AllocateUpload_For_ReleaseClosed_Allowed);
            allRightsContainer.AddRight(Enum_UserRights.SpecificationRelease_Remove);
            allRightsContainer.AddRight(Enum_UserRights.Specification_Demote);

            rightsMgr.Stub(r => r.GetRights(PERSON_HAS_NO_RIGHT)).Return(noRightContainer);
            rightsMgr.Stub(r => r.GetRights(PERSON_HAS_BASIC_RIGHT)).Return(basicRightsContainer);
            rightsMgr.Stub(r => r.GetRights(PERSON_HAS_RIGHT)).Return(allRightsContainer);
            ManagerFactory.Container.RegisterInstance(rightsMgr);
        }

        private List<Release> GetFirstReleaseList()
        {
            return new List<Release>
            {
                new Release { Pk_ReleaseId = REL_OPEN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open }, SortOrder = 1},
                new Release { Pk_ReleaseId = REL_FROZEN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Frozen}, SortOrder = 2},
                new Release { Pk_ReleaseId = REL_CLOSED_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Closed}, SortOrder = 3},
                new Release { Pk_ReleaseId = SPEC_REL_WITHDRAWN_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open}, SortOrder = 4},
                new Release { Pk_ReleaseId = SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open}, SortOrder = 5}
            };
        }

        private List<Release> GetSecondReleaseList()
        {
            return new List<Release>
            {
                new Release { Pk_ReleaseId = INITIAL_REL_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open }, SortOrder = 0},
                new Release { Pk_ReleaseId = SECOND_REL_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open}, SortOrder = 1},
                new Release { Pk_ReleaseId = THIRD_REL_ID, Enum_ReleaseStatus = new Enum_ReleaseStatus { Code = Enum_ReleaseStatus.Open}, SortOrder = 2},
            };
        }

        private void SetUpGetSpecReleaseRightsMocks(int releaseListId)
        {
            // Set up the release Repository
            var releaseManager = MockRepository.GenerateMock<IReleaseManager>();
            var _releaseList = new List<Release>();

            switch (releaseListId)
            {
                case FIRST_RELEASES_LIST_MOCK:
                    _releaseList = GetFirstReleaseList();
                    break;

                case SECOND_RELEASES_LIST_MOCK:
                    _releaseList = GetSecondReleaseList();
                    break;

                default:
                    break;
            }

            releaseManager.Stub(r => r.GetAllReleases(Arg<int>.Is.Anything)).Return(new KeyValuePair<List<Release>, UserRightsContainer>(_releaseList, new UserRightsContainer()));
            ManagerFactory.Container.RegisterInstance(releaseManager);

            //Set up versions linked to a spec
            var specVersionRepoMock = MockRepository.GenerateMock<ISpecVersionsRepository>();
            specVersionRepoMock.Stub(x => x.GetVersionsBySpecId(Arg<int>.Is.Anything)).Return(new List<SpecVersion> { new SpecVersion { Fk_ReleaseId = SPEC_REL_ALREADY_FORCED_TRANSPOSITION_ID, Fk_SpecificationId = SpecId } });
            RepositoryFactory.Container.RegisterInstance(specVersionRepoMock);
        }

        private void SetUpGetSpecReleaseRightsMocks()
        {
            SetUpGetSpecReleaseRightsMocks(FIRST_RELEASES_LIST_MOCK);
        }
        #endregion
    }
}
