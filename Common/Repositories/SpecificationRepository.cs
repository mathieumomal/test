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
    public class SpecificationRepository : ISpecificationRepository
    {
        public IUltimateUnitOfWork UoW{ get; set; }
        public SpecificationRepository(IUltimateUnitOfWork iUoW) 
        { 
        }

        #region IEntityRepository<Specification> Membres

        /// <summary>
        /// Ret
        /// </summary>
        public IQueryable<Specification> All
        {
            get
            {
                return AllIncluding(t => t.SpecificationResponsibleGroups);
            }
        }
        
        public IQueryable<Specification> AllIncluding(params System.Linq.Expressions.Expression<Func<Specification, object>>[] includeProperties)
        {
            IQueryable<Specification> query = UoW.Context.Specifications;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Specification Find(int id)
        {
            return AllIncluding(t => t.Enum_Serie, t=> t.Enum_SpecificationStage, t => t.Remarks, t => t.Histories, 
                t=>t.SpecificationTechnologies, t=>t.SpecificationResponsibleGroups,t=>t.SpecificationRapporteurs,
                t=>t.Specification_Release).Where(x => x.Pk_SpecificationId == id).FirstOrDefault();
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Specification entity");
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion
    }

    public interface ISpecificationRepository
    {
    }
}
