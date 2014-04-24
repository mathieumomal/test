﻿using System;
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
        #region Values to test
        public DateTime updateDate = DateTime.Now;
        public const string url = "#http://www.3gpp.org/ftp/Specs/archive/02_series/02.72/#";
        public const string remark = "test remarks. test remarks 2...";
        #endregion

        [Test]
        public void Specification_Test_Clean()
        {
            /*var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            newDbSet.Add(new Domain.Specification() { Pk_SpecificationId = 1 });
            var specTechDbSet = new SpecificationTechnologyFakeDBSet();
            specTechDbSet.Add(new Domain.SpecificationTechnology() { Pk_SpecificationTechnologyId = 1, Fk_Specification = 1 });
            specTechDbSet.Add(new Domain.SpecificationTechnology() { Pk_SpecificationTechnologyId = 2, Fk_Specification = 2 });
            var remarkSet = new RemarkFakeDbSet();
            remarkSet.Add(new Domain.Remark() { Fk_SpecificationId = 1, Pk_RemarkId = 1});
            remarkSet.Add(new Domain.Remark() { Fk_SpecificationId = 15, Pk_RemarkId = 2 });

            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationTechnologies).Return(specTechDbSet);
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);

            var import = new SpecificationImport() { LegacyContext = null, NewContext = newContext, Report = null };
            import.CleanDatabase();

            Assert.AreEqual(0, newDbSet.All().Count);
            Assert.AreEqual(1, specTechDbSet.All().Count);
            Assert.AreEqual(1, remarkSet.All().Count);*/
        }


        [Test]
        public void Specification_Test_FillDatabase()
        {
            // New context mock
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            var techSpecSet = new SpecificationTechnologyFakeDBSet();
            var remarkSet = new RemarkFakeDbSet();
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationTechnologies).Return(techSpecSet);
            newContext.Stub(ctx => ctx.Enum_Technology).Return(GetTechnos());
            newContext.Stub(ctx => ctx.Enum_Serie).Return(GetSeries());
            newContext.Stub(ctx => ctx.Remarks).Return(remarkSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G_release_info).Return(GetReleaseInfos_legacy());
            legacyContext.Stub(ctx => ctx.C2001_04_25_schedule).Return(GetSchedule_Legacy());
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());



            // Report
            var report = new Domain.ImportReport();
            // Execute
            var import = new SpecificationImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import.FillDatabase();



            // Tests globals
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

            //REMARKS
            Assert.AreEqual(2, newSpec.Remarks.Count);
        }

        [Test]
        public void Test_FillDatabase_SpecificationResponsibleGroup()
        {
            // --- Clean ---
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationResponsibleGroupFakeDbSet();
            newContext.Stub(ctx => ctx.SpecificationResponsibleGroups).Return(newDbSet);
            /*newDbSet.Add(new Domain.SpecificationResponsibleGroup() { Pk_SpecificationResponsibleGroupId = 1 });
            var import_clean = new SpecificationResponsibleGroupImport() { LegacyContext = null, NewContext = newContext, Report = null };
            import_clean.CleanDatabase();

            Assert.AreEqual(0, newDbSet.All().Count);*/

            // --- Fill datas ---
            // New context mock

            newContext.Stub(ctx => ctx.Specifications).Return(GetSpecs());
            newContext.Stub(ctx => ctx.Communities).Return(GetCommunities());
            
            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new SpecificationLegacyFakeDBSet();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());



            // Report
            var report = new Domain.ImportReport();
            // Execute
            var import_fill = new SpecificationResponsibleGroupImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();


            Assert.AreEqual(3,newDbSet.All().Count);
            Assert.AreEqual(1, newDbSet.All()[0].Fk_commityId);
            Assert.AreEqual(true, newDbSet.All()[0].IsPrime);
            Assert.AreEqual(4, newDbSet.All()[0].Fk_SpecificationId);
            Assert.AreEqual(2, newDbSet.All()[1].Fk_commityId);
            Assert.AreEqual(true, newDbSet.All()[1].IsPrime);
            Assert.AreEqual(4, newDbSet.All()[1].Fk_SpecificationId);
            Assert.AreEqual(3, newDbSet.All()[2].Fk_commityId);
            Assert.AreEqual(false, newDbSet.All()[2].IsPrime);
            Assert.AreEqual(4, newDbSet.All()[2].Fk_SpecificationId);
        }

        [Test]
        public void Test_FillDatabase_SpecificationsGenealogy()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            foreach (var elt in GetSpecs())
            {
                newDbSet.Add(elt);
            }
            // --- Fill datas ---
            // New context mock
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            
            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.C2001_11_06_filius_patris).Return(GetFilius_legacy());



            //Report
            var report = new Domain.ImportReport();
            // Execute
            var import_fill = new SpecificationsGenealogyImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();

            Assert.AreEqual(4, newDbSet.All().Count);
            Assert.AreEqual(1, newDbSet.All()[1].SpecificationParents.Count);
            Assert.AreEqual(1, newDbSet.All()[2].SpecificationChilds.Count);
        }

        [Test]
        public void Test_FillDatabase_SpecificationRapporteur()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            var specRapporteurDbSet = new SpecificationRapporteurFakeDbSet();
            foreach (var elt in GetSpecs())
            {
                newDbSet.Add(elt);
            }
            // --- Fill datas ---
            // New context mock
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationRapporteurs).Return(specRapporteurDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());



            //Report
            var report = new Domain.ImportReport();
            // Execute
            var import_fill = new SpecificationRapporteurImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();

            Assert.AreEqual(4, newDbSet.All().Count);
            Assert.AreEqual(1, specRapporteurDbSet.All().Count);
            Assert.AreEqual(4, specRapporteurDbSet.All()[0].Fk_SpecificationId);
            Assert.AreEqual(26063, specRapporteurDbSet.All()[0].Fk_RapporteurId);
            
        }



        #region New datas
        private IDbSet<Domain.Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Domain.Specification() { Pk_SpecificationId = 1, Number = "01.01",  });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 2, Number = "01.02" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 3, Number = "01.03" });
            list.Add(new Domain.Specification() { Pk_SpecificationId = 4, Number = "02.72" });
            return list;
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

        private IDbSet<Domain.Community> GetCommunities()
        {
            var list = new CommunityFakeDBSet();
            list.Add(new Domain.Community() { TbId = 1, ParentTbId = 100, ShortName = "S1",  TbName = "test", ActiveCode = "ACTIVE" });
            list.Add(new Domain.Community() { TbId = 2, ParentTbId = 200, ShortName = "CD", TbName = "test", ActiveCode = "FINISHED" });
            list.Add(new Domain.Community() { TbId = 3, ParentTbId = 300, ShortName = "CP", TbName = "test", ActiveCode = "FINISHED" });

            return list;
        }
        #endregion



        #region legacy datas
        private IDbSet<C2001_11_06_filius_patris> GetFilius_legacy()
        {
            var list = new FiliusPatrisFakeDBSet();
            list.Add(new C2001_11_06_filius_patris() { Row_id = 1, patrem = "01.03", filius = "01.02" });
            return list;
        }


        private IDbSet<Specs_GSM_3G_release_info> GetReleaseInfos_legacy()
        {
            var list = new ReleaseInfoFakeDBSet();
            list.Add(
                new Specs_GSM_3G_release_info()
                {
                    Row_id = 15,
                    Spec = "02.72",
                    inhibitUpgrade = false
                }
            );
            list.Add(
                new Specs_GSM_3G_release_info()
                {
                    Row_id = 67,
                    Spec = "02.72",
                    inhibitUpgrade = true
                }
            );
            return list;
        }

        private IDbSet<C2001_04_25_schedule> GetSchedule_Legacy(){
            var list = new ScheduleLegacyFakeDBSet();
            list.Add(
                new C2001_04_25_schedule()
                {
                    spec = "02.72",
                    MAJOR_VERSION_NB = 5
                }
            );
            list.Add(
                new C2001_04_25_schedule()
                {
                    spec = "02.72",
                    MAJOR_VERSION_NB = 1
                }
            );
            return list;
        }

        private IDbSet<Specs_GSM_3G> GetSpecs_Legacy()
        {
            var list = new SpecificationLegacyFakeDBSet();
            list.Add(
                new Specs_GSM_3G()
                {
                    Row_id = 8,
                    Type = "TS",
                    Number = "02.72",
                    For_publication = true,
                    Title = "Call Deflection Service description; Stage 1",
                    former_WG = "S1  /  SMG01",
                    WG_prime = "S1 , CD",
                    WG_other = "CP",
                    rapporteur_id = 26063,
                    general_remarks = remark,
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
            return list;
        }
        #endregion

    }


}
