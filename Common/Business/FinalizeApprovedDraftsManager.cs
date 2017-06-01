using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.Business.Versions;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business
{
    public class FinalizeApprovedDraftsManager : IFinalizeApprovedDraftsManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// System should finalize approved drafts (DRAFT TS, DRAFT TR or TS or TR) by:
        /// - transforming spec as UCC
        /// - allocating version for just changed to UCC specifications link to the current finalized meeting (source property)
        /// BUSINESS RULES:
        /// - system should do nothing if spec already UCC
        /// - system should not create multiple versions for the same spec/release
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="mtgId"></param>
        /// <param name="approvedDrafts">Tuple with following values (ordered):
        /// - int: tdoc id
        /// - int: specificationId
        /// - int: releaseId
        /// </param>
        /// <returns>bool (true if success)</returns>
        public ServiceResponse<bool> FinalizeApprovedDrafts(int personId, int mtgId, List<Tuple<int, int, int>> approvedDrafts)
        {
            ExtensionLogger.Info("FINALIZE DRAFTS: System is finalizing approved drafts...", new List<KeyValuePair<string, object>>{ 
                new KeyValuePair<string,object>("personId", personId),
                new KeyValuePair<string,object>("mtgId", mtgId),
                new KeyValuePair<string,object>("approvedDrafts", approvedDrafts)
            });

            //0) Define variables
            var response = new ServiceResponse<bool>();
            var versionAlreadyAllocated = new List<SpecVersion>();
            
            //1) Find related specs
            var specMgr = RepositoryFactory.Resolve<ISpecificationRepository>();
            specMgr.UoW = UoW;
            var specsFound = specMgr.GetSpecifications(approvedDrafts.Select(x => x.Item2).ToList());

            //2) Allocate version for approved drafts not UCC
            foreach (var approvedDraft in approvedDrafts)
            {
                //2.1) First check if spec is already UCC, if system should do nothing
                var currentSpec = specsFound.FirstOrDefault(x => x.Pk_SpecificationId == approvedDraft.Item2);
                if (currentSpec == null || currentSpec.IsUnderChangeControl.GetValueOrDefault())
                {
                    continue;
                }

                //2.2) Do not allocate multiple versions for the same spec/release !
                if (versionAlreadyAllocated.Any(
                        x => x.Fk_SpecificationId == approvedDraft.Item2 && x.Fk_ReleaseId == approvedDraft.Item3))
                {
                    continue;
                }

                //2.3) Get next allocation version number by considering that spec is already UCC 
                //(even if this is not really the case in database. Allocation will automaticaly update specification to set spec as UCC)

                var getNextReleaseAction = ManagerFactory.Resolve<IGetNextReleaseAction>();
                getNextReleaseAction.UoW = UoW;
                var nextVersionNumber = getNextReleaseAction.GetNextVersionForSpec(personId, approvedDraft.Item2,
                    approvedDraft.Item3, false, true);
                response.Report.ErrorList.AddRange(nextVersionNumber.Report.ErrorList);
                response.Report.WarningList.AddRange(nextVersionNumber.Report.WarningList);
                response.Report.InfoList.AddRange(nextVersionNumber.Report.InfoList);
                if (response.Report.GetNumberOfErrors() > 0)
                {
                    LogManager.Error(string.Format("FINALIZE DRAFTS:    An unexpected error occured when system is trying to get next version number during FinalizeApprovedDrafts for mtg: {0} and approvedDraft: {1} - {2} - {3}. Please find list of errors return by GetNextVersionForSpec: {4}", mtgId, approvedDraft.Item1, approvedDraft.Item2, approvedDraft.Item3, string.Join(",", response.Report.ErrorList)));
                    response.Result = false;
                    return response;
                }

                //2.4) Allocate new version
                var newVersion = new SpecVersion
                {
                    Fk_ReleaseId = approvedDraft.Item3,
                    Fk_SpecificationId = approvedDraft.Item2,
                    MajorVersion = nextVersionNumber.Result.NewSpecVersion.MajorVersion,
                    TechnicalVersion = nextVersionNumber.Result.NewSpecVersion.TechnicalVersion,
                    EditorialVersion = nextVersionNumber.Result.NewSpecVersion.EditorialVersion,
                    Source = mtgId,
                    ProvidedBy = personId
                };

                var allocateAction = ManagerFactory.Resolve<ISpecVersionAllocateAction>();
                allocateAction.UoW = UoW;
                var createdVersionResult = allocateAction.AllocateVersion(personId, newVersion);//This method will change automaticaly specification as underchangecontrol because of MajorVersion number value
                response.Report.ErrorList.AddRange(createdVersionResult.Report.ErrorList);
                response.Report.WarningList.AddRange(createdVersionResult.Report.WarningList);
                response.Report.InfoList.AddRange(createdVersionResult.Report.InfoList);
                if (response.Report.GetNumberOfErrors() > 0)
                {
                    LogManager.Error(string.Format("FINALIZE DRAFTS:    An unexpected error occured when system trying to allocate version during FinalizeApprovedDrafts for mtg: {0} and approvedDraft: {1} - {2} - {3}. Please find list of errors return by AllocateVersion: {4}", mtgId, approvedDraft.Item1, approvedDraft.Item2, approvedDraft.Item3, string.Join(",", response.Report.ErrorList)));
                    response.Result = false;
                    return response;
                }

                versionAlreadyAllocated.Add(newVersion);
            }

            response.Result = true;
            var message =
                "FINALIZE DRAFTS:    Please find list of allocated versions (and their UCC spec) during FinalizeApprovedDrafts process: " +
                string.Join("\n",
                    versionAlreadyAllocated.Select(
                        x =>
                            string.Format("Version: {0} - ({1},{2},{3}), Spec: {4}, Rel: {5}", x.Pk_VersionId,
                                x.MajorVersion, x.TechnicalVersion, x.EditorialVersion, x.Release.ShortName,
                                x.Specification.Number)).ToList());
            LogManager.Info(message);
            LogManager.Info("FINALIZE DRAFTS: Finalization of approved drafts done successfully. END.");
            return response;
        }
    }

    public interface IFinalizeApprovedDraftsManager
    {
        IUltimateUnitOfWork UoW { get; set; }

        ServiceResponse<bool> FinalizeApprovedDrafts(int personId, int mtgId, List<Tuple<int, int, int>> approvedDrafts);
    }
}
