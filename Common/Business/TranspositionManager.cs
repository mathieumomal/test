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


namespace Etsi.Ultimate.Business
{
    public class TranspositionManager : ITranspositionManager
    {
       

        #region ITranspositionManager Members

        public static string CONST_WEBCONFIG_TRANSP_PATH = "TranspositionFolderPath";

        public IUltimateUnitOfWork _uoW { get; set; }

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
                //Add record to WPMDB   
                WpmRecordCreator creator = new WpmRecordCreator(_uoW);
                int WKI_ID = creator.AddWpmRecords(version);
                //Add ETSI_WKI_ID field in version TABLE
                //TODO
                return result;
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
                
                using (WebClient client = new WebClient())
                {
                    //Download the file and copy it to the dedicated folder
                    client.DownloadFile(new Uri(versionURL), transpositionFolder + FileName);
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
            specMgr.UoW = _uoW;
            IReleaseManager releaseMgr = ManagerFactory.Resolve<IReleaseManager>();
            releaseMgr.UoW = _uoW;
            IEnum_ReleaseStatusRepository relStatusRepo = RepositoryFactory.Resolve<IEnum_ReleaseStatusRepository>();
            relStatusRepo.UoW = _uoW;

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

            var UCC = spec.IsUnderChangeControl ?? false;
            var isFrozen = release.Fk_ReleaseStatus.Equals(frozen.Enum_ReleaseStatusId);
            var specReleaseTranspoForced = specRelease.isTranpositionForced ?? false;
            var specIsForPublication = spec.IsForPublication ?? false;
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
