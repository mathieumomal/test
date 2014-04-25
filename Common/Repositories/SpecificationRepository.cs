using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using System.Web;
using Etsi.Ultimate.Utils;
using System.Data.Entity.Core.Objects;

namespace Etsi.Ultimate.Repositories
{
    public class SpecificationRepository : ISpecificationRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }
        public SpecificationRepository(IUltimateUnitOfWork iUoW)
        {
            UoW = iUoW;
        }

        #region IEntityRepository<Specification> Membres

        /// <summary>
        /// Ret
        /// </summary>
        public IQueryable<Specification> All
        {
            get
            {
                return AllIncluding(t => t.SpecificationResponsibleGroups);
            }
        }

        public IQueryable<Specification> AllIncluding(params System.Linq.Expressions.Expression<Func<Specification, object>>[] includeProperties)
        {
            IQueryable<Specification> query = UoW.Context.Specifications;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Specification Find(int id)
        {
            return AllIncluding(t => t.Enum_Serie, t => t.Enum_SpecificationStage, t => t.Remarks, t => t.Histories,
                t => t.SpecificationTechnologies, t => t.SpecificationResponsibleGroups, t => t.SpecificationRapporteurs,
                t => t.Specification_Release).Where(x => x.Pk_SpecificationId == id).FirstOrDefault();
        }

        public void InsertOrUpdate(Specification specification)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Specification entity");
        }


        public List<Specification> GetSpecificationBySearchCriteria(SpecificationSearch searchObject)
        {
            return AllIncluding(x => x.SpecificationTechnologies.Select(y => y.Enum_Technology)).ToList();
            //return AllIncluding(t => t.SpecificationTechnologies).ToList();
            //return AllIncluding(x => (x.Title.ToLower().Contains(searchObject.Title.ToLower()) || x.Number.ToLower().Contains(searchObject.Title.ToLower()))
            //    && x.Specification_Release.All(y => searchObject.SelectedReleaseIds.Contains(y.Pk_Specification_ReleaseId))
            //    && (searchObject.IsUnderCC ? (x.IsActive && x.IsUnderChangeControl.Value) : false ||
            //         searchObject.IsDraft ? (x.IsActive && !x.IsUnderChangeControl.Value) : false ||
            //         searchObject.IsWithACC ? (!x.IsActive && x.IsUnderChangeControl.Value) : false ||
            //         searchObject.IsWithBCC ? (!x.IsActive && !x.IsUnderChangeControl.Value) : false)
            //         && (searchObject.Type != null ? x.Type.Value == searchObject.Type.Value : false)
            //         && (searchObject.NumberNotYetAllocated != null ? x.Type.Value == searchObject.NumberNotYetAllocated.Value : false)
            //         && (searchObject.IsForPublication != null ? x.IsForPublication.Value == searchObject.IsForPublication.Value : false)
            //         && (searchObject.Technologies.Count > 0 ? x.SpecificationTechnologies.All(y => searchObject.Technologies.Contains(y.Pk_SpecificationTechnologyId)) : false)
            //         && (searchObject.SelectedCommunityIds.Count > 0 ? x.SpecificationResponsibleGroups.All(y => searchObject.SelectedCommunityIds.Contains(y.Pk_SpecificationResponsibleGroupId)) : false)).ToList();
        }

        public List<Enum_Technology> GetTechnologyList()
        {
            return UoW.Context.Enum_Technology.ToList();
        }

        public List<Enum_Serie> GetSeries()
        {
            return UoW.Context.Enum_Serie.ToList();
        }
        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
        }

        #endregion
    }

    public interface ISpecificationRepository : IEntityRepository<Specification>
    {
        List<Specification> GetSpecificationBySearchCriteria(SpecificationSearch searchObj);

        List<Enum_Technology> GetTechnologyList();

        List<Enum_Serie> GetSeries();
    }
}
