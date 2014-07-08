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

        public static string CONST_WEBCONFIG_TRANSP_PATH = "TranspositionFolderPath";

        public IUltimateUnitOfWork UoW { get; set; }

        public TranspositionManager()
        {
        }


        public bool Transpose(DomainClasses.Specification spec, DomainClasses.SpecVersion version)
        {
            if (this.TransposeAllowed(version))
            {
                if ((version != null) && (version.Location != null))
                {
                    //Two steps to perform transposition
                    string versionURL = version.Location;
                    //STEP1: Transfer of the version to a dedicated folder
                    bool result =  transferVersionToDedicatedFolder(versionURL);
                    //STEP2: Add record to WPMDB   
                    WpmRecordCreator creator = new WpmRecordCreator(UoW);
                    int WKI_ID = creator.AddWpmRecords(version);
                    //STEP3: Add ETSI_WKI_ID field in version TABLE IF(WKI_ID != -1)
                    //TODO
                    return (result && (WKI_ID != -1));
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
                //Get file name from the location
                string FileName = versionURL.Split('/').LastOrDefault();
                //Get the target folder path
                string transpositionFolder = ConfigVariables.TranspositionFolderPath;
                string filePath = versionURL.Replace(ConfigVariables.FtpBaseAddress, ConfigVariables.FtpBasePhysicalPath).Replace("/", "\\");
                                
                if (Directory.Exists(transpositionFolder)) 
                {
                    File.Copy(filePath, transpositionFolder + FileName);
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

            var UCC = spec.IsUnderChangeControl.GetValueOrDefault();
            var isFrozen = release.Fk_ReleaseStatus.Equals(frozen.Enum_ReleaseStatusId);
            var specReleaseTranspoForced = specRelease.isTranpositionForced.GetValueOrDefault();
            var specIsForPublication = spec.IsForPublication.GetValueOrDefault();
            if (!UCC)
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
