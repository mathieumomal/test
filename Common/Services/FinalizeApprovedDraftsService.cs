using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class FinalizeApprovedDraftsService : IFinalizeApprovedDraftsService
    {
        public ServiceResponse<bool> FinalizeApprovedDrafts(int personId, int mtgId,
            List<Tuple<int, int, int>> approvedDrafts)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var finalizeApprovedDraftsMgr = ManagerFactory.Resolve<IFinalizeApprovedDraftsManager>();
                    finalizeApprovedDraftsMgr.UoW = uow;

                    var result = finalizeApprovedDraftsMgr.FinalizeApprovedDrafts(personId, mtgId, approvedDrafts);
                    if(result.Report.GetNumberOfErrors() == 0)
                        uow.Save();
                    return result;
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { personId, mtgId, approvedDrafts }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                response.Result = false;
                response.Report.ErrorList.Add(Localization.GenericError);
                return response;
            }
        }
    }

    public interface IFinalizeApprovedDraftsService
    {
        ServiceResponse<bool> FinalizeApprovedDrafts(int personId, int mtgId,
            List<Tuple<int, int, int>> approvedDrafts);
    }
}
