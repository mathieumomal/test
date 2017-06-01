using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Services
{
    public class WorkPlanFileService : IWorkPlanFileService
    {
        public void AddWorkPlanFile(WorkPlanFile workPlanFile)
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var workPlanFileManager = new WorkPlanFileManager();
                    workPlanFileManager.UoW = uoW;
                    workPlanFileManager.AddWorkPlanFile(workPlanFile);
                    uoW.Save();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object> { workPlanFile }, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }

        public WorkPlanFile GetLastWorkPlanFile()
        {
            try
            {
                using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
                {
                    var workPlanFileManager = new WorkPlanFileManager();
                    workPlanFileManager.UoW = uoW;
                    return workPlanFileManager.GetLastWorkPlanFile();
                }
            }
            catch (Exception e)
            {
                ExtensionLogger.Exception(e, new List<object>(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                throw e;
            }
        }
    }
}
