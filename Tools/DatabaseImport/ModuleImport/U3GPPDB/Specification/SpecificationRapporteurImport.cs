using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    public class SpecificationRapporteurImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/Rapporteur]";

        #region IModuleImport Membres

        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G and for IsUnderChangeControlCase : 2001-04-25_schedule
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            UltimateContext.SetAutoDetectChanges(false);
            CreateDatas();
            UltimateContext.SetAutoDetectChanges(true);
            try{
                UltimateContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var test = ex;
                Console.WriteLine(ex.InnerException);
                Console.ReadLine();
            }
        }

        #endregion

        #region migration methods
        private void CreateDatas()
        {
            var total = LegacyContext.Specs_GSM_3G.Count();
            var count = 0;
            foreach (var legacySpecifification in LegacyContext.Specs_GSM_3G)
            {
                var specRapporteur = new SpecificationRapporteur();

                if (legacySpecifification.rapporteur_id != null)
                {
                    var spec = UltimateContext.Specifications.Where(x => x.Number == legacySpecifification.Number).FirstOrDefault();
                    var person = UltimateContext.View_Persons.Where(x => x.PERSON_ID == legacySpecifification.rapporteur_id).FirstOrDefault();
                    if (spec == null)
                    {
                        Report.LogWarning(RefImportForLog + " Spec : " + legacySpecifification.Number + " not found.");
                    }
                    else if (person == null)
                    {
                        Report.LogWarning(RefImportForLog + " Rapporteur id : " + legacySpecifification.rapporteur_id + " not found.");
                    }
                    else
                    {
                        specRapporteur.IsPrime = true;
                        specRapporteur.Fk_SpecificationId = spec.Pk_SpecificationId;
                        specRapporteur.Fk_RapporteurId = Utils.CheckInt(legacySpecifification.rapporteur_id, RefImportForLog + "Rapporteur id undefined", spec.Number, Report);
                        UltimateContext.SpecificationRapporteurs.Add(specRapporteur);
                    }
                }

                
                count++;
                if (count % 100 == 0)
                    Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }
        #endregion

    }
}
