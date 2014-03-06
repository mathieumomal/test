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
    class MeetingsFakeRepository : IMeetingRepository
    {
        public static readonly int SA63_MTG_ID = 1;
        public static readonly int PCG32_MTG_ID = 2;
        public static readonly int SA64_MTG_ID = 3;
        public static readonly int PCG33_MTG_ID = 4;


        public MeetingsFakeRepository() { }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEntityRepository<View_Persons> Members

        public IUltimateUnitOfWork UoW
        {
            get;
            set;
        }

        public IQueryable<Meeting> All
        {
            get { return GenerateList(); }
        }

        private IQueryable<Meeting> GenerateList()
        {
            var dbSet = new MeetingFakeDBSet();

            dbSet.Add(new Meeting() { MTG_ID = SA63_MTG_ID, MTG_REF = "3GPPSA#63" });
            dbSet.Add(new Meeting() { MTG_ID = PCG32_MTG_ID, MTG_REF = "3GPPPCG#32" });
            dbSet.Add(new Meeting() { MTG_ID = SA64_MTG_ID, MTG_REF = "3GPPSA#64" });
            dbSet.Add(new Meeting() { MTG_ID = PCG33_MTG_ID, MTG_REF = "3GPPPCG#33" });


            return dbSet.AsQueryable();
        }



        public IQueryable<Meeting> AllIncluding(params System.Linq.Expressions.Expression<Func<Meeting, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Meeting Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Meeting entity)
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
