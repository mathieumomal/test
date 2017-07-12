using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Specifications;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class SpecificationService : ISpecificationService
    {
        #region ISpecificationService Members

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specId"></param>
        /// <returns></returns>
        public ServiceResponse<List<SpecVersionFoundationCrs>> GetSpecVersionsFoundationCrs(int personId, int specId)
        {
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uow };
                    return specificationManager.GetSpecVersionsFoundationCrs(personId, specId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, specId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public string ExportSpecification(int personId, SpecificationSearch searchObj, string baseurl)
        {
            try
            {
                string exportPath;
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var csvExport = new SpecificationExporter(uoW);
                    exportPath = csvExport.ExportSpecification(personId, searchObj, baseurl);
                }
                return exportPath;
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, searchObj, baseurl }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecifications(int personId, List<int> ids)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.GetSpecifications(personId, ids);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, ids }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        /// <summary>
        /// Gets the specifications by numbers.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specNumbers">The specification numbers.</param>
        /// <returns>List of specifications</returns>
        public List<Specification> GetSpecificationsByNumbers(int personId, List<string> specNumbers)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = ManagerFactory.Resolve<ISpecificationManager>();
                    specificationManager.UoW = uoW;
                    return specificationManager.GetSpecificationsByNumbers(personId, specNumbers);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, specNumbers }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public KeyValuePair<Specification, UserRightsContainer> GetSpecificationDetailsById(int personId, int specificationId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                    communityManager.UoW = uoW;

                    KeyValuePair<Specification, UserRightsContainer> result = specificationManager.GetSpecificationById(personId, specificationId);

                    var spec = result.Key;

                    if (spec != null)
                    {
                        //Set prime and secondary responsible groups
                        specificationManager.SetPrimeAndSecondaryResponsibleGroupsData(spec);
                        specificationManager.SetParentAndChildrenPrimeResponsibleGroupsData(spec);

                        // Getting list of current specification technologies
                        var specTechnologiesManager = new SpecificationTechnologiesManager { UoW = uoW };
                        spec.SpecificationTechnologiesList = specTechnologiesManager.GetASpecificationTechnologiesBySpecId(spec.Pk_SpecificationId);

                        // Getting list of specification workItems including responsible groups
                        var workItemsList = new List<WorkItem>();
                        var workItemsManager = new WorkItemManager(uoW);
                        foreach (Specification_WorkItem item in spec.Specification_WorkItem.ToList())
                        {
                            var wiBuffer = workItemsManager.GetWorkItemById(personId, item.Fk_WorkItemId).Key;
                            if (wiBuffer != null)
                            {
                                wiBuffer.IsPrimary = (item.isPrime != null) && item.isPrime.Value;
                                wiBuffer.IsUserAddedWi = (item.IsSetByUser != null) && item.IsSetByUser.Value;
                                workItemsList.Add(wiBuffer);
                            }
                        }
                        spec.SpecificationWIsList = workItemsList;


                        // Getting list of specification releases
                        var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                        releaseManager.UoW = uoW;
                        spec.SpecificationReleases = releaseManager.GetReleasesLinkedToASpec(specificationId);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, specificationId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObject)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = ManagerFactory.Resolve<ISpecificationManager>();
                    specificationManager.UoW = uoW;
                    var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                    communityManager.UoW = uoW;
                    KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> result = specificationManager.GetSpecificationBySearchCriteria(personId, searchObject, false);
                    result.Key.Key.ForEach(x => x.PrimeResponsibleGroupShortName = (x.PrimeResponsibleGroup == null) ? String.Empty : communityManager.GetCommmunityshortNameById(x.PrimeResponsibleGroup.Fk_commityId));
                    return result;
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, searchObject }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<Specification> GetSpecificationBySearchCriteria(int personId, String searchString)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.GetSpecificationByNumberAndTitle(personId, searchString);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, searchString }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<Specification> GetSpecificationBySearchCriteriaWithExclusion(int personId, String searchString, List<string> toExclude)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.GetSpecificationByNumberAndTitle(personId, searchString).Where(s => !toExclude.Contains(s.Number.Trim())).ToList();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, searchString, toExclude }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<Enum_Technology> GetTechnologyList()
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.GetTechnologyList();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object>(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<Enum_Serie> GetSeries()
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.GetSeries();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object>(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public List<Enum_Technology> GetAllSpecificationTechnologies()
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specTechnologiesManager = new SpecificationTechnologiesManager { UoW = uoW };
                    return specTechnologiesManager.GetAllSpecificationTechnologies();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object>(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.CheckFormatNumber(specNumber);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { specNumber }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public KeyValuePair<bool, List<string>> LookForNumber(string specNumber)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationManager = new SpecificationManager { UoW = uoW };
                    return specificationManager.LookForNumber(specNumber);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { specNumber }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction { UoW = uoW };
                    var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                    releaseManager.UoW = uoW;
                    int targetReleaseId = releaseManager.GetNextRelease(initialReleaseId).Pk_ReleaseId;
                    return specificationsMassivePromotionAction.GetSpecificationForMassivePromotion(personId, initialReleaseId, targetReleaseId);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, initialReleaseId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public bool PerformMassivePromotion(int personId, List<Specification> specifications, int initialReleaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specificationIds = new List<int>(specifications.Count);
                    var specificationsForVersionAllocation = new List<Specification>();
                    specifications.ForEach(s =>
                    {
                        specificationIds.Add(s.Pk_SpecificationId);
                        if (s.IsNewVersionCreationEnabled)
                        {
                            specificationsForVersionAllocation.Add(s);
                        }
                    });
                    var releaseManager = new ReleaseManager {UoW = uoW};
                    var targetReleaseId = releaseManager.GetNextRelease(initialReleaseId).Pk_ReleaseId;
                    var targetRelease = releaseManager.GetReleaseById(personId, targetReleaseId).Key;

                    var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction {UoW = uoW};
                    var versionManager = new SpecVersionsManager {UoW = uoW};

                    specificationsMassivePromotionAction.PromoteMassivelySpecification(personId, specificationIds, targetReleaseId);
                    versionManager.AllocateVersionFromMassivePromote(specificationsForVersionAllocation, targetRelease, personId); 
                   
                    uoW.Save();
                    return true; 
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specifications, initialReleaseId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    var report = new Report();
                    report.LogError(e.Message);
                    return false; 
                }
            }
        }

        /// <summary>
        /// See Interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <param name="baseurl"></param>
        /// <returns></returns>
        public KeyValuePair<int, Report> CreateSpecification(int personId, Specification spec, string baseurl)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var createAction = new SpecificationCreateAction {UoW = uoW};

                try
                {
                    var newSpec = createAction.Create(personId, spec, baseurl);
                    uoW.Save();
                    var newSpecId = newSpec.Key.Pk_SpecificationId;
                    var report = newSpec.Value;
                    createAction.MailAlertSpecManager(newSpec.Key, report, baseurl, personId);
                    return new KeyValuePair<int, Report>(newSpecId, report);
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, spec, baseurl }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    var report = new Report();
                    report.LogError(e.Message);
                    return new KeyValuePair<int, Report>(-1, report);
                }

            }
        }

        /// <summary>
        /// See interface definition
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public KeyValuePair<int, Report> EditSpecification(int personId, Specification spec)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var editAction = new SpecificationEditAction {UoW = uoW};
                try
                {
                    var editSpec = editAction.EditSpecification(personId, spec);
                    uoW.Save();
                    return new KeyValuePair<int, Report>(editSpec.Key.Pk_SpecificationId, editSpec.Value);
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, spec }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    var report = new Report();
                    report.LogError(e.Message);
                    return new KeyValuePair<int, Report>(-1, report);
                }
            }
        }

        /// <summary>
        /// See interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specificationId"></param>
        /// <param name="withdrawalMeetingId"></param>
        /// <returns></returns>
        public bool DefinitivelyWithdrawSpecification(int personId, int specificationId, int withdrawalMeetingId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var withdrawalAction = new SpecificationDefinitiveWithdrawalAction(uoW);
                try
                {
                    withdrawalAction.WithdrawDefinivelySpecification(personId, specificationId, withdrawalMeetingId);
                    uoW.Save();
                    return true;
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specificationId, withdrawalMeetingId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// See interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        public bool ForceTranspositionForRelease(int personId, int releaseId, int specificationId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var transpAction = new SpecificationForceUnforceTranspositionAction { UoW = uow };
                    if (transpAction.ForceTranspositionForRelease(personId, releaseId, specificationId))
                        uow.Save();
                    else
                        return false; 
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, releaseId, specificationId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// See interface.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="releaseId"></param>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        public bool UnforceTranspositionForRelease(int personId, int releaseId, int specificationId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var transpAction = new SpecificationForceUnforceTranspositionAction { UoW = uow };
                    transpAction.UnforceTranspositionForRelease(personId, releaseId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, releaseId, specificationId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// See interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        public bool SpecificationInhibitPromote(int personId, int specificationId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var inhibitPromoteAction = new SpecificationInhibitRemoveInhibitAction { UoW = uow };
                    inhibitPromoteAction.SpecificationInhibitPromote(personId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specificationId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// See interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="specificationId"></param>
        /// <returns></returns>
        public bool SpecificationRemoveInhibitPromote(int personId, int specificationId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var inhibitPromoteAction = new SpecificationInhibitRemoveInhibitAction { UoW = uow };
                    inhibitPromoteAction.SpecificationRemoveInhibitPromote(personId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specificationId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Default implementation. Returns the right of the user for each spec release. 
        /// See interface for more details.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public List<KeyValuePair<Specification_Release, UserRightsContainer>> GetRightsForSpecReleases(int personId, Specification spec)
        {
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
                    specMgr.UoW = uow;
                    return specMgr.GetRightsForSpecReleases(personId, spec, true);
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, spec }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        /// <summary>
        /// Default implementation of the ISpecificationService. See interface for more info.
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="releaseId">Release ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="withdrawalMtgId">Withdraw Meeting ID</param>
        /// <returns>True/False</returns>
        public bool WithdrawForRelease(int personId, int releaseId, int specificationId, int withdrawalMtgId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specWithdrawAction = new SpecificationWithdrawAction {UoW = uow};
                try
                {
                    specWithdrawAction.WithdrawFromRelease(personId, releaseId, specificationId, withdrawalMtgId);
                    uow.Save();
                    return true;
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, releaseId, specificationId, withdrawalMtgId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
        }

        public bool UnWithdrawnForRelease(int personID, int releaseID, int specificationID)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specWithdrawnAction = new SpecificationWithdrawAction { UoW = uow };
                try
                {
                    specWithdrawnAction.UnWithdrawFromRelease(personID, releaseID, specificationID);
                    uow.Save();
                    return true;
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personID, releaseID, specificationID }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                    
                }
            }
        }

        /// <summary>
        /// Promote Specification to next release
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        /// <returns>True/False</returns>
        public bool PromoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specPromoteAction = new SpecificationPromoteAction(uow);
                    specPromoteAction.PromoteSpecification(personId, specificationId, currentReleaseId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specificationId, currentReleaseId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Demote Specification to previous release
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        /// <returns>True/False</returns>
        public bool DemoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var specDemoteAction = new SpecificationDemoteAction(uow);
                    specDemoteAction.DemoteSpecification(personId, specificationId, currentReleaseId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { personId, specificationId, currentReleaseId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Specification details by using Number
        /// </summary>
        /// <param name="number">Specification Number</param>
        /// <returns>Specification Details</returns>
        public Specification GetSpecificationByNumber(string number)
        {
            var spec = new Specification();
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var manager = new SpecificationManager {UoW = uow};
                    spec = manager.GetSpecificationByNumber(number);
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { number }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return spec;
        }

        /// <summary>
        /// Changes the specifications status to under change control.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="specIdsForUcc">The spec ids.</param>
        /// <returns>Status report</returns>
        public ServiceResponse<bool> ChangeSpecificationsStatusToUnderChangeControl(int personId, List<int> specIdsForUcc)
        {
            var statusChangeReport = new ServiceResponse<bool>();

            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specChangeToUccAction = new SpecificationChangeToUnderChangeControlAction {UoW = uoW};
                try
                {
                    statusChangeReport = specChangeToUccAction.ChangeSpecificationsStatusToUnderChangeControl(personId, specIdsForUcc);
                    if (statusChangeReport.Result)
                        uoW.Save();                        
                }
                catch (Exception ex)
                {
                    ExtensionLogger.Exception(ex, new List<object> { personId, specIdsForUcc }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    statusChangeReport.Report.InfoList.Clear();
                    statusChangeReport.Result = false;
                    statusChangeReport.Report.LogError("Failed to change specifications status to under change control");
                }
            }

            return statusChangeReport;
        }

        /// <summary>
        /// Delete spec release
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="releaseId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> RemoveSpecRelease(int specId, int releaseId, int personId)
        {
            var response = new ServiceResponse<bool>{Result = true};
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var manager = new SpecificationManager { UoW = uow };
                    response = manager.RemoveSpecRelease(specId, releaseId, personId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { specId, releaseId, personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    response.Report.LogError(Localization.GenericError);
                    response.Result = false;
                }
            }
            return response;
        }

        #region Delete specification
        /// <summary>
        /// Delete spec with all its related records
        /// </summary>
        /// <param name="specId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public ServiceResponse<bool> DeleteSpecification(int specId, int personId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    LogManager.InfoFormat("Trying to remove spec: {0} by user: {1}", specId, personId);
                    var manager = new SpecificationManager { UoW = uow };
                    var response = manager.DeleteSpecification(specId, personId);

                    if (response.Result && response.Report.GetNumberOfErrors() <= 0)
                        uow.Save();
                    else
                    {
                        LogManager.ErrorFormat("System not able to delete spec: {0} for user: {1} because of next reason(s)", specId, personId);
                        response.Report.ErrorList.ForEach(LogManager.Error);
                    }
                        
                    return response;
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { specId, personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return new ServiceResponse<bool> { Result = false, Report = new Report { ErrorList = new List<string> { Localization.GenericError } } };
                }
            }
        }

        /// <summary>
        /// Check if spec deletion is allowed
        /// </summary>
        /// <param name="specId">Spec id</param>
        /// <param name="personId">Person id</param>
        /// <returns>True if it's allowed and false with the list of error for the other case</returns>
        public ServiceResponse<bool> CheckDeleteSpecificationAllowed(int specId, int personId)
        {
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var manager = new SpecificationManager { UoW = uow };
                    return manager.CheckDeleteSpecificationAllowed(specId, personId);
                }
                catch (Exception e)
                {
                    ExtensionLogger.Exception(e, new List<object> { specId, personId }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return new ServiceResponse<bool> { Result = false, Report = new Report { ErrorList = new List<string> { Localization.GenericError } } };
                }
            }
        }

        #endregion

        #endregion
    }
}

