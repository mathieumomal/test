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

namespace DatabaseImport.ModuleImport
{
    public class SpecificationsGenealogyImport : IModuleImport
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
            //Procedure does this work
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
            var countTooSmallNumber = 0;
            foreach (var legacyGenealogie in LegacyContext.C2001_11_06_filius_patris)
            {
                if (legacyGenealogie.patrem != null && legacyGenealogie.filius != null && legacyGenealogie.patrem.Length >= 4 && legacyGenealogie.filius.Length >= 4)
                {
                    var parent = NewContext.Specifications.Where(x => x.Number == legacyGenealogie.patrem).FirstOrDefault();
                    var child = NewContext.Specifications.Where(x => x.Number == legacyGenealogie.filius).FirstOrDefault();

                    if (child != null && parent != null)
                    {
                        parent.SpecificationChilds.Add(child);
                        child.SpecificationParents.Add(parent);
                    }
                    else
                    {
                        if(child == null)
                            Report.LogWarning("[Spec Genealogy] Spec : " + legacyGenealogie.filius + "not found.");
                        if(parent == null)
                            Report.LogWarning("[Spec Genealogy] Spec : " + legacyGenealogie.patrem + "not found.");
                    }
                }
                else
                {
                    countTooSmallNumber++;
                }
            }
            Report.LogWarning("[Spec Genealogy] TOTAL Spec Number not valid (lgth<4 or null): " + countTooSmallNumber + ".");
        }
        #endregion
    }
}
