using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Tests
{
    class EffortUnitOfWork: IUltimateUnitOfWork
    {
        private IUltimateContext context;

        #region IUltimateUnitOfWork Members

        public DataAccess.IUltimateContext Context
        {
            get { return context; }
        }

        public EffortUnitOfWork()
        {
            context = RepositoryFactory.Resolve<IUltimateContext>();
        }

        public void SetAutoDetectChanges(bool detect)
        {
           
        }

        public void MarkDeleted<T>(T Entity)
        {
            context.SetDeleted(Entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
