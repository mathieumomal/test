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
    public abstract class WorkItemFakeRepository : IWorkItemRepository
    {
        public enum Sources { Empty, OneLine_Nominal };

        public WorkItemFakeRepository() { }

        #region IEntityRepository<Release> Members

        public IUltimateUnitOfWork UoW
        {
            get;
            set;
        }

        /// <summary>
        /// Dedicated input source
        /// </summary>
        public abstract Sources Source { get;  }

        public IQueryable<Ultimate.DomainClasses.WorkItem> All
        {
            get
            {
                return GenerateList(Source);
            }
        }

        private IQueryable<Ultimate.DomainClasses.WorkItem> GenerateList(Sources source)
        {
            switch (source)
            {
                case Sources.Empty:
                    return new WorkItemFakeDBSet();

                case Sources.OneLine_Nominal:
                    var list = new WorkItemFakeDBSet();
                    var remarks = new List<Remark>() { new Remark() { RemarkText = "Triggered by Rel-13 TR 22.897 Study on Isolated E-UTRAN Operation for Public Safety (FS_IOPS)" } };
                    var responsibleGroups = new List<WorkItems_ResponsibleGroups>() { new WorkItems_ResponsibleGroups() { IsPrimeResponsible = true, ResponsibleGroup = "S1" }, new WorkItems_ResponsibleGroups() { IsPrimeResponsible = false, ResponsibleGroup = "S2" } };
                    list.Add(new WorkItem()
                    {
                        Pk_WorkItemUid = 630000,
                        WorkplanId = 17,
                        Name = "Isolated E-UTRAN Operation for Public Safety",
                        Acronym = "IOPS",
                        WiLevel = 0,
                        Fk_ReleaseId = ReleaseFakeRepository.OPENED_RELEASE_ID,
                        WorkItems_ResponsibleGroups = responsibleGroups,
                        StartDate = new DateTime(2014, 3, 7),
                        EndDate = new DateTime(2014, 9, 17),
                        Completion = 0,
                        Wid = "SP-140069",
                        StatusReport = "SP-140070",
                        RapporteurCompany = "General Dynamics Broadband",
                        RapporteurId = 27904,
                        RapporteurStr = "mathieu.mangion@etsi.org",
                        Remarks = remarks,
                        TssAndTrs = "Stage 1",
                        TsgApprovalMtgRef = "SP-63",
                        PcgApprovalMtgRef = "PCG-32",
                        TsgStoppedMtgRef = "SP-64",
                        PcgStoppedMtgRef = "PCG-33",
                        CreationDate = new DateTime(2014,2,5),
                        LastModification = new DateTime(2014,2,6)
                        
                        
                    });
                    return list;

            }
            return null;

        }

        public IQueryable<Ultimate.DomainClasses.WorkItem> AllIncluding(params System.Linq.Expressions.Expression<Func<Ultimate.DomainClasses.WorkItem, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Ultimate.DomainClasses.WorkItem Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Ultimate.DomainClasses.WorkItem entity)
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

    public class WorkItemOneLineFakeRepository : WorkItemFakeRepository
    {
        public override Sources Source { get { return Sources.OneLine_Nominal; } }
    }

    public class WorkItemEmptyFakeRepository : WorkItemFakeRepository
    {
        public override Sources Source { get { return Sources.Empty; } }
    }
}
