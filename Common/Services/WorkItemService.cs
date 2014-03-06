using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Services
{
    public class WorkItemService : IWorkItemService
    {

        #region IWorkItemService Membres

        public KeyValuePair<int, ImportReport> AnalyseWorkItemForImport(String path)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvImport = new WorkItemImporter() { UoW = uoW };
                return csvImport.TryImportCsv(path);
            }
        }

        public string ImportWorkItem(int token)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
