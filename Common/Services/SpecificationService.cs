using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.SpecVersionBusiness;
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
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager {UoW = uow};
                return specificationManager.GetSpecVersionsFoundationCrs(personId, specId);
            }
        }

        public string ExportSpecification(int personId, SpecificationSearch searchObj, string baseurl)
        {
            string exportPath;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvExport = new SpecificationExporter(uoW);
                exportPath = csvExport.ExportSpecification(personId, searchObj, baseurl);
            }
            return exportPath;
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecifications(int personId, List<int> ids)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.GetSpecifications(personId, ids);
            }
        }

        public KeyValuePair<Specification, UserRightsContainer> GetSpecificationDetailsById(int personId, int specificationId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW;

                KeyValuePair<Specification, UserRightsContainer> result = specificationManager.GetSpecificationById(personId, specificationId);
                List<Community> communityList = new List<Community>();

                var spec = result.Key;

                if (spec != null)
                {
                    if (spec.SpecificationResponsibleGroups != null && spec.SpecificationResponsibleGroups.Count > 0)
                    {
                        if (spec.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList() != null && spec.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().Count > 0)
                        {
                            spec.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().ForEach(g => communityList.Add(communityManager.GetCommmunityById(g.Fk_commityId)));
                            spec.SecondaryResponsibleGroupsFullNames = string.Join(",", communityList.Select(x=>x.TbName).ToArray());
                        }

                        if (spec.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList() != null & spec.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().Count > 0)
                        {
                            spec.PrimeResponsibleGroupFullName
                                = communityManager.GetCommmunityById(spec.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault().Fk_commityId).TbName;
                            spec.PrimeResponsibleGroupShortName
                                = communityManager.GetCommmunityById(spec.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault().Fk_commityId).ShortName;
                        }
                    }

                    if (spec.SpecificationParents != null && spec.SpecificationParents.Count > 0)
                    {
                        foreach (Specification s in spec.SpecificationParents)
                        {
                            if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                            {
                                s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId);
                            }
                        }
                    }

                    if (spec.SpecificationChilds != null && spec.SpecificationChilds.Count > 0)
                    {
                        foreach (Specification s in spec.SpecificationChilds)
                        {
                            if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                            {
                                s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId);
                            }
                        }
                    }

                    // Getting list of current specification technologies
                    var specTechnologiesManager = new SpecificationTechnologiesManager();
                    specTechnologiesManager.UoW = uoW;
                    spec.SpecificationTechnologiesList = specTechnologiesManager.GetASpecificationTechnologiesBySpecId(spec.Pk_SpecificationId);

                    // Getting list of specification workItems including responsible groups
                    List<WorkItem> workItemsList = new List<WorkItem>();
                    WorkItem WIBuffer;
                    var workItemsManager = new WorkItemManager(uoW);
                    foreach (Specification_WorkItem item in spec.Specification_WorkItem.ToList())
                    {
                        WIBuffer = workItemsManager.GetWorkItemById(personId, item.Fk_WorkItemId).Key;
                        if (WIBuffer != null)
                        {
                            WIBuffer.IsPrimary = (item.isPrime != null) ? item.isPrime.Value : false;
                            WIBuffer.IsUserAddedWi = (item.IsSetByUser != null) ? item.IsSetByUser.Value : false;
                            workItemsList.Add(WIBuffer);
                        }
                    }
                    spec.SpecificationWIsList = workItemsList;


                    // Getting list of specification releases
                    List<Release> releases = new List<Release>();
                    Release releaseObj;
                    var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                    releaseManager.UoW = uoW;
                    foreach (Specification_Release item in spec.Specification_Release.ToList())
                    {
                        releaseObj = releaseManager.GetReleaseById(personId, item.Fk_ReleaseId).Key;
                        if (releaseObj != null)
                            releases.Add(releaseObj);
                    }
                    spec.SpecificationReleases = releases;
                }
                return result;
            }
        }

        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObject)
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

        public List<Specification> GetSpecificationBySearchCriteria(int personId, String searchString)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.GetSpecificationByNumberAndTitle(personId, searchString);
            }
        }

        public List<Specification> GetSpecificationBySearchCriteriaWithExclusion(int personId, String searchString, List<string> toExclude)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.GetSpecificationByNumberAndTitle(personId, searchString).Where(s => !toExclude.Contains(s.Number.Trim())).ToList();
            }
        }

        public List<Enum_Technology> GetTechnologyList()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.GetTechnologyList();
            }
        }

        public List<Enum_Serie> GetSeries()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.GetSeries();
            }
        }

        public List<Enum_Technology> GetAllSpecificationTechnologies()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specTechnologiesManager = new SpecificationTechnologiesManager();
                specTechnologiesManager.UoW = uoW;
                return specTechnologiesManager.GetAllSpecificationTechnologies();
            }
        }

        public KeyValuePair<bool, List<string>> CheckFormatNumber(string specNumber)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.CheckFormatNumber(specNumber);
            }
        }

        public KeyValuePair<bool, List<string>> LookForNumber(string specNumber)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.LookForNumber(specNumber);
            }
        }

        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction();
                specificationsMassivePromotionAction.UoW = uoW;
                var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                releaseManager.UoW = uoW;
                int targetReleaseId = releaseManager.GetNextRelease(initialReleaseId).Pk_ReleaseId; 
                return specificationsMassivePromotionAction.GetSpecificationForMassivePromotion(personId, initialReleaseId, targetReleaseId);
            }
        }

        public bool PerformMassivePromotion(int personId, List<Specification> specifications, int initialReleaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    List<int> specificationIds = new List<int>(specifications.Count);
                    List<Specification> specificationsForVersionAllocation = new List<Specification>();
                    specifications.ForEach(s =>
                    {
                        specificationIds.Add(s.Pk_SpecificationId);
                        if (s.IsNewVersionCreationEnabled)
                        {
                            specificationsForVersionAllocation.Add(s);
                        }
                    });
                    var releaseManager = new ReleaseManager();
                    releaseManager.UoW = uoW;
                    int targetReleaseId = releaseManager.GetNextRelease(initialReleaseId).Pk_ReleaseId;
                    Release targetRelease = releaseManager.GetReleaseById(personId, targetReleaseId).Key;

                    var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction();
                    specificationsMassivePromotionAction.UoW = uoW;
                    var versionManager = new SpecVersionsManager();
                    versionManager.UoW = uoW;

                    specificationsMassivePromotionAction.PromoteMassivelySpecification(personId, specificationIds, targetReleaseId);
                    versionManager.AllocateVersionFromMassivePromote(specificationsForVersionAllocation, targetRelease, personId); 
                   
                    uoW.Save();
                    return true; 
                }
                catch (Exception e)
                {
                    LogManager.Error("Error while massively promoting a set of specification: " + e.Message);
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
        /// <returns></returns>
        public KeyValuePair<int, Report> CreateSpecification(int personId, Specification spec, string baseurl)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var createAction = new SpecificationCreateAction();
                createAction.UoW = uoW;

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
                    LogManager.Error("Error while creating specification: " + e.Message);
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
                var editAction = new SpecificationEditAction();
                editAction.UoW = uoW;
                try
                {
                    var editSpec = editAction.EditSpecification(personId, spec);
                    uoW.Save();
                    return new KeyValuePair<int, Report>(editSpec.Key.Pk_SpecificationId, editSpec.Value);

                    /*var status = editAction.EditSpecification(personId, spec);
                    if (!status)
                        throw new Exception("Could not update specification");
                    uoW.Save();
                    return new KeyValuePair<bool, Report>(true, new Report());*/
                }
                catch (Exception e)
                {
                    LogManager.Error("Error while editing specification: " + e.Message);
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
        /// <param name="WithdrawalMeetingId"></param>
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
                    LogManager.Error("Error during definitive withdrawal of the specification: " + e.Message);
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
                    var transpAction = new SpecificationForceUnforceTranspositionAction() { UoW = uow };
                    if (transpAction.ForceTranspositionForRelease(personId, releaseId, specificationId))
                        uow.Save();
                    else
                        return false; 
                }
                catch (Exception e)
                {
                    LogManager.Error("ForceTransposition error: " + e.Message);
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
                    var transpAction = new SpecificationForceUnforceTranspositionAction() { UoW = uow };
                    transpAction.UnforceTranspositionForRelease(personId, releaseId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    LogManager.Error("UnforceTransposition error: " + e.Message);
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
                    var inhibitPromoteAction = new SpecificationInhibitRemoveInhibitAction() { UoW = uow };
                    inhibitPromoteAction.SpecificationInhibitPromote(personId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    LogManager.Error("InhibitPromote error: " + e.Message);
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
                    var inhibitPromoteAction = new SpecificationInhibitRemoveInhibitAction() { UoW = uow };
                    inhibitPromoteAction.SpecificationRemoveInhibitPromote(personId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    LogManager.Error("UnforceTransposition error: " + e.Message);
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
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
                specMgr.UoW = uow;
                return specMgr.GetRightsForSpecReleases(personId, spec);
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
                var specWithdrawAction = new SpecificationWithdrawAction();
                specWithdrawAction.UoW = uow;
                try
                {
                    specWithdrawAction.WithdrawFromRelease(personId, releaseId, specificationId, withdrawalMtgId);
                    uow.Save();
                    return true;
                }
                catch (Exception e)
                {
                    LogManager.Error("Error when Withdrawing from release: " + e.Message);
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
                    LogManager.Error("Promote Specification Error: " + e.Message);
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
            Specification spec = new Specification();
            using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                try
                {
                    var manager = new SpecificationManager();
                    manager.UoW = uow;
                    spec = manager.GetSpecificationByNumber(number);
                }
                catch (Exception e)
                {
                    LogManager.Error("Specification search error: " + e.Message);
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
                var editAction = new SpecificationEditAction();
                editAction.UoW = uoW;
                try
                {
                    statusChangeReport = editAction.ChangeSpecificationsStatusToUnderChangeControl(personId, specIdsForUcc);
                    if (statusChangeReport.Result)
                        uoW.Save();                        
                }
                catch (Exception ex)
                {
                    LogManager.Error(String.Format("Error while changing specifications status to under change control: {0}{1}", ex.Message, ((ex.InnerException != null) ? "\n InnterException:" + ex.InnerException : String.Empty)));
                    statusChangeReport.Report.InfoList.Clear();
                    statusChangeReport.Result = false;
                    statusChangeReport.Report.LogError("Failed to change specifications status to under change control");
                }
            }

            return statusChangeReport;
        }

        #endregion
    }
}

