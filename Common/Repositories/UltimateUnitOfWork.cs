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

        /// <summary>
        /// Set entity state to deleted
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="Entity">Entity</param>
        public void MarkDeleted<T>(T Entity)
        {
            context.SetDeleted(Entity);
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
