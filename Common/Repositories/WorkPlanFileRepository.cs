using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{
    public class WorkPlanFileRepository : IWorkPlanFileRepository
    {
        private IUltimateContext context;

        public WorkPlanFileRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        #region IEntityRepository<WorkPlanFileRepository> Membres

        public IQueryable<WorkPlanFile> All
        {
            get
            {
                return context.WorkPlanFiles;
            }
        }

        public IQueryable<WorkPlanFile> AllIncluding(params System.Linq.Expressions.Expression<Func<WorkPlanFile, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public WorkPlanFile Find(int id)
        {
            return context.WorkPlanFiles.Find(id, "N");
        }        

        public void InsertOrUpdate(WorkPlanFile entity)
        {
            if (entity.Pk_WorkPlanFileId == default(int))
            {
                UoW.Context.SetAdded(entity);
            }
            else
            {
                UoW.Context.SetModified(entity);
            }            
        }

       
        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface IWorkPlanFileRepository : IEntityRepository<WorkPlanFile>
    {
        void InsertOrUpdate(WorkPlanFile entity);
    }
}
