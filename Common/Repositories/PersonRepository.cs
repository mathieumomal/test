using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.Utils;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Default implementation of the PersonRepository. Users the View_Persons in database.
    /// </summary>
    public class PersonRepository : IPersonRepository
    {
        public PersonRepository()
        {
        }

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

        public View_Persons Find(int id)
        {
            return UoW.Context.View_Persons.Find(id, "N");
        }

        public void InsertOrUpdate(View_Persons entity)
        {
            throw new InvalidOperationException("Cannot add or update a person");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion

        
        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface IPersonRepository : IEntityRepository<View_Persons>
    {

    }
}
