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
    public class EnumReleaseRepository : IEnumReleaseRepository
    {
        private static string CACHE_KEY = "ULT_REPO_ENUM_RELEASE_STATUS_ALL";
        
        private IUltimateContext context;
        public EnumReleaseRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        

        #region IEntityRepository<Enum_ReleaseStatus> Membres

        public IQueryable<Enum_ReleaseStatus> All
        {
            get { 
                var cachedData = (IQueryable<Enum_ReleaseStatus>) CacheManager.Get(CACHE_KEY);
                if (cachedData == null)
                {
                    cachedData = context.Enum_ReleaseStatus;
                    CacheManager.Insert(CACHE_KEY, cachedData);
                }
                return cachedData; 
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

    public interface IEnumReleaseRepository : IEntityRepository<Enum_ReleaseStatus>
    {

    }
}
