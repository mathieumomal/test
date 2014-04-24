using Etsi.Ultimate.DataAccess;
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
    class SpecificationWIFakeRepository : ISpecificationWorkItemRepository
    {


        public SpecificationWIFakeRepository()
        {
        }

        #region IEntityRepository<SpecificationWIFakeRepository> Membres

        public IQueryable<Specification_WorkItem> All
        {
            get { return GenerateList(); }
        }

        private IQueryable<Specification_WorkItem> GenerateList()
        {
            var tmpList = new List<Specification_WorkItem>()
            {
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 1,
                    isPrime = true,
                    WorkItem = new WorkItem(){
                        Pk_WorkItemUid = 1,
                        Acronym = "acro1",
                        WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =1,
                                ResponsibleGroup = "S1",
                                IsPrimeResponsible = true
                            },
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =2,
                                ResponsibleGroup = "S3"
                            }
                        }
                    }
                },
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 2,
                    isPrime = false,
                    WorkItem = new WorkItem(){
                        Pk_WorkItemUid = 2,
                        Acronym = "acro2",
                        WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =1,
                                ResponsibleGroup = "S2"
                            },
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =2,
                                ResponsibleGroup = "S1",
                                IsPrimeResponsible = true
                            }
                        }
                    }  
                },
                new Specification_WorkItem(){
                    Pk_Specification_WorkItemId = 3,
                    isPrime = false,
                    WorkItem = new WorkItem(){
                        Pk_WorkItemUid = 3,
                        Acronym = "acro3",
                        WorkItems_ResponsibleGroups = new List<WorkItems_ResponsibleGroups>(){
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =1,
                                ResponsibleGroup = "S3"
                            },
                            new WorkItems_ResponsibleGroups(){
                                Pk_WorkItemResponsibleGroups =2,
                                ResponsibleGroup = "S2",
                                IsPrimeResponsible = true
                            }
                        }
                    }  
                }
            };  
            var dbSet = new SpecificationWorkItemFakeDBSet();
            tmpList.ForEach(e =>  dbSet.Add(e));           

            return dbSet.AsQueryable();
        }

        public IQueryable<Specification_WorkItem> AllIncluding(params System.Linq.Expressions.Expression<Func<Specification_WorkItem, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public Specification_WorkItem Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Specification_WorkItem entity)
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
