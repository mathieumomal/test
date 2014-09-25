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
        private const string ConstErrorTemplateGetSpecificationsByIds = "Ultimate Service Error [GetSpecificationsByIds]: {0}";
        private const string ConstErrorTemplateCreateChangeRequest = "Ultimate Service Error [CreateChangeRequest]: {0}";
        private const string ConstErrorTemplateCreateChangeRequestCategories = "Ultimate Service Error [GetChangeRequestCategories]: {0}";

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the releases.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>List of releases</returns>
        internal List<UltimateServiceEntities.Release> GetReleases(int personId)
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
        internal UltimateServiceEntities.Release GetReleaseById(int personId, int releaseId)
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
        internal List<UltimateServiceEntities.WorkItem> GetWorkItemsByIds(int personId, List<int> workItemIds)
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
        internal List<UltimateServiceEntities.WorkItem> GetWorkItemsByKeyWord(int personId, string keyword)
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
        internal List<UltimateServiceEntities.Specification> GetSpecificationsByKeyWord(int personId, string keyword)
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
        internal UltimateServiceEntities.Specification GetSpecificationById(int personID, int specificationId)
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

        /// <summary>
        /// Gets the specifications by ids.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="specificationIds">The specification ids.</param>
        /// <returns>
        /// List of specifications
        /// </returns>
        internal List<UltimateServiceEntities.Specification> GetSpecificationsByIds(int personID, List<int> specificationIds)
        {
            var specifications = new List<UltimateServiceEntities.Specification>();

            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<ISpecificationService>();
                foreach (var specificationId in specificationIds)
                {
                    var specificationRightsObjects = svc.GetSpecificationDetailsById(personID, specificationId);
                    if (specificationRightsObjects.Key != null)
                        specifications.Add(ConvertUltimateSpecificationToServiceSpecification(specificationRightsObjects.Key));
                    else
                        LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, "Unable to get specification for specification id=" + specificationId));
                }
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateGetSpecificationsByIds, ex.Message));
            }

            return specifications;
        }

        /// <summary>
        /// Gets the change request by identifier.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequestId">The change request identifier.</param>
        /// <returns>Change Request entity</returns>
        internal UltimateServiceEntities.ChangeRequest GetChangeRequestById(int personID, int changeRequestId)
        {
            return new UltimateServiceEntities.ChangeRequest();
        }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personID">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        internal int CreateChangeRequest(int personID, UltimateServiceEntities.ChangeRequest changeRequest)
        {
            int primaryKeyOfNewCR = 0;
            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<ICRService>();
                var createCRResponse = svc.CreateChangeRequest(personID, ConvertServiceCRToUltimateCR(changeRequest));
                if (createCRResponse.Key)
                    primaryKeyOfNewCR = createCRResponse.Value;
                else
                    LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequest, "Failed to get create CR record"));
            }
            catch (Exception ex)
            {
                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequest, ex.Message));
            }
            return primaryKeyOfNewCR;
        }

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>UltimateService ChangeRequestCategory</returns>
        internal List<UltimateServiceEntities.ChangeRequestCategory> GetChangeRequestCategories(int personId)
        {
            var changeRequestCategory = new List<UltimateServiceEntities.ChangeRequestCategory>();
            try
            {
                //TODO:: Following line will be removed after UserRights integration with Ultimate Solution
                RepositoryFactory.Container.RegisterType<IUserRightsRepository, UserRights.UserRights>(new TransientLifetimeManager());
                var svc = ServicesFactory.Resolve<ICRService>();
                var svcChangeRequestCategory = svc.GetChangeRequestCategories(personId);
                if (svcChangeRequestCategory.Key)
                    changeRequestCategory = ConvertUltimateCRCategoriesToServiceCategories(svcChangeRequestCategory.Value);
            }
            catch (Exception ex)
            {

                LogManager.UltimateServiceLogger.Error(String.Format(ConstErrorTemplateCreateChangeRequestCategories, ex.Message));
            }
            return changeRequestCategory;
        }

        /// <summary>
        /// Converts the ultimate cr categories to service categories.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>UltimateService ChangeRequestCategory</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private List<UltimateServiceEntities.ChangeRequestCategory> ConvertUltimateCRCategoriesToServiceCategories(List<UltimateEntities.Enum_CRCategory> ultimateCRCategory)
        {
            var svcCRCategories = new List<UltimateServiceEntities.ChangeRequestCategory>();
            foreach (var item in ultimateCRCategory)
            {
                svcCRCategories.Add(new UltimateServiceEntities.ChangeRequestCategory
                {
                    Pk_EnumCRCategory = item.Pk_EnumCRCategory,
                    Code = item.Code,
                    Description = item.Description
                });
            }
            return svcCRCategories;
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

        /// <summary>
        /// Converts the service cr to ultimate CR.
        /// </summary>
        /// <param name="serviceCR">The service CR.</param>
        /// <returns>Ultimate CR</returns>
        private UltimateEntities.ChangeRequest ConvertServiceCRToUltimateCR(UltimateServiceEntities.ChangeRequest serviceCR)
        {
            var ultimateCR = new UltimateEntities.ChangeRequest();
            if (serviceCR != null)
            {
                ultimateCR.Pk_ChangeRequest = serviceCR.Pk_ChangeRequest;
                ultimateCR.CRNumber = serviceCR.CRNumber;
                ultimateCR.Revision = serviceCR.Revision;
                ultimateCR.Subject = serviceCR.Subject;
                ultimateCR.Fk_TSGStatus = serviceCR.Fk_TSGStatus;
                ultimateCR.Fk_WGStatus = serviceCR.Fk_WGStatus;
                ultimateCR.Subject = serviceCR.Subject;
                ultimateCR.CreationDate = serviceCR.CreationDate;
                ultimateCR.TSGSourceOrganizations = serviceCR.TSGSourceOrganizations;
                ultimateCR.WGSourceOrganizations = serviceCR.WGSourceOrganizations;
                ultimateCR.TSGMeeting = serviceCR.TSGMeeting;
                ultimateCR.TSGTarget = serviceCR.TSGTarget;
                ultimateCR.WGSourceForTSG = serviceCR.WGSourceForTSG;
                ultimateCR.WGMeeting = serviceCR.WGMeeting;
                ultimateCR.WGTarget = serviceCR.WGTarget;
                ultimateCR.Fk_Enum_CRCategory = serviceCR.Fk_Enum_CRCategory;
                ultimateCR.Fk_Specification = serviceCR.Fk_Specification;
                ultimateCR.Fk_Release = serviceCR.Fk_Release;
                ultimateCR.Fk_CurrentVersion = serviceCR.Fk_CurrentVersion;
                ultimateCR.Fk_NewVersion = serviceCR.Fk_NewVersion;
                ultimateCR.Fk_Impact = serviceCR.Fk_Impact;
                ultimateCR.TSGTDoc = serviceCR.TSGTDoc;
                ultimateCR.WGTDoc = serviceCR.WGTDoc;
                ultimateCR.Fk_Enum_CRCategory = serviceCR.Fk_Enum_CRCategory;
                ultimateCR.Fk_Enum_CRCategory = serviceCR.Fk_Enum_CRCategory;
            }
            return ultimateCR;
        }

        #endregion
    }
}