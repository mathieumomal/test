using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using System.Data.Entity;

namespace Etsi.Ultimate.Repositories
{
    public class SpecVersionsRepository : ISpecVersionsRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }

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

        public List<SpecVersion> GetVersionsByReleaseId(int releaseId)
        {
            return AllIncluding().Where(x => (x.Fk_ReleaseId != null) ? x.Fk_ReleaseId.Value == releaseId : false).ToList();
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

        /// <summary>
        /// Remove version
        /// </summary>
        /// <param name="version"></param>
        public void Delete(SpecVersion version)
        {
            UoW.Context.SpecVersions.Remove(version);
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

        /// <summary>
        /// See interface
        /// </summary>
        /// <param name="relatedTdoc"></param>
        /// <returns></returns>
        public List<SpecVersion> GetVersionsByRelatedTDoc(string relatedTdoc)
        {
            return UoW.Context.SpecVersions.Where(x => x.RelatedTDoc == relatedTdoc).ToList();
        }

        /// <summary>
        /// Update version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public void UpdateVersion(SpecVersion version)
        {
            version.Remarks.ToList().ForEach(x =>
            {
                if (x.Pk_RemarkId != default(int))
                    UoW.Context.SetModified(x);
            });

            UoW.Context.SetAdded(version);

            if (version.Pk_VersionId != default(int))
                UoW.Context.SetModified(version);
        }

        #endregion

        #region IDisposable Membres

        public void Dispose()
        {
            //context.Dispose();
        }

        #endregion

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get already uploaded versions for a spec
        /// </summary>
        /// <returns>List of versions of this spec already uploaded</returns>
        public List<SpecVersion> AlreadyUploadedVersionsForSpec(int specId)
        {
            return
                UoW.Context.SpecVersions.Where(x => x.Fk_SpecificationId == specId && (!string.IsNullOrEmpty(x.Location) || x.DocumentUploaded != null))
                    .ToList();
        }

        /// <summary>
        /// Get versions of a spec linked to some CRs 
        /// </summary>
        /// <param name="specId">Spec id</param>
        /// <returns>List of versions of this spec linked to CRs (with CRs objects)</returns>
        public List<SpecVersion> VersionsLinkedToChangeRequestsForSpec(int specId)
        {
            return
                UoW.Context.SpecVersions.Include(x => x.CurrentChangeRequests).Include(x => x.FoundationsChangeRequests)
                    .Where(x => x.Fk_SpecificationId == specId && (x.CurrentChangeRequests.Count > 0 || x.FoundationsChangeRequests.Count > 0))
                    .ToList();
        }

        /// <summary>
        /// Find list of crs linked to a version
        /// </summary>
        /// <param name="versionId">version id</param>
        /// <returns>List of versions</returns>
        public SpecVersion FindCrsLinkedToAVersion(int versionId)
        {
            return
                UoW.Context.SpecVersions.Include(x => x.CurrentChangeRequests).Include(x => x.FoundationsChangeRequests)
                    .FirstOrDefault(x => x.Pk_VersionId == versionId);
        }

        /// <summary>
        /// Get the latest version of each spec release where Spec is UCC and Release is Open or Frozen
        /// </summary>
        /// <returns>list of specVersion</returns>
        public List<SpecVersion> GetLatestVersionGroupedBySpecRelease()
        {
            return UoW.Context.SpecVersions.Include(x => x.Release).Include(x => x.Specification)
                        .Where(sv => (sv.Specification.IsActive && sv.Specification.IsUnderChangeControl.HasValue 
                            && sv.Specification.IsUnderChangeControl.Value)
                                && (sv.Release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Open
                                    || sv.Release.Enum_ReleaseStatus.Code == Enum_ReleaseStatus.Frozen))
                                        .GroupBy(x => new { x.Fk_SpecificationId, x.Fk_ReleaseId })
                                            .Select(group => new
                                            {
                                                key = group.Key,
                                                lastVersion = group.OrderByDescending(major => major.MajorVersion)
                                                    .ThenByDescending(technical => technical.TechnicalVersion)
                                                        .ThenByDescending(editorial => editorial.EditorialVersion).FirstOrDefault()
                                            }).Select(x => x.lastVersion).ToList();
        }
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
        /// <param name="releaseId">Release identifier</param>
        /// <returns></returns>
        List<SpecVersion> GetVersionsByReleaseId(int releaseId);

        /// <summary>
        /// Count the number of version which pending upload for a release
        /// </summary>
        /// <param name="releaseMajorNumber"> The 3G decimal number of version, that is used to determine.</param>
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
        /// <param name="specIds"></param>
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

        /// <summary>
        /// See implementation
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        void UpdateVersion(SpecVersion version);

        /// <summary>
        /// Remove version
        /// </summary>
        /// <param name="version"></param>
        void Delete(SpecVersion version);

        /// <summary>
        /// Get uploaded versions for a spec
        /// </summary>
        /// <returns>List of versions of this spec already uploaded</returns>
        List<SpecVersion> AlreadyUploadedVersionsForSpec(int specId);

        /// <summary>
        /// Get versions of a spec linked to some CRs 
        /// </summary>
        /// <param name="specId">Spec id</param>
        /// <returns>List of versions of this spec linked to CRs</returns>
        List<SpecVersion> VersionsLinkedToChangeRequestsForSpec(int specId);

        /// <summary>
        /// Find list of crs linked to a version
        /// </summary>
        /// <param name="versionId">version id</param>
        /// <returns>List of versions</returns>
        SpecVersion FindCrsLinkedToAVersion(int versionId);

        /// <summary>
        /// Get the latest version of each spec release where Spec is UCC and Release is Open or Frozen
        /// </summary>
        /// <returns>list of specVersion</returns>
        List<SpecVersion> GetLatestVersionGroupedBySpecRelease();
    }
}
