﻿using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;

namespace Etsi.Ultimate.Tests.Services
{
    public class CrPackServiceTest : BaseEffortTest
    {
        #region constants
        private const string TsgTdoc = "TSG_ABC";
        #endregion

        #region integration tests
        [Test]
        public void UpdateChangeRequestPackRelatedCrs_NominalCase()
        {
            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(CrForDecisions());
            Assert.IsTrue(response.Result);

            var crWg1 = UoW.Context.ChangeRequests.Find(4);
            var crWg2 = UoW.Context.ChangeRequests.Find(5);
            var crWg5 = UoW.Context.ChangeRequests.Find(9);

            //Check status
            Assert.AreEqual(1, crWg1.ChangeRequestTsgDatas.First(x => x.TSGTdoc == TsgTdoc).Fk_TsgStatus);
            Assert.AreEqual(2, crWg2.ChangeRequestTsgDatas.First(x => x.TSGTdoc == TsgTdoc).Fk_TsgStatus);
            Assert.IsNull(crWg5.ChangeRequestTsgDatas.First(x => x.TSGTdoc == TsgTdoc).Fk_TsgStatus);
        }

        [Test]
        public void UpdateChangeRequestPackRelatedCrs_CrNotFound()
        {
            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(TsgTDocWithUnexistCr());
            Assert.IsFalse(response.Result);

            Assert.AreEqual("Change request not found: " + TsgTDocWithUnexistCr().First().Key.GetIdentifierForLog(), response.Report.ErrorList[0]);
        }

        [Test(Description = "If invalid status found, method is stopped and no CRs saved")]
        public void UpdateChangeRequestPackRelatedCrs_StatusNotValid()
        {
            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(TsgTDocWithInvalidStatus());
            Assert.IsFalse(response.Result);

            var crWg1 = UoW.Context.ChangeRequests.Find(4);
            var crWg2 = UoW.Context.ChangeRequests.Find(5);

            //Check status
            Assert.AreEqual(5, crWg1.ChangeRequestTsgDatas.First(x => x.TSGTdoc == TsgTdoc).Fk_TsgStatus);
            Assert.AreEqual(5, crWg2.ChangeRequestTsgDatas.First(x => x.TSGTdoc == TsgTdoc).Fk_TsgStatus);
            Assert.AreEqual("Status not found: " + TsgTDocWithInvalidStatus().First().Value, response.Report.ErrorList[0]);
        }
        #endregion 

        #region Reissued CR
        [Test(Description = "If CR with same revision already exist and this CR is not related to the same TsgTdocNumber: this CR is reissued. We add a TSG data entry to this CR.")]
        public void UpdateChangeRequestPackRelatedCrs_ReissuedCase()
        {
            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(TsgTDocForReissued());
            Assert.IsTrue(response.Result);

            var crWg1 = UoW.Context.ChangeRequests.Find(4);

            Assert.AreEqual(2, crWg1.ChangeRequestTsgDatas.Count);
            Assert.AreEqual(5, crWg1.ChangeRequestTsgDatas.First().Fk_TsgStatus);
            Assert.AreEqual(1, crWg1.ChangeRequestTsgDatas.Last().Fk_TsgStatus);
        }
        #endregion

        #region Revised CR
        [Test(Description = "If CR not already exists (cause by new revision number): this CR is a revision. We create a new CR.")]
        public void UpdateChangeRequestPackRelatedCrs_RevisedCase()
        {
            var beforeCrCount = UoW.Context.ChangeRequests.Count();

            var svc = new ChangeRequestService();
            var response = svc.UpdateChangeRequestPackRelatedCrs(TsgTDocForRevised());
            Assert.IsTrue(response.Result);

            var crRepository = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepository.UoW = UoW;
            var revisedCr = crRepository.GetCrByKey(TsgTDocForRevised().First().Key);
            var crWg1 = UoW.Context.ChangeRequests.Find(4);

            //Check status
            var afterCrCount = UoW.Context.ChangeRequests.Count();
            Assert.AreEqual(beforeCrCount + 1, afterCrCount);
            Assert.IsNotNull(revisedCr);
            Assert.AreEqual(5, crWg1.ChangeRequestTsgDatas.First().Fk_TsgStatus);
            Assert.AreEqual(1, revisedCr.ChangeRequestTsgDatas.First().Fk_TsgStatus);
        }

        [Test(Description = "If CR not already exists (cause by new revision number) but already revise inside the same CR pack (TsgTdoc)")]
        public void UpdateChangeRequestPackRelatedCrs_RevisedCase_IfAlreadyReviseForThisCrPack()
        {
            var svc = new ChangeRequestService();
            var crAlreadyRevisedInSameTsgTdoc = TsgTDocForRevised();
            crAlreadyRevisedInSameTsgTdoc.First().Key.TsgTdocNumber = "TSGTOREVISE";
            var response = svc.UpdateChangeRequestPackRelatedCrs(crAlreadyRevisedInSameTsgTdoc);
            Assert.IsFalse(response.Result);

            Assert.AreEqual(String.Format(Localization.CR_Cannot_Revise_For_Same_TsgTdocNumber, TsgTDocForRevised().First().Key.GetIdentifierForLog(), crAlreadyRevisedInSameTsgTdoc.First().Key.TsgTdocNumber), response.Report.ErrorList[0]);
        }
        #endregion

        #region Send Crs To Cr-Pack

        [TestCase(1, 24, 0,true,Description = "CR should be successfully added")]
        [TestCase(1, 23, 1, false, Description = "CR could not be sent to CR-Pack")]
        [TestCase(9999, 24, 1, false, Description = "CR-Pack doesn't exist")]
        [TestCase(1, 9999, 1, false, Description = "CR doestn't exist")]
        public void SendCrsToCrPack(int crPackId, int oneCrId, int errorsExpected, bool generatedExpected)
        {
            var contribSvcMocked = MockRepository.GenerateMock<IContributionService>();
            contribSvcMocked.Stub(
                x =>
                    x.GenerateTdocListsAfterSendingCrsToCrPack(Arg<int>.Is.Anything, Arg<int>.Is.Anything,
                        Arg<List<int>>.Is.Anything)).Repeat.Once();
            ServicesFactory.Container.RegisterInstance(typeof(IContributionService), contribSvcMocked);
            var svc = new ChangeRequestService();
            var response = svc.SendCrsToCrPack(1, new List<int> { oneCrId }, crPackId);

            Assert.IsNotNull(response);
            if (errorsExpected > 0)
            {
                Assert.IsFalse(response.Result);
                Assert.AreEqual(errorsExpected, response.Report.GetNumberOfErrors());
                Assert.AreEqual(Localization.GenericError, response.Report.ErrorList.First());
            }
            else
            {
                Assert.IsTrue(response.Result);
                Assert.AreEqual(0, response.Report.GetNumberOfErrors());
                var tsgDataCount = UoW.Context.ChangeRequestTsgDatas.Count(x => x.Fk_ChangeRequest == oneCrId);
                Assert.AreEqual(1, tsgDataCount);
                var tsgData = UoW.Context.ChangeRequestTsgDatas.FirstOrDefault(x => x.Fk_ChangeRequest == oneCrId);
                var crPack = UoW.Context.View_CrPacks.FirstOrDefault(x => x.pk_Contribution == crPackId);
                Assert.IsNotNull(tsgData);
                Assert.AreEqual(crPack.uid, tsgData.TSGTdoc);
                Assert.AreEqual(crPack.Denorm_Source, tsgData.TSGSourceOrganizations);
                Assert.AreEqual(crPack.fk_Meeting, tsgData.TSGMeeting);
            }
            if (generatedExpected)
            {
                contribSvcMocked.VerifyAllExpectations();
            }
            

        }

        #endregion

        #region Dedicated business : CrPackService

        [Test]
        public void UpdateCrsInsideCrPack_InvalidParams()
        {
            var crPackSvc = new CrPackService();
            var resultOne = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade(),
                new List<ChangeRequestInsideCrPackFacade>(), 0);
            var resultTwo = crPackSvc.UpdateCrsInsideCrPack(null,
                new List<ChangeRequestInsideCrPackFacade>(), 0);
            var resultThree = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade(),
                null, 0);

            Assert.IsFalse(resultOne.Result);
            Assert.AreEqual("Invalid arguments: CR-Pack or CRs provided are incorrect", resultOne.Report.ErrorList.First());

            Assert.IsFalse(resultTwo.Result);
            Assert.AreEqual("Invalid arguments: CR-Pack or CRs provided are incorrect", resultTwo.Report.ErrorList.First());

            Assert.IsFalse(resultThree.Result);
            Assert.AreEqual("Invalid arguments: CR-Pack or CRs provided are incorrect", resultThree.Report.ErrorList.First());
        }

        [Test]
        public void UpdateCrsInsideCrPack_Warnings()
        {
            var crRepo = MockRepository.GenerateMock<IChangeRequestRepository>();
            crRepo.Stub(x => x.GetTsgDataForCrPack(Arg<string>.Is.Anything))
                .Return(new List<ChangeRequestTsgData> ());
            RepositoryFactory.Container.RegisterInstance(crRepo);

            var crPackSvc = new CrPackService();
            var resultOne = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade{Uid = "A", Source = string.Empty, MeetingId = 1},
                new List<ChangeRequestInsideCrPackFacade>(), 0);
            var resultTwo = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade { Uid = "A", Source = "ETSI", MeetingId = 0 },
                new List<ChangeRequestInsideCrPackFacade>(), 0);

            Assert.IsTrue(resultOne.Result);
            Assert.AreEqual(0, resultOne.Report.GetNumberOfErrors());
            Assert.AreEqual(1, resultOne.Report.GetNumberOfWarnings());
            Assert.AreEqual(0, resultOne.Report.GetNumberOfInfos());
            Assert.AreEqual("CrPack source and/or meeting not provided and will not be insert as CrTsgData", resultOne.Report.WarningList.First());

            Assert.IsTrue(resultTwo.Result);
            Assert.AreEqual(0, resultTwo.Report.GetNumberOfErrors());
            Assert.AreEqual(1, resultTwo.Report.GetNumberOfWarnings());
            Assert.AreEqual(0, resultTwo.Report.GetNumberOfInfos());
            Assert.AreEqual("CrPack source and/or meeting not provided and will not be insert as CrTsgData", resultTwo.Report.WarningList.First());
        }

        [Test]
        public void UpdateCrsInsideCrPack_NothingTodo()
        {
            var crRepo = MockRepository.GenerateMock<IChangeRequestRepository>();
            crRepo.Stub(x => x.GetTsgDataForCrPack(Arg<string>.Is.Anything))
                .Return(new List<ChangeRequestTsgData> {new ChangeRequestTsgData {Fk_ChangeRequest = 1}});
            RepositoryFactory.Container.RegisterInstance(crRepo);

            var contribSvc = MockRepository.GenerateMock<IContributionService>();
            contribSvc.Stub(
                x =>
                    x.GenerateTdocListsAfterSendingCrsToCrPack(Arg<int>.Is.Equal(637), Arg<int>.Is.Equal(0),
                        Arg<List<int>>.Is.Equal(new List<int>()))).Repeat.Never();
            ServicesFactory.Container.RegisterInstance(contribSvc);

            var crPackSvc = new CrPackService();
            var result = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade { Uid = "A", Source = "ETSI", MeetingId = 1 },
                new List<ChangeRequestInsideCrPackFacade>{new ChangeRequestInsideCrPackFacade{Id = 1}}, 637);

            Assert.IsTrue(result.Result);
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(0, result.Report.GetNumberOfWarnings());
            Assert.AreEqual(0, result.Report.GetNumberOfInfos());
            contribSvc.VerifyAllExpectations();
        }

        [Test]
        public void UpdateCrsInsideCrPack_AddAndRemoveCrsFromCrPack()
        {
            var uowMock = MockRepository.GenerateMock<IUltimateUnitOfWork>();
            uowMock.Stub(x => x.Save());
            RepositoryFactory.Container.RegisterInstance(uowMock);

            var crRepo = MockRepository.GenerateMock<IChangeRequestRepository>();
            crRepo.Stub(x => x.GetTsgDataForCrPack(Arg<string>.Is.Anything))
                .Return(new List<ChangeRequestTsgData> { new ChangeRequestTsgData { Fk_ChangeRequest = 1 } });
            crRepo.Stub(x => x.FindCrsByIds(Arg<List<int>>.Is.Anything)).Return(new List<ChangeRequest>{new ChangeRequest{ChangeRequestTsgDatas = new List<ChangeRequestTsgData>()}});
            RepositoryFactory.Container.RegisterInstance(crRepo);

            var contribSvc = MockRepository.GenerateMock<IContributionService>();
            contribSvc.Stub(
                x =>
                    x.GenerateTdocListsAfterSendingCrsToCrPack(Arg<int>.Is.Equal(637), Arg<int>.Is.Equal(0),
                        Arg<List<int>>.Is.Equal(new List<int>{1, 2}))).Repeat.Never();
            ServicesFactory.Container.RegisterInstance(contribSvc);

            var crPackSvc = new CrPackService();
            var result = crPackSvc.UpdateCrsInsideCrPack(new ChangeRequestPackFacade { Uid = "A", Source = "ETSI", MeetingId = 1 },
                new List<ChangeRequestInsideCrPackFacade> { new ChangeRequestInsideCrPackFacade { Id = 2 } }, 637);

            Assert.IsTrue(result.Result);
            Assert.AreEqual(0, result.Report.GetNumberOfErrors());
            Assert.AreEqual(0, result.Report.GetNumberOfWarnings());
            Assert.AreEqual(0, result.Report.GetNumberOfInfos());
            contribSvc.VerifyAllExpectations();
        }

        #endregion

        #region data
        /// <summary>
        /// TSGs TDoc for update decisions.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<CrKeyFacade, string>> CrForDecisions()
        {
            var tsgTDocDecisions = new List<KeyValuePair<CrKeyFacade, string>>
            {
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AB013", SpecId = 136080, SpecNumber = "22.101", Revision = 1, TsgTdocNumber = "TSG_ABC"}, "Agreed"),//id : 4
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AC014", SpecId = 136080, SpecNumber = "22.101", Revision = 1, TsgTdocNumber = "TSG_ABC"}, "Approved"),//id : 5
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AZEE", SpecId = 136083, SpecNumber = "22.104", Revision = 3, TsgTdocNumber = "TSG_ABC"}, "") //id : 9
            };
            return tsgTDocDecisions;
        }

        /// <summary>
        /// TSGs TDoc for update decisions.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<CrKeyFacade, string>> TsgTDocWithUnexistCr()
        {
            var tsgTDocDecisions = new List<KeyValuePair<CrKeyFacade, string>>
            {
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AB01g3", SpecId = 1360980, SpecNumber = "222.101", Revision = 1, TsgTdocNumber = "TSG_ABC"}, "Agreed")
            };
            return tsgTDocDecisions;
        }

        /// <summary>
        /// TSGs TDoc for update decisions.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<CrKeyFacade, string>> TsgTDocWithInvalidStatus()
        {
            var tsgTDocDecisions = new List<KeyValuePair<CrKeyFacade, string>>
            {
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AB013", SpecId = 136080, SpecNumber = "22.101", Revision = 1, TsgTdocNumber = "TSG_ABC"}, "ABCD"),
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AC014", SpecId = 136080, SpecNumber = "22.101", Revision = 1, TsgTdocNumber = "TSG_ABC"}, "Approved")
            };
            return tsgTDocDecisions;
        }

        /// <summary>
        /// TSGs TDoc for revision.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<CrKeyFacade, string>> TsgTDocForRevised()
        {
            var tsgTDocDecisions = new List<KeyValuePair<CrKeyFacade, string>>
            {
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "TOREVISE", SpecId = 140001, SpecNumber = "22.108", Revision = 1, TsgTdocNumber = "TSGTOREVISE2"}, "Agreed")
            };
            return tsgTDocDecisions;
        }

        /// <summary>
        /// TSGs TDoc for reissued.
        /// </summary>
        /// <returns></returns>
        private static List<KeyValuePair<CrKeyFacade, string>> TsgTDocForReissued()
        {
            var tsgTDocDecisions = new List<KeyValuePair<CrKeyFacade, string>>
            {
                new KeyValuePair<CrKeyFacade,string>(new CrKeyFacade{CrNumber = "AB013", SpecId = 136080, SpecNumber = "22.101", Revision = 1, TsgTdocNumber = "TSG_ABCC"}, "Agreed")
            };
            return tsgTDocDecisions;
        }

        /// <summary>
        /// Gets the Tsg data
        /// </summary>
        private IEnumerable<object[]> RemoveCrsFromCrPackData
        {
            get
            {
                yield return new object[] {  "Change request description6", new List<int>{ 6, 7 }, 3, 1, 24, 22 };
                yield return new object[] { "TSG_ABC", new List<int> { 4 }, 3, 2, 24, 23 };
                yield return new object[] { "RP-CR0001", new List<int> { 11 }, 1, 0, 24, 23 };
                yield return new object[] { "RP-CR0001", new List<int> { 2 }, 1, 1, 24, 24 };
            }
        }

        #endregion
    }
}
