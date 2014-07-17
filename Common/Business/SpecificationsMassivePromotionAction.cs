﻿using Etsi.Ultimate.Repositories;
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
        public IUltimateUnitOfWork UoW { get; set; }
        #endregion

        #region Constructor
        public SpecificationsMassivePromotionAction() { }
        #endregion

        #region Methods
        public KeyValuePair<List<Specification>, UserRightsContainer> GetSpecificationForMassivePromotion(int personId, int initialReleaseId, int targetReleaseId)
        {
            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = UoW;
            var personRights = rightManager.GetRights(personId);

            var releaseManager = ManagerFactory.Resolve<IReleaseManager>();
            releaseManager.UoW = UoW;

            // Get source release Specifications 
            var SourceSpecs = new  List<Specification>();
            releaseManager.GetReleaseById(personId, initialReleaseId).Key.Specification_Release.ToList().ForEach(e => SourceSpecs.Add(e.Specification));
            
            
            // Get target release Specifications 
            var targetReleaseSpecIds = new  List<int>();
            releaseManager.GetReleaseById(personId, targetReleaseId).Key.Specification_Release.ToList().ForEach(e => targetReleaseSpecIds.Add(e.Fk_SpecificationId));

            //Remove element exisiting in target release
            SourceSpecs.RemoveAll(s => targetReleaseSpecIds.Contains(s.Pk_SpecificationId));
            SourceSpecs.RemoveAll(s => !s.IsActive);
            //If specification is not a draft, not inhibited from promotion, and has a version in the initial relase => New version allocation is enabled
            //Get specification ids havig a version for the initial release 
            ISpecVersionsRepository versionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            versionRepo.UoW = UoW;
            List<int> buffer = new List<int>();
            versionRepo.GetVersionsByReleaseId(initialReleaseId).ForEach(v => buffer.Add(v.Fk_SpecificationId.GetValueOrDefault()));
            SourceSpecs.Where(s => (!s.promoteInhibited.GetValueOrDefault()) && !(s.IsActive && !s.IsUnderChangeControl.GetValueOrDefault()) && (buffer.Contains(s.Pk_SpecificationId))).ToList().ForEach(s => { s.IsNewVersionCreationEnabled = true; });

            var communityManager = ManagerFactory.Resolve<ICommunityManager>();
            communityManager.UoW = UoW;
            foreach (Specification s in SourceSpecs)
            {
                if (s.SpecificationResponsibleGroups != null && s.SpecificationResponsibleGroups.Count > 0 && s.PrimeResponsibleGroup != null)
                {
                    s.PrimeResponsibleGroupFullName = communityManager.GetCommmunityById(s.PrimeResponsibleGroup.Fk_commityId).TbName;
                    s.PrimeResponsibleGroupShortName = communityManager.GetCommmunityById(s.PrimeResponsibleGroup.Fk_commityId).ShortName;
                }
            }

            return new KeyValuePair<List<Specification>, UserRightsContainer>(SourceSpecs, personRights); 
        }

        public void PromoteMassivelySpecification(int personId, List<int> specificationIds, int targetReleaseId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            DateTime actionDate = DateTime.UtcNow;
            foreach (int id in specificationIds)
            {
                specMgr.GetSpecificationById(personId, id).Key.Specification_Release.Add(new Specification_Release() { isWithdrawn = false, CreationDate = actionDate, UpdateDate = actionDate, Fk_ReleaseId = targetReleaseId });
            }
        }
        #endregion
    }
}
