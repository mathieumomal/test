using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service = Etsi.Ultimate.Services;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;
using System.Data.Entity.Validation;

namespace DatabaseImport.ModuleImport
{
    /// <summary>
    /// Generate Specification and SpecificationTechnologies table datas
    /// </summary>
    public class SpecificationImport : IModuleImport
    {
        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G and for IsUnderChangeControlCase : 2001-04-25_schedule
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            foreach (var specification in NewContext.Specifications.ToList())
            {
                var specTech = NewContext.SpecificationTechnologies.Where(r => r.Fk_Specification == specification.Pk_SpecificationId).ToList();
                for (int i = 0; i < specTech.Count; ++i)
                    NewContext.SpecificationTechnologies.Remove(specTech[i]);
                NewContext.Specifications.Remove(specification);
            }
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            NewContext.SaveChanges();
        }

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.Specs_GSM_3G.Count();
            var count = 0;
            foreach (var legacySpec in LegacyContext.Specs_GSM_3G)
            {
                var newSpec = new Domain.Specification();

                IsTsCase(newSpec, legacySpec);

                newSpec.Number = CheckString(legacySpec.Number, 20, "Spec Number", newSpec.Number);

                newSpec.IsActive = NullBooleanCheck(legacySpec.definitively_withdrawn, "IsActive (<=> OLD DefinitivelyWithdrawn)", false);

                IsUnderChangeControlCase(newSpec, legacySpec);

                IsPromoteInhibitedCase(newSpec, legacySpec);

                newSpec.IsForPublication = legacySpec.For_publication;

                newSpec.Title = CheckString(legacySpec.Title, 2000, "Spec Title", newSpec.Number);

                newSpec.ComIMS = legacySpec.ComIMS;

                newSpec.EPS = legacySpec.EPS;

                newSpec.C_2gCommon = legacySpec.C2g_common;

                newSpec.CreationDate = legacySpec.creation_date;

                newSpec.MOD_TS = legacySpec.update_date;

                newSpec.MOD_BY = CheckString(null, 20, "Spec MOD_BY", newSpec.Number);

                newSpec.TitleVerified = legacySpec.title_verified;

                newSpec.URL = URLCase(legacySpec.URL);

                newSpec.ITU_Description = CheckString(legacySpec.description, 1000, "Spec ITU_Description", newSpec.Number);

                SerieCase(newSpec, legacySpec);

                newSpec.Fk_SpecificationStageId = null;//For the moment... QUESTION TO ETSI

                TechnologieCase(newSpec, legacySpec);


                NewContext.Specifications.Add(newSpec);
                count++;
                Console.WriteLine(String.Format("Spec {0}/{1}", count, total));
            }
        }

        /// <summary>
        /// This method attributes the type = true if the old type is "TS" and false for "TR" 
        /// </summary>
        /// <param name="newSpec"></param>
        /// <param name="legacySpec"></param>
        private void IsTsCase(Domain.Specification newSpec, OldDomain.Specs_GSM_3G legacySpec)
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
                    Report.LogError("Specification type is not TS or TR but (" + legacySpec.Type + ").");
                    break;
            }
        }

        private void IsUnderChangeControlCase(Domain.Specification newSpec, OldDomain.Specs_GSM_3G legacySpec)
        {
            var schedules = LegacyContext.C2001_04_25_schedule.Where(x => x.spec.Equals(newSpec.Number)).ToList();
            var isUnderChangeControl = false;
            foreach (var schedule in schedules)
            {
                if (schedule.MAJOR_VERSION_NB > 2)
                    isUnderChangeControl = true;
            }
            if (!isUnderChangeControl)
                //Report.LogWarning("Default value : FALSE, attributed for spec : " + newSpec.Number + " because MAJOR_VERSION_NB > 2 not found.");
            newSpec.IsUnderChangeControl = isUnderChangeControl;
        }

        private void IsPromoteInhibitedCase(Domain.Specification newSpec, OldDomain.Specs_GSM_3G legacySpec)
        {
            var spec_releases = LegacyContext.Specs_GSM_3G_release_info.Where(x => x.Spec.Equals(newSpec.Number)).OrderBy(x => x.Row_id).ToList();
            var isPromoteInhibitedCase = false;
            foreach (var spec_release in spec_releases)
            {
                if (NullBooleanCheck(spec_release.inhibitUpgrade, "IsPromoteInhibited", false))
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
        private void TechnologieCase(Domain.Specification newSpec, OldDomain.Specs_GSM_3G legacySpec)
        {
            //SpecificationTechnologies relation table creation
            if (NullBooleanCheck(legacySpec.C3g, "C3g", false))
            {
                Domain.Enum_Technology enumTechno = NewContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._3gCode)).FirstOrDefault();
                Domain.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                NewContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
            if (NullBooleanCheck(legacySpec.C2g, "C2g", false))
            {
                Domain.Enum_Technology enumTechno = NewContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._2gCode)).FirstOrDefault();
                Domain.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                NewContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
            if (NullBooleanCheck(legacySpec.LTE, "LTE", false))
            {
                Domain.Enum_Technology enumTechno = NewContext.Enum_Technology.Where(x => x.Code.Equals(Enum_TechnologyImport._lteCode)).FirstOrDefault();
                Domain.SpecificationTechnology relationSpecTechno = new SpecificationTechnology()
                {
                    Enum_Technology = enumTechno,
                    Specification = newSpec
                };
                NewContext.SpecificationTechnologies.Add(relationSpecTechno);
                newSpec.SpecificationTechnologies.Add(relationSpecTechno);
            }
        }

        private void SerieCase(Domain.Specification newSpec, OldDomain.Specs_GSM_3G legacySpec)
        {
            var serie = newSpec.Number.Split('.')[0];
            var code = new StringBuilder().Append(Enum_SerieImport.CodePrefixe).Append(serie).ToString();
            var enumSerie = NewContext.Enum_Serie.Where(x => x.Code == code).FirstOrDefault();

            if (enumSerie != null)
            {
                newSpec.Enum_Serie = enumSerie;
                newSpec.Fk_SerieId = enumSerie.Pk_Enum_SerieId;
            }
            else
            {
                Report.LogWarning("Serie not found : " + serie + ", for Spec : " + legacySpec.Number);
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
        /// Convert bool? to bool and delivered a warning message, if the default value is applied, which contains a specific message that we could, partialy, defined (logDescriptionCase)
        /// </summary>
        /// <param name="boo"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        private bool NullBooleanCheck(bool? boo,string logDescriptionCase, bool defaultValue)
        {
            if (!boo.HasValue)
            {
                Report.LogWarning(logDescriptionCase + " not defined as true or false. By default convert to " + defaultValue.ToString() + ".");
                boo = defaultValue;
            }
            return (bool)boo;
        }

        /// <summary>
        /// Check string length + check null + Trim
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lenght"></param>
        /// <param name="logDescriptionCase"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private string CheckString(string str, int lenght, string logDescriptionCase, string id)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
            }
            else if (str.Length > lenght)
            {
                Report.LogWarning(logDescriptionCase + " : string too long for : " + id);
                return "";
            }
            return str.Trim();
        }
        #endregion
    }
}
