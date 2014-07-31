using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using System.IO;


namespace Etsi.Ultimate.Business
{
    public class TranspositionManager : ITranspositionManager
    {
        #region ITranspositionManager Members

        public IUltimateUnitOfWork UoW { get; set; }

        public TranspositionManager()
        {
        }


        public bool Transpose(DomainClasses.Specification spec, DomainClasses.SpecVersion version)
        {
            bool importResult = false;
            bool transferResult = false; 

            if (this.TransposeAllowed(version))
            {
                if ((version != null) && (version.Location != null))
                {
                    //Two steps to perform transposition
                    string versionURL = version.Location;
                    //STEP1: Transfer of the version to a dedicated folder
                    transferResult = transferVersionToDedicatedFolder(versionURL);
                    if (transferResult)
                    {
                        //STEP2: Add record to WPMDB   
                        WpmRecordCreator creator = new WpmRecordCreator(UoW);
                        importResult = creator.AddWpmRecords(version);
                        version.DocumentPassedToPub = DateTime.Now;
                    }                    
                    return (transferResult && importResult);
                }
                else
                    return false; 
            }
            return false;
        }

        /// <summary>
        /// Transfert of version File to dedicated folder
        /// </summary>
        /// <param name="versionURL"></param>
        /// <returns></returns>
        private bool transferVersionToDedicatedFolder(string versionURL)
        {
            try
            {
                
                //Get the target folder path
                string transpositionFolder = ConfigVariables.TranspositionFolderPath;
                string filePath;                
                string fileName; 
                if (versionURL.Trim().StartsWith(ConfigVariables.FtpBaseAddress))
                {
                    filePath = versionURL.Replace(ConfigVariables.FtpBaseAddress, ConfigVariables.FtpBasePhysicalPath).Replace("/", "\\");
                    fileName = versionURL.Split('/').LastOrDefault();
                }
                else
                {
                    filePath = versionURL;
                    fileName = versionURL.Split('\\').LastOrDefault();
                }

                if (File.Exists(filePath) && Directory.Exists(transpositionFolder)) 
                {
                    File.Copy(filePath, transpositionFolder + fileName, true);
                }
                return true;
            }
            catch (Exception e)
            {
                Utils.LogManager.Error("ForceTransposition error: " + e.Message);
                return false;
            }
        }

        public bool TransposeAllowed(SpecVersion specVersion)
        {
            ISpecificationManager specMgr = ManagerFactory.Resolve<ISpecificationManager>();
            specMgr.UoW = UoW;
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = UoW;
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = UoW;

            Specification_Release specRelease = null;
            Specification spec = null;
            Release release = null;

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

            specRelease = specMgr.GetSpecReleaseBySpecIdAndReleaseId(spec.Pk_SpecificationId, release.Pk_ReleaseId);
            if (specRelease == null)
                return false;
            var frozen = relStatusRepo.All.Where(x => x.Code == Enum_ReleaseStatus.Frozen).FirstOrDefault();
            if (frozen == null)
                throw new InvalidOperationException("Error for get the frozen status.");

            //Define withdrawn criterias
            var specIsActive = spec.IsActive;//No withdrawn
            var specReleaseIsWithdrawn = specRelease.isWithdrawn.GetValueOrDefault();
            //Define others criterias
            var UCC = spec.IsUnderChangeControl.GetValueOrDefault();
            var isFrozen = release.Fk_ReleaseStatus.Equals(frozen.Enum_ReleaseStatusId);
            var specReleaseTranspoForced = specRelease.isTranpositionForced.GetValueOrDefault();
            var specIsForPublication = spec.IsForPublication.GetValueOrDefault();

            //No transposition if :
            //- the spec isn't UCC
            //- the spec isn't active (withdrawn)
            //- the spec_release is withdrawn
            if (!UCC || !specIsActive || specReleaseIsWithdrawn)
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
