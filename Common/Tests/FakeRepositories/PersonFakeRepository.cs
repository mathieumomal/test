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
    class PersonFakeRepository : IPersonRepository
    {

        public PersonFakeRepository() { }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEntityRepository<View_Persons> Members

        public IUltimateUnitOfWork UoW
        {
            get; set;
        }

        public IQueryable<View_Persons> All
        {
            get { return GenerateList(); }
        }

        private IQueryable<View_Persons> GenerateList()
        {
            var dbSet = new PersonFakeDBSet();

            dbSet.Add(new View_Persons() { PERSON_ID = 27904, Email = "mathieu.mangion@etsi.org", FIRSTNAME = "Mathieu", LASTNAME = "Mangion", ORGA_ID = 10, ORGA_NAME = "ETSI", Username = "mangion" });
            dbSet.Add(new View_Persons() { PERSON_ID = 9568, Email = "laurent.vreck@etsi.org", FIRSTNAME = "Laurent", LASTNAME = "Vreck", ORGA_ID = 10, ORGA_NAME = "ETSI", Username = "vreck" });
            dbSet.Add(new View_Persons() { PERSON_ID = 9568, Email = "", FIRSTNAME = "Xy", LASTNAME = "Zhang", ORGA_ID = 10, ORGA_NAME = "ETSI", Username = "Xyzhang" });

            return dbSet.AsQueryable();
        }

        

        public IQueryable<View_Persons> AllIncluding(params System.Linq.Expressions.Expression<Func<View_Persons, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public View_Persons Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(View_Persons entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region IPersonRepository Members

        public List<View_Persons> FindByIds(List<int> personIds)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
