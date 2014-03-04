using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class WorkItemService : IWorkItemService
    {

        #region IWorkItemService Membres

        public KeyValuePair<int, ImportReport> AnalyseWorkItemForImport(String path)
        {
            throw new NotImplementedException();
        }

        public string ImportWorkItem(int token)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
