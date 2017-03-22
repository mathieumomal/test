using System;
using System.Collections.Generic;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DomainClasses.Facades;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class CrPackService : ICrPackService
    {
        public ServiceResponse<bool> UpdateCrsInsideCrPack(ChangeRequestPackFacade crPack,
            List<ChangeRequestInsideCrPackFacade> crs, int personId)
        {
            try
            {
                using (var uow = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var crPackMgr = ManagerFactory.Resolve<ICrPackManager>();
                    crPackMgr.UoW = uow;
                    var result = crPackMgr.UpdateCrsInsideCrPack(crPack, crs, personId);
                    if (result.Result.Key)
                    {
                        uow.Save();
                        
                        //IMPORTANT NOTE: 
                        //-- Comment for bundle of MARCH 2017 (See DO: 2016-4270). Actually, it was the more logic way to proceed but not able to synchronize EventWaitHandle between contribution (generate tdoc list of Cr-Pack) and ContributionService (generate tdoc lists of related CRs). 
                        //-- This problem should disapeared when all the code of tdoc list generation will be centralized inside the Contribution Service
                        //-- This problem should also disapeared if we think differently the way of generate asynchronously tdoc lists (create background task with queue in database...)
                        /* * Refresh necessary tdoc lists
                        if (result.Result.Value.Count > 0)
                        {
                            var contributionService = ServicesFactory.Resolve<IContributionService>();
                            contributionService.GenerateTdocListsAfterSendingCrsToCrPack(personId, 0, result.Result.Value);
                        }*/
                    }
                        
                    result.Report.ErrorList.ForEach(LogManager.Error);
                    result.Report.WarningList.ForEach(LogManager.Warn);
                    result.Report.InfoList.ForEach(LogManager.Info);
                    return new ServiceResponse<bool>{Report = result.Report, Result = result.Result.Key};
                }
            }
            catch (Exception e)
            {
                LogManager.Error("CrPackService - UpdateCrsInsideCrPack", e);
                return new ServiceResponse<bool>{Result = false, Report = new Report{ErrorList = new List<string>{Localization.GenericError}}};
            }
        }
    }

    public interface ICrPackService
    {
        ServiceResponse<bool> UpdateCrsInsideCrPack(ChangeRequestPackFacade crPack, List<ChangeRequestInsideCrPackFacade> crs,
            int personId);
    }
}
