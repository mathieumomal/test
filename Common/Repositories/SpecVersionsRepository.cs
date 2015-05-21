using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.DataAccess;
using System.Data.Entity;

namespace Etsi.Ultimate.Repositories
{
    public class SpecVersionsRepository : ISpecVersionsRepository
    {
        private IUltimateContext _context;
        public SpecVersionsRepository(IUltimateUnitOfWork iUoW)
        {
            _context = iUoW.Context;
        }

        #region IEntityRepository<SpecVersionRepository> Membres

        public IQueryable<SpecVersion> All
        {
            get
            {
                return AllIncluding(v => v.Remarks, v => v.Release, v => v.Specification.SpecificationRapporteurs);
            }
        }

        public IQueryable<SpecVersion> AllIncluding(params System.Linq.Expressions.Expression<Func<SpecVersion, object>>[] includeProperties)
        {
            IQueryable<SpecVersion> query = UoW.Context.SpecVersions;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public SpecVersion Find(int id)
        {
            return AllIncluding(v => v.Remarks, v=> v.Release, v => v.Specification).Where(v => v.Pk_VersionId == id).FirstOrDefault();
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        public List<SpecVersion> GetVersionsWithFoundationsCrsBySpecId(int specId)
        {
            return AllIncluding(x => x.FoundationsChangeRequests).Where(x => (x.Fk_SpecificationId != null) && x.Fk_SpecificationId.Value == specId).ToList();
        } 

        public List<SpecVersion> GetVersionsForSpecRelease(int specId, int releaseId)
        {
            return AllIncluding(v => v.Remarks).Where(v => v.Fk_SpecificationId == specId && v.Fk_ReleaseId == releaseId).ToList();
        }

        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            return All.Where(x => (x.Fk_SpecificationId != null) ? x.Fk_SpecificationId.Value == specificationId : false).ToList();
        }

        public List<SpecVersion> GetVersionsByReleaseId(int ReleaseId)
        {
            return AllIncluding().Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == ReleaseId : false).ToList();
        }

        public int CountVersionsPendingUploadByReleaseId(int releaseMajorVersion)
        {
            return UoW.Context.SpecVersions.Where(x => x.MajorVersion == releaseMajorVersion && (x.Location == null || x.Location == "")).Count();  
        }

        public void InsertOrUpdate(SpecVersion entity)
        {
            //[1] Add Existing Childs First
            entity.Remarks.ToList().ForEach(x =>
            {
                if (x.Pk_RemarkId != default(int))
                    UoW.Context.SetModified(x);                
            });
            
            //[2] Add the Entity (It will add the childs as well)
            UoW.Context.SetAdded(entity);

            
        }

        /// <summary>
        /// Get latest versions of each release for the given spec Ids
        /// </summary>
        /// <param name="specIds">The specification identifiers</param>
        /// <returns>List of latest spec versions for each release</returns>
        public List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds)
        {
            var versions = UoW.Context.SpecVersions.Where(z => specIds.Contains(z.Fk_SpecificationId ?? 0)).ToList();

            var latestVersions = versions.GroupBy(x => new { x.Fk_SpecificationId, x.Fk_ReleaseId })
                                         .Select(y => y.OrderByDescending(major => major.MajorVersion)
                                                       .ThenByDescending(technical => technical.TechnicalVersion)
                                                       .ThenByDescending(editorial => editorial.EditorialVersion).First())
                                         .ToList();
            return latestVersions;
        }

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="specIds"></param>
        /// <param name="allowedMajorVersions"></param>
        /// <returns></returns>
        public List<SpecVersion> GetVersionsBySpecIds(List<int> specIds, List<int> allowedMajorVersions)
        {
            var specIdsOptional = new List<int?>();
            specIds.ForEach(s => specIdsOptional.Add(s));

            var versionsOptional = new List<int?>();
            allowedMajorVersions.ForEach(s => versionsOptional.Add(s));

            return UoW.Context.SpecVersions.Where(v => specIdsOptional.Contains(v.Fk_SpecificationId)
                                                && versionsOptional.Contains(v.MajorVersion)).ToList();
        }

        public void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete Version entity");
        }

        /// <summary>
        /// Get Version info based on the given parameters
        /// </summary>
        /// <param name="specId">The specification identifier</param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <returns>Version entity</returns>
        public SpecVersion GetVersion(int specId, int majorVersion, int technicalVersion, int editorialVersion)
        {
            return UoW.Context.SpecVersions.FirstOrDefault(x => x.Fk_SpecificationId == specId
                                                             && x.MajorVersion == majorVersion
                                                             && x.TechnicalVersion == technicalVersion
                                                             && x.EditorialVersion == editorialVersion);
        }

        public List<SpecVersion> GetVersionsByRelatedTDoc(string relatedTdoc)
        {
            return UoW.Context.SpecVersions.Where(x => x.RelatedTDoc == relatedTdoc).ToList();
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            //context.Dispose();
        }

        #endregion

        public IUltimateUnitOfWork UoW { get; set; }
    }
    public interface ISpecVersionsRepository : IEntityRepository<SpecVersion>
    {
        /// <summary>
        /// Get versions with associated foundations CRs
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        List<SpecVersion> GetVersionsWithFoundationsCrsBySpecId(int specId);

        /// <summary>
        /// Return a list of SpecVersion for a specification release
        /// </summary>
        /// <param name="specId">Specification id</param>
        /// <param name="releaseId">Release id</param>
        /// <returns>List of specVersions</returns>
        List<SpecVersion> GetVersionsForSpecRelease(int specId, int releaseId);

        /// <summary>
        /// Return a list of SpecVersion for a specification (including Releases of the specification)
        /// </summary>
        /// <param name="specificationId">Identifier og the specification</param>
        /// <returns>List of specVersions</returns>
        List<SpecVersion> GetVersionsBySpecId(int specificationId);

        /// <summary>
        /// Return a list of SpecVersion for a release
        /// </summary>
        /// <param name="ReleaseId">Release identifier</param>
        /// <returns></returns>
        List<SpecVersion> GetVersionsByReleaseId(int ReleaseId);

        /// <summary>
        /// Count the number of version which pending upload for a release
        /// </summary>
        /// <param name="ReleaseMajorNumber"> The 3G decimal number of version, that is used to determine.</param>
        /// <returns></returns>
        int CountVersionsPendingUploadByReleaseId(int releaseMajorNumber);

        /// <summary>
        /// Get latest versions of each release for the given spec Ids
        /// </summary>
        /// <param name="specIds">The specification identifiers</param>
        /// <returns>List of latest spec versions for each release</returns>
        List<SpecVersion> GetLatestVersionsBySpecIds(List<int> specIds);

        /// <summary>
        /// Returns all specifications given a specification Ids and a list of allowed major versions
        /// </summary>
        /// <param name="list"></param>
        /// <param name="allowedMajorVersions"></param>
        /// <returns></returns>
        List<SpecVersion> GetVersionsBySpecIds(List<int> specIds, List<int> allowedMajorVersions);

        /// <summary>
        /// Get Version info based on the given parameters
        /// </summary>
        /// <param name="specId">The specification identifier</param>
        /// <param name="majorVersion">Major version</param>
        /// <param name="technicalVersion">Technical version</param>
        /// <param name="editorialVersion">Editorial version</param>
        /// <returns>Version entity</returns>
        SpecVersion GetVersion(int specId, int majorVersion, int technicalVersion, int editorialVersion);

        /// <summary>
        /// Returns versions matching related TDoc.
        /// 
        /// Note: there should be only one version, but because of legacy data, we defensively return a list.
        /// </summary>
        /// <param name="relatedTdoc"></param>
        /// <returns></returns>
        List<SpecVersion> GetVersionsByRelatedTDoc(string relatedTdoc);
    }
}
