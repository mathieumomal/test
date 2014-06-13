using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;

namespace Etsi.Ultimate.Business
{
    public class SpecificationsMassivePromotionAction
    {
        #region Properties

        public IUltimateUnitOfWork _uoW { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="UoW">Ultimate UnitOfWork</param>
        public SpecificationsMassivePromotionAction(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Methods
        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId, int targetReleaseId)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            var personRights = rightManager.GetRights(personId);

            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            releaseManager.UoW = _uoW;

            // Get source release Specifications 
            var SourceSpecs = new  List<Specification>();
            releaseManager.GetReleaseById(personId, initialReleaseId).Key.Specification_Release.ToList().ForEach(e => SourceSpecs.Add(e.Specification));
            
            
            // Get target release Specifications 
            var targetReleaseSpecIds = new  List<int>();
            releaseManager.GetReleaseById(personId, targetReleaseId).Key.Specification_Release.ToList().ForEach(e => targetReleaseSpecIds.Add(e.Fk_SpecificationId));

            //Remove element exisiting in target release
            SourceSpecs.RemoveAll(s => targetReleaseSpecIds.Contains(s.Pk_SpecificationId));
            //If specification is not a draft, not inhibited from promotion, and has a version in the initial relase => New version allocation is enabled
            //Get specification ids havig a version for the initial release 
            ISpecVersionsRepository versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepo.UoW = _uoW;
            List<int> buffer = new List<int>();
            versionRepo.GetVersionsByReleaseId(initialReleaseId).ForEach(v => buffer.Add(v.Fk_SpecificationId.GetValueOrDefault()));
            SourceSpecs.Where(s => (!s.promoteInhibited.GetValueOrDefault()) && !(s.IsActive && !s.IsUnderChangeControl.GetValueOrDefault()) && (buffer.Contains(s.Pk_SpecificationId))).ToList().ForEach(s => { s.IsNewVersionCreationEnabled = true; });

            return new KeyValuePair<List<Specification>, UserRightsContainer>(SourceSpecs, personRights); 
        }
        #endregion
    }
}
