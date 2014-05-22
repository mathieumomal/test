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
        #region Values to test
        public DateTime updateDate = DateTime.Now;
        public DateTime createDate = DateTime.Now.AddDays(-1);
        public const string url = "#http://www.3gpp.org/ftp/Specs/archive/02_series/02.72/#";
        public const string remark = "test remarks. test remarks 2...";
        #endregion

        [Test]
        public void Specification_Test_Clean(){}

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
            var report = new Domain.Report();
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
            Assert.AreEqual(1, newSpec.Remarks.Count);
        }

        [Test]
        public void Test_FillDatabase_SpecificationResponsibleGroup()
        {
            // --- Clean ---
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationResponsibleGroupFakeDbSet();
            newContext.Stub(ctx => ctx.SpecificationResponsibleGroups).Return(newDbSet);

            // --- Fill datas ---
            // New context mock

            newContext.Stub(ctx => ctx.Specifications).Return(GetSpecs());
            newContext.Stub(ctx => ctx.Communities).Return(GetCommunities());
            
            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            var legacyDbSet = new SpecificationLegacyFakeDBSet();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());

            // Report
            var report = new Domain.Report();
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
            var report = new Domain.Report();
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
            var personDbSet = new ViewPersonFakeDBSet();
            foreach (var elt in GetSpecs())
            {
                newDbSet.Add(elt);
            }
            // --- Fill datas ---
            // New context mock
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.SpecificationRapporteurs).Return(specRapporteurDbSet);
            newContext.Stub(ctx => ctx.View_Persons).Return(GetPersons());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());



            //Report
            var report = new Domain.Report();
            // Execute
            var import_fill = new SpecificationRapporteurImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();

            Assert.AreEqual(4, newDbSet.All().Count);
            Assert.AreEqual(1, specRapporteurDbSet.All().Count);
            Assert.AreEqual(4, specRapporteurDbSet.All()[0].Fk_SpecificationId);
            Assert.AreEqual(26063, specRapporteurDbSet.All()[0].Fk_RapporteurId);
            
        }

        [Test]
        public void Test_FillDatabase_SpecificationWorkItem()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            var specWorkItemDbSet = new SpecificationWorkItemFakeDBSet();
            foreach (var elt in GetSpecs())
            {
                newDbSet.Add(elt);
            }
            // --- Fill datas ---
            // New context mock
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.Specification_WorkItem).Return(specWorkItemDbSet);
            newContext.Stub(ctx => ctx.WorkItems).Return(GetWorkItems());

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G).Return(GetSpecs_Legacy());
            legacyContext.Stub(ctx => ctx.C2008_03_08_Specs_vs_WIs).Return(GetSpecWorkItem_legacy());



            //Report
            var report = new Domain.Report();
            // Execute
            var import_fill = new SpecificationWorkitemImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();

            Assert.AreEqual(1, specWorkItemDbSet.All().Count);
            Assert.AreEqual(true, specWorkItemDbSet.All()[0].isPrime);
        }

        [Test]
        public void Test_FillDatabase_SpecificationRelease()
        {
            var newContext = MockRepository.GenerateMock<IUltimateContext>();
            var newDbSet = new SpecificationFakeDBSet();
            var releaseDbSet = new ReleaseFakeDBSet();
            var specReleaseDbSet = new SpecificationReleaseFakeDBSet();
            var meetingsDbSet = new MeetingFakeDBSet();
            foreach (var elt in GetSpecs())
                newDbSet.Add(elt);
            foreach (var elt in GetReleases())
                releaseDbSet.Add(elt);
            foreach (var elt in GetMeetings())
                meetingsDbSet.Add(elt);
            // --- Fill datas ---
            // New context mock
            newContext.Stub(ctx => ctx.Specifications).Return(newDbSet);
            newContext.Stub(ctx => ctx.Releases).Return(releaseDbSet);
            newContext.Stub(ctx => ctx.Specification_Release).Return(specReleaseDbSet);
            newContext.Stub(ctx => ctx.Meetings).Return(meetingsDbSet);

            // Legacy context mock
            var legacyContext = MockRepository.GenerateMock<ITmpDb>();
            legacyContext.Stub(ctx => ctx.Specs_GSM_3G_release_info).Return(GetReleaseInfos_legacy());


            //Report
            var report = new Domain.Report();
            // Execute
            var import_fill = new SpecificationReleaseImport() { LegacyContext = legacyContext, NewContext = newContext, Report = report };
            import_fill.FillDatabase();

            Assert.AreEqual(1, specReleaseDbSet.All().Count);

            Assert.AreEqual(45, specReleaseDbSet.All()[0].Fk_ReleaseId);
            Assert.AreEqual(4, specReleaseDbSet.All()[0].Fk_SpecificationId);
            Assert.AreEqual(true, specReleaseDbSet.All()[0].isWithdrawn);
            Assert.AreEqual(createDate, specReleaseDbSet.All()[0].CreationDate);
            Assert.AreEqual(updateDate, specReleaseDbSet.All()[0].UpdateDate);
            Assert.AreEqual(23, specReleaseDbSet.All()[0].WithdrawMeetingId);
            //Assert.AreEqual(true, specWorkItemDbSet.All()[0].isPrime);
        }






        #region New datas
        private IDbSet<Domain.Specification> GetSpecs()
        {
            var list = new SpecificationFakeDBSet();
            list.Add(new Domain.Specification() { Pk_SpecificationId = 1, Number = "01.01" });
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

        private IDbSet<Domain.View_Persons> GetPersons()
        {
            var list = new ViewPersonFakeDBSet();
            list.Add(new Domain.View_Persons() { PERSON_ID = 26063 });
            //list.Add(new Domain.View_Persons() { PERSON_ID = 26064 });

            return list;
        }

        private IDbSet<Domain.WorkItem> GetWorkItems()
        {
            var list = new WorkItemFakeDBSet();
            list.Add(new Domain.WorkItem() { Pk_WorkItemUid = 14012 });
            list.Add(new Domain.WorkItem() { Pk_WorkItemUid = 14013 });
            list.Add(new Domain.WorkItem() { Pk_WorkItemUid = 14045 });
            return list;
        }

        private IDbSet<Domain.Release> GetReleases()
        {
            var list = new ReleaseFakeDBSet();
            list.Add(new Domain.Release() { Pk_ReleaseId = 1, Code = "Ph2" });
            list.Add(new Domain.Release() { Pk_ReleaseId = 2, Code = "Ph1" });
            list.Add(new Domain.Release() { Pk_ReleaseId = 45, Code = "R98" });
            return list;
        }
        private IDbSet<Domain.Meeting> GetMeetings()
        {
            var list = new MeetingFakeDBSet();
            list.Add(new Domain.Meeting() { MTG_ID = 23, MtgShortRef = "ABC" });
            list.Add(new Domain.Meeting() { MTG_ID = 67, MtgShortRef = "ABCD" });
            list.Add(new Domain.Meeting() { MTG_ID = 230, MtgShortRef = "EFG" });
            list.Add(new Domain.Meeting() { MTG_ID = 15, MtgShortRef = "EFGH" });
            return list;
        }

        #endregion



        #region legacy datas
        private IDbSet<C2008_03_08_Specs_vs_WIs> GetSpecWorkItem_legacy()
        {
            var list = new SpecificationWILegacyFakeDBSet();
            list.Add(new C2008_03_08_Specs_vs_WIs() { Row_id = 1, Spec = "02.72", WI_UID = 14012 });
            list.Add(new C2008_03_08_Specs_vs_WIs() { Row_id = 2, Spec = "01.01", WI_UID = 14013 });
            list.Add(new C2008_03_08_Specs_vs_WIs() { Row_id = 3, Spec = "02.02", WI_UID = 14014 });

            return list;
        }

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
                    inhibitUpgrade = false,
                    Release = "Ph24",
                    withdrawn = true,
                    update_date = updateDate,
                    creation_date = createDate,
                    freeze_meeting = "EFG",
                    stopped_at_meeting = "EFGH"
                }
            );
            list.Add(
                new Specs_GSM_3G_release_info()
                {
                    Row_id = 67,
                    Spec = "02.72",
                    inhibitUpgrade = true,
                    Release = "R98",
                    withdrawn = true,
                    update_date = updateDate,
                    creation_date = createDate,
                    freeze_meeting = "ABCD",
                    stopped_at_meeting = "ABC"
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
                    WI_UID = 14012,
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
