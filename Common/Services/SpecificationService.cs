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
        public KeyValuePair<Specification, UserRightsContainer> GetSpecificationDetailsById(int personId, int specificationId)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;  
                var communityManager = new CommunityManager(uoW);
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
                        WIBuffer = workItemsManager.GetWorkItemById(personId,item.Fk_WorkItemId).Key;
                        if (WIBuffer != null)
                            workItemsList.Add(WIBuffer);
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
                var communityManager = new CommunityManager(uoW);
                KeyValuePair<KeyValuePair<List<Specification>, int>, UserRightsContainer> result = specificationManager.GetSpecificationBySearchCriteria(personId, searchObject);
                result.Key.Key.ForEach(x => x.PrimeResponsibleGroupShortName = (x.PrimeResponsibleGroup == null) ? String.Empty : communityManager.GetCommmunityshortNameById(x.PrimeResponsibleGroup.Fk_commityId));
                return result;
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

        #region ISpecificationService Membres


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

        #endregion

        #region ISpecificationService Members


        public int CreateSpecification(int personId, Specification spec)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var createAction = new SpecificationCreateAction();
                createAction.UoW = uoW;

                try
                {
                    return createAction.Create(personId, spec);
                }
                catch (Exception e)
                {
                    Utils.LogManager.Error("Error while creating specification: "+e.Message);
                    return 0;
                }

            }
        }

        public bool EditSpecification(int personId, Specification spec)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

