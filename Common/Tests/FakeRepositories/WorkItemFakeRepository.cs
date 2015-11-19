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
        public enum Sources { Empty, OneLine_Nominal, OneLine_Level0 };

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
        public abstract Sources Source { get; }

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
                        Effective_Acronym = "IOPS",
                        WiLevel = 0,
                        Fk_ReleaseId = ReleaseFakeRepository.OpenedReleaseId,
                        WorkItems_ResponsibleGroups = responsibleGroups,
                        StartDate = new DateTime(2014, 3, 7),
                        EndDate = new DateTime(2014, 9, 17),
                        Completion = 0,
                        Wid = "SP-140069",
                        StatusReport = "SP-140070",
                        RapporteurCompany = "General Dynamics Broadband",
                        RapporteurId = 27904,
                        RapporteurStr = "Mathieu Mangion(mathieu.mangion@etsi.org)",
                        Remarks = remarks,
                        TssAndTrs = "Stage 1",
                        TsgApprovalMtgRef = "SP-63",
                        PcgApprovalMtgRef = "PCG-32",
                        TsgStoppedMtgRef = "SP-64",
                        PcgStoppedMtgRef = "PCG-33",
                        CreationDate = new DateTime(2014, 2, 5),
                        LastModification = new DateTime(2014, 2, 6)


                    });
                    return list;

                case Sources.OneLine_Level0:
                    var level0List = new WorkItemFakeDBSet();
                    level0List.Add(new WorkItem()
                    {
                        Pk_WorkItemUid = 100000005,
                        WorkplanId = 6,
                        Name = "Release 13 Features",
                    });
                    return level0List;
            }
            return null;

        }

        public IQueryable<Ultimate.DomainClasses.WorkItem> AllIncluding(params System.Linq.Expressions.Expression<Func<Ultimate.DomainClasses.WorkItem, object>>[] includeProperties)
        {
            return GenerateList(Source);
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

        #region IWorkItemRepository Members

        public List<WorkItem> GetWorkItemsBySearchCriteria(List<int> releaseIds, int granularity, string wiAcronym, string wiName, List<int> tbIds)
        {
            throw new NotImplementedException();
        }

        public int GetWorkItemsCountBySearchCriteria(List<int> releaseIds, int granularity, bool hidePercentComplete, string wiAcronym, string wiName, List<int> tbIds)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllAcronyms()
        {
            throw new NotImplementedException();
        }

        public List<WorkItem> GetWorkItemsBySearchCriteria(string searchString, bool shouldHaveAcronym)
        {
            throw new NotImplementedException();
        }

        public List<WorkItem> GetWorkItemsByIds(List<int> workItems)
        {
            throw new NotImplementedException();
        }

        public List<WorkItem> GetAllWorkItemsForReleases(List<int> releaseIds)
        {
            throw new NotImplementedException();
        }

        public List<WorkItem> GetWorkItemsByKeywords(List<string> keywords)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class WorkItemOneLineLevel0FakeRepository : WorkItemFakeRepository
    {
        public override Sources Source { get { return Sources.OneLine_Level0; } }
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
