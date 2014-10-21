using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace DatabaseImport.ModuleImport.CR
{
    public class EnumCrImpactImport : IModuleImport
    {
        public const string RefImportForLog = "[ENUM_CRImpact]";

        private readonly List<string> _impacts = new List<string>{
            "UICS Apps",
            "ME",
            "Radio Access Network",
            "Core Network"
        };

        /// <summary>
        /// Old table(s) : 
        /// NO EXIST (MANUALLY CREATED)
        /// </summary>
        #region IModuleImport Membres

        public Etsi.Ultimate.DataAccess.IUltimateContext NewContext { get; set; }
        public Etsi.Ultimate.Tools.TmpDbDataAccess.ITmpDb LegacyContext { get; set; }
        public Etsi.Ultimate.DomainClasses.Report Report { get; set; }

        public void CleanDatabase() { }

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
            foreach (var impact in _impacts)
            {
                var newCrImpact = new Etsi.Ultimate.DomainClasses.Enum_CRImpact
                {
                    Code = impact,
                    Description = new StringBuilder().Append(impact).Append(" - ").ToString()
                };
                NewContext.Enum_CRImpact.Add(newCrImpact);
            }
        }
        #endregion
    }
}
