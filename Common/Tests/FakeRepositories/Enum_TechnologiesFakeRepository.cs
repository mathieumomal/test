using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class Enum_TechnologiesFakeRepository : ISpecificationTechnologiesRepository
    {
        public Enum_TechnologiesFakeRepository(){}

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEntityRepository<Enum_Technology> Members

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        public IQueryable<SpecificationTechnology> All
        {
            get { return GenerateList(); }
        }

        public IQueryable<SpecificationTechnology> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecificationTechnology, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        private IQueryable<SpecificationTechnology> GenerateList()
        {
            Enum_Technology e = new Enum_Technology() { Pk_Enum_TechnologyId = 1, Code = "2G", Description = "2G" };
            Enum_Technology e1 = new Enum_Technology(){ Pk_Enum_TechnologyId = 2, Code = "3G", Description = "3G"};
            Enum_Technology e2 = new Enum_Technology() { Pk_Enum_TechnologyId = 3, Code = "LTE", Description = "LTE" };

            var dbSet = new TechnologiesFakeDBSet();

            dbSet.Add(new SpecificationTechnology() { Pk_SpecificationTechnologyId = 1, Fk_Specification = 1, Fk_Enum_Technology = 1, Enum_Technology = e });
            dbSet.Add(new SpecificationTechnology() { Pk_SpecificationTechnologyId = 2, Fk_Specification = 1, Fk_Enum_Technology = 2, Enum_Technology = e1 });
            dbSet.Add(new SpecificationTechnology() { Pk_SpecificationTechnologyId = 3, Fk_Specification = 1, Fk_Enum_Technology = 3, Enum_Technology = e2 });

            return dbSet.AsQueryable();
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
