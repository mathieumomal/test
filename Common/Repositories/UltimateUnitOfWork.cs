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

        #endregion

      
        #region IDisposable Members

        public void Dispose()
        {
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
