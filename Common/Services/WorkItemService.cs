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

        /// <summary>
        /// Performs an analysis of the work plan in csv format. Unzips the work plan if necessary.
        /// 
        /// Throws FileNotFoundException if file cannot be found.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public KeyValuePair<string, ImportReport> AnalyseWorkPlanForImport(String path)
        {
           
            
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvImport = new WorkItemImporter() { UoW = uoW };
                return csvImport.TryImportCsv(path);
            }
        }

        public bool ImportWorkPlan(string token, string exportPath)
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
                        csvImport.ExportWorkPlan(exportPath);
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

        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="releaseIds">Release Ids</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsByRelease(int personId, List<int> releaseIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                 return workItemManager.GetWorkItemsByRelease(personId, releaseIds);
            }
        }

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemById(personId, workItemId);
            }
        }

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountByRelease(List<int> releaseIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsCountByRelease(releaseIds);
            }
        }

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        public List<string> GetAllAcronyms()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetAllAcronyms();
            }
        }
        
        #endregion

    }
}
