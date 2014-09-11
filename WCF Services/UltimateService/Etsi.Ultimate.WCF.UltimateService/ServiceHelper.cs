using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Services;
using Etsi.UserRights.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using UltimateEntities = Etsi.Ultimate.DomainClasses;
using UltimateServiceEntities = Etsi.Ultimate.WCF.Interface.Entities;

namespace Etsi.Ultimate.WCF.Service
{
    /// <summary>
    /// Helper class to provide necessary information to service
    /// </summary>
    public class ServiceHelper
    {
        #region Constants

        private const string CONST_ERROR_TEMPLATE_GET_RELEASES = "Ultimate Service Error [GetReleases]: {0}";
        private const string CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_IDS = "Ultimate Service Error [GetWorkItemsByIds]: {0}";
        private const string CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_KEYWORD = "Ultimate Service Error [GetWorkItemsByKeyWord]: {0}";

        #endregion
        #region Public Methods

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <returns>List of releases</returns>
        public List<UltimateServiceEntities.Release> GetReleases(int personID)
        {
            List<UltimateServiceEntities.Release> releases = new List<UltimateServiceEntities.Release>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                IReleaseService svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObjects = svc.GetAllReleases(personID);
                if (releaseRightsObjects.Key != null)
                    releaseRightsObjects.Key.ForEach(x => releases.Add(ConvertUltimateReleaseToServiceRelease(x)));
                else
                    LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_RELEASES, "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_RELEASES, ex.Message));
            }

            return releases;
        }

        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<UltimateServiceEntities.WorkItem> GetWorkItemsByIds(int personID, List<int> workItemIds)
        {
            List<UltimateServiceEntities.WorkItem> workItems = new List<UltimateServiceEntities.WorkItem>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                IWorkItemService svc = ServicesFactory.Resolve<IWorkItemService>();
                foreach (int workItemID in workItemIds)
                {
                    var workItemRightsObject = svc.GetWorkItemById(personID, workItemID);
                    if (workItemRightsObject.Key != null)
                        workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(workItemRightsObject.Key));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_IDS, "Unable to get workitem for work item id=" + workItemID));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_IDS, ex.Message));
            }

            return workItems;
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<UltimateServiceEntities.WorkItem> GetWorkItemsByKeyWord(int personID, string keyword)
        {
            List<UltimateServiceEntities.WorkItem> workItems = new List<UltimateServiceEntities.WorkItem>();

            if (String.IsNullOrEmpty(keyword))
            {
                LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_KEYWORD, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                    RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                    IWorkItemService svc = ServicesFactory.Resolve<IWorkItemService>();
                    var workItemRightsObjects = svc.GetWorkItemsBySearchCriteria(personID, keyword);
                    if (workItemRightsObjects.Key != null)
                        workItemRightsObjects.Key.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_KEYWORD, "Failed to get workitem details"));
                }
                catch (Exception ex)
                {
                    LogManager.UltimateServiceLogger.Error(String.Format(CONST_ERROR_TEMPLATE_GET_WORKITEMS_BY_KEYWORD, ex.Message));
                }
            }
            return workItems;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts the ultimate release to service release.
        /// </summary>
        /// <param name="ultimateRelease">The ultimate release.</param>
        /// <returns>Service release entity</returns>
        private UltimateServiceEntities.Release ConvertUltimateReleaseToServiceRelease(UltimateEntities.Release ultimateRelease)
        {
            UltimateServiceEntities.Release serviceRelease = new UltimateServiceEntities.Release();
            if (ultimateRelease != null)
            {
                serviceRelease.Pk_ReleaseId = ultimateRelease.Pk_ReleaseId;
                serviceRelease.Name = ultimateRelease.Name;
                serviceRelease.ShortName = ultimateRelease.ShortName;
                serviceRelease.Status = ultimateRelease.Enum_ReleaseStatus.Description;
            }
            return serviceRelease;
        }

        /// <summary>
        /// Converts the ultimate work item to service work item.
        /// </summary>
        /// <param name="ultimateWorkItem">The ultimate work item.</param>
        /// <returns>Service work item entity</returns>
        private UltimateServiceEntities.WorkItem ConvertUltimateWorkItemToServiceWorkItem(UltimateEntities.WorkItem ultimateWorkItem)
        {
            UltimateServiceEntities.WorkItem serviceWorkItem = new UltimateServiceEntities.WorkItem();
            if (ultimateWorkItem != null)
            {
                serviceWorkItem.Pk_WorkItemUid = ultimateWorkItem.Pk_WorkItemUid;
                serviceWorkItem.UID = ultimateWorkItem.UID;
                serviceWorkItem.Acronym = ultimateWorkItem.Acronym;
                serviceWorkItem.Name = ultimateWorkItem.Name;
                serviceWorkItem.ResponsibleGroups = ultimateWorkItem.ResponsibleGroups;
            }
            return serviceWorkItem;
        } 

        #endregion
    }
}