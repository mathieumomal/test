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
        private IUltimateContext context;

        public ReleaseRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        #region IReleaseRepository Membres

        public List<Release> GetAllReleaseByIdReleaseStatus(Enum_ReleaseStatus releaseStatus)
        {
            return context.Release.Where(id => id.Fk_ReleaseStatus == releaseStatus.Enum_ReleaseStatusId).ToList();
        }

        #endregion

        #region IEntityRepository<Release> Membres

        public IQueryable<Release> All
        {
            get { return context.Release; }
        }

        public IQueryable<Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Release, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Release Find(int id)
        {
            return context.Release.Find(id);
        }

        public void InsertOrUpdate(Release entity)
        {
            if (entity.ReleaseId == default(int))
            {
                context.SetAdded(entity);
            }
            else
            {
                context.SetModified(entity);
            }
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release entity");
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion
    }

    public interface IReleaseRepository : IEntityRepository<Release>
    {
        List<Release> GetAllReleaseByIdReleaseStatus(Enum_ReleaseStatus releaseStatus);
    }
}
