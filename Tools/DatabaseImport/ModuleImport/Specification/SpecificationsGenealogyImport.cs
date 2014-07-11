using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace DatabaseImport.ModuleImport
{
    public class SpecificationsGenealogyImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/Genealogy]";
        /// <summary>
        /// Old table(s) : 
        /// 2001-11-06_filius-patris
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

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
                            Report.LogWarning(RefImportForLog + " Spec : " + legacyGenealogie.filius + "not found.");
                        if(parent == null)
                            Report.LogWarning(RefImportForLog + " Spec : " + legacyGenealogie.patrem + "not found.");
                    }
                }
                else
                {
                    countTooSmallNumber++;
                }
            }
            Report.LogWarning(RefImportForLog + "!!! TOTAL !!! Spec Number not valid (lgth<4 or null): " + countTooSmallNumber + ".");
        }
        #endregion
    }
}
