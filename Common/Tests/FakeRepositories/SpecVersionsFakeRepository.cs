using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class SpecVersionsFakeRepository : ISpecVersionsRepository
    {
        #region IEntityRepository<SpecificationWIFakeRepository> Membres

        public IQueryable<SpecVersion> All
        {
            get { return GenerateList(); }
        }

        public List<SpecVersion> GetVersionsWithFoundationsCrsBySpecId(int specId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsForSpecRelease(int SpecId, int releaseId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsByReleaseId(int releaseId)
        {
            return All.Where(e => e.Fk_ReleaseId == releaseId).ToList();
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

        #region ISpecVersionsRepository Membres

        public IUltimateUnitOfWork UoW { get; set; }

        public int CountVersionsPendingUploadByReleaseId(int releaseId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsBySpecIds(List<int> specIds, List<int> allowedMajorVersions)
        {
            throw new NotImplementedException();
        }

        public SpecVersion GetVersion(int specId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetVersionsByRelatedTDoc(string relatedTdoc)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds)
        {
            throw new NotImplementedException();
        }

        public void UpdateVersion(SpecVersion version)
        {
            throw new NotImplementedException();
        }
        #endregion


        public void Delete(SpecVersion version)
        {
            throw new NotImplementedException();
        }


        public List<SpecVersion> AlreadyUploadedVersionsForSpec(int specId)
        {
            throw new NotImplementedException();
        }

        public List<SpecVersion> VersionsLinkedToChangeRequestsForSpec(int specId)
        {
            throw new NotImplementedException();
        }


        public SpecVersion FindCrsLinkedToAVersion(int versionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the latest version of each spec release where Spec is UCC and Release is Open or Frozen
        /// </summary>
        /// <returns>list of specVersion</returns>
        public List<SpecVersion> GetLatestVersionGroupedBySpecRelease()
        {
            throw new NotImplementedException();
        }


        public bool CheckIfVersionExists(int specId, int releaseId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            throw new NotImplementedException();
        }
    }
}
