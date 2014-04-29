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
                        //result.Key.SpecificationParents.Where(s => s.SpecificationResponsibleGroups != null ).ToList().ForEach(s => s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId));
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
                        //result.Key.SpecificationChilds.Where(s => s.SpecificationResponsibleGroups != null).ToList().ForEach(s => s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId));
                    }
                    result.Key.SpecificationTechnologiesList = GetASpecificationTechnologiesBySpecId(result.Key.Pk_SpecificationId);
                    result.Key.SpecificationWIsList = GetSpecificationWorkItemsBySpecId(result.Key.Pk_SpecificationId);
                    //return specificationManager.GetSpecificationById(personId, specificationId);
                }
                return result;
            }        
        }

        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationBySearchCriteria(int personId, SpecificationSearch searchObject)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specificationManager = new SpecificationManager();
                specificationManager.UoW = uoW;
                var communityManager = new CommunityManager(uoW);
                KeyValuePair<List<Specification>, UserRightsContainer> result = specificationManager.GetSpecificationBySearchCriteria(personId, searchObject);
                result.Key.ForEach(x => x.PrimeResponsibleGroupShortName = (x.PrimeResponsibleGroup == null) ? String.Empty : communityManager.GetCommmunityshortNameById(x.PrimeResponsibleGroup.Fk_commityId));
                return specificationManager.GetSpecificationBySearchCriteria(personId, searchObject);
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

        public List<Enum_Technology> GetASpecificationTechnologiesBySpecId(int id)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specTechnologiesManager = new SpecificationTechnologiesManager();
                specTechnologiesManager.UoW = uoW;
                return specTechnologiesManager.GetASpecificationTechnologiesBySpecId(id);
            }
        }

        public List<WorkItem> GetSpecificationWorkItemsBySpecId(int id)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var specWorkItemsManager = new SpecificationWorkItemManager();
                specWorkItemsManager.UoW = uoW;
                return specWorkItemsManager.GetSpecificationWorkItemsBySpecId(id);
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
    }
}

