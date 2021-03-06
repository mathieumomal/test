﻿using System;
using System.Collections.Generic;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public interface IWorkItemService
    {
        /// <summary>
        /// Analyse imported workItems and return errors and warnings logs + cache token
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        KeyValuePair<string, Report> AnalyseWorkPlanForImport(String path);

        /// <summary>
        /// Import workitems uploaded on the server
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="exportPath">Export Path</param>
        /// <returns>Success/Failure</returns>
        bool ImportWorkPlan(string token, string exportPath);

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
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds);

        /// <summary>
        /// Get the list of workitems based on searchString
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="searchString">Search String</param>
        /// <param name="shouldHaveAcronym">WIs should have acronym</param>
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemsBySearchCriteria(int personId, string searchString, bool shouldHaveAcronym = false);

        /// <summary>
        /// Gets all work items.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>Work items</returns>
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetAllWorkItems(int personId);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemById(int personId, int workItemId);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item along with right container</returns>
        KeyValuePair<List<WorkItem>, UserRightsContainer> GetWorkItemByIds(int personId, List<int> workItemId);

        /// <summary>
        /// Get work items by keywords (acronyms, uids)
        /// </summary>
        /// <param name="personId">The person identifier</param>
        /// <param name="keywords">keywords to identify workitems</param>
        /// <returns>List of workitems</returns>
        List<WorkItem> GetWorkItemsByKeywords(int personId, List<string> keywords);

        /// <summary>
        /// Get the workitem based on the id
        /// </summary>
        /// <param name="personId">Person Id</param>
        /// <param name="workItemId">Work Item Id</param>
        /// <returns>Work Item, right container and other required properties</returns>
        KeyValuePair<WorkItem, UserRightsContainer> GetWorkItemByIdExtend(int personId, int workItemId);

        /// <summary>
        /// Get primary work item by specification ID
        /// </summary>
        /// <param name="specificationID"> specification ID</param>
        /// <returns>WorkItem if found, else null</returns>
        WorkItem GetPrimeWorkItemBySpecificationID(int specificationID);

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
        int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds);

        /// <summary>
        /// The aim of this method is to return the release of the first WI found with lower WiLevel among a list of WI 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="workitemsIds"></param>
        /// <returns></returns>
        Release GetReleaseRelatedToOneOfWiWithTheLowerWiLevel(int personId, List<int> workitemsIds);

        /// <summary>
        /// Get list of distinct Acronyms from various releases
        /// </summary>
        /// <returns>List of Acronyms</returns>
        List<string> GetAllAcronyms();

        /// <summary>
        /// Look for acronyms which start with (keyword)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>List of acronyms found</returns>
        List<string> LookForAcronyms(string keyword);
    }
}
