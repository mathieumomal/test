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
    public class TestSpecificationImport
    {
        [Test]
        public void Test_Clean()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            newDbSet.Add(new Domain.Specification() { Pk_SpecificationId = 1 });
            var specTechDbSet = new SpecificationTechnologyFakeDBSet();
            specTechDbSet.Add(new Domain.SpecificationTechnology() { Pk_SpecificationTechnologyId = 1, Fk_Specification = 1 });
            specTechDbSet.Add(new Domain.SpecificationTechnology() { Pk_SpecificationTechnologyId = 2, Fk_Specification = 2 });

            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationTechnologies).Return(specTechDbSet);

            var import = new SpecificationImport() { LegacyContext = null, NewContext = newContext, Report = null };
            import.CleanDatabase();

            Assert.AreEqual(0, newDbSet.All().Count);
            Assert.AreEqual(1, specTechDbSet.All().Count); 
        }


        [Test]
        public void Test_FillDatabase_CommonFields()
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            var techSpecSet = new SpecificationTechnologyFakeDBSet();
            var serieSpecSet = new Enum_SerieFakeDBSet();
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationTechnologies).Return(techSpecSet);
            newContext.Stub(ctx => ctx.Enum_Technology).Return(GetTechnos());
            newContext.Stub(ctx => ctx.Enum_Serie).Return(GetSeries());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new SpecificationLegacyFakeDBSet();
            var legacyDbSetForSchedule = new ScheduleLegacyFakeDBSet();
            var legacyDbSetForReleaseInfo = new ReleaseInfoFakeDBSet();

            //Values to test
            var updateDate = DateTime.Now;
            var url = "#http://www.3gpp.org/ftp/Specs/archive/02_series/02.72/#";


            legacyDbSet.Add(
                new Specs_GSM_3G()
                {
                    Row_id = 8,
                    Type = "TS",
                    Number = "02.72",
                    For_publication = true,
                    Title = "Call Deflection Service description; Stage 1",
                    former_WG = "S1  /  SMG01",
                    WG_prime = "S1",
                    WG_other = "-",
                    rapporteur_id = 26063,
                    general_remarks = null,
                    C2g = true,
                    C3g = false,
                    txfer_to_3GPP_notes = null,
                    transferred_to_3GPP = "22.072",
                    C2g_common = false,
                    definitively_withdrawn = false,
                    creation_date = null,
                    update_date = updateDate,
                    title_verified = null,
                    URL = url,
                    Q1741_2 = false,
                    M1457 = false,
                    WI_UID = null,
                    description = null,
                    LTE = false,
                    ComIMS = false,
                    EPS = false,
                    rapporteurChairman = false,
                    otherAx = false,
                    MCC_id = 10343
                }
            );

            legacyDbSetForSchedule.Add(
                new C2001_04_25_schedule()
                {
                    spec = "02.72",
                    MAJOR_VERSION_NB = 5
                }
            );
            legacyDbSetForSchedule.Add(
                new C2001_04_25_schedule()
                {
                    spec = "02.72",
                    MAJOR_VERSION_NB = 1
                }
            );

            legacyDbSetForReleaseInfo.Add(
                new Specs_GSM_3G_release_info()
                {
                    Row_id = 15,
                    Spec = "02.72",
                    inhibitUpgrade = false
                }
            );
            legacyDbSetForReleaseInfo.Add(
                new Specs_GSM_3G_release_info()
                {
                    Row_id = 67,
                    Spec = "02.72",
                    inhibitUpgrade = true
                }
            );
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G_release_info).Return(legacyDbSetForReleaseInfo);
            legacyContext.Stub(ctx => ctx.C2001_04_25_schedule).Return(legacyDbSetForSchedule);
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(legacyDbSet);



            // Report
            var report = new Domain.ImportReport();

            // Execute
            var import = new SpecificationImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();


            // Test results
            Assert.AreEqual(1, newDbSet.All().Count);
            Assert.AreEqual(1, techSpecSet.All().Count);

            //SPEC
            var newSpec = newDbSet.All()[0];
            
            Assert.AreEqual(true, newSpec.IsTS);
            Assert.AreEqual("02.72", newSpec.Number);
            Assert.AreEqual(null,newSpec.CreationDate);
            Assert.AreEqual(updateDate, newSpec.MOD_TS);
            Assert.AreEqual("/", newSpec.URL.Substring(newSpec.URL.Length-1, 1));
            Assert.AreEqual("h", newSpec.URL.Substring(0, 1));
            

            //SPEC/TECHNO
            var newSpectechno = techSpecSet.All()[0];
            Assert.AreEqual(Enum_TechnologyImport._2gCode, newSpectechno.Enum_Technology.Code);

            //VERSION ISUNDERCONTROL (OLD SCHEDULE)
            Assert.AreEqual(true, newSpec.promoteInhibited);

            //SERIE
            Assert.AreEqual(2, newSpec.Fk_SerieId);
            
        }


        private IDbSet<Domain.Enum_Technology> GetTechnos()
        {
            var list = new Enum_TechnologyFakeDBSet();
            list.Add(new Domain.Enum_Technology() { Pk_Enum_TechnologyId = 1, Code = Enum_TechnologyImport._2gCode, Description = "" });
            list.Add(new Domain.Enum_Technology() { Pk_Enum_TechnologyId = 2, Code = Enum_TechnologyImport._3gCode, Description = "" });
            list.Add(new Domain.Enum_Technology() { Pk_Enum_TechnologyId = 3, Code = Enum_TechnologyImport._lteCode, Description = "" });

            return list;
        }

        private IDbSet<Domain.Enum_Serie> GetSeries()
        {
            var list = new Enum_SerieFakeDBSet();
            list.Add(new Domain.Enum_Serie() { Pk_Enum_SerieId = 1, Code = "SER_01" });
            list.Add(new Domain.Enum_Serie() { Pk_Enum_SerieId = 2, Code = "SER_02" });

            return list;
        }
    }


}
