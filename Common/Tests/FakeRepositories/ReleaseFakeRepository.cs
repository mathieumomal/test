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
    class ReleaseFakeRepository : IReleaseRepository
    {

        public ReleaseFakeRepository() { }

        #region IEntityRepository<Release> Members

        public IUltimateUnitOfWork UoW
        { get; set;
        }

        public IQueryable<Ultimate.DomainClasses.Release> All
        {
            get
            {
                return GenerateList();
            }
        }

        private IQueryable<Ultimate.DomainClasses.Release> GenerateList()
        {
            //Just essentials informations for the tests
            var releases = new ReleaseFakeDBSet();
            var openStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" };
            var frozenStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" };
            releases.Add(new Release() { Pk_ReleaseId = 1, Name = "First release", Fk_ReleaseStatus = 1, Enum_ReleaseStatus = openStatus });
            releases.Add(new Release() { Pk_ReleaseId = 2, Name = "Second release", Fk_ReleaseStatus = 2, Enum_ReleaseStatus = frozenStatus });
            releases.Add(new Release() { Pk_ReleaseId = 3, Name = "Third release", Fk_ReleaseStatus = 2, Enum_ReleaseStatus = frozenStatus });

            return releases.AsQueryable();
        }

        public IQueryable<Ultimate.DomainClasses.Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Ultimate.DomainClasses.Release, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Ultimate.DomainClasses.Release Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Ultimate.DomainClasses.Release entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
