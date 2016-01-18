using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public enum Enum_ChangeRequestStatuses { Agreed, Approved, Noted, Postponed, Rejected, Revised, Merged, TechEndorsed, Withdrawn, Reissued };
    /// <summary>
    /// Default implementation of the IChangeRequestStatusManager
    /// </summary>
    public class ChangeRequestStatusManager: IChangeRequestStatusManager
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        private const string CacheKey = "ULTIMATE_BIZ_CHANGEREQUEST_STATUSES";

        /// <summary>
        /// Fetches the list of all change request statuses by the repository. 
        /// Caches the information.
        /// </summary>
        /// <returns></returns>
        public List<Enum_ChangeRequestStatus> GetAllChangeRequestStatuses()
        {
            var cachedData = (List<Enum_ChangeRequestStatus>)CacheManager.Get(CacheKey);
            if(cachedData == null)
            {
                var crStatusRepo = RepositoryFactory.Resolve<IChangeRequestStatusRepository>();
                crStatusRepo.UoW = UoW;

                cachedData = crStatusRepo.All.ToList();
                CacheManager.Insert(CacheKey, cachedData);
            }
            return cachedData;
        }

        /// <summary>
        /// Update CR status
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateCrStatus(string uid, string status)
        {
            var response = new ServiceResponse<bool> { Result = false };

            int? statusId = null;
            if (!string.IsNullOrEmpty(status))
            {
                var crStatusMgr = ManagerFactory.Resolve<IChangeRequestStatusManager>();
                crStatusMgr.UoW = UoW;
                var crStatus = crStatusMgr.GetAllChangeRequestStatuses();
                var currentStatus = crStatus.FirstOrDefault(x => x.Code.ToUpper() == status.ToUpper());
                if (currentStatus == null)
                {
                    response.Report.ErrorList.Add(string.Format(Localization.CR_Status_Not_Found, status));
                    return response;
                }
                statusId = currentStatus.Pk_EnumChangeRequestStatus;
            }

            var crRepo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepo.UoW = UoW;

            response.Result = crRepo.UpdateCrStatus(uid, statusId);
            return response;
        }

        /// <summary>
        /// Update CRs status of CR Pack
        /// </summary>
        /// <param name="crsOfCrPack"></param>
        /// <returns></returns>
        public ServiceResponse<bool> UpdateCrsStatusOfCrPack(List<CrOfCrPackFacade> crsOfCrPack)
        {
            var response = new ServiceResponse<bool> { Result = false };

            var crStatusMgr = ManagerFactory.Resolve<IChangeRequestStatusManager>();
            crStatusMgr.UoW = UoW;
            var crStatus = crStatusMgr.GetAllChangeRequestStatuses();

            /* Fill Pk_EnumStatus */
            foreach (var item in crsOfCrPack)
            {
                if (!string.IsNullOrEmpty(item.Status))
                {
                    var status = crStatus.FirstOrDefault(x => x.Code.ToUpper() == item.Status.ToUpper());
                    if (status != null)
                        item.PkEnumStatus = status.Pk_EnumChangeRequestStatus;
                    else
                    {
                        response.Report.ErrorList.Add(string.Format(Localization.CR_Status_Not_Found, item.Status));
                        return response;
                    }
                }
                else
                    item.PkEnumStatus = null;
            }

            var crRepo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepo.UoW = UoW;

            response.Result = crRepo.UpdateCrsStatusOfCrPack(crsOfCrPack);
            return response;
        }  

    }
}
