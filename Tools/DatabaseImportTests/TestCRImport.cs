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
        /// Test sur l'insertion des CR Categories
        /// </summary>
        [Test]
        public void Test_FillDatabase_CRCategory()
        {
            var categoryExample = "A";
            var meaningExample = "test";

            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new Enum_CRCategoryFakeDbSet();

            newContext.Stub(ctx => ctx.Enum_CRCategories).Return(newDbSet);

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

            var newRelease = newDbSet.All()[0];
            Assert.AreEqual(meaningExample, newRelease.Meaning);
            Assert.AreEqual(categoryExample, newRelease.Category);
        }
    }
}
