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

namespace DatabaseImport.ModuleImport.Specification
{
    public class SpecificationsGenealogy : IModuleImport
    {

        /// <summary>
        /// Old table(s) : 
        /// 2001-11-06_filius-patris
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.ImportReport Report { get; set; }

        public void CleanDatabase()
        {
            foreach (var specification in NewContext.Specifications.ToList())
            {
                //var specTech = NewContext.SpecificationTechnologies.Where(r => r.Fk_Specification == specification.Pk_SpecificationId).ToList();
                //for (int i = 0; i < specTech.Count; ++i)
                //    NewContext.SpecificationTechnologies.Remove(specTech[i]);
                //NewContext.Specifications.Remove(specification);
            }
        }

        public void FillDatabase()
        {
            CreateDatas();
            NewContext.SaveChanges();
        }

        #region migration methods
        private void CreateDatas()
        {
            foreach (var legacyGenealogie in LegacyContext.C2001_11_06_filius_patris)
            {
                var parent = NewContext.Specifications.Where(x => x.Number == legacyGenealogie.patrem).FirstOrDefault();
                if (parent != null)
                {
                    //Fonctionnement de l'ancienne table ?????
                }


                

                //NewContext.Specifications.Add(newSpec);
            }
        }
        #endregion
    }
}
