using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DataAccess;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class ResponsibleGroupSecretaryRepository : IResponsibleGroupSecretaryRepository
    {
        public ResponsibleGroupSecretaryRepository()
        {
        }

        #region IEntityRepository<ResponsibleGroupSecretaryRepository> Membres

        public IQueryable<ResponsibleGroup_Secretary> All
        {
            get {
                return UoW.Context.ResponsibleGroupSecretaries;
            }
        }

        public IQueryable<ResponsibleGroup_Secretary> AllIncluding(params System.Linq.Expressions.Expression<Func<ResponsibleGroup_Secretary, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public ResponsibleGroup_Secretary Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(ResponsibleGroup_Secretary entity)
        {
            throw new InvalidOperationException("Cannot add or update a person");
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Release status entity");
        }

        public IQueryable<ResponsibleGroup_Secretary> FindAllByCommiteeId(int id)
        {
            return UoW.Context.ResponsibleGroupSecretaries.Where(x => x.TbId == id).AsQueryable();
        }


        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            UoW.Context.Dispose();
        }

        #endregion

        
        public IUltimateUnitOfWork UoW { get; set; }
    }

    public interface IResponsibleGroupSecretaryRepository : IEntityRepository<ResponsibleGroup_Secretary>
    {
        IQueryable<ResponsibleGroup_Secretary> FindAllByCommiteeId(int id);
    }
}
