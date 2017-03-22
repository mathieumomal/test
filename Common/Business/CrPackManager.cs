using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Business
{
    public class CrPackManager : ICrPackManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// For the moment only add or remove CRs inside a CR-Pack. 
        /// In the future, the ambition of this method is to provide a centralized way to update until type of decisions of CRs inside CR-Pack
        /// </summary>
        /// <param name="crPack">CR-Pack</param>
        /// <param name="crs">List of CRs which should belong to CR-Pack</param>
        /// <param name="personId">Person id</param>
        /// <returns></returns>
        public ServiceResponse<KeyValuePair<bool, List<int>>> UpdateCrsInsideCrPack(ChangeRequestPackFacade crPack, List<ChangeRequestInsideCrPackFacade> crs, int personId)
        {
            var response = new ServiceResponse<KeyValuePair<bool, List<int>>> { Result = new KeyValuePair<bool, List<int>>(false, new List<int>()) };

            //Sanity checks
            if (crPack == null
                || crs == null
                || string.IsNullOrEmpty(crPack.Uid))
            {
                response.Report.LogError(Localization.UpdateCrsInsideCrPack_Invalid_Arguments);
                return response;
            }
            if (crPack.MeetingId == 0
                || string.IsNullOrEmpty(crPack.Source))
            {
                response.Report.LogWarning(Localization.UpdateCrsInsideCrPack_Warning_CrPack_Source_Meeting_Not_Provided);
            }

            var freshListOfCrIdLinkedToCurrentCrPack = crs.Select(x => x.Id).ToList();
            var crRepo = RepositoryFactory.Resolve<IChangeRequestRepository>();
            crRepo.UoW = UoW;

            //Find CRs TsgData which currently belong to this CR-Pack (db) (<=> TsgTdoc set with CR-Pack UID)
            var crTsgDataWhichBelongToCurrentCrPack = crRepo.GetTsgDataForCrPack(crPack.Uid);
            var crsIdsWhichBelongToCurrentCrPack = crTsgDataWhichBelongToCurrentCrPack.Select(x => x.Fk_ChangeRequest).ToList();

            //According to fresh list of CRs provided as argument -> Find CRs to remove from this CR-Pack, then remove them
            var crsToRemoveFromThisCrPack =
                crsIdsWhichBelongToCurrentCrPack.Where(x => !freshListOfCrIdLinkedToCurrentCrPack.Contains(x)).ToList();
            crTsgDataWhichBelongToCurrentCrPack.Where(x => crsToRemoveFromThisCrPack.Contains(x.Fk_ChangeRequest)).ToList().ForEach(y => UoW.MarkDeleted(y));

            //According to fresh list of CRs provided as argument -> Find CRs to add to this CR-Pack, then add them
            var crsToAddToThisCrPack =
                freshListOfCrIdLinkedToCurrentCrPack.Where(x => !crsIdsWhichBelongToCurrentCrPack.Contains(x)).ToList();
            if (crsToAddToThisCrPack.Count > 0)
            {
                var crsToAdd = crRepo.FindCrsByIds(crsToAddToThisCrPack);
                foreach (var cr in crsToAdd)
                {
                    cr.ChangeRequestTsgDatas.Add(new ChangeRequestTsgData
                    {
                        TSGMeeting = crPack.MeetingId,
                        TSGSourceOrganizations = crPack.Source,
                        TSGTdoc = crPack.Uid
                    });
                }
            }

            //(Future: Update CRs...)

            var listOfCrIdsForWhichWeNeedToRefreshTdocList = crsToRemoveFromThisCrPack;
            listOfCrIdsForWhichWeNeedToRefreshTdocList.AddRange(crsToAddToThisCrPack);
            response.Result = new KeyValuePair<bool, List<int>>(true, listOfCrIdsForWhichWeNeedToRefreshTdocList);
            return response;
        }
    }

    public interface ICrPackManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        ServiceResponse<KeyValuePair<bool, List<int>>> UpdateCrsInsideCrPack(ChangeRequestPackFacade crPack, List<ChangeRequestInsideCrPackFacade> crs,
            int personId);
    }
}
