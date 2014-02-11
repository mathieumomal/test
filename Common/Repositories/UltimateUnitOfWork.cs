using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Repositories
{
    public class UltimateUnitOfWork : IUltimateUnitOfWork   
    {

        private UltimateContext context;
           

        public UltimateUnitOfWork()
        {
            context = new UltimateContext();
        }

        #region INgppUnitOfWork Members

        public void Save()
        {
            context.SaveChanges();
        }

        public UltimateContext GetContext()
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
