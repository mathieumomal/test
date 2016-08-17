using System;
using Etsi.Ultimate.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Microsoft.Practices.Unity;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Services
{
    public class FinalizeApprovedDraftsTests : BaseEffortTest
    {
        #region tests
        [TestCase(1, 30000, 136082, 2883, 13, 0, 0)]
        public void FinalizeApprovedDrafts_NominalCase(int personId, int mtgId, int specId, int relId, int major, int tech, int edit)
        {
            MockuserRights();

            //Execute
            var finalizeApprovedDrafts = ServicesFactory.Resolve<IFinalizeApprovedDraftsService>();
            finalizeApprovedDrafts.FinalizeApprovedDrafts(personId, mtgId,
                new List<Tuple<int, int, int>> {new Tuple<int, int, int>(1, specId, relId)});

            //Tests
            var lastVersion = UoW.Context.SpecVersions.Where(x => x.Fk_ReleaseId == relId && x.Fk_SpecificationId == specId).OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion).First();
            Assert.AreEqual(major, lastVersion.MajorVersion);
            Assert.AreEqual(tech, lastVersion.TechnicalVersion);
            Assert.AreEqual(edit, lastVersion.EditorialVersion);
            Assert.AreEqual(relId, lastVersion.Fk_ReleaseId);
            Assert.AreEqual(specId, lastVersion.Fk_SpecificationId);
            Assert.AreEqual(mtgId, lastVersion.Source);
            Assert.AreEqual(personId, lastVersion.ProvidedBy);
        }

        [TestCase(1, 30000, 136084, 2883, 13, 0, 0)]
        public void FinalizeApprovedDrafts_SpecAlreadyUcc(int personId, int mtgId, int specId, int relId, int major, int tech, int edit)
        {
            var versionCount = UoW.Context.SpecVersions.Count();
            //Execute
            var finalizeApprovedDrafts = ServicesFactory.Resolve<IFinalizeApprovedDraftsService>();
            finalizeApprovedDrafts.FinalizeApprovedDrafts(personId, mtgId,
                new List<Tuple<int, int, int>> { new Tuple<int, int, int>(1, specId, relId) });

            //Tests
            Assert.AreEqual(versionCount, UoW.Context.SpecVersions.Count());
            var lastVersion = UoW.Context.SpecVersions.Where(x => x.Fk_ReleaseId == relId && x.Fk_SpecificationId == specId).OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion).First();
            Assert.AreEqual(major, lastVersion.MajorVersion);
            Assert.AreEqual(tech, lastVersion.TechnicalVersion);
            Assert.AreEqual(edit, lastVersion.EditorialVersion);
        }

        [TestCase(1, 30000, 136082, 2883, 13, 0, 0)]
        public void FinalizeApprovedDrafts_ShouldNotAllocateMultipleVersionForSameSpecRelease(int personId, int mtgId, int specId, int relId, int major, int tech, int edit)
        {
            MockuserRights();

            var versionCount = UoW.Context.SpecVersions.Count();
            //Execute
            var finalizeApprovedDrafts = ServicesFactory.Resolve<IFinalizeApprovedDraftsService>();
            finalizeApprovedDrafts.FinalizeApprovedDrafts(personId, mtgId,
                new List<Tuple<int, int, int>> { new Tuple<int, int, int>(1, specId, relId), new Tuple<int, int, int>(2, specId, relId) });

            //Tests
            Assert.AreEqual(versionCount + 1, UoW.Context.SpecVersions.Count());
            var lastVersion = UoW.Context.SpecVersions.Where(x => x.Fk_ReleaseId == relId && x.Fk_SpecificationId == specId).OrderByDescending(v => v.MajorVersion).ThenByDescending(v => v.TechnicalVersion).ThenByDescending(v => v.EditorialVersion).First();
            Assert.AreEqual(major, lastVersion.MajorVersion);
            Assert.AreEqual(tech, lastVersion.TechnicalVersion);
            Assert.AreEqual(edit, lastVersion.EditorialVersion);
        }
        #endregion

        #region mocks
        private void MockuserRights()
        {
            //Mocks
            var rightmgrMock = MockRepository.GenerateMock<IRightsManager>();
            var rightContainer = new UserRightsContainer();
            rightContainer.AddRight(Enum_UserRights.Versions_Allocate);
            rightContainer.AddRight(Enum_UserRights.Specification_EditFull);
            rightmgrMock.Stub(s => s.GetRights(Arg<int>.Is.Anything))
                .Return(rightContainer);
            ManagerFactory.Container.RegisterInstance(typeof(IRightsManager), rightmgrMock);
        }
        #endregion
    }
}
