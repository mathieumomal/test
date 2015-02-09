using System;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationInhibitRemoveInhibitAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public bool SpecificationInhibitPromote(int personId, int specId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId).Key;

            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.OrderByDescending(x => x.Key.Release.SortOrder).FirstOrDefault();
            if (!rightsForRelease.Value.HasRight(Enum_UserRights.Specification_InhibitPromote))
            {
                throw new InvalidOperationException("User " + personId + " does not have right to inhibit promote");
            }

            //Set spec to promoteInhibited
            spec.promoteInhibited = true;
            
            return true;
        }

        public bool SpecificationRemoveInhibitPromote(int personId, int specId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specId).Key;

            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.OrderByDescending(x => x.Key.Release.SortOrder).FirstOrDefault();
            if (!rightsForRelease.Value.HasRight(Enum_UserRights.Specification_RemoveInhibitPromote))
            {
                throw new InvalidOperationException("User " + personId + " does not have right to remove inhibit promote");
            }

            //Remove promoteInhibited flag from spec
            spec.promoteInhibited = false;

            return true;

        }
    }

}
