using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Tests.FakeSets;

namespace Etsi.Ultimate.Tests.FakeRepositories
{
    class WorkPlanFileFakeRepository : IWorkPlanFileRepository
    {
        public WorkPlanFileFakeRepository()
        {
        }

        #region IEntityRepository<WorkPlanFileFakeRepository> Membres

        public IQueryable<WorkPlanFile> All
        {
            get { return GenerateList(); }
        }

        public IQueryable<WorkPlanFile> AllIncluding(params System.Linq.Expressions.Expression<Func<WorkPlanFile, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public WorkPlanFile Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(WorkPlanFile entity)
        {
            
        }


        public void Delete(int id)
        {
            throw new InvalidOperationException();
        }

        private IQueryable<WorkPlanFile> GenerateList()
        {
            var tmpList = new List<WorkPlanFile>()
            {
                new WorkPlanFile()
                {
                    Pk_WorkPlanFileId = 1,
                    CreationDate = DateTime.Today.AddDays(-2),
                    WorkPlanFilePath="Path1"
                },
                new WorkPlanFile()
                {
                    Pk_WorkPlanFileId = 2,
                    CreationDate = DateTime.Today.AddDays(-4),
                    WorkPlanFilePath="Path2"
                },
                new WorkPlanFile()
                {
                    Pk_WorkPlanFileId = 3,
                    CreationDate = DateTime.Today.AddDays(-1),
                    WorkPlanFilePath="Path3"
                },
                new WorkPlanFile()
                {
                    Pk_WorkPlanFileId = 4,
                    CreationDate = DateTime.Today.AddDays(-3),
                    WorkPlanFilePath="Path4"
                }
            };

            var dbSet = new WorkPlanFileFakeDBSet();
            tmpList.ForEach(e => dbSet.Add(e));

            return dbSet.AsQueryable(); 
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
