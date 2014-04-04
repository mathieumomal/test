using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Business;

namespace Etsi.Ultimate.Services
{
    public class WorkPlanFileService : IWorkPlanFileService
    {
        public void AddWorkPlanFile(WorkPlanFile workPlanFile)
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workPlanFileManager = new WorkPlanFileManager();
                workPlanFileManager.UoW = uoW;
                workPlanFileManager.AddWorkPlanFile(workPlanFile);
                uoW.Save();
            }
        }

        public WorkPlanFile GetLastWorkPlanFile()
        {
            using (var uoW = RepositoryFactory.Resolve<IUltimateUnitOfWork>())
            {
                var workPlanFileManager = new WorkPlanFileManager();
                workPlanFileManager.UoW = uoW;
                return workPlanFileManager.GetLastWorkPlanFile();
            }
        }
    }
}
