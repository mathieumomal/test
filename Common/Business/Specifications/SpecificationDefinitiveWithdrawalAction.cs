using System;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationDefinitiveWithdrawalAction
    {
        #region Properties

        public IUltimateUnitOfWork UoW { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uoW">Ultimate UnitOfWork</param>
        public SpecificationDefinitiveWithdrawalAction(IUltimateUnitOfWork uoW)
        {
            UoW = uoW;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Definitively withdraw a specification
        /// </summary>
        /// <param name="personId">Person ID</param>
        /// <param name="specificationId">Specification ID</param>
        /// <param name="withdrawalMeetingId"></param>
        public void WithdrawDefinivelySpecification(int personId, int specificationId, int withdrawalMeetingId)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            //Get specification by identifier
            var spec = specMgr.GetSpecificationById(personId, specificationId).Key;
            if (spec == null)
            {
                throw new InvalidOperationException("Could not find the requested specification");
            }
            
            var persMgr = ManagerFactory.Resolve<IPersonManager>();
            persMgr.UoW = UoW;

            DateTime actionDate = DateTime.Now;
            //Edit specification' release that are not withdrawn
            spec.Specification_Release.Where(e => !e.isWithdrawn.GetValueOrDefault()).ToList().ForEach(e => { e.isWithdrawn = true; e.WithdrawMeetingId = withdrawalMeetingId; e.UpdateDate = actionDate; e.EntityStatus = Enum_EntityStatus.Modified; });
            //Set the specification as withdrawn
            spec.IsActive = false;            
            spec.MOD_TS = actionDate;
            //spec.MOD_BY = persMgr.GetPersonDisplayName(personId);
            spec.EntityStatus = Enum_EntityStatus.Modified;
            //Update history
            var historyRepo = RepositoryFactory.Resolve<IHistoryRepository>();
            historyRepo.UoW = UoW;
            historyRepo.InsertOrUpdate(new History
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
