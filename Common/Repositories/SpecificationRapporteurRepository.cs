using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class SpecificationRapporteurRepository : ISpecificationRapporteurRepository
    {
        #region IEntityRepository methods
        public IUltimateUnitOfWork UoW { get; set; }

        public IQueryable<SpecificationRapporteur> All
        {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<SpecificationRapporteur> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecificationRapporteur, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public SpecificationRapporteur Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(SpecificationRapporteur entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        public List<SpecificationRapporteur> FindBySpecId(int specId)
        {
            return UoW.Context.SpecificationRapporteurs.Where(x => x.Fk_SpecificationId == specId).ToList();
        }
    }

    public interface ISpecificationRapporteurRepository : IEntityRepository<SpecificationRapporteur>
    {
        /// <summary>
        /// Fin rapporteurs of a spec
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        List<SpecificationRapporteur> FindBySpecId(int specId);
    }
}
