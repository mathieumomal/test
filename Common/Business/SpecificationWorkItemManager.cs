using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecificationWorkItemManager
    {
        public IUltimateUnitOfWork UoW {get; set;}

        public SpecificationWorkItemManager()
        {
        }


        public List<WorkItem> GetSpecificationWorkItemsBySpecId(int id)
        {
            List<WorkItem> result = new List<WorkItem>();
            ISpecificationWorkItemRepository repo = RepositoryFactory.Resolve<ISpecificationWorkItemRepository>();
            repo.UoW = UoW;
            repo.All.ToList().Where(s => s.Fk_SpecificationId == id).ToList().ForEach(e => result.Add(e.WorkItem));
            return result;
        }
    }
}
