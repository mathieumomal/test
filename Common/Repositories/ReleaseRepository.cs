using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;

namespace Etsi.Ultimate.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        public IUltimateUnitOfWork UoW
        {
            get;
            set;
        }

        public ReleaseRepository()
        {
        }

        #region IReleaseRepository Membres

        public List<Release> GetAllReleaseByIdReleaseStatus(Enum_ReleaseStatus releaseStatus)
        {
            return UoW.Context.Releases.Where(id => id.Fk_ReleaseStatus == releaseStatus.Enum_ReleaseStatusId).ToList();
        }

        #endregion

        #region IEntityRepository<Release> Membres

        public IQueryable<Release> All
        {
            get { return UoW.Context.Releases; }
        }

        public IQueryable<Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Release, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Release Find(int id)
        {
            return UoW.Context.Releases.Find(id);
        }

        public void InsertOrUpdate(Release entity)
        {
            if (entity.ReleaseId == default(int))
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
            throw new InvalidOperationException("Cannot delete Release entity");
        }

        public void Save()
        {
            UoW.Context.SaveChanges();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            UoW.Context.Dispose();
        }

        #endregion
    }

    public interface IReleaseRepository : IEntityRepository<Release>
    {
        List<Release> GetAllReleaseByIdReleaseStatus(Enum_ReleaseStatus releaseStatus);
    }
}
