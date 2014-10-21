using System;
using System.Data.Entity.Infrastructure;

namespace DatabaseImport.ModuleImport.CR
{
    public class EnumCrCategoryImport : IModuleImport
    {
        private const string RefImportForLog = "[ENUM_CRCategory]";

        /// <summary>
        /// Old table(s) : 
        /// CR-categories
        /// </summary>
        #region IModuleImport Membres
        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase()
        {
            NewContext.CR_CleanAll();
        }

        public void FillDatabase()
        {
            CreateTable();
            try
            {
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
        private void CreateTable()
        {
            foreach (var elt in LegacyContext.CR_categories)
            {
                var newCrCategory = new Etsi.Ultimate.DomainClasses.Enum_CRCategory
                {
                    Code = elt.CR_category,
                    Description =
                        Utils.CheckString(elt.meaning, 200, RefImportForLog + " category", elt.CR_category, Report)
                };

                NewContext.Enum_CRCategory.Add(newCrCategory);
            }
        }
        #endregion
    }
}
