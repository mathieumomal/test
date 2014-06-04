﻿using Etsi.Ultimate.Business.Security;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Business
{
    public class SpecVersionsManager
    {
        private const string CONST_QUALITY_CHECK_REVISIONMARK = "Some revision marks left unaccepted";
        private const string CONST_QUALITY_CHECK_VERSION_HISTORY = "Invalid version in history";
        private const string CONST_QUALITY_CHECK_VERSION_COVERPAGE = "Invalid version in cover page";
        private const string CONST_QUALITY_CHECK_DATE_COVERPAGE = "Invalid date in cover page";
        private const string CONST_QUALITY_CHECK_YEAR_COPYRIGHT = "Year not valid in copyright statement";
        private const string CONST_QUALITY_CHECK_TITLE_COVERPAGE = "Incorrect title in cover page";

        private IUltimateUnitOfWork _uoW;

        public SpecVersionsManager(IUltimateUnitOfWork UoW)
        {
            _uoW = UoW;
        }

        /// <summary>
        /// Returns a list of specVersion of a specification
        /// </summary>
        /// <param name="specificationId">Specification Id</param>
        /// <returns>List of SpecVersions including related releases</returns>
        public List<SpecVersion> GetVersionsBySpecId(int specificationId)
        {
            ISpecVersionsRepository specVersionRepo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            specVersionRepo.UoW = _uoW;

            var specVersions = specVersionRepo.GetVersionsBySpecId(specificationId);
            return new List<SpecVersion>(specVersions);
        }

        /// <summary>
        /// Returns the list of versions of a specification release
        /// </summary>
        /// <param name="specificationId">The specification identifier</param>
        /// <param name="releaseId">The identifier of the specification's release</param>
        /// <returns>List of versions objects</returns>
        public List<SpecVersion> GetVersionsForASpecRelease(int specificationId, int releaseId)
        {
            List<SpecVersion> result = new List<SpecVersion>(); 
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            result = repo.GetVersionsForSpecRelease(specificationId, releaseId);
            return result;
        }

        /// <summary>
        /// Return a SpecVersion and current user rights objects using identifiers
        /// </summary>
        /// <param name="versionId">The identifier of the requested version</param>
        /// <returns>A couple (version,userrights)</returns>
        public KeyValuePair<SpecVersion, UserRightsContainer> GetSpecVersionById(int versionId, int personId)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;

            //New version
            if(versionId == -1)
                return new KeyValuePair<SpecVersion, UserRightsContainer>(new SpecVersion { MajorVersion = -1, TechnicalVersion = -1, EditorialVersion = -1 }, null);

            SpecVersion version = repo.Find(versionId);
            if (version == null)
                return new KeyValuePair<SpecVersion, UserRightsContainer>(null, null);

            // Computes the rights of the user. These are independant from the releases.
            var rightManager = ManagerFactory.Resolve<IRightsManager>();
            rightManager.UoW = _uoW;
            var personRights = rightManager.GetRights(personId);

            // Get information about the releases, in particular the status.
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = _uoW;
            var releases = releaseMgr.GetAllReleases(personId).Key;

            var specificationManager = new SpecificationManager();
            specificationManager.UoW = _uoW;
            //Get calculated rights
            KeyValuePair<Specification_Release, UserRightsContainer> specRelease_Rights = specificationManager.GetRightsForSpecRelease(personRights, personId, version.Specification, version.Release.Pk_ReleaseId, releases);            
            
            return new KeyValuePair<SpecVersion, UserRightsContainer>(version, specRelease_Rights.Value);
        }

        /// <summary>
        /// Enable to Allocate a version or to to upload it from scratch
        /// </summary>
        /// <param name="version">The new version to allocate or upload</param>
        public void UploadOrAllocateVersion(SpecVersion version)
        {
            ISpecVersionsRepository repo = RepositoryFactory.Resolve<ISpecVersionsRepository>();
            repo.UoW = _uoW;
            repo.InsertOrUpdate(version);
        }

        /// <summary>
        /// Check if the version to insert is allowed
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool CheckIfVersionAlloawed(SpecVersion version, SpecVersion oldVersion, bool isUpload)
        {

            if (oldVersion == null)
                return false;
            if (isUpload)
            {
                // Check if any version pending for load
                List<SpecVersion> versions = GetVersionsForASpecRelease(version.Fk_SpecificationId.GetValueOrDefault(), version.Fk_ReleaseId.GetValueOrDefault()).Where(v => v.DocumentUploaded == null).ToList();
                if (versions != null && versions.Count > 0)
                {
                    foreach (SpecVersion v in versions)
                    {
                        if (v.MajorVersion == version.MajorVersion && v.TechnicalVersion == version.TechnicalVersion && v.EditorialVersion == version.EditorialVersion)
                        {
                            return true;
                        }
                    }
                }
            }
            //Check if version is valid 
            if(version.MajorVersion >= oldVersion.MajorVersion)
            {
                if (version.MajorVersion == oldVersion.MajorVersion)
                {
                    if (version.TechnicalVersion >= oldVersion.TechnicalVersion)
                    {
                        if (version.TechnicalVersion == oldVersion.TechnicalVersion)
                        {
                            if (version.EditorialVersion > oldVersion.EditorialVersion)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validate Uploaded version document & provide validation summary
        /// </summary>
        /// <param name="fileExtension">File Extension (.doc/.docx)</param>
        /// <param name="memoryStream">Memory Stream</param>
        /// <param name="version">Specification Version</param>
        /// <param name="title">Specification Title</param>
        /// <param name="release">Specification Release</param>
        /// <param name="meetingDate">Meeting Date</param>
        /// <returns>Validation Summary</returns>
        public Report ValidateVersionDocument(string fileExtension, MemoryStream memoryStream, string version, string title, string release, DateTime meetingDate)
        {
            Report validationReport = new Report();

            IQualityChecks qualityChecks;

            if (fileExtension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
                qualityChecks = new DocXQualityChecks(memoryStream);
            else
                qualityChecks = new DocQualityChecks();

            if (qualityChecks.HasTrackedRevisions())
                validationReport.LogWarning(CONST_QUALITY_CHECK_REVISIONMARK);

            if(!qualityChecks.IsHistoryVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_HISTORY);

            if (!qualityChecks.IsCoverPageVersionCorrect(version))
                validationReport.LogWarning(CONST_QUALITY_CHECK_VERSION_COVERPAGE);

            if (!qualityChecks.IsCoverPageDateCorrect(meetingDate))
                validationReport.LogWarning(CONST_QUALITY_CHECK_DATE_COVERPAGE);

            if (!qualityChecks.IsCopyRightYearCorrect())
                validationReport.LogWarning(CONST_QUALITY_CHECK_YEAR_COPYRIGHT);

            if (!qualityChecks.IsTitleCorrect(title, release))
                validationReport.LogWarning(CONST_QUALITY_CHECK_TITLE_COVERPAGE);

            return validationReport;
        }
    }
}
