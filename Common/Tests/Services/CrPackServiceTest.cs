using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.Ultimate.Utils;
using NUnit.Framework;

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
        #endregion
    }
}
