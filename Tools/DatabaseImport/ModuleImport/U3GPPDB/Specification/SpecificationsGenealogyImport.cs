using System.Linq;

namespace DatabaseImport.ModuleImport.U3GPPDB.Specification
{
    public class SpecificationsGenealogyImport : IModuleImport
    {
        public const string RefImportForLog = "[Specification/Genealogy]";
        /// <summary>
        /// Old table(s) : 
        /// 2001-11-06_filius-patris
        /// </summary>

        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase()
        {
            //Procedure does this work
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
            var countTooSmallNumber = 0;
            foreach (var legacyGenealogie in LegacyContext.C2001_11_06_filius_patris)
            {
                if (legacyGenealogie.patrem != null && legacyGenealogie.filius != null && legacyGenealogie.patrem.Length >= 4 && legacyGenealogie.filius.Length >= 4)
                {
                    var parent = UltimateContext.Specifications.Where(x => x.Number == legacyGenealogie.patrem).FirstOrDefault();
                    var child = UltimateContext.Specifications.Where(x => x.Number == legacyGenealogie.filius).FirstOrDefault();

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
