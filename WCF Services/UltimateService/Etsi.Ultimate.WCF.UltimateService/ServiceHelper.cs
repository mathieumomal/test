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

        private const string ConstErrorTemplateGetReleases = "Ultimate Service Error [GetReleases]: {0}";
        private const string ConstErrorTemplateGetWorkitemsByIds = "Ultimate Service Error [GetWorkItemsByIds]: {0}";
        private const string ConstErrorTemplateGetWorkitemsByKeyword = "Ultimate Service Error [GetWorkItemsByKeyWord]: {0}";
        private const string ConstErrorTemplateGetSpecificationsByKeyword = "Ultimate Service Error [GetSpecificationsByKeyWord]: {0}";
        private const string ConstErrorTemplateGetSpecificationById = "Ultimate Service Error [GetSpecificationById]: {0}";

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>List of releases</returns>
        public List<UltimateServiceEntities.Release> GetReleases(int personId)
        {
            var releases = new List<UltimateServiceEntities.Release>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObjects = svc.GetAllReleases(personId);
                if (releaseRightsObjects.Key != null)
                    releaseRightsObjects.Key.ForEach(x => releases.Add(ConvertUltimateReleaseToServiceRelease(x)));
                else
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return releases;
        }

        /// <summary>
        /// Gets the release by identifier.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <returns>Return release</returns>
        public UltimateServiceEntities.Release GetReleaseById(int personId, int releaseId)
        {
            var release = new UltimateServiceEntities.Release();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<IReleaseService>();
                var releaseRightsObject = svc.GetReleaseById(personId, releaseId);
                if (releaseRightsObject.Key != null)
                    release = ConvertUltimateReleaseToServiceRelease(releaseRightsObject.Key);
                else
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases,
                        "Failed to get release details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetReleases, ex.Message));
            }

            return release;
        }

        /// <summary>
        /// Gets the work items by ids.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<UltimateServiceEntities.WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds)
        {
            var workItems = new List<UltimateServiceEntities.WorkItem>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<IWorkItemService>();
                foreach (var workItemId in workItemIds)
                {
                    var workItemRightsObject = svc.GetWorkItemById(personId, workItemId);
                    if (workItemRightsObject.Key != null)
                        workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(workItemRightsObject.Key));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, "Unable to get workitem for work item id=" + workItemId));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByIds, ex.Message));
            }

            return workItems;
        }

        /// <summary>
        /// Gets the work items by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of work items
        /// </returns>
        public List<UltimateServiceEntities.WorkItem> GetWorkItemsByKeyWord(int personId, string keyword)
        {
            var workItems = new List<UltimateServiceEntities.WorkItem>();

            if (String.IsNullOrEmpty(keyword))
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                    RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                    var svc = ServicesFactory.Resolve<IWorkItemService>();
                    var workItemRightsObjects = svc.GetWorkItemsBySearchCriteria(personId, keyword);
                    if (workItemRightsObjects.Key != null)
                        workItemRightsObjects.Key.ForEach(x => workItems.Add(ConvertUltimateWorkItemToServiceWorkItem(x)));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, "Failed to get workitem details"));
                }
                catch (Exception ex)
                {
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetWorkitemsByKeyword, ex.Message));
                }
            }
            return workItems;
        }

        /// <summary>
        /// Gets the specifications by key word.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        public List<UltimateServiceEntities.Specification> GetSpecificationsByKeyWord(int personId, string keyword)
        {
            var specifications = new List<UltimateServiceEntities.Specification>();

            if (String.IsNullOrEmpty(keyword))
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Keyword should not empty"));
            }
            else
            {
                try
                {
                    //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                    RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                    var svc = ServicesFactory.Resolve<ISpecificationService>();
                    var specificationsObjects = svc.GetSpecificationBySearchCriteria(personId, keyword);
                    if (specificationsObjects != null)
                        specificationsObjects.ForEach(x => specifications.Add(ConvertUltimateSpecificationToServiceSpecification(x)));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, "Failed to get specification details"));
                }
                catch (Exception ex)
                {
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByKeyword, ex.Message));
                }
            }
            return specifications;
        }

        /// <summary>
        /// Gets the specification by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>
        /// Specification entity
        /// </returns>
        public UltimateServiceEntities.Specification GetSpecificationById(int personID, int specificationId)
        {
            var specification = new UltimateServiceEntities.Specification();
            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                var specificationRightsObjects = svc.GetSpecificationDetailsById(personID, specificationId);
                if (specificationRightsObjects.Key != null)
                    specification = ConvertUltimateSpecificationToServiceSpecification(specificationRightsObjects.Key);
                else
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationById, "Failed to get specification details"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationById, ex.Message));
            }
            return specification;
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
            var serviceRelease = new UltimateServiceEntities.Release();
            if (ultimateRelease != null)
            {
                serviceRelease.Pk_ReleaseId = ultimateRelease.Pk_ReleaseId;
                serviceRelease.Name = ultimateRelease.Name;
                serviceRelease.ShortName = ultimateRelease.ShortName;
                serviceRelease.Status = ultimateRelease.Enum_ReleaseStatus.Description;
                serviceRelease.SortOrder = ultimateRelease.SortOrder;
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
            var serviceWorkItem = new UltimateServiceEntities.WorkItem();
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

        /// <summary>
        /// Converts the ultimate work item to service work item.
        /// </summary>
        /// <param name="ultimateSpecification">The ultimate specification.</param>
        /// <returns>Service specification entity</returns>
        private UltimateServiceEntities.Specification ConvertUltimateSpecificationToServiceSpecification(UltimateEntities.Specification ultimateSpecification)
        {
            var serviceSpecification = new UltimateServiceEntities.Specification();
            if (ultimateSpecification != null)
            {
                serviceSpecification.Pk_SpecificationId = ultimateSpecification.Pk_SpecificationId;
                serviceSpecification.Title = ultimateSpecification.Title;
                serviceSpecification.Number = ultimateSpecification.Number;
                serviceSpecification.SpecNumberAndTitle = ultimateSpecification.SpecNumberAndTitle;
            }
            return serviceSpecification;
        }

        #endregion
    }
}