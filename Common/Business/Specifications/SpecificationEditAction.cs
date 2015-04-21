using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.ModelMails;

namespace Etsi.Ultimate.Business.Specifications
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
        public KeyValuePair<Specification, Report> EditSpecification(int personId, Specification spec)
        {
            // Check that user has right to perform operation
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;
            var report = new Report();
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

            //Check specification number if it changed
            if (!spec.Number.Equals(oldSpec.Number))
            {
                CheckNumber(spec, userRights, UoW, specRepo);
                MailAlertNumberEdited(spec, report);
            }

            //Default primary rapporteur rule : 
            //if only one rapporteur define -> should be Prime rapporteur by default
            if (spec.SpecificationRapporteurs != null && spec.SpecificationRapporteurs.Count == 1)
                spec.SpecificationRapporteurs.First().IsPrime = true;

            //Default primary WI rule :
            //if only one WI related to the spec -> should be Prime WI by default
            if (spec.Specification_WorkItem != null && spec.Specification_WorkItem.Count == 1)
                spec.Specification_WorkItem.First().isPrime = true;

            // Compare the fields of the two specifications.
            CompareSpecs(spec, oldSpec, personId, specRepo);

            // Set mod_ts and mod_by
            oldSpec.MOD_TS = DateTime.UtcNow;
            var personMgr = ManagerFactory.Resolve<IPersonManager>();
            personMgr.UoW = UoW;
            oldSpec.MOD_BY = personMgr.FindPerson(personId).Username;

            return new KeyValuePair<Specification, Report>(spec, report);
        }

        /// <summary>
        /// Check the spec number format
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="userRights"></param>
        /// <param name="uoW"></param>
        /// <param name="specRepo"></param>
        public static void CheckNumber(Specification spec, UserRightsContainer userRights, IUltimateUnitOfWork uoW, ISpecificationRepository specRepo)
        {
            //Throw an error if the user don't have the right to modify the spec number
            if (!userRights.HasRight(Enum_UserRights.Specification_EditFull))
            {
                throw new InvalidOperationException("You don't have the right to edit a specification number");
            }
            if (!String.IsNullOrEmpty(spec.Number))
            {
                var specMgr = new SpecificationManager { UoW = uoW };
                var check = specMgr.CheckFormatNumber(spec.Number);
                if (!check.Key)
                {
                    throw new InvalidOperationException("Specification number is invalid: " + String.Join(" # -- # ", check.Value));
                }
                var checkAlreadyExist = specMgr.LookForNumber(spec.Number);
                if (!checkAlreadyExist.Key)
                {
                    throw new InvalidOperationException("Specification number already exists : " + String.Join(" # -- # ", checkAlreadyExist.Value));
                }

                // Link serie
                var specSerie = spec.Number.Split('.')[0];
                var serie = specRepo.GetSeries().FirstOrDefault(s => s.Code == "SER_" + specSerie);
                if (serie != null)
                {
                    spec.Fk_SerieId = serie.Pk_Enum_SerieId;
                }

                //Check the spec number and define if this number define the spec as inhibit to promote
                specMgr.PutSpecAsInhibitedToPromote(spec);
            }
            else
            {
                spec.Fk_SerieId = null;
            }

        }

        /// <summary>
        /// Compares the specification, and modifies the old version.
        /// </summary>
        /// <param name="newSpec">Specification from UI</param>
        /// <param name="currentSpec">Specification from Database</param>
        /// <param name="personId">Person Id</param>
        /// <param name="specRepo">Specification Repository</param>
        private void CompareSpecs(Specification newSpec, Specification currentSpec, int personId, ISpecificationRepository specRepo)
        {
            if (currentSpec == null || newSpec == null)
                return;

            // Starting by the title
            if (currentSpec.Title != newSpec.Title) currentSpec.Title = newSpec.Title;
            if (currentSpec.IsTS != newSpec.IsTS) currentSpec.IsTS = newSpec.IsTS;
            if (currentSpec.IsForPublication != newSpec.IsForPublication) currentSpec.IsForPublication = newSpec.IsForPublication;
            if (currentSpec.promoteInhibited != newSpec.promoteInhibited) currentSpec.promoteInhibited = newSpec.promoteInhibited;
            if (currentSpec.ComIMS != newSpec.ComIMS) currentSpec.ComIMS = newSpec.ComIMS;

            //Number & serie
            if (currentSpec.Number != null && newSpec.Number != null)
            {
                if (!currentSpec.Number.Equals(newSpec.Number))
                {
                    currentSpec.Number = newSpec.Number;
                    currentSpec.Fk_SerieId = newSpec.Fk_SerieId;
                }
            }
            //Specification Technologies (Insert / Delete)
            var specTechnologiesToInsert = newSpec.SpecificationTechnologies.ToList().Where(x => currentSpec.SpecificationTechnologies.ToList().All(y => y.Fk_Enum_Technology != x.Fk_Enum_Technology));
            specTechnologiesToInsert.ToList().ForEach(x => currentSpec.SpecificationTechnologies.Add(x));
            var specTechnologiesToDelete = currentSpec.SpecificationTechnologies.ToList().Where(x => newSpec.SpecificationTechnologies.ToList().All(y => y.Fk_Enum_Technology != x.Fk_Enum_Technology));
            specTechnologiesToDelete.ToList().ForEach(x => specRepo.MarkDeleted(x));

            //Remarks (Insert / Update)
            var remarksToInsert = newSpec.Remarks.ToList().Where(x => currentSpec.Remarks.ToList().All(y => y.Pk_RemarkId != x.Pk_RemarkId)).ToList();
            remarksToInsert.ToList().ForEach(x => x.Fk_PersonId = personId);
            remarksToInsert.ToList().ForEach(x => currentSpec.Remarks.Add(x));
            var remarksToUpdate = newSpec.Remarks.ToList().Where(x => currentSpec.Remarks.ToList().Any(y => y.Pk_RemarkId == x.Pk_RemarkId && y.IsPublic != x.IsPublic));
            remarksToUpdate.ToList().ForEach(x => currentSpec.Remarks.ToList().Find(y => y.Pk_RemarkId == x.Pk_RemarkId).IsPublic = x.IsPublic);

            //WorkItems (Insert / Update / Delete)
            var specWorkItemToInsert = newSpec.Specification_WorkItem.ToList().Where(x => currentSpec.Specification_WorkItem.ToList().All(y => y.Fk_WorkItemId != x.Fk_WorkItemId));
            specWorkItemToInsert.ToList().ForEach(x => currentSpec.Specification_WorkItem.Add(x));
            var specWorkItemToUpdate = newSpec.Specification_WorkItem.ToList().Where(x => currentSpec.Specification_WorkItem.ToList().Any(y => y.Fk_WorkItemId == x.Fk_WorkItemId && y.isPrime != x.isPrime));
            specWorkItemToUpdate.ToList().ForEach(x => currentSpec.Specification_WorkItem.ToList().Find(y => y.Fk_WorkItemId == x.Fk_WorkItemId).isPrime = x.isPrime);
            var specWorkItemToDelete = currentSpec.Specification_WorkItem.ToList().Where(x => newSpec.Specification_WorkItem.ToList().All(y => y.Fk_WorkItemId != x.Fk_WorkItemId));
            specWorkItemToDelete.ToList().ForEach(specRepo.MarkDeleted);

            //Spec Parents (Insert / Delete)
            var specParentsToInsert = newSpec.SpecificationParents.ToList().Where(x => currentSpec.SpecificationParents.ToList().All(y => y.Pk_SpecificationId != x.Pk_SpecificationId));
            specParentsToInsert.ToList().ForEach(x =>
            {
                var spec = specRepo.Find(x.Pk_SpecificationId); //Here we have disconnected specification objects. So, load them from DB & then add. Otherwise, EF will try to insert new Specification
                if (spec != null)
                    currentSpec.SpecificationParents.Add(spec);
            });
            var specParentsToDelete = currentSpec.SpecificationParents.ToList().Where(x => newSpec.SpecificationParents.ToList().All(y => y.Pk_SpecificationId != x.Pk_SpecificationId));
            specParentsToDelete.ToList().ForEach(x => currentSpec.SpecificationParents.Remove(x));

            //Spec Childs (Insert / Delete)
            var specChildsToInsert = newSpec.SpecificationChilds.ToList().Where(x => currentSpec.SpecificationChilds.ToList().All(y => y.Pk_SpecificationId != x.Pk_SpecificationId));
            specChildsToInsert.ToList().ForEach(x =>
            {
                var spec = specRepo.Find(x.Pk_SpecificationId); //Here we have disconnected specification objects. So, load them from DB & then add. Otherwise, EF will try to insert new Specification
                if (spec != null)
                    currentSpec.SpecificationChilds.Add(spec);
            });
            var specChildsToDelete = currentSpec.SpecificationChilds.ToList().Where(x => newSpec.SpecificationChilds.ToList().All(y => y.Pk_SpecificationId != x.Pk_SpecificationId));
            specChildsToDelete.ToList().ForEach(x => currentSpec.SpecificationChilds.Remove(x));

            //Responsible Groups (History / Insert / Update / Delete)
            // Log an history entry if prime responsible group differ.
            var newPrimeRespGroup = newSpec.SpecificationResponsibleGroups.FirstOrDefault(g => g.IsPrime).Fk_commityId;
            var oldPrimeRespGroup = currentSpec.SpecificationResponsibleGroups.FirstOrDefault(g => g.IsPrime).Fk_commityId;
            if (newPrimeRespGroup != oldPrimeRespGroup)
            {
                var communityMgr = ManagerFactory.Resolve<ICommunityManager>();
                communityMgr.UoW = UoW;
                var communities = communityMgr.GetCommunities();

                var newCom = communities.FirstOrDefault(c => c.TbId == newPrimeRespGroup);
                var oldCom = communities.FirstOrDefault(c => c.TbId == oldPrimeRespGroup);
                if (newCom != null && oldCom != null)
                {
                    currentSpec.Histories.Add(new History
                    {
                        Fk_SpecificationId = currentSpec.Pk_SpecificationId,
                        CreationDate = DateTime.UtcNow,
                        Fk_PersonId = personId,
                        HistoryText = String.Format(Localization.History_Specification_Changed_Prime_Group, newCom.TbName, oldCom.TbName),

                    });
                }
            }

            //Primary Responsible Group
            var oldPrimaryResponsibleGroup = currentSpec.SpecificationResponsibleGroups.ToList().FirstOrDefault(x => x.IsPrime);
            var newPrimaryResponsibleGroup = newSpec.SpecificationResponsibleGroups.ToList().FirstOrDefault(x => x.IsPrime);
            if (oldPrimaryResponsibleGroup != null && newPrimaryResponsibleGroup != null && oldPrimaryResponsibleGroup.Fk_commityId != newPrimaryResponsibleGroup.Fk_commityId)
                oldPrimaryResponsibleGroup.Fk_commityId = newPrimaryResponsibleGroup.Fk_commityId;

            //Secondary Responsible Groups
            var oldSecondaryResponsibleGroups = currentSpec.SpecificationResponsibleGroups.ToList().Where(x => !x.IsPrime).ToList();
            var newSecondaryResponsibleGroups = newSpec.SpecificationResponsibleGroups.ToList().Where(x => !x.IsPrime).ToList();

            var specResponsibleGroupsToInsert = newSecondaryResponsibleGroups.Where(x => oldSecondaryResponsibleGroups.All(y => y.Fk_commityId != x.Fk_commityId));
            specResponsibleGroupsToInsert.ToList().ForEach(x => currentSpec.SpecificationResponsibleGroups.Add(x));
            var specResponsibleGroupsToDelete = oldSecondaryResponsibleGroups.Where(x => newSecondaryResponsibleGroups.All(y => y.Fk_commityId != x.Fk_commityId));
            specResponsibleGroupsToDelete.ToList().ForEach(specRepo.MarkDeleted);

            //Rapporteurs (History / Insert / Update / Delete)
            // Log an history entry if prime rapporteur changed.
            var newPrimeRapporteur = newSpec.SpecificationRapporteurs.FirstOrDefault(r => r.IsPrime);
            var oldPrimeRapporteur = currentSpec.SpecificationRapporteurs.FirstOrDefault(r => r.IsPrime);

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
                personManager.UoW = UoW;
                var newRapporteurName = "(None)";
                var oldRapporteurName = "(None)";

                if (newPrimeRapporteurId.HasValue)
                {
                    var newRappPerson = personManager.FindPerson(newPrimeRapporteurId.Value);
                    newRapporteurName = newRappPerson.FIRSTNAME + " " + newRappPerson.LASTNAME;
                }
                if (oldPrimeRapporteurId.HasValue)
                {
                    var oldRappPerson = personManager.FindPerson(oldPrimeRapporteurId.Value);
                    oldRapporteurName = oldRappPerson.FIRSTNAME + " " + oldRappPerson.LASTNAME;
                }

                currentSpec.Histories.Add(new History
                {
                    Fk_PersonId = personId,
                    HistoryText = String.Format(Localization.History_Specification_Changed_Prime_Rapporteur, newRapporteurName, oldRapporteurName),
                    CreationDate = DateTime.UtcNow,
                    Fk_SpecificationId = currentSpec.Pk_SpecificationId

                });
            }

            var specRapporteursToInsert = newSpec.SpecificationRapporteurs.ToList().Where(x => currentSpec.SpecificationRapporteurs.ToList().All(y => y.Fk_RapporteurId != x.Fk_RapporteurId));
            specRapporteursToInsert.ToList().ForEach(x => currentSpec.SpecificationRapporteurs.Add(x));
            var specRapporteursToUpdate = newSpec.SpecificationRapporteurs.ToList().Where(x => currentSpec.SpecificationRapporteurs.ToList().Any(y => y.Fk_RapporteurId == x.Fk_RapporteurId && y.IsPrime != x.IsPrime));
            specRapporteursToUpdate.ToList().ForEach(x => currentSpec.SpecificationRapporteurs.ToList().Find(y => y.Fk_RapporteurId == x.Fk_RapporteurId).IsPrime = x.IsPrime);
            var specRapporteursToDelete = currentSpec.SpecificationRapporteurs.ToList().Where(x => newSpec.SpecificationRapporteurs.ToList().All(y => y.Fk_RapporteurId != x.Fk_RapporteurId));
            specRapporteursToDelete.ToList().ForEach(specRepo.MarkDeleted);
        }

        /// <summary>
        /// If we edit the spec number we send a mail to the workplan manager and Secretary of the prime responsible working group 
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="report"></param>
        private void MailAlertNumberEdited(Specification spec, Report report)
        {
            var specWorkitemManager = new SpecificationWorkItemManager { UoW = UoW };
            var roleManager = new RolesManager { UoW = UoW };
            var personManager = new PersonManager { UoW = UoW };

            var subject = String.Format(Localization.Specification_ReferenceNumberAssigned_Subject, spec.Number);
            var listSpecWiLabel = specWorkitemManager.GetSpecificationWorkItemsLabels(spec.Pk_SpecificationId);

            //Send to workplan manager
            var workplanMgrsEmail = roleManager.GetWpMgrEmail();
            var to = workplanMgrsEmail;

            //Send to Prime Responsible Group secretaries as well
            var primeResponsibleGroupCommityId = spec.PrimeResponsibleGroup.Fk_commityId;
            var listSecretariesEmail = personManager.GetEmailSecretariesFromAPrimeResponsibleGroupByCommityId(primeResponsibleGroupCommityId);
            to = to.Concat(listSecretariesEmail).ToList();

            var body = new SpecReferenceNumberAssignedMailTemplate((String.IsNullOrEmpty(spec.Number) ? "" : spec.Number), (String.IsNullOrEmpty(spec.Title) ? "" : spec.Title), listSpecWiLabel);
            var mailInstance = UtilsFactory.Resolve<IMailManager>();
            if (!mailInstance.SendEmail(null, to, null, null, subject, body.TransformText()))
            {
                report.LogError(Localization.Specification_ERR101_FailedToSendEmailToSecretaryAndWorkplanManager);
            }
        }
    }
}
