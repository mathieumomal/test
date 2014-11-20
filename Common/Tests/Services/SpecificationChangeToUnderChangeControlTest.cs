using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Tests.Services
{
    public class SpecificationChangeToUnderChangeControlTest : BaseEffortTest
    {
        #region Constants

        private const int NO_EDIT_RIGHT_USER = 2;
        private const int EDIT_FULL_RIGHT_USER = 3;
        private const int EDIT_LIMITED_RIGHT_USER = 4;
        private const int SPEC_ID_UNDER_CHANGE_CONTROL = 136080;
        private const int SPEC_ID_DRAFT = 136082;
        private const int SPEC_ID_UCC_FLAG_NOT_SET = 140001;
        private const int SPEC_ID_WITHDRAWN_BEFORE_CHANGE_CONTROL = 140002;
        private List<int> specIdList = new List<int> { SPEC_ID_UNDER_CHANGE_CONTROL, SPEC_ID_DRAFT, SPEC_ID_UCC_FLAG_NOT_SET };
        private List<int> specIdsUcc = new List<int> { SPEC_ID_UNDER_CHANGE_CONTROL };
        private List<int> specIdsWithdrawnBeforeChangeControl = new List<int> { SPEC_ID_WITHDRAWN_BEFORE_CHANGE_CONTROL }; 

        #endregion

        #region Setup

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var noRights = new UserRightsContainer();

            var editFullRights = new UserRightsContainer();
            editFullRights.AddRight(Enum_UserRights.Specification_EditFull);

            var editLimitedRights = new UserRightsContainer();
            editLimitedRights.AddRight(Enum_UserRights.Specification_EditLimitted);

            var userRights = MockRepository.GenerateMock<IRightsManager>();
            userRights.Stub(r => r.GetRights(NO_EDIT_RIGHT_USER)).Return(noRights);
            userRights.Stub(r => r.GetRights(EDIT_FULL_RIGHT_USER)).Return(editFullRights);
            userRights.Stub(r => r.GetRights(EDIT_LIMITED_RIGHT_USER)).Return(editLimitedRights);

            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);
        }

        #endregion

        #region Tests

        [Test, TestCaseSource("GetSpecsForStatusChange")]
        public void EditSpecification_ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIds, bool result, string errorMessage)
        {
            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var specSvcResponse = specSvc.ChangeSpecificationsStatusToUnderChangeControl(personId, specIds);
            Assert.AreEqual(result, specSvcResponse.Result);
            if (result)
                Assert.IsTrue(specSvcResponse.Report.InfoList.Contains(errorMessage));
            else
                Assert.IsTrue(specSvcResponse.Report.ErrorList.Contains(errorMessage));
        }

        [Test]
        public void ChangeSpecToUcc_HistoryTest()
        {
            var specIds = new List<int> { SPEC_ID_DRAFT };

            var specSvc = ServicesFactory.Resolve<ISpecificationService>();
            var specSvcResponse = specSvc.ChangeSpecificationsStatusToUnderChangeControl(EDIT_FULL_RIGHT_USER, specIds);

            Assert.IsTrue(specSvcResponse.Result);
            Assert.IsTrue(specSvcResponse.Report.InfoList.Contains("Following specifications changed to Under Change Control.\n\t22.103: A draft spec"));

            var specAfterModification = UoW.Context.Specifications.Find(SPEC_ID_DRAFT);

            Assert.IsTrue(specAfterModification.IsUnderChangeControl.GetValueOrDefault());
            Assert.AreEqual(1, specAfterModification.Histories.Count);
            Assert.AreEqual(SPEC_ID_DRAFT, specAfterModification.Histories.First().Fk_SpecificationId);
            Assert.AreEqual(EDIT_FULL_RIGHT_USER, specAfterModification.Histories.First().Fk_PersonId);
            Assert.AreEqual(Utils.Localization.History_Specification_Status_Changed_UnderChangeControl, specAfterModification.Histories.First().HistoryText);
        } 

        #endregion

        #region Data

        private IEnumerable<object[]> GetSpecsForStatusChange
        {
            get
            {
                yield return new object[] { NO_EDIT_RIGHT_USER, specIdList, false, Localization.GenericError };
                yield return new object[] { EDIT_FULL_RIGHT_USER, specIdList, true, "Following specifications changed to Under Change Control.\n\t22.103: A draft spec\n\t22.108: A specification without ucc flag" };
                yield return new object[] { EDIT_LIMITED_RIGHT_USER, specIdList, true, "Following specifications changed to Under Change Control.\n\t22.103: A draft spec\n\t22.108: A specification without ucc flag" };
                yield return new object[] { EDIT_FULL_RIGHT_USER, specIdsUcc, true, "None of the specifications changed to Under Change Control." };
                yield return new object[] { EDIT_FULL_RIGHT_USER, specIdsWithdrawnBeforeChangeControl, true, "None of the specifications changed to Under Change Control." };
            }
        } 

        #endregion
    }
}
