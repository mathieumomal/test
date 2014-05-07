﻿using System;
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
        public SpecificationRepository()
        {
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
            // Clear the fields that we don't want, to keep only the FKs
            specification.Enum_Serie = null;

            // First set the modified items.
            specification.SpecificationTechnologies.ToList().ForEach(x =>
            {
                if (x.Pk_SpecificationTechnologyId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.Histories.ToList().ForEach(x =>
            {
                if (x.Pk_HistoryId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.Remarks.ToList().ForEach(x =>
            {
                if (x.Pk_RemarkId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.SpecificationRapporteurs.ToList().ForEach(x =>
            {
                if (x.Pk_SpecificationRapporteurId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.SpecificationResponsibleGroups.ToList().ForEach(x =>
            {
                if (x.Pk_SpecificationResponsibleGroupId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.Specification_WorkItem.ToList().ForEach(x =>
            {
                if (x.Pk_Specification_WorkItemId != default(int))
                    UoW.Context.SetModified(x);
            });
            specification.Specification_Release.ToList().ForEach(x =>
            {
                if (x.Pk_Specification_ReleaseId != default(int))
                    UoW.Context.SetModified(x);
            });


            //[2] Add the Entity (It will add the childs as well)
            UoW.Context.SetAdded(specification);

            if (specification.Pk_SpecificationId != default(int))
                UoW.Context.SetModified(specification);
            
            
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Specification entity");
        }


        public KeyValuePair<List<Specification>, int> GetSpecificationBySearchCriteria(SpecificationSearch searchObject)
        {
            IQueryable<Specification> query = AllIncluding(x => x.SpecificationResponsibleGroups, x => x.SpecificationTechnologies.Select(y => y.Enum_Technology))
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
                      && ((searchObject.Technologies.Count == 0) || x.SpecificationTechnologies.Any(y => searchObject.Technologies.Contains(y.Fk_Enum_Technology)))  //Technology Search
                      && ((searchObject.Series.Count == 0) || searchObject.Series.Contains(x.Enum_Serie.Pk_Enum_SerieId)) //Series Search
                      && ((searchObject.SelectedReleaseIds.Count == 0) || x.Specification_Release.Any(y=> searchObject.SelectedReleaseIds.Contains(y.Fk_ReleaseId))) //Release Search
                      && ((searchObject.SelectedCommunityIds.Count == 0) || x.SpecificationResponsibleGroups.Any(y => searchObject.SelectedCommunityIds.Contains(y.Fk_commityId))) //Community Search
                      ));

            // Treat case WiUid is not empty
            if (searchObject.WiUid != default(int))
            {
                query = query.Where(s => s.Specification_WorkItem.Any(wi => wi.Fk_WorkItemId == searchObject.WiUid && wi.IsSetByUser != null && wi.IsSetByUser == true));
            }

            // Manage the order
            switch (searchObject.Order)
            {
                case SpecificationSearch.SpecificationOrder.NumberDesc:
                    query = query.OrderByDescending(s => s.Number);
                    break;
                case SpecificationSearch.SpecificationOrder.Title:
                    query = query.OrderBy(s => s.Title);
                    break;
                case SpecificationSearch.SpecificationOrder.TitleDesc:
                    query = query.OrderByDescending(s => s.Title);
                    break;
                default:
                    query = query.OrderBy(s => s.Number);
                    break;
            }


            return new KeyValuePair<List<Specification>,int>(query.Skip(searchObject.SkipRecords).Take(searchObject.PazeSize).ToList(), query.Count());                
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
        /// <summary>
        /// Returns a page of specifications that are matching the search criteria, along with the total number of specifications matching this criteria.
        /// </summary>
        /// <param name="searchObj"></param>
        /// <returns></returns>
        KeyValuePair<List<Specification>, int> GetSpecificationBySearchCriteria(SpecificationSearch searchObj);

        List<Enum_Technology> GetTechnologyList();

        List<Enum_Serie> GetSeries();
    }
}
