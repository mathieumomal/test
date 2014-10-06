using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Default implementation of the IChangeREquestStatusRepository. Uses Entity Framework to 
    /// fetch the change request statuses in database.
    /// </summary>
    public class ChangeRequestStatusRepository: IChangeRequestStatusRepository
    {
        #region IEntityRepository<Enum_ChangeRequestStatus> Members

        /// <summary>
        /// The unit of work.
        /// </summary>
        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        /// <summary>
        /// Returns complete list of Change request statuses
        /// </summary>
        public IQueryable<Enum_ChangeRequestStatus> All
        {
            get {
                return UoW.Context.Enum_ChangeRequestStatus;
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public IQueryable<Enum_ChangeRequestStatus> AllIncluding(params System.Linq.Expressions.Expression<Func<Enum_ChangeRequestStatus, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public Enum_ChangeRequestStatus Find(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public void InsertOrUpdate(Enum_ChangeRequestStatus entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members


        public void Dispose()
        {
        }

        #endregion
    }

    /// <summary>
    /// Interface enabling to retrieve information concerning the CR status
    /// </summary>
    public interface IChangeRequestStatusRepository : IEntityRepository<Enum_ChangeRequestStatus>
    {

    }
}
