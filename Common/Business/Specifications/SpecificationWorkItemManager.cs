using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.Specifications
{
    public class SpecificationWorkItemManager
    {
        public IUltimateUnitOfWork UoW {get; set;}

        public List<WorkItem> GetSpecificationWorkItemsBySpecId(int id)
        {
            var result = new List<WorkItem>();
            var repo = RepositoryFactory.Resolve<ISpecificationWorkItemRepository>();
            repo.UoW = UoW;
            repo.All.ToList().Where(s => s.Fk_SpecificationId == id).ToList().ForEach(e => result.Add(e.WorkItem));
            return result;
        }

        /// <summary>
        /// Return workitems (#[UID] - [Acronym] - [NAME]) label list of a spec
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        public List<string> GetSpecificationWorkItemsLabels(int specId)
        {
            var repo = RepositoryFactory.Resolve<ISpecificationWorkItemRepository>();
            repo.UoW = UoW;
            List<WorkItem> result = repo.GetWorkItemsForSpec(specId);
            result = result.OrderByDescending(x => x.IsPrimary).ToList();

            var workItemLabels = new List<string>();
            foreach (var wi in result)
            {
                string label;
                if (wi.IsPrimary)
                {
                    label = new StringBuilder()
                    .Append("<strong>")
                    .Append("#")
                    .Append(wi.Pk_WorkItemUid)
                    .Append(" - ")
                    .Append(wi.Acronym)
                    .Append(" - ")
                    .Append(wi.Name)
                    .Append("</strong>")
                    .ToString();
                }
                else
                {
                    label = new StringBuilder()
                    .Append("#")
                    .Append(wi.Pk_WorkItemUid)
                    .Append(" - ")
                    .Append(wi.Acronym)
                    .Append(" - ")
                    .Append(wi.Name)
                    .ToString();
                }
                workItemLabels.Add(label);
            }
            return workItemLabels;
        }
    }
}
