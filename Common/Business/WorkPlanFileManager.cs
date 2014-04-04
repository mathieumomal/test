using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    public class WorkPlanFileManager
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public WorkPlanFileManager() {}

        public void AddWorkPlanFile(WorkPlanFile workPlanFile)
        {
            IWorkPlanFileRepository repo = RepositoryFactory.Resolve<IWorkPlanFileRepository>();
            repo.UoW = UoW;
            repo.InsertOrUpdate(workPlanFile);
            
        }

        public WorkPlanFile GetLastWorkPlanFile()
        {
            IWorkPlanFileRepository repo = RepositoryFactory.Resolve<IWorkPlanFileRepository>();
            repo.UoW = UoW;
            List<WorkPlanFile> workPlanFiles = repo.All.ToList();
                   
            if (workPlanFiles == null || workPlanFiles.Count == 0)
                return null;
            else
                return workPlanFiles.OrderByDescending(w => w.CreationDate).FirstOrDefault();
        }
    }
}
