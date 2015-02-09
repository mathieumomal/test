using System;
using System.IO;
using System.Linq;
using Etsi.Ultimate.Business.Specifications.Interfaces;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Business.Versions
{
    public class TranspositionManager : ITranspositionManager
    {
        #region ITranspositionManager Members

        public IUltimateUnitOfWork UoW { get; set; }

        public bool Transpose(Specification spec, SpecVersion version)
        {
            var importResult = false;

            if (TransposeAllowed(version))
            {
                if ((version != null) && (version.Location != null))
                {
                    //Two steps to perform transposition
                    var versionUrl = version.Location;
                    //STEP1: Transfer of the version to a dedicated folder
                    var transferResult = transferVersionToDedicatedFolder(versionUrl);
                    if (transferResult)
                    {
                        //STEP2: Add record to WPMDB   
                        var creator = new WpmRecordCreator(UoW);
                        importResult = creator.AddWpmRecords(version);
                        version.DocumentPassedToPub = DateTime.Now;
                    }                    
                    return (transferResult && importResult);
                }
            }
            return false;
        }

        /// <summary>
        /// Transfert of version File to dedicated folder
        /// </summary>
        /// <param name="versionUrl"></param>
        /// <returns></returns>
        private bool transferVersionToDedicatedFolder(string versionUrl)
        {
            try
            {
                
                //Get the target folder path
                string transpositionFolder = ConfigVariables.TranspositionFolderPath;
                string filePath;                
                string fileName; 
                if (versionUrl.Trim().StartsWith(ConfigVariables.FtpBaseAddress))
                {
                    filePath = versionUrl.Replace(ConfigVariables.FtpBaseAddress, ConfigVariables.FtpBasePhysicalPath).Replace("/", "\\");
                    fileName = versionUrl.Split('/').LastOrDefault();
                }
                else
                {
                    filePath = versionUrl;
                    fileName = versionUrl.Split('\\').LastOrDefault();
                }

                if (File.Exists(filePath) && Directory.Exists(transpositionFolder)) 
                {
                    File.Copy(filePath, transpositionFolder + fileName, true);
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.Error("ForceTransposition error: " + e.Message);
                return false;
            }
        }

        public bool TransposeAllowed(SpecVersion specVersion)
        {
            var specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            var releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            var relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;

            Specification spec;
            Release release;

            //Check that we have all informations to transpose the version
            if (specVersion == null)
                return false;
            
            // Do not transpose an already transposed version
            if (specVersion.ETSI_WKI_ID.GetValueOrDefault() != 0)
                return false;

            if (specVersion.Specification == null){
                spec = specMgr.GetSpecificationById(0, specVersion.Fk_SpecificationId ?? 0).Key;
                if (spec == null)
                    return false;
            }
            else
                spec = specVersion.Specification;

            if (specVersion.Release == null){
                release = releaseMgr.GetReleaseById(0, specVersion.Fk_ReleaseId ?? 0).Key;
                if (release == null)
                    return false;
            }
            else
                release = specVersion.Release;

            Specification_Release specRelease = specMgr.GetSpecReleaseBySpecIdAndReleaseId(spec.Pk_SpecificationId, release.Pk_ReleaseId);
            if (specRelease == null)
                return false;
            var frozen = relStatusRepo.All.FirstOrDefault(x => x.Code == Enum_ReleaseStatus.Frozen);
            if (frozen == null)
                throw new InvalidOperationException("Error for get the frozen status.");

            //Define withdrawn criterias
            var specIsActive = spec.IsActive;//No withdrawn
            var specReleaseIsWithdrawn = specRelease.isWithdrawn.GetValueOrDefault();
            //Define others criterias
            var ucc = spec.IsUnderChangeControl.GetValueOrDefault();
            var isFrozen = release.Fk_ReleaseStatus.Equals(frozen.Enum_ReleaseStatusId);
            var specReleaseTranspoForced = specRelease.isTranpositionForced.GetValueOrDefault();
            var specIsForPublication = spec.IsForPublication.GetValueOrDefault();

            //No transposition if :
            //- the spec isn't UCC
            //- the spec isn't active (withdrawn)
            //- the spec_release is withdrawn
            if (!ucc || !specIsActive || specReleaseIsWithdrawn)
                return false;
            if (specReleaseTranspoForced)
                return true;
            if (specIsForPublication && isFrozen)
                return true;

            return false;
        }


        #endregion
    }
}
