using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Default implementation of the PersonRepository. Users the View_Persons in database.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }

        #region IEntityRepository<PersonRepository> Membres

        public IQueryable<View_Persons> All
        {
            get { 
                return UoW.Context.View_Persons;
            }
        }

        public IQueryable<View_Persons> AllIncluding(params System.Linq.Expressions.Expression<Func<View_Persons, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(View_Persons entity)
        {
            throw new InvalidOperationException("Cannot add or update a person");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        public List<View_Persons> FindByIds(List<int> personIds)
        {
            return UoW.Context.View_Persons.Where(p => personIds.Contains(p.PERSON_ID)).ToList();
        }

        /// <summary>
        /// Return only not deleted persons
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns></returns>
        public View_Persons Find(int id)
        {
            return UoW.Context.View_Persons.Find(id, "N");
        }

        /// <summary>
        /// Return deleted or not person
        /// </summary>
        /// <param name="id">Person ID</param>
        /// <returns></returns>
        public View_Persons FindDeletedOrNot(int id)
        {
            return UoW.Context.View_Persons.FirstOrDefault(x => x.PERSON_ID == id);
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion

        
        
    }

    public interface IPersonRepository : IEntityRepository<View_Persons>
    {
        List<View_Persons> FindByIds(List<int> personIds);
        View_Persons FindDeletedOrNot(int id);
    }
}
