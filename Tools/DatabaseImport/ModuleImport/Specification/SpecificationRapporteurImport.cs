﻿using System;
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
using System.Data.Entity.Infrastructure;


namespace DatabaseImport.ModuleImport
{
    public class SpecificationRapporteurImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/Rapporteur]";
        #region IModuleImport Membres

        /// <summary>
        /// Old table(s) : 
        /// Specs_GSM+3G and for IsUnderChangeControlCase : 2001-04-25_schedule
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
        }

        public void FillDatabase()
        {
            NewContext.SetAutoDetectChanges(false);
            CreateDatas();
            NewContext.SetAutoDetectChanges(true);
            try{
                NewContext.SaveChanges();
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
                    var spec = NewContext.Specifications.Where(x => x.Number == legacySpecifification.Number).FirstOrDefault();
                    var person = NewContext.View_Persons.Where(x => x.PERSON_ID == legacySpecifification.rapporteur_id).FirstOrDefault();
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
                        NewContext.SpecificationRapporteurs.Add(specRapporteur);
                    }
                }

                
                count++;
                Console.Write(String.Format("\r" + RefImportForLog + " {0}/{1}  ", count, total));
            }
        }
        #endregion

    }
}
