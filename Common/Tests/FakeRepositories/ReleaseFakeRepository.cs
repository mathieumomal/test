using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class ReleaseFakeRepository : IReleaseRepository
    {
        public static readonly int OpenedReleaseId = 1;
        public static readonly int FrozenReleaseId = 2;
        public static readonly int ClosedReleaseId = 4;
        
        #region IEntityRepository<Release> Members

        public IUltimateUnitOfWork UoW
        { get; set;
        }

        public IQueryable<Release> All
        {
            get
            {
                return GenerateList();
            }
        }

        private IQueryable<Release> GenerateList()
        {
            var aRemark = new Remark { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OpenedReleaseId, IsPublic = true, Pk_RemarkId = 1, RemarkText = "test" };
            var aHistory1 = new History { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OpenedReleaseId, HistoryText = "text 1", Pk_HistoryId = 1 };
            var aHistory2 = new History { CreationDate = DateTime.Now, Fk_PersonId = 0, Fk_ReleaseId = OpenedReleaseId, HistoryText = "text 2", Pk_HistoryId = 2 };

            //Statuses
            var openStatus = new Enum_ReleaseStatus { Enum_ReleaseStatusId = 1, Code = "Open" };
            var frozenStatus = new Enum_ReleaseStatus { Enum_ReleaseStatusId = 2, Code = "Frozen" };
            var closedStatus = new Enum_ReleaseStatus { Enum_ReleaseStatusId = 3, Code = "Closed" };

            //Releases
            var releases = new ReleaseFakeDBSet
            {
                new Release
                {
                    Pk_ReleaseId = OpenedReleaseId,
                    Name = "First release",
                    Code = "Rel-1",
                    ShortName = "R1",
                    Fk_ReleaseStatus = 1,
                    Enum_ReleaseStatus = openStatus,
                    Remarks = new List<Remark> {aRemark},
                    Histories = new List<History> {aHistory1, aHistory2},
                    SortOrder = 10
                },
                new Release
                {
                    Pk_ReleaseId = FrozenReleaseId,
                    Name = "Second release",
                    Fk_ReleaseStatus = 2,
                    Enum_ReleaseStatus = frozenStatus,
                    Code = "Rel-2",
                    EndDate = new DateTime(2014, 04, 10),
                    EndMtgRef = "Test",
                    SortOrder = 20
                },
                new Release
                {
                    Pk_ReleaseId = 3,
                    Name = "Third release",
                    Fk_ReleaseStatus = 2,
                    Enum_ReleaseStatus = frozenStatus,
                    Code = "Rel-3",
                    SortOrder = 30
                },
                new Release
                {
                    Pk_ReleaseId = ClosedReleaseId,
                    Name = "Fourth release",
                    Fk_ReleaseStatus = 3,
                    Enum_ReleaseStatus = closedStatus,
                    Code = "Rel-4",
                    SortOrder = 40
                }
            };

            return releases.AsQueryable();
        }

        public IQueryable<Release> AllIncluding(params System.Linq.Expressions.Expression<Func<Release, object>>[] includeProperties)
        {
            return GenerateList();
        }

        public Release Find(int id)
        {
            return GenerateList().FirstOrDefault(r => r.Pk_ReleaseId == id);
        }

        public void InsertOrUpdate(Release entity)
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

        public List<Release> GetReleasesLinkedToASpec(int specId)
        {
            throw new NotImplementedException();
        }
    }
}
