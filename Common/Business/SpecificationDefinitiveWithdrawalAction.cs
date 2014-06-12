using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecificationDefinitiveWithdrawalAction
    {
        #region Properties

        public IUltimateUnitOfWork _uoW { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="UoW">Ultimate UnitOfWork</param>
        public SpecificationDefinitiveWithdrawalAction(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Definitively withdraw a specification
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="currentReleaseId">Current Release ID</param>
        public void WithdrawDefinivelySpecification(int personId, int specificationId, int withdrawalMeetingId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = _uoW;
            //Get specification by identifier
            var spec = specMgr.GetSpecificationById(personId, specificationId).Key;
            if (spec == null)
            {
                throw new InvalidOperationException("Could not find the requested specification");
            }
            
            var persMgr = ManagerFactory.Resolve<IPersonManager>();
            persMgr.UoW = _uoW;

            DateTime actionDate = DateTime.Now;
            //Edit specification' release that are not withdrawn
            spec.Specification_Release.Where(e => !e.isWithdrawn.GetValueOrDefault()).ToList().ForEach(e => { e.isWithdrawn = true; e.WithdrawMeetingId = withdrawalMeetingId; e.UpdateDate = actionDate; e.EntityStatus = Enum_EntityStatus.Modified; });
            //Set the specification as withdrawn
            spec.IsActive = false;            
            spec.MOD_TS = actionDate;
            //spec.MOD_BY = persMgr.GetPersonDisplayName(personId);
            spec.EntityStatus = Enum_EntityStatus.Modified;
            //Update history
            IHistoryRepository historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = _uoW;
            historyRepo.InsertOrUpdate(new History()
            {
                Fk_PersonId = personId,
                CreationDate = actionDate,
                Fk_SpecificationId = specificationId,
                HistoryText = "Specification has been definitively withdrawn."
            });
        }
        #endregion
    }
}
