using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Domain = Etsi.Ultimate.DomainClasses;
using OldDomain = Etsi.Ultimate.Tools.TmpDbDataAccess;


namespace DatabaseImport.ModuleImport
{
    public class Enum_CRCategoryImport : IModuleImport
    {
        public const string RefImportForLog = "[ENUM_CRCategory]";

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
                var newCRCategory = new Domain.Enum_CRCategory();

                newCRCategory.Code = elt.CR_category;
                newCRCategory.Description = Utils.CheckString(new StringBuilder().Append(elt.CR_category).Append(" - ").Append(elt.meaning).ToString(), 200, RefImportForLog + " category", elt.CR_category, Report);

                NewContext.Enum_CRCategory.Add(newCRCategory);
            }
        }
        #endregion
    }
}
