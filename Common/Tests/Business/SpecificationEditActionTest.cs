using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Tests.Business
{
    public class SpecificationEditActionTest : BaseEffortTest
    {
        #region Constants

        private const int SPEC_ID_DRAFT = 136082;
        private const int EDIT_RIGHT_USER = 3; 

        #endregion

        #region Tests

        [Test]
        public void ChangeSpecToUcc_HistoryTest()
        {
            var rights = new UserRightsContainer();
            rights.AddRight(Enum_UserRights.Specification_EditFull);

            var userRights = MockRepository.GenerateMock<IRightsManager>();
            userRights.Stub(r => r.GetRights(EDIT_RIGHT_USER)).Return(rights);
            ManagerFactory.Container.RegisterInstance<IRightsManager>(userRights);

            var specIds = new List<int> { SPEC_ID_DRAFT };

            var specEditAction = new SpecificationEditAction();
            specEditAction.UoW = UoW;
            var specSvcResponse = specEditAction.ChangeSpecificationsStatusToUnderChangeControl(EDIT_RIGHT_USER, specIds);

            Assert.IsTrue(specSvcResponse.Result);
            Assert.IsTrue(specSvcResponse.Report.InfoList.Contains("Following specifications changed to Under Change Control.\n\t22.103: A draft spec"));

            var specAfterModification = UoW.Context.Specifications.Find(SPEC_ID_DRAFT);

            Assert.IsTrue(specAfterModification.IsUnderChangeControl ?? false);
            Assert.AreEqual(1, specAfterModification.Histories.Count);
            Assert.AreEqual(SPEC_ID_DRAFT, specAfterModification.Histories.First().Fk_SpecificationId);
            Assert.AreEqual(EDIT_RIGHT_USER, specAfterModification.Histories.First().Fk_PersonId);
            Assert.AreEqual(Utils.Localization.History_Specification_Status_Changed_UnderChangeControl, specAfterModification.Histories.First().HistoryText);
        } 

        #endregion
    }
}
