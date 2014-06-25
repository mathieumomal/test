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
    public class Enum_TDocStatusImport : IModuleImport
    {
        public const string RefImportForLog = "[ENUM_TDocStatus]";

        /// <summary>
        /// Old table(s) : 
        /// CR-status-values
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
            foreach (var elt in LegacyContext.CR_status_values)
            {
                var status = Utils.CheckString(elt.CR_status_value, 50, RefImportForLog + "", elt.CR_status_value, Report);
                if (!status.Equals("-"))
                {
                    var newTDocStatus = new Domain.Enum_TDocStatus();

                    newTDocStatus.Code = status;
                    newTDocStatus.SortOrder = Utils.CheckInt(elt.sort_order, " tdoc status (sortOrder)", elt.CR_status_value, Report);
                    newTDocStatus.Description = Utils.CheckString(new StringBuilder().Append(status).Append(" - ").Append(elt.use).ToString(), 200, RefImportForLog + " tdoc status (status)", elt.CR_status_value, Report);
                    newTDocStatus.TSGUsable = Utils.NullBooleanCheck(elt.TSG, " tdoc status (TSG Usable) -> FALSE BY DEFAULT", false, Report);
                    newTDocStatus.WGUsable = Utils.NullBooleanCheck(elt.WG, " tdoc status (TSG Usable) -> FALSE BY DEFAULT", false, Report);

                    NewContext.Enum_TDocStatus.Add(newTDocStatus);
                }
            }
        }
        #endregion
    }
}
