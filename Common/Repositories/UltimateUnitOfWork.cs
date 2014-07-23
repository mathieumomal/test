using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Repositories
{
    public class UltimateUnitOfWork : IUltimateUnitOfWork   
    {

        private IUltimateContext context;
           

        public UltimateUnitOfWork()
        {
            context = RepositoryFactory.Resolve<IUltimateContext>();
        }

        #region IUltimateUnitOfWork Members

        public void Save()
        {
            context.SaveChanges();
        }

        public IUltimateContext GetContext()
        {
            return context;
        }

        public void SetAutoDetectChanges(bool detect)
        {
            context.SetAutoDetectChanges(detect);
        }
        #endregion

      
        #region IDisposable Members

        public void Dispose()
        {
            //if ((((System.Data.Entity.DbContext)(context)).Database.Connection).DataSource != "in-process") //for Effort testing
                context.Dispose();
        }

        #endregion

        #region IUltimateUnitOfWork Members

        IUltimateContext IUltimateUnitOfWork.Context
        {
            get { return context; }
        }

        #endregion
    }
}
