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
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public KeyValuePair<Specification, ImportReport> Create(int personId, Specification spec)
        {
            // Create repository and report
            var repo = RepositoryFactory.Resolve<ISpecificationRepository>();
            repo.UoW = UoW;
            var report = new ImportReport();

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

            // Perform checks on specification
            CheckSpecification(spec);
            
            // Define default fields
            DefineDefaultValues(spec, personId);

            // If spec number is entered, link the serie.
            if (!string.IsNullOrEmpty(spec.Number))
            {
                var specSerie = spec.Number.Split('.')[0];
                var serie = repo.GetSeries().Where(s => s.Code == "SER_"+specSerie).FirstOrDefault();
                if (serie != null)
                    spec.Fk_SerieId = serie.Pk_Enum_SerieId;
            }

            // Create an history entry
            var releaseMgr = new ReleaseManager() { UoW = UoW };
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

            return new KeyValuePair<Specification,ImportReport>(spec, report);
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
        private void CheckSpecification(Specification spec)
        {
            // Specification should be linked to one release.
            if (spec.Specification_Release.Count != 1 || spec.Specification_Release.First().Fk_ReleaseId == default(int))
                throw new InvalidOperationException("Specification must be linked to one release");

            // Specification must have valid number.
            if (!String.IsNullOrEmpty(spec.Number))
            {
                var specMgr = new SpecificationManager() { UoW = UoW };
                var check = specMgr.CheckNumber(spec.Number);
                if (!check.Key)
                {
                    throw new InvalidOperationException("Specification number is invalid: " + String.Join(" # -- # ", check.Value));
                }
            }
        }
    }
}
