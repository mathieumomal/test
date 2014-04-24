using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using System.Web;
using Etsi.Ultimate.Utils;
using System.Data.Entity.Core.Objects;



namespace Etsi.Ultimate.Repositories
{
    public class SpecificationTechnologiesRepository : ISpecificationTechnologiesRepository
    {

        private IUltimateContext context;

        public SpecificationTechnologiesRepository(IUltimateUnitOfWork iUoW)
        {
            context = iUoW.Context;
        }

        public IQueryable<SpecificationTechnology> All
        {
            get { 
                return AllIncluding(s => s.Enum_Technology);
            }
        }

        public IQueryable<SpecificationTechnology> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecificationTechnology, object>>[] includeProperties)
        {
            IQueryable<SpecificationTechnology> query = UoW.Context.SpecificationTechnologies;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SpecificationTechnology Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(SpecificationTechnology entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        #region IDisposable Membres

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion


        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface ISpecificationTechnologiesRepository : IEntityRepository<SpecificationTechnology>
    {
    }
}
