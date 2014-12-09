using System;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    
    /// <summary>
    /// Generate Specification and SpecificationTechnologies table datas
    /// </summary>
    public class SpecificationImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification]";
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G and for IsUnderChangeControlCase : 2001-04-25_schedule
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            UltimateContext.Specifications_CleanAll();
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            UltimateContext.SaveChanges();
        }

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.Specs_GSM_3G.Count();
            var count = 0;
            foreach (var legacySpec in LegacyContext.Specs_GSM_3G)
            {
                var newSpec = new Etsi.Ultimate.DomainClasses.Specification();

                IsTsCase(newSpec, legacySpec);

                newSpec.Number = Utils.CheckString(legacySpec.Number, 20, RefImportForLog + " Number", newSpec.Number);

                newSpec.IsActive = !Utils.NullBooleanCheck(legacySpec.definitively_withdrawn, RefImportForLog + "IsActive (<=> OLD DefinitivelyWithdrawn)", false);

                IsUnderChangeControlCase(newSpec, legacySpec);

                IsPromoteInhibitedCase(newSpec, legacySpec);

                newSpec.IsForPublication = legacySpec.For_publication;

                newSpec.Title = Utils.CheckString(legacySpec.Title, 2000, RefImportForLog + " Title", newSpec.Number);

                newSpec.ComIMS = legacySpec.ComIMS;

                newSpec.EPS = legacySpec.EPS;

                newSpec.C_2gCommon = legacySpec.C2g_common;

                newSpec.CreationDate = legacySpec.creation_date;

                newSpec.MOD_TS = legacySpec.update_date;

                newSpec.MOD_BY = Utils.CheckString(null, 20, RefImportForLog + " MOD_BY", newSpec.Number);

                newSpec.TitleVerified = legacySpec.title_verified;

                newSpec.URL = URLCase(legacySpec.URL);

                newSpec.ITU_Description = Utils.CheckString(legacySpec.description, 1000, RefImportForLog + " ITU_Description", newSpec.Number);

                SerieCase(newSpec, legacySpec);

                TechnologieCase(newSpec, legacySpec);

                RemarksCase(newSpec, legacySpec);

                UltimateContext.Specifications.Add(newSpec);
                count++;
                if(count%100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }

        /// <summary>
        /// This method attributes the type = true if the old type is "TS" and false for "TR" 
        /// </summary>
        /// <param name="newSpec"></param>
        /// <param name="legacySpec"></param>
        private void IsTsCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            switch (legacySpec.Type)
            {
                case "TS":
                    newSpec.IsTS = true;
                    break;
                case "TR":
                    newSpec.IsTS = false;
                    break;
                default:
                    newSpec.IsTS = true;
                    LogManager.LogWarning(RefImportForLog + " type is not TS or TR but (" + legacySpec.Type + ") (We applied TS by default).");
                    break;
            }
        }

        private void IsUnderChangeControlCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            var schedules = LegacyContext.C2001_04_25_schedule.Where(x => x.spec == newSpec.Number && x.MAJOR_VERSION_NB > 2).FirstOrDefault();
            newSpec.IsUnderChangeControl = (schedules != null);
        }

        private void IsPromoteInhibitedCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            var spec_releases = LegacyContext.Specs_GSM_3G_release_info.Where(x => x.Spec == newSpec.Number).OrderBy(x => x.Row_id).ToList();
            var isPromoteInhibitedCase = false;
            foreach (var spec_release in spec_releases)
            {
                if (Utils.NullBooleanCheck(spec_release.inhibitUpgrade, RefImportForLog + "IsPromoteInhibited", false))
                    isPromoteInhibitedCase = true;
                else
                    isPromoteInhibitedCase = false;
            }
            newSpec.promoteInhibited = isPromoteInhibitedCase;
        }

        /// <summary>
        /// This method creates the relation datas in Specification_technology table between Specs and 3g/2g/lte
        /// </summary>
        /// <param name="newSpec"></param>
        /// <param name="legacySpec"></param>
        private void TechnologieCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            //SpecificationTechnologies relation table creation
            if (Utils.NullBooleanCheck(legacySpec.C2g, RefImportForLog + "C2g", false))
            {
                Etsi.Ultimate.DomainClasses.Enum_Technology enumTechno = UltimateContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._2gCode)).FirstOrDefault();
                Etsi.Ultimate.DomainClasses.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                UltimateContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
            if (Utils.NullBooleanCheck(legacySpec.C3g, RefImportForLog + "C3g", false))
            {
                Etsi.Ultimate.DomainClasses.Enum_Technology enumTechno = UltimateContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._3gCode)).FirstOrDefault();
                Etsi.Ultimate.DomainClasses.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                UltimateContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
            if (Utils.NullBooleanCheck(legacySpec.LTE, RefImportForLog + "LTE", false))
            {
                Etsi.Ultimate.DomainClasses.Enum_Technology enumTechno = UltimateContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._lteCode)).FirstOrDefault();
                Etsi.Ultimate.DomainClasses.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                UltimateContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
        }

        private void SerieCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            var serie = newSpec.Number.Split('.')[0];
            var code = new StringBuilder().Append(Enum_SerieImport.CodePrefixe).Append(serie).ToString();
            var enumSerie = UltimateContext.Enum_Serie.Where(x => x.Code == code).FirstOrDefault();

            if (enumSerie != null)
            {
                newSpec.Enum_Serie = enumSerie;
                newSpec.Fk_SerieId = enumSerie.Pk_Enum_SerieId;
            }
            else
            {
                LogManager.LogWarning(RefImportForLog + "Serie not found : " + serie + " for Spec : " + legacySpec.Number);
            }
        }

        /// <summary>
        /// Format URL From #URL# to URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private string URLCase(string URL)
        {
            return URL.Replace("#", "");
        }

        /// <summary>
        /// Handled specification remarks
        /// </summary>
        /// <param name="newSpec"></param>
        /// <param name="legacySpec"></param>
        private void RemarksCase(Etsi.Ultimate.DomainClasses.Specification newSpec, Etsi.Ultimate.Tools.TmpDbDataAccess.Specs_GSM_3G legacySpec)
        {
            var remarksField = Utils.CheckString(legacySpec.general_remarks, 1000, RefImportForLog + " remarks text", newSpec.Number);
            
            if (!String.IsNullOrEmpty(remarksField))
            {
                var rmk = new Remark()
                {
                    CreationDate = DateTime.Now,
                    IsPublic = true,
                    RemarkText = remarksField,
                    Fk_SpecificationId = newSpec.Pk_SpecificationId
                };
                newSpec.Remarks.Add(rmk);
            }
        }

        #endregion
    }
}
