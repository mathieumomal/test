using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DatabaseImport.ModuleImport.NGPPDB.Contribution;
using DatabaseImportTests.FakeDBSets;
using Etsi.Ngppdb.DataAccess;
using Etsi.Ngppdb.DomainClasses;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeSets;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using NUnit.Framework;
using Rhino.Mocks;

namespace DatabaseImportTests
{
    public class TestContributionImport
    {
        #region Contribution
        [TestCaseSource("GetContributionForTests")]
        public void FillDatabase_TDoc(C2006_03_17_tdocs legacyObject, Contribution expectedObject)
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<INGPPDBContext>();
            var newDbSet = new ContributionFakeDbSet();
            newContext.Stub(ctx => ctx.Contribution).Return(newDbSet);
            newContext.Stub(ctx => ctx.Enum_ContributionStatus).Return(GetContributionStatus());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new TdocLegacyFakeDbSet {legacyObject};
            legacyContext.Stub(ctx => ctx.C2006_03_17_tdocs).Return(legacyDbSet);

            // Report
            var report = new Report();

            // Execute
            var import = new ContributionImport() { LegacyContext = legacyContext, Report = report, NgppdbContext = newContext};
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.Count());

            var newTDoc = newDbSet.FirstOrDefault(x => x.uid == "C1-131002");
            Assert.IsNotNull(newTDoc);
            Assert.AreEqual(expectedObject.uid, newTDoc.uid);
            Assert.AreEqual(expectedObject.title, newTDoc.title);
            //Assert.AreEqual(expectedObject.fk_Enum_ContributionStatus, newTDoc.fk_Enum_ContributionStatus);
        }
        #endregion
        

        #region data

        private IEnumerable<TestCaseData> GetContributionForTests
        {
            get
            {
                yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 1,
                        doc_tdoc = "C1-131002",
                        doc_title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        doc_source = "Vodafone",
                        doc_remarks = "aGreed"
                    },
                    new Contribution()
                    {
                        uid = "C1-131002",
                        title = "Tunneling of UE services over restrictive access networks; Protocol Details",
                        //source = "Vodafone",
                        fk_Enum_ContributionStatus = 1,
                    }
                );

                /*yield return new TestCaseData(
                    new C2006_03_17_tdocs()
                    {
                        Row_id = 2,
                        doc_tdoc = "C1-131004",
                        doc_title = "Pseudo-CR on iFire Firewall Detection and Traversal",
                        doc_source = "Vodafone/Acme Packet",
                        doc_remarks = "postponed"
                    },
                    new Contribution()
                    {
                        uid = "C1-131004",
                        title = "Pseudo-CR on iFire Firewall Detection and Traversal",
                        Source = "Vodafone/Acme Packet",
                        Fk_TDocStatus = 3,
                    }
                );*/


            }
        }

        private EnumContributionStatusFakeDbSet GetContributionStatus()
        {
            var dbSet = new EnumContributionStatusFakeDbSet()
            {
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 8, Enum_Value = "Reserved", Enum_Code = "Reserved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 9, Enum_Value = "Available", Enum_Code = "Available"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 5, Enum_Value = "Withdrawn", Enum_Code = "Withdrawn"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 6, Enum_Value = "Revised", Enum_Code = "Revised"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 10, Enum_Value = "Approved", Enum_Code = "Approved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 11, Enum_Value = "Agreed", Enum_Code = "Agreed"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 12, Enum_Value = "Partially approved", Enum_Code = "PartApproved"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 13, Enum_Value = "Technically endorsed", Enum_Code = "TechEndorsed"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 14, Enum_Value = "Merged", Enum_Code = "Merged"},
                new Enum_ContributionStatus() {pk_Enum_ContributionStatus = 15, Enum_Value = "Reissued", Enum_Code = "Reissued"}
            };
            return dbSet;
        }
        #endregion
    }
}
