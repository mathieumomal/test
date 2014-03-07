using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class WorkItemImporter
    {
        private static readonly string CACHE_KEY = "WI_IMPORT_";

        public IUltimateUnitOfWork UoW { get; set; }

        public KeyValuePair<string, ImportReport> TryImportCsv(string filePath)
        {
            string token = "";

            var csvParser = new WorkItemCsvParser();
            csvParser.UoW = UoW ;
            var result = csvParser.ParseCsv(filePath);
            if (result.Value.GetNumberOfErrors() == 0)
            {
                token = new Guid().ToString();
                CacheManager.InsertForLimitedTime(CACHE_KEY+token, result.Key, 10);
            }
            return new KeyValuePair<string, ImportReport>(token, result.Value);

        }

        public bool ImportWorkPlan(string token)
        {
            // Fetch the data in cache. If there is no data, then it's outdated.
            var workPlan = (List<WorkItem>)CacheManager.Get(token);
            if (workPlan == null)
            {
                return false;
            }

            // Else, call the repository
            var wiRepo = RepositoryFactory.Resolve<IWorkItemRepository>();
            wiRepo.UoW = UoW;

            foreach (var wi in workPlan)
            {
                wiRepo.InsertOrUpdate(wi);
            }

            return true;
        }
    }
}
