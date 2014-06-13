using System;
using DatabaseImport.ModuleImport;
using DatabaseImportTests.LegacyDBSets;
using Etsi.Ultimate.DataAccess;
using Domain = Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tools.TmpDbDataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using Etsi.Ultimate.Tests.FakeSets;
using System.Collections.Generic;
using System.Data.Entity;


namespace DatabaseImportTests
{
    public class TestCRImport
    {
        /// <summary>
        /// CR Category insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRCategory()
        {
            var categoryExample = "A";
            var meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRCategoryFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_CRCategory).Return(newDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new CRCategoryLegacyFakeDbSet();
            legacyDbSet.Add(
                new CR_categories()
                {
                    Row_id = 1,
                    CR_category = "A",
                    meaning = meaningExample
                }
            );
            legacyDbSet.Add(
                new CR_categories()
                {
                    Row_id = 2,
                    CR_category = "B",
                    meaning = meaningExample
                }
            );
            legacyContext.Stub(ctx => ctx.CR_categories).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new Enum_CRCategoryImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(2, newDbSet.All().Count);

            var newCRCategory = newDbSet.All()[0];
            Assert.AreEqual(meaningExample, newCRCategory.Meaning);
            Assert.AreEqual(categoryExample, newCRCategory.Category);
        }

        /// <summary>
        /// TDoc Status insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_TDocStatus()
        {
            var validStatus = "tech endorsed";
            var statusToRemove = " - ";
            var meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_TDocStatusFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_TDocStatus).Return(newDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new CRStatusValuesLegacyFakeDbSet();
            legacyDbSet.Add(
                new CR_status_values()
                {
                    Row_id = 1,
                    CR_status_value = statusToRemove,
                    sort_order = 43,
                    use = meaningExample,
                    WG = true,
                    TSG = false
                }
            );
            legacyDbSet.Add(
                new CR_status_values()
                {
                    Row_id = 2,
                    CR_status_value = validStatus,
                    sort_order = 60,
                    use = meaningExample,
                    WG = true,
                    TSG = true
                }
            );
            legacyContext.Stub(ctx => ctx.CR_status_values).Return(legacyDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new Enum_TDocStatusImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);

            var newTDocStatus = newDbSet.All()[0];
            Assert.AreEqual(meaningExample, newTDocStatus.Meaning);
            Assert.AreEqual(validStatus, newTDocStatus.Status);
            Assert.AreEqual(true, newTDocStatus.WGUsable);
            Assert.AreEqual(true, newTDocStatus.TSGUsable);
            Assert.AreEqual(60, newTDocStatus.SortOrder);
        }

        /// <summary>
        /// TDoc Status insertion test
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRImpact()
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRImpactFakeDbSet();
            newContext.Stub(ctx => ctx.Enum_CRImpact).Return(newDbSet);

            // Report
            var report = new Domain.Report();

            // Execute
            var import = new Enum_CRImpactImport() { LegacyContext = null, NewContext = newContext, Report = report };
            import.FillDatabase();

            // Test results
            Assert.AreEqual(4, newDbSet.All().Count);

            var newTDocStatus = newDbSet.All()[0];
            Assert.AreEqual("UICS Apps", newTDocStatus.Impact);
        }
    }
}
