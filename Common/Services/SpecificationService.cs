using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Services
{
    public class SpecificationService : ISpecificationService
    {
        #region ISpecificationService Members

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

                if (result.Key != null)
                {
                    if (result.Key.SpecificationResponsibleGroups != null && result.Key.SpecificationResponsibleGroups.Count > 0)
                    {
                        if (result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList() != null && result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().Count > 0)
                        {
                            result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().ForEach(g => communityList.Add(communityManager.GetCommmunityById(g.Fk_commityId)));
                            result.Key.SecondaryResponsibleGroupsFullNames = string.Join(",", communityList.Select(x=>x.TbName).ToArray());
                        }

                        if (result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList() != null & result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().Count > 0)
                        {
                            result.Key.PrimeResponsibleGroupFullName
                                = communityManager.GetCommmunityById(result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault().Fk_commityId).TbName;
                            result.Key.PrimeResponsibleGroupShortName
                                = communityManager.GetCommmunityById(result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault().Fk_commityId).ShortName;
                        }
                    }

                    if (result.Key.SpecificationParents != null && result.Key.SpecificationParents.Count > 0)
                    {
                        foreach (Specification s in result.Key.SpecificationParents)
                        {
                            if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                            {
                                s.PrimeResponsibleGroupFullName = communityManager.GetCommmunityById(s.PrimeResponsibleGroup.Fk_commityId).TbName;
                            }
                        }
                    }

                    if (result.Key.SpecificationChilds != null && result.Key.SpecificationChilds.Count > 0)
                    {
                        foreach (Specification s in result.Key.SpecificationChilds)
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
                    result.Key.SpecificationTechnologiesList = specTechnologiesManager.GetASpecificationTechnologiesBySpecId(result.Key.Pk_SpecificationId);

                    // Getting list of specification workItems including responsible groups
                    List<WorkItem> workItemsList = new List<WorkItem>();
                    WorkItem WIBuffer;
                    var workItemsManager = new WorkItemManager(uoW);
                    foreach (Specification_WorkItem item in result.Key.Specification_WorkItem.ToList())
                    {
                        WIBuffer = workItemsManager.GetWorkItemById(personId, item.Fk_WorkItemId).Key;
                        if (WIBuffer != null)
                        {
                            WIBuffer.IsPrimary = (item.isPrime != null) ? item.isPrime.Value : false;
                            WIBuffer.IsUserAddedWi = (item.IsSetByUser != null) ? item.IsSetByUser.Value : false;
                            workItemsList.Add(WIBuffer);
                        }
                    }
                    result.Key.SpecificationWIsList = workItemsList;


                    // Getting list of specification releases
                    List<Release> releases = new List<Release>();
                    Release releaseObj;
                    var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
                    releaseManager.UoW = uoW;
                    foreach (Specification_Release item in result.Key.Specification_Release.ToList())
                    {
                        releaseObj = releaseManager.GetReleaseById(personId, item.Fk_ReleaseId).Key;
                        if (releaseObj != null)
                            releases.Add(releaseObj);
                    }
                    result.Key.SpecificationReleases = releases;
                }
                return result;
            }
        }

        public KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObject)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                var communityManager = ManagerFactory.Resolve<ICommunityManager>();
                communityManager.UoW = uoW;
                KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> result = specificationManager.GetSpecificationBySearchCriteria(personId, searchObject);
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

        public bool CheckInhibitedToPromote(string specNumber)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.CheckInhibitedToPromote(specNumber);
            }
        }

        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction(uoW);
                ReleaseManager releaseManager = new ReleaseManager();
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
                    ReleaseManager releaseManager = new ReleaseManager();
                    releaseManager.UoW = uoW;
                    int targetReleaseId = releaseManager.GetNextRelease(initialReleaseId).Pk_ReleaseId;
                    Release targetRelease = releaseManager.GetReleaseById(personId, targetReleaseId).Key;

                    var specificationsMassivePromotionAction = new SpecificationsMassivePromotionAction(uoW);
                    specificationsMassivePromotionAction.PromoteMassivelySpecification(personId, specificationIds, targetReleaseId);

                    SpecVersionsManager versionManager = new SpecVersionsManager(uoW);
                    versionManager.AllocateVersionFromMassivePromote(specificationsForVersionAllocation, targetRelease); 
                   
                    uoW.Save();
                    return true; 
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("Error while massively promoting a set of specification: " + e.Message);
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
                    return new KeyValuePair<int, Report>(newSpec.Key.Pk_SpecificationId, newSpec.Value);
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("Error while creating specification: " + e.Message);
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
                    Utils.LogManager.Error("Error while editing specification: " + e.Message);
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
                    Utils.LogManager.Error("Error during definitive withdrawal of the specification: " + e.Message);
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
                    transpAction.ForceTranspositionForRelease(personId, releaseId, specificationId);
                    uow.Save();
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("ForceTransposition error: " + e.Message);
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
                    Utils.LogManager.Error("UnforceTransposition error: " + e.Message);
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
                    Utils.LogManager.Error("InhibitPromote error: " + e.Message);
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
                    Utils.LogManager.Error("UnforceTransposition error: " + e.Message);
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
                    Utils.LogManager.Error("Error when Withdrawing from release: " + e.Message);
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
                    Utils.LogManager.Error("Promote Specification Error: " + e.Message);
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}

