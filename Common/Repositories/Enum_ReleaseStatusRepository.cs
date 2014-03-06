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

    /// <summary>
    /// Default implement of the IEnum_ReleaseStatusRepository.
    /// 
    /// Goes fetch the data in the Enum_ReleaseStatus table in database.
    /// </summary>
    public class Enum_ReleaseStatusRepository : IEnum_ReleaseStatusRepository
    {
        private IUltimateContext context;
        public Enum_ReleaseStatusRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        

        #region IEntityRepository<Enum_ReleaseStatus> Membres

        public IQueryable<Enum_ReleaseStatus> All
        {
            get { 
                return context.Enum_ReleaseStatus;
            }
        }

        public IQueryable<Enum_ReleaseStatus> AllIncluding(params System.Linq.Expressions.Expression<Func<Enum_ReleaseStatus, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Enum_ReleaseStatus Find(int id)
        {
            return context.Enum_ReleaseStatus.Find(id);
        }

        public void InsertOrUpdate(Enum_ReleaseStatus entity)
        {
            throw new NotImplementedException();
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

    public interface IEnum_ReleaseStatusRepository : IEntityRepository<Enum_ReleaseStatus>
    {

    }
}
