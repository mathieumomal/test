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

        public int GetWgNumber(int wgId, int parentTbId)
        {
            return (context.Communities.Where(c => c.ParentTbId == parentTbId).ToList().Select((v, i) => new { v, i }).Where(x => x.v.TbId == wgId).Select(x => x.i).FirstOrDefault() +1 );
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
        /// <summary>
        /// Return the number of a WG in a TB childs list
        /// </summary>
        /// <param name="wgId">wg id</param>
        /// <param name="parentTbId">Parent TB id</param>
        /// <returns>WG's number</returns>
        int GetWgNumber(int wgId, int parentTbId);
    }
}
