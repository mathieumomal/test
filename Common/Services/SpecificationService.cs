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
                result.Key.SpecificationResponsibleGroups.ToList().ForEach(g => secondaryRGShortName.Add(communityManager.GetCommmunityshortNameById(g.Fk_commityId)));                
                result.Key.SecondaryResponsibleGroupsShortNames= string.Join(",", secondaryRGShortName.ToArray());
                result.Key.SpecificationParents.ToList().ForEach(s => s.PrimeResponsibleGroupShortName  = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId));
                result.Key.SpecificationChilds.ToList().ForEach(s => s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityshortNameById(s.PrimeResponsibleGroup.Fk_commityId)); 
                return specificationManager.GetSpecificationById(personId, specificationId);
            }        
        }
    }
}
