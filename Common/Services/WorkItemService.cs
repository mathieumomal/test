using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

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
        public KeyValuePair<string, Report> AnalyseWorkPlanForImport(String path)
        {

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvImport = new WorkItemImporter() { UoW = uoW };
                return csvImport.TryImportCsv(path);
            }
        }

        /// <summary>
        /// Import workitems uploaded on the server
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="exportPath">Export Path</param>
        /// <returns>Success/Failure</returns>
        public bool ImportWorkPlan(string token, string exportPath)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var wiImporter = ManagerFactory.Resolve<IWorkItemImporter>();
                    wiImporter.UoW = uoW;
                    var workplanExporter = ManagerFactory.Resolve<IWorkPlanExporter>();
                    workplanExporter.UoW = uoW;

                    var isImportSuccess = wiImporter.ImportWorkPlan(token);

                    if (isImportSuccess)
                    {
                        uoW.Save();
                        if (ConfigVariables.ActivateWorkPlanExportAfterImport)
                        {                        
                            return workplanExporter.ExportWorkPlan(exportPath);
                        }
                    }
                    return isImportSuccess;
                }
            }
            catch (Exception e)
            {
                LogManager.Error("Failed to import Workplan: " + e.Message + "\n" + e.StackTrace);
                Exception e2 = e;
                while (e2 != null)
                {
                    if (e2.InnerException != null)
                    {
                        LogManager.Error("Workplan import innerException: " + e2.InnerException.Message);
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
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>List of workitems along with rights container</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsBySearchCriteria(personId, releaseIds, granularity, hidePercentComplete, wiAcronym, wiName, tbIds);
            }
        }

        /// <summary>
        /// Get the list of workitems based on searchString
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="searchString">Search String</param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        /// <returns>Work items</returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, string searchString, bool shouldHaveAcronym = false)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsBySearchCriteria(personId, searchString, shouldHaveAcronym);
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
        /// Gets the work item by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns></returns>
        public KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemByIds(int personId, List<int> workItemIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemByIds(personId, workItemIds);
            }
        }

        /// <summary>
        /// Get work items by keywords (acronyms, uids)
        /// </summary>
        /// <param name="personId">The person identifier</param>
        /// <param name="keywords">keywords to identify workitems</param>
        /// <returns>List of workitems</returns>
        public List<WorkItem> GetWorkItemsByKeywords(int personId, List<string> keywords)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsByKeywords(personId, keywords);
            }        
        }

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        public WorkItem GetPrimeWorkItemBySpecificationID(int specificationID)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var workItemManager = new WorkItemManager(uoW);
                    return workItemManager.GetPrimeWorkItemBySpecificationID(specificationID);
                }   
            }
            catch (Exception e)
            {
                LogManager.Error(Localization.GenericError, e);
                return null;
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
        /// <param name="tbIds">List of Technical Bodies</param>
        /// <returns>Work Item Count</returns>
        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workItemManager = new WorkItemManager(uoW);
                return workItemManager.GetWorkItemsCountBySearchCriteria(releaseIds, granularity, hidePercentComplete, wiAcronym, wiName, tbIds);
            }
        }

        /// <summary>
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        public Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(int personId, List<int> workitemsIds)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var workItemManager = new WorkItemManager(uoW);
                    return workItemManager.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(personId, workitemsIds);
                }
            }
            catch (Exception e)
            {
                LogManager.Error("An unexpected error occured inside WorkItemServcie.GetReleaseRelatedToOneOfWiWithTheLowerWiLevel", e);
                return null;
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

        /// <summary>
        /// Look for acronyms which start with (keyword)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>List of acronyms found</returns>
        public List<string> LookForAcronyms(string keyword)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var workItemManager = new WorkItemManager(uoW);
                    return workItemManager.LookForAcronyms(keyword);
                }
            }
            catch (Exception e)
            {
                LogManager.Error("Unexpected error occured during LookForAcronyms: ", e);
                return new List<string>();
            }
        }

        #endregion
    }
}
