using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class Enum_CommunitiesShortNameRepository : IEnum_CommunitiesShortNameRepository
    {
        #region IEntityRepository<Enum_CommunitiesShortName> Membres

        public IUltimateUnitOfWork UoW { get; set; }
        public Enum_CommunitiesShortNameRepository() { }

        public IQueryable<Enum_CommunitiesShortName> All
        {
            get { return UoW.Context.Enum_CommunitiesShortName; }
        }

        public IQueryable<Enum_CommunitiesShortName> AllIncluding(params System.Linq.Expressions.Expression<Func<Enum_CommunitiesShortName, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Enum_CommunitiesShortName Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Enum_CommunitiesShortName entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            UoW.Context.Dispose();
        }

        #endregion
    }

    public interface IEnum_CommunitiesShortNameRepository : IEntityRepository<Enum_CommunitiesShortName>
    {

    }
}
