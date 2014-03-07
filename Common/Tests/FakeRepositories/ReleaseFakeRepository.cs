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
        public static readonly int OPENED_RELEASE_ID = 1;
        public static readonly int FROZEN_RELEASE_ID = 2;
        public static readonly int CLOSED_RELEASE_ID = 4;
        

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
            var aRemark = new Remark() { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OPENED_RELEASE_ID, IsPublic = true, Pk_RemarkId = 1, RemarkText = "test" };
            var aHistory1 = new History() { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OPENED_RELEASE_ID, HistoryText = "text 1", Pk_HistoryId = 1 };
            var aHistory2 = new History() { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OPENED_RELEASE_ID, HistoryText = "text 2", Pk_HistoryId = 2 };


            //Just essentials informations for the tests
            var releases = new ReleaseFakeDBSet();
            var openStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" };
            var frozenStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen" };
            var closedStatus = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" };
            releases.Add(new Release() { Pk_ReleaseId = OPENED_RELEASE_ID, Name = "First release", Code="Rel-1", ShortName="R1", Fk_ReleaseStatus = 1, Enum_ReleaseStatus = openStatus,
                Remarks = new List<Remark>() { aRemark }, Histories = new List<History>() { aHistory1, aHistory2 } });
            releases.Add(new Release() { Pk_ReleaseId = FROZEN_RELEASE_ID, Name = "Second release", Fk_ReleaseStatus = 2, Enum_ReleaseStatus = frozenStatus });
            releases.Add(new Release() { Pk_ReleaseId = 3, Name = "Third release", Fk_ReleaseStatus = 2, Enum_ReleaseStatus = frozenStatus });
            releases.Add(new Release() { Pk_ReleaseId = CLOSED_RELEASE_ID, Name = "Fourth release", Fk_ReleaseStatus = 3, Enum_ReleaseStatus = closedStatus });

            return releases.AsQueryable();
        }

        public IQueryable<Ultimate.DomainClasses.Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Ultimate.DomainClasses.Release, object>>[] includeProperties)
        {
            return GenerateList();
        }

        public Ultimate.DomainClasses.Release Find(int id)
        {
            return GenerateList().Where(r => r.Pk_ReleaseId == id).FirstOrDefault();
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
