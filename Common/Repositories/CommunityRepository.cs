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
    /// Repository managing the list of Communities (e.g TSG, WG).
    /// </summary>
    public class CommunityRepository : ICommunityRepository
    {
        
        private IUltimateContext context;
        public CommunityRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }



        #region IEntityRepository<PersonRepository> Membres

        public IQueryable<Community> All
        {
            get { 
                return context.Communities;
            }
        }

        public IQueryable<Community> AllIncluding(params System.Linq.Expressions.Expression<Func<Community, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Community Find(int id)
        {
            return context.Communities.Find(id);
        }

        public void InsertOrUpdate(Community entity)
        {
            throw new InvalidOperationException("Cannot add or update a community");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete community entity");
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

    public interface ICommunityRepository : IEntityRepository<Community>
    {

    }
}
