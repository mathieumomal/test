using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.ModelMails;

namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// This class is in charge of all the business logic concerning the creation of a specification.
    /// 
    /// Note: Unit tests for this class are located in the SpecificationServiceTest file.
    /// 
    /// TODO: implement the email that should be sent:
    /// --> If spec number is assigned (Specification reference number assigned)
    /// --> Or spec number is not assigned and pending assignment by Spec manager (New specification awaiting reference number)
    /// </summary>
    public class SpecificationCreateAction
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Verifies all the fields of a specification, then send it to database.
        /// Number case :
        /// - In the case of an MCC MEMBER (have the Specification_EditLimitted right) WE NEED TO SEND A NULL OR EMPTY SPEC NUMBER 
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public KeyValuePair<Specification, Report> Create(int personId, Specification spec, string baseurl)
        {
            // Create repository and report
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var report = new Report();

            // Specification must not already have a primary key.
            if (spec.Pk_SpecificationId != default(int))
            {
                throw new InvalidOperationException("Cannot create specification that already has a primary key");
            }

            // Check user rights. User must have rights to create specifications
            var rightsMgr = ManagerFactory.Resolve<IRightsManager>();
            rightsMgr.UoW = UoW;
            var userRights = rightsMgr.GetRights(personId);
            if (!userRights.HasRight(Enum_UserRights.Specification_Create))
            {
                throw new InvalidOperationException("User " + personId + " is not allowed to create a specification");
            }

            // Perform checks on specification and associated serie
            CheckSpecificationAndSerie(spec, userRights, repo);
            
            // Define default fields
            DefineDefaultValues(spec, personId);

            //Replace disconnected objects (Spec Parents/Childs) with context objects
            ReplaceDisconnectedObjects(spec, repo);

            // Create an history entry
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW ;

            var releaseShortName = releaseMgr.GetReleaseById(0, spec.Specification_Release.First().Fk_ReleaseId).Key.ShortName;
            spec.Histories.Clear();
            spec.Histories.Add(
                new History()
                {
                    HistoryText = String.Format(Utils.Localization.History_Specification_Created, releaseShortName),
                    CreationDate = DateTime.UtcNow,
                    Fk_PersonId = personId
                }
            );

            
            repo.InsertOrUpdate(spec);

            //If the user didn't have the right to edit spec number we send a mail to the spec manager
            if (!userRights.HasRight(Enum_UserRights.Specification_EditFull))
            {
                MailAlertSpecManager(spec, report, baseurl, personId);
            }

            if (report.GetNumberOfErrors() == 0 && report.GetNumberOfWarnings() == 0)
                report.LogInfo(Localization.Specification_MSG002_SpecCreatedMailSendToSpecManager);
            return new KeyValuePair<Specification,Report>(spec, report);
        }

        /// <summary>
        /// Replace disconnected objects (Spec Parents/Childs) with context objects
        /// </summary>
        /// <param name="spec">Specification</param>
        /// <param name="specRepo">Specification Repository</param>
        private void ReplaceDisconnectedObjects(Specification spec, ISpecificationRepository specRepo)
        {
            //Specification Parents
            List<Specification> specParents = new List<Specification>();
            spec.SpecificationParents.ToList().ForEach(x =>
            {
                var specParent = specRepo.Find(x.Pk_SpecificationId); //Here we have disconnected specification objects. So, load them from DB & then add. Otherwise, EF will try to insert new Specification
                if (specParent != null)
                    specParents.Add(specParent);
            });
            spec.SpecificationParents.Clear();
            specParents.ForEach(x => spec.SpecificationParents.Add(x));

            //Specification Childs
            List<Specification> specChilds = new List<Specification>();
            spec.SpecificationChilds.ToList().ForEach(x =>
            {
                var specChild = specRepo.Find(x.Pk_SpecificationId); //Here we have disconnected specification objects. So, load them from DB & then add. Otherwise, EF will try to insert new Specification
                if (specChild != null)
                    specChilds.Add(specChild);
            });
            spec.SpecificationChilds.Clear();
            specChilds.ForEach(x => spec.SpecificationChilds.Add(x));
        }

        /// <summary>
        /// Sets up the default values for a specification at creation time.
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="personId"></param>
        private void DefineDefaultValues(Specification spec, int personId)
        {
            // New spec is by default active and not under change control
            spec.IsActive = true;
            spec.IsUnderChangeControl = false;

            // Specification is by default a TS.
            if (!spec.IsTS.HasValue)
            {
                spec.IsTS = true;
            }

            // Set default values for remarks.
            foreach (var remark in spec.Remarks)
            {
                remark.Fk_PersonId = personId;
                remark.CreationDate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Checks the specification. In particular, ensures that:
        /// - Spec release record exists.
        /// - Spec number is valid.
        /// </summary>
        /// <param name="spec"></param>
        private void CheckSpecificationAndSerie(Specification spec, UserRightsContainer userRights,ISpecificationRepository specRepo)
        {
            // Specification should be linked to one release.
            if (spec.Specification_Release.Count != 1 || spec.Specification_Release.First().Fk_ReleaseId == default(int))
                throw new InvalidOperationException("Specification must be linked to one release");

            // Specification must have valid number.
            if (!String.IsNullOrEmpty(spec.Number) && !userRights.HasRight(Enum_UserRights.Specification_EditFull))
                throw new InvalidOperationException("You don't have the right to create a specification number");
            else if (!String.IsNullOrEmpty(spec.Number))
            {
                var specMgr = new SpecificationManager() { UoW = UoW };
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

                //promoteInhibited
                spec.promoteInhibited = specMgr.CheckInhibitedToPromote(spec.Number);

                // If spec number is entered, link the serie.
                var specSerie = spec.Number.Split('.')[0];
                var serie = specRepo.GetSeries().Where(s => s.Code == "SER_" + specSerie).FirstOrDefault();
                if (serie != null)
                    spec.Fk_SerieId = serie.Pk_Enum_SerieId;
            }
        }

        /// <summary>
        /// Send a mail to the Spec Manager(s)
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="report"></param>
        private void MailAlertSpecManager(Specification spec , Report report, string baseurl, int personId)
        {
            //get connected user name
            var connectedUsername = String.Empty;
            var personManager = new PersonManager();
            personManager.UoW = UoW;
            var connectedUser = personManager.FindPerson(personId);
            if (connectedUser != null)
            {
                connectedUsername = new StringBuilder()
                    .Append((connectedUser.FIRSTNAME != null) ? connectedUser.FIRSTNAME : "")
                    .Append(" ")
                    .Append((connectedUser.LASTNAME != null) ? connectedUser.LASTNAME : "")
                    .ToString();
            }
            
            //Subject
            var specTitleSubject = String.Empty;
            if (spec.Title != null && spec.Title.Length > 60)
                specTitleSubject = spec.Title.Substring(0, 60);
            else
                specTitleSubject = spec.Title;
            var subject = String.Format(Localization.Specification_AwaitingReferenceNumberMail_Subject, specTitleSubject);

            //Body
            var specUrl = new StringBuilder().Append("/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=").Append(spec.Pk_SpecificationId).ToString();
            var specLink = new StringBuilder().Append(baseurl).Append(specUrl).ToString();
            var body = new SpecAwaitingReferenceNumberMailTemplate(connectedUsername, (String.IsNullOrEmpty(spec.Title) ? "" : spec.Title), specLink);

            var roleManager = new RolesManager();
            roleManager.UoW = UoW;
            var to = roleManager.GetSpecMgrEmail();

            var mailManager = UtilsFactory.Resolve<IMailManager>();
            if (!mailManager.SendEmail(null, to, null, null, subject, body.TransformText()))
            {
                report.LogError(Localization.Specification_ERR001_FailedToSendEmailToSpecManagers);
            }
        }
    }
}
