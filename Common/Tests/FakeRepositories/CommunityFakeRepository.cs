using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class CommunityFakeRepository : ICommunityRepository
    {

        public CommunityFakeRepository() { }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEntityRepository<Community> Members

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        public IQueryable<Community> All
        {
            get { return GenerateList(); }
        }

        private IQueryable<Community> GenerateList()
        {
            var dbSet = new CommunityFakeDBSet();

            dbSet.Add(new Community() { TbId = 1, ActiveCode = "ACTIVE", ParentTbId = 0, ShortName = "SP", TbName = "3GPP SA" });
            dbSet.Add(new Community() { TbId = 2, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S1", TbName = "3GPP SA 1" });
            dbSet.Add(new Community() { TbId = 3, ActiveCode = "ACTIVE", ParentTbId = 1, ShortName = "S2", TbName = "3GPP SA 2" });

            return dbSet.AsQueryable();
        }

        

        public IQueryable<Community> AllIncluding(params System.Linq.Expressions.Expression<Func<Community, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Community Find(int id)
        {
            throw new NotImplementedException();
        }

        public int GetWgNumber(int wgId, int parentTbId)
        {
            return (All.Where(c => c.ParentTbId == parentTbId).ToList().Select((v, i) => new { v, i }).Where(x => x.v.TbId == wgId).Select(x => x.i).FirstOrDefault() +1 );
        }

        public void InsertOrUpdate(Community entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
