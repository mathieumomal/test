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
            return AllIncluding(x => x.SpecificationTechnologies.Select(y => y.Enum_Technology))
                .Where(x => ((String.IsNullOrEmpty(searchObject.Title) || (x.Title.ToLower().Trim().Contains(searchObject.Title.ToLower().Trim()) || x.Number.ToLower().Trim().Contains(searchObject.Title.ToLower().Trim())))
                  && ((searchObject.Type == null) || (x.IsTS == searchObject.Type.Value))                                      //Type Search
                  && ((searchObject.IsForPublication == null) || (x.IsForPublication == searchObject.IsForPublication.Value))  //Publication Search
                  && (searchObject.NumberNotYetAllocated ? (String.IsNullOrEmpty(x.Number)) : true)  //Number not yet allocated
                  && ((!(searchObject.IsUnderCC || searchObject.IsDraft || searchObject.IsWithACC || searchObject.IsWithBCC))  //Status Search
                  || ((searchObject.IsUnderCC && searchObject.IsDraft && searchObject.IsWithACC && searchObject.IsWithBCC))
                  ||(searchObject.IsUnderCC ? (x.IsActive && x.IsUnderChangeControl.Value) : false ||
                     searchObject.IsDraft ? (x.IsActive && !x.IsUnderChangeControl.Value) : false ||
                     searchObject.IsWithACC ? (!x.IsActive && x.IsUnderChangeControl.Value) : false ||
                     searchObject.IsWithBCC ? (!x.IsActive && !x.IsUnderChangeControl.Value) : false))
                  && ((searchObject.Technologies.Count == 0) || searchObject.Technologies.All(y => x.SpecificationTechnologies.Any(z => z.Fk_Enum_Technology == y)))    //x.SpecificationTechnologies.Any(y => !searchObject.Technologies.Contains(y.Fk_Enum_Technology))) //Technology Search
                  && ((searchObject.Series.Count == 0) || searchObject.Series.Contains(x.Enum_Serie.Pk_Enum_SerieId)) //Series Search
                  && ((searchObject.SelectedReleaseIds.Count == 0) || x.Specification_Release.Any(y=> searchObject.SelectedReleaseIds.Contains(y.Fk_ReleaseId))) //Release Search
                  && ((searchObject.SelectedCommunityIds.Count == 0) || x.SpecificationResponsibleGroups.Any(y => searchObject.SelectedCommunityIds.Contains(y.Fk_commityId))) //Community Search
                  ))
                .ToList();
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
