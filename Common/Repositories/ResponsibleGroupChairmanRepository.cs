using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    public class ResponsibleGroupChairmanRepository : IResponsibleGroupChairmanRepository
    {
        #region IEntityRepository<ResponsibleGroup_Chairman> Membres

        public IUltimateUnitOfWork UoW { get; set; }

        public IQueryable<ResponsibleGroup_Chairman> All
        {
            get { return UoW.Context.ResponsibleGroupChairmans; }
        }

        public IQueryable<ResponsibleGroup_Chairman> AllIncluding(params System.Linq.Expressions.Expression<Func<ResponsibleGroup_Chairman, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public ResponsibleGroup_Chairman Find(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(ResponsibleGroup_Chairman entity)
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

        #region IResponsibleGroupChairmanRepository Membres

        public IQueryable<ResponsibleGroup_Chairman> FindAllByCommiteeId(int id)
        {
            return UoW.Context.ResponsibleGroupChairmans.Where(x => x.TbId == id).AsQueryable();
        }

        #endregion
    }

    public interface IResponsibleGroupChairmanRepository : IEntityRepository<ResponsibleGroup_Chairman>
    {
        IQueryable<ResponsibleGroup_Chairman> FindAllByCommiteeId(int id);
    }
}
