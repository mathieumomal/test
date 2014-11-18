using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Linq;

namespace Etsi.Ultimate.Business
{
    public class SpecificationPromoteAction
    {
        #region Properties

        public IUltimateUnitOfWork _uoW { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="UoW">Ultimate UnitOfWork</param>
        public SpecificationPromoteAction(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Promote Specification to next release
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        public void PromoteSpecification(int personId, int specificationId, int currentReleaseId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = _uoW;
            var spec = specMgr.GetSpecificationById(personId, specificationId).Key;

            // Get the rights for all the releases.
            var rights = specMgr.GetRightsForSpecReleases(personId, spec);
            var rightsForRelease = rights.OrderByDescending(x => x.Key.Release.SortOrder).FirstOrDefault();
            if (!rightsForRelease.Value.HasRight(Enum_UserRights.Specification_Promote))
            {
                throw new InvalidOperationException("You don't have right to promote specification");
            }

            //Get latest Release
            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            var nextRelease = releaseManager.GetNextRelease(currentReleaseId);

            if (nextRelease == null)
                throw new InvalidOperationException("There is no next release found in the system. Hence, you cannot promote specification");

            //Add new Spec Release record to promote to next release
            spec.Specification_Release.Add(new Specification_Release() { isWithdrawn = false, CreationDate = DateTime.UtcNow, UpdateDate = DateTime.UtcNow, Fk_ReleaseId = nextRelease.Pk_ReleaseId });            
        }

        #endregion
    }
}
