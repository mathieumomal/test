using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class WorkItemServiceMock : IWorkItemService
    {
        #region IWorkItemService Membres

        public KeyValuePair<string, ImportReport> AnalyseWorkPlanForImport(String path)
        {
            ImportReport myImportReport = new ImportReport();
            myImportReport.ErrorList.Add("Error 1");
            myImportReport.ErrorList.Add("Error 2");
            myImportReport.ErrorList.Add("Error 3");
            myImportReport.WarningList.Add("Warning 1");
            myImportReport.WarningList.Add("Warning 2");
            myImportReport.WarningList.Add("Warning 3");
            var analyseWorkItemForImportResult = new KeyValuePair<string, ImportReport>("15", myImportReport);
            return analyseWorkItemForImportResult;
        }

        public bool ImportWorkPlan(string token, string exportPath)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId)
        {
            throw new NotImplementedException();
        }

        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllAcronyms()
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemByIdExtend(int personId, int workItemId)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetAllWorkItems(int personId)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
