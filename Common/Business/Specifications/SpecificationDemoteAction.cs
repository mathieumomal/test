using System;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationDemoteAction
    {
        #region Properties

        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uoW">Ultimate UnitOfWork</param>
        public SpecificationDemoteAction(IUltimateUnitOfWork uoW)
        {
            UoW = uoW;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Demote Specification to previous release
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        public void DemoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var spec = specMgr.GetSpecificationById(personId, specificationId).Key;

            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.OrderBy(x => x.Key.Release.SortOrder).FirstOrDefault();
            if (!rightsForRelease.Value.HasRight(Enum_UserRights.Specification_Demote))
            {
                throw new InvalidOperationException("You don't have right to demote specification");
            }

            //Get previous Release
            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            var previousRelease = releaseManager.GetPreviousRelease(currentReleaseId);

            if (previousRelease == null)
                throw new InvalidOperationException("There is no previous release found in the system. Hence, you cannot demote specification");

            //Add new Spec Release record to promote to next release
            spec.Specification_Release.Add(new Specification_Release { isWithdrawn = false,  CreationDate = DateTime.UtcNow, UpdateDate = DateTime.UtcNow, Fk_ReleaseId = previousRelease.Pk_ReleaseId });            
        }

        #endregion
    }
}
