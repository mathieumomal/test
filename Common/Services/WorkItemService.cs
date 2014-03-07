using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Services
{
    public class WorkItemService : IWorkItemService
    {

        #region IWorkItemService Membres

        public KeyValuePair<string, ImportReport> AnalyseWorkPlanForImport(String path)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvImport = new WorkItemImporter() { UoW = uoW };
                return csvImport.TryImportCsv(path);
            }
        }

        public bool ImportWorkPlan(string token)
        {
            bool isImportSuccess = false;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var csvImport = new WorkItemImporter() { UoW = uoW };
                    isImportSuccess = csvImport.ImportWorkPlan(token);

                    if (isImportSuccess)
                    {
                        uoW.Save();
                    }
                    return isImportSuccess;                    
                }
            }
            catch (Exception e)
            {
                LogManager.Error("Failed to import Workplan: " + e.Message+"\n"+e.StackTrace);
                return false;
            }

        }

        #endregion
    }
}
