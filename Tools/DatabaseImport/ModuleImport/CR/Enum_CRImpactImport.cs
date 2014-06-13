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
    public class Enum_CRImpactImport : IModuleImport
    {
        public const string RefImportForLog = "[ENUM_CRImpact]";

        public List<string> Impacts = new List<string>(){
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
            foreach (var impact in Impacts)
            {
                var newCRImpact = new Domain.Enum_CRImpact();
                newCRImpact.Impact = impact;
                NewContext.Enum_CRImpact.Add(newCRImpact);
            }
        }
        #endregion
    }
}
