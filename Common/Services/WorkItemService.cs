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
            bool isExportSuccess = false;
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var csvImport = new WorkItemImporter() { UoW = uoW };
                    isImportSuccess = csvImport.ImportWorkPlan(token);

                    if (isImportSuccess)
                    {
                        uoW.Save();
                        var csvExport = new WorkPlanExporter(uoW);
                        isExportSuccess = csvExport.ExportWorkPlan(exportPath);                        
                    }
                                     
                }                
                return (isImportSuccess && isExportSuccess);   
            }
            catch (Exception e)
            {
                LogManager.Error("Failed to import Workplan: " + e.Message+"\n"+e.StackTrace);
                Exception e2 = e;
                while (e2 != null)
                {
                    if (e2.InnerException != null)
                    {
                        LogManager.Error("Workplan import innerException: "+ e2.InnerException.Message);
                        e2 = e2.InnerException;
                    }
                    else
                    {
                        break;
                    }
                }
                return false;
            }

        }

        /// <summary>
        /// Get the list of workitems based on the release
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="releaseIds">Release Ids</param>
        /// <param name="granularity">Granularity Level</param>
        /// <param name="hidePercentComplete">Percentage Complete</param>
        /// <param name="wiAcronym">Acronym</param>
        /// <param name="wiName">Name</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsBySearchCriteria(personId, releaseIds, granularity, hidePercentComplete, wiAcronym, wiName);
            }
        }

        /// <summary>
        /// Get all the list workitems
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetAllWorkItems(int personId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetAllWorkItems(personId);
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
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item, right container and other required properties</returns>
        public KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemByIdExtend(int personId, int workItemId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemByIdExtend(personId, workItemId);
            }
        }

        /// <summary>
        /// Get count of WorkItems
        /// </summary>
        /// <param name="releaseIds">List of Release Ids</param>
        /// <param name="granularity">Granularity Level</param>
        /// <param name="hidePercentComplete">Percentage Complete</param>
        /// <param name="wiAcronym">Acronym</param>
        /// <param name="wiName">Name</param>
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsCountBySearchCriteria(releaseIds, granularity, hidePercentComplete, wiAcronym, wiName);
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
