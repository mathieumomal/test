using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public class WorkItemImporter
    {

        public IUltimateUnitOfWork UoW { get; set; }

        public KeyValuePair<int, ImportReport> TryImportCsv(string filePath)
        {
            int token = 1;

            var csvParser = new WorkItemCsvParser();
            csvParser.UoW = UoW ;
            var result = csvParser.ParseCsv(filePath);

            return new KeyValuePair<int, ImportReport>(token, result.Value);

        }
    }
}
