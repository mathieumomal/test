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
    class SpecVersionsFakeRepository : ISpecVersionsRepository
    {
        public SpecVersionsFakeRepository()
        {
        }

        #region IEntityRepository<SpecificationWIFakeRepository> Membres

        public IQueryable<SpecVersion> All
        {
            get { return GenerateList(); }
        }

        public List<SpecVersion> GetVersionsForSpecRelease(int SpecId, int releaseId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            throw new NotImplementedException();
        }

        private IQueryable<SpecVersion> GenerateList()
        {
            var rmkDbSet = new RemarkFakeDbSet();
            rmkDbSet.Add(new Remark() { Pk_RemarkId = 1, RemarkText = "Remark 1", Fk_VersionId = 1 });
            rmkDbSet.Add(new Remark() { Pk_RemarkId = 2, RemarkText = "Remark 2", Fk_VersionId = 2 });

            var dbSet = new SpecVersionFakeDBSet();
            dbSet.Add(new SpecVersion()
            {
                Pk_VersionId = 1,
                Multifile = false,
                ForcePublication = false,
                Location = "Location1",
                Fk_ReleaseId = 1,
                Fk_SpecificationId = 1,
                Remarks = new List<Remark>() { rmkDbSet.ToList()[0] },
                Release = new Release(),
                Specification = new Specification()
            });
            dbSet.Add(new SpecVersion()
            {
                Pk_VersionId = 2,
                Multifile = false,
                ForcePublication = false,
                Location = "Location2",
                Fk_ReleaseId = 2,
                Fk_SpecificationId = 1,
                Remarks = new List<Remark>() { rmkDbSet.ToList()[1] },
                Release = new Release(),
                Specification = new Specification(),
            });
            return dbSet.AsQueryable();
        }

        public IQueryable<SpecVersion> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecVersion, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public SpecVersion Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(SpecVersion entity)
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
            throw new NotImplementedException();
        }

        #endregion

        public IUltimateUnitOfWork UoW { get; set; }
    }
}
