using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Repositories
{
    /// <summary>
    /// Remark : this view doesn't consider CR data. Only data concerning other type of contributions are returned
    /// </summary>
    public class ViewContributionsWithAditionnalDataRepository : IViewContributionsWithAditionnalDataRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Find all contributions related to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <returns>List of contributions found</returns>
        public List<View_ContributionsWithAditionnalData> FindContributionsRelatedToASpec(int specId)
        {
            return
                UoW.Context.View_ContributionsWithAditionnalData.Where(
                    x =>
                        x.fk_SpecificationId == specId)
                    .ToList();
        }

        /// <summary>
        /// Find all contributions related to a spec and a specific version
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <param name="major">Major version number</param>
        /// <param name="technical">Technical version number</param>
        /// <param name="editorial">Editorial version number</param>
        /// <returns>List of contributions found</returns>
        public List<View_ContributionsWithAditionnalData> FindContributionsRelatedToASpecAndVersionNumber(int specId,
            int major, int technical, int editorial)
        {
            return
                UoW.Context.View_ContributionsWithAditionnalData.Where(
                    x =>
                        x.fk_SpecificationId == specId &&
                        x.Specification_MajorVersion == major &&
                        x.Specification_TechnicalVersion == technical && 
                        x.Specification_EditorialVersion == editorial)
                    .ToList();
        }

        #region inherited methods

        public System.Linq.IQueryable<View_ContributionsWithAditionnalData> All
        {
            get { throw new System.NotImplementedException(); }
        }

        public System.Linq.IQueryable<View_ContributionsWithAditionnalData> AllIncluding(params System.Linq.Expressions.Expression<System.Func<View_ContributionsWithAditionnalData, object>>[] includeProperties)
        {
            throw new System.NotImplementedException();
        }

        public View_ContributionsWithAditionnalData Find(int id)
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdate(View_ContributionsWithAditionnalData entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

    public interface IViewContributionsWithAditionnalDataRepository : IEntityRepository<View_ContributionsWithAditionnalData>
    {
        /// <summary>
        /// Find all contributions related to a spec
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <returns>List of contributions found</returns>
        List<View_ContributionsWithAditionnalData> FindContributionsRelatedToASpec(int specId);

        /// <summary>
        /// Find all contributions related to a spec and a specific version
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <param name="major">Major version number</param>
        /// <param name="technical">Technical version number</param>
        /// <param name="editorial">Editorial version number</param>
        /// <returns>List of contributions found</returns>
        List<View_ContributionsWithAditionnalData> FindContributionsRelatedToASpecAndVersionNumber(int specId, int major, int technical, int editorial);
    }
}
