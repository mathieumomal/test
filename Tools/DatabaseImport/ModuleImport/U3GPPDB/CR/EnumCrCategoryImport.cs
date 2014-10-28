using System;
using System.Data.Entity.Infrastructure;

namespace DatabaseImport.ModuleImport.U3GPPDB.CR
{
    public class EnumCrCategoryImport : IModuleImport
    {
        private const string RefImportForLog = "[ENUM_CRCategory]";

        /// <summary>
        /// Old table(s) : 
        /// CR-categories
        /// </summary>
        #region IModuleImport Membres
        public Etsi.Ultimate.DataAccess.IUltimateContext UltimateContext { get; set; }
        public Etsi.Ngppdb.DataAccess.INGPPDBContext NgppdbContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public MeetingHelper MtgHelper { get; set; }

        public void CleanDatabase()
        {
            UltimateContext.CR_CleanAll();
        }

        public void FillDatabase()
        {
            CreateTable();
            try
            {
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
        private void CreateTable()
        {
            foreach (var elt in LegacyContext.CR_categories)
            {
                var newCrCategory = new Etsi.Ultimate.DomainClasses.Enum_CRCategory
                {
                    Code = elt.CR_category,
                    Description =
                        Utils.CheckString(elt.meaning, 200, RefImportForLog + " category", elt.CR_category)
                };

                UltimateContext.Enum_CRCategory.Add(newCrCategory);
            }
        }
        #endregion
    }
}
