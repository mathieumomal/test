using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// This class is in charge of editing an existing specification, which encompasses:
    /// - Retrieving the specification
    /// - Comparing the values two by two and checking if any history entry should be added.
    /// - Sending email if a specification Number has been assigned.
    /// </summary>
    public class SpecificationEditAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Only entry point for the SpecificationEditAction class. Does perform the edit of the specification
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool EditSpecification(int personId, Specification spec)
        {
            // Check that user has right to perform operation
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;
            var userRights = rightsMgr.GetRights(personId);
            if (!userRights.HasRight(Enum_UserRights.Specification_EditFull) && !userRights.HasRight(Enum_UserRights.Specification_EditLimitted))
            {
                throw new InvalidOperationException("User not allowed to edit a specification");
            }

            // Check that specification exists
            var specRepo = RepositoryFactory.Resolve<ISpecificationRepository>();
            specRepo.UoW = UoW;
            var oldSpec = specRepo.Find(spec.Pk_SpecificationId);
            if (oldSpec == null)
            {
                throw new InvalidOperationException("Edited specification does not exist");
            }

            // Compare the fields of the two specifications.
            CompareSpecs(spec, oldSpec, personId);
            specRepo.InsertOrUpdate(oldSpec);

            return true;
        }

        /// <summary>
        /// Compares the specification, and modifies the old version.
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="oldSpec"></param>
        private void CompareSpecs(Specification newSpec, Specification currentSpec, int personId)
        {
            // Starting by the title
            currentSpec.Title = newSpec.Title;
            currentSpec.IsTS = newSpec.IsTS;
            currentSpec.IsForPublication = newSpec.IsForPublication;
            currentSpec.ComIMS = newSpec.ComIMS;

            // Manage the radio technologies
            foreach (var tech in newSpec.SpecificationTechnologies)
            {
                // If technology does not exist, add it.
                if (currentSpec.SpecificationTechnologies.Where(t => t.Fk_Enum_Technology == tech.Fk_Enum_Technology).FirstOrDefault() == null)
                {
                    currentSpec.SpecificationTechnologies.Add(tech);
                    tech.EntityStatus = Enum_EntityStatus.New;
                }
            }
            foreach (var tech in currentSpec.SpecificationTechnologies)
            {
                // If technology does not exist in the new spec, it means it's been deleted.
                if (newSpec.SpecificationTechnologies.Where(t => t.Fk_Enum_Technology == tech.Fk_Enum_Technology).FirstOrDefault() == null)
                {
                    tech.EntityStatus = Enum_EntityStatus.Deleted;
                }
            }

            // Manage the remarks. Users cannot delete remarks.
            foreach (var rk in newSpec.Remarks)
            {
                if (rk.Pk_RemarkId != default(int))
                {
                    var updRk = currentSpec.Remarks.Where(r => r.Pk_RemarkId == rk.Pk_RemarkId).FirstOrDefault();
                    if (updRk != null)
                        updRk.IsPublic = rk.IsPublic;
                }
                else
                {
                    rk.Fk_PersonId = personId;
                    rk.Fk_SpecificationId = newSpec.Pk_SpecificationId;
                    currentSpec.Remarks.Add(rk);
                }
            }

            // Manage responsible groups
            // Log an history entry if prime responsible group differ.
            var newPrimeRespGroup = newSpec.SpecificationResponsibleGroups.Where(g => g.IsPrime).FirstOrDefault().Fk_commityId;
            var oldPrimeRespGroup = currentSpec.SpecificationResponsibleGroups.Where(g => g.IsPrime).FirstOrDefault().Fk_commityId;
            if (newPrimeRespGroup != oldPrimeRespGroup)
            {
                var communityMgr = ManagerFactory.Resolve<ICommunityManager>();
                communityMgr.UoW = UoW;
                var communities = communityMgr.GetCommunities();

                var newCom = communities.Where(c => c.TbId == newPrimeRespGroup).FirstOrDefault();
                var oldCom = communities.Where(c => c.TbId == oldPrimeRespGroup).FirstOrDefault();
                if (newCom != null && oldCom != null)
                {
                    currentSpec.Histories.Add(new History()
                    {
                        Fk_SpecificationId = currentSpec.Pk_SpecificationId,
                        CreationDate = DateTime.UtcNow,
                        Fk_PersonId = personId,
                        HistoryText = String.Format(Utils.Localization.History_Specification_Changed_Prime_Group, newCom.TbName, oldCom.TbName),
                        
                    });

                }
            }
           
            // List all the additions and modifications
            foreach (var group in newSpec.SpecificationResponsibleGroups)
            {
                var currentGroup = currentSpec.SpecificationResponsibleGroups.Where(g => g.Fk_commityId == group.Fk_commityId).FirstOrDefault();
                if ( currentGroup == null)
                {
                    group.Fk_SpecificationId = currentSpec.Pk_SpecificationId;
                    currentSpec.SpecificationResponsibleGroups.Add(group);
                }
                else
                {
                    currentGroup.IsPrime = group.IsPrime;
                }
            }

            // List all the deletions
            foreach (var group in currentSpec.SpecificationResponsibleGroups)
            {
                var newGroup = newSpec.SpecificationResponsibleGroups.Where(g => g.Fk_commityId == group.Fk_commityId).FirstOrDefault();
                if (newGroup == null)
                {
                    group.EntityStatus = Enum_EntityStatus.Deleted;
                }
            }



            // Manage rapporteurs
            var newPrimeRapporteur = newSpec.SpecificationRapporteurs.Where(r => r.IsPrime).FirstOrDefault();
            var oldPrimeRapporteur = currentSpec.SpecificationRapporteurs.Where(r => r.IsPrime).FirstOrDefault();

            int? newPrimeRapporteurId = null;
            int? oldPrimeRapporteurId = null;

            if (newPrimeRapporteur != null)
                newPrimeRapporteurId = newPrimeRapporteur.Fk_RapporteurId;
            if (oldPrimeRapporteur != null)
                oldPrimeRapporteurId = oldPrimeRapporteur.Fk_RapporteurId;

            // Check rapporteur change
            if (newPrimeRapporteurId.GetValueOrDefault() != oldPrimeRapporteurId.GetValueOrDefault())
            {
                var personManager = ManagerFactory.Resolve<IPersonManager>();
                personManager.UoW = UoW ;
                var newRapporteurName = "(None)";
                var oldRapporteurName = "(None)";

                if (newPrimeRapporteurId.GetValueOrDefault() != 0)
                {
                    var newRappPerson = personManager.FindPerson(newPrimeRapporteurId.Value);
                    newRapporteurName = newRappPerson.FIRSTNAME + " " + newRappPerson.LASTNAME;
                }
                if (oldPrimeRapporteurId.GetValueOrDefault() != 0)
                {
                    var oldRappPerson = personManager.FindPerson(oldPrimeRapporteurId.Value);
                    oldRapporteurName = oldRappPerson.FIRSTNAME + " " + oldRappPerson.LASTNAME;
                }

                currentSpec.Histories.Add(new History()
                {
                    Fk_PersonId = personId,
                    HistoryText = String.Format(Utils.Localization.History_Specification_Changed_Prime_Rapporteur, newRapporteurName, oldRapporteurName),
                    CreationDate = DateTime.UtcNow,
                    Fk_SpecificationId = currentSpec.Pk_SpecificationId

                });

            }

            // Check rapporteur addition           
            foreach (var rapp in newSpec.SpecificationRapporteurs)
            {
                var currentRapp = currentSpec.SpecificationRapporteurs.Where(g => g.Fk_RapporteurId == rapp.Fk_RapporteurId).FirstOrDefault();
                if (currentRapp == null)
                {
                    rapp.Fk_SpecificationId = currentSpec.Pk_SpecificationId;
                    currentSpec.SpecificationRapporteurs.Add(rapp);
                }
                else
                {
                    currentRapp.IsPrime = rapp.IsPrime;
                }
            }

            //  Check rapporteur deletion
            foreach (var rapp in currentSpec.SpecificationRapporteurs)
            {
                var newRapp = newSpec.SpecificationRapporteurs.Where(g => g.Fk_RapporteurId== rapp.Fk_RapporteurId).FirstOrDefault();
                if (newRapp == null)
                {
                    rapp.EntityStatus = Enum_EntityStatus.Deleted;
                }
            }


            // ------------ Check work items -------------
            // Check work item addition           
            foreach (var wi in newSpec.Specification_WorkItem)
            {
                var newWi = currentSpec.Specification_WorkItem.Where(g => g.Fk_WorkItemId == wi.Fk_WorkItemId).FirstOrDefault();
                if (newWi == null)
                {
                    wi.Fk_SpecificationId = currentSpec.Pk_SpecificationId;
                    currentSpec.Specification_WorkItem.Add(wi);
                }
                else
                {
                    newWi.isPrime = wi.isPrime;
                }
            }

            //  Check work item deletion
            foreach (var wi in currentSpec.Specification_WorkItem)
            {
                var newWi = newSpec.Specification_WorkItem.Where(g => g.Fk_WorkItemId== wi.Fk_WorkItemId).FirstOrDefault();
                if (newWi == null)
                {
                    wi.EntityStatus = Enum_EntityStatus.Deleted;
                }
            }


            // -------------- Check parent specs
            // Parent spec addition
            foreach (var sp in newSpec.SpecificationParents)
            {
                var newSp = currentSpec.SpecificationParents.Where(g => g.Pk_SpecificationId == sp.Pk_SpecificationId).FirstOrDefault();
                if (newSp == null)
                    currentSpec.SpecificationParents.Add(sp);
            }
            if (newSpec.SpecificationParents != null)
            {
                var specTodelete = new List<Specification>();
                foreach (var sp in currentSpec.SpecificationParents)
                {
                    var newSp = newSpec.SpecificationParents.Where(g => g.Pk_SpecificationId == sp.Pk_SpecificationId).FirstOrDefault();
                    if (newSp == null)
                        specTodelete.Add(sp);
                }
                specTodelete.ForEach(x => currentSpec.SpecificationParents.Remove(x));
            }
        }
    }
}
