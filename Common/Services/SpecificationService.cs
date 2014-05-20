using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Services
{
    public class SpecificationService : ISpecificationService
    {
        public string ExportSpecification(int personId, SpecificationSearch searchObj)
        {
            string exportPath;
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var csvExport = new SpecificationExporter(uoW);
                exportPath = csvExport.ExportSpecification(personId, searchObj);
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
                List<string> secondaryRGShortName = new List<string>();

                if (result.Key != null)
                {
                    if (result.Key.SpecificationResponsibleGroups != null && result.Key.SpecificationResponsibleGroups.Count > 0)
                    {
                        if (result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList() != null && result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().Count > 0)
                        {
                            result.Key.SpecificationResponsibleGroups.Where(g => !g.IsPrime).ToList().ForEach(g => secondaryRGShortName.Add(communityManager.GetCommmunityshortNameById(g.Fk_commityId)));
                            result.Key.SecondaryResponsibleGroupsShortNames = string.Join(",", secondaryRGShortName.ToArray());
                        }

                        if (result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList() != null & result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().Count > 0)
                        {
                            result.Key.PrimeResponsibleGroupShortName
                                = communityManager.GetCommmunityshortNameById(result.Key.SpecificationResponsibleGroups.Where(g => g.IsPrime).ToList().FirstOrDefault().Fk_commityId);
                        }
                    }

                    if (result.Key.SpecificationParents != null && result.Key.SpecificationParents.Count > 0)
                    {
                        foreach (Specification s in result.Key.SpecificationParents)
                        {
                            if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                            {
                                s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId);
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

        public List<Specification> GetSpecificationBySearchCriteriaWithExclusion(int personId,String searchString, List<string> toExclude)
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

        public KeyValuePair<bool, List<string>> CheckNumber(string specNumber)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                return specificationManager.CheckNumber(specNumber);
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

        /// <summary>
        /// See Interface definition.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public KeyValuePair<int, ImportReport> CreateSpecification(int personId, Specification spec)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var createAction = new SpecificationCreateAction();
                createAction.UoW = uoW;

                try
                {
                    var newSpec = createAction.Create(personId, spec);
                    uoW.Save();
                    return new KeyValuePair<int, ImportReport>(newSpec.Key.Pk_SpecificationId, newSpec.Value);
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("Error while creating specification: " + e.Message);
                    var report = new ImportReport();
                    report.LogError(e.Message);
                    return new KeyValuePair<int, ImportReport>(-1, report);
                }

            }
        }

        /// <summary>
        /// See interface definition
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public KeyValuePair<bool, ImportReport> EditSpecification(int personId, Specification spec)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var editAction = new SpecificationEditAction();
                editAction.UoW = uoW;
                try
                {
                    var status = editAction.EditSpecification(personId, spec);
                    if (!status)
                        throw new Exception("Could not update specification");
                    uoW.Save();
                    return new KeyValuePair<bool, ImportReport>(true, new ImportReport());
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("Error while editing specification: " + e.Message);
                    var report = new ImportReport();
                    report.LogError(e.Message);
                    return new KeyValuePair<bool, ImportReport>(false, report);
                }
            }
        }


        #region ISpecificationService Members


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
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("ForceTransposition error: " + e.Message);
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}

