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
    public class EnumTechnologiesRepository : IEnumTechnologiesRepository
    {
        private IUltimateContext context;

        public EnumTechnologiesRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        #region IEntityRepository<Enum_Technology> Membres

        public IQueryable<Enum_Technology> All
        {
            get { 
                return context.Enum_Technology;
            }
        }

        public IQueryable<Enum_Technology> AllIncluding(params System.Linq.Expressions.Expression<Func<Enum_Technology, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Enum_Technology Find(int id)
        {
            return context.Enum_Technology.Find(id);
        }

        public void InsertOrUpdate(Enum_Technology entity)
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

    public interface IEnumTechnologiesRepository : IEntityRepository<Enum_Technology>
    {

    }
}
