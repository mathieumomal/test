using Etsi.Ultimate.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Etsi.Ultimate.Business
{
    public class TranspositionManager : ITranspositionManager
    {

        #region ITranspositionManager Members

        public static string CONST_WEBCONFIG_TRANSP_PATH = "TranspositionFolderPath";


        public bool Transpose(DomainClasses.Specification spec, DomainClasses.SpecVersion version)
        {

            if ((version != null) && (version.Location != null))
            {
                //Two steps to perform transposition
                string versionURL = version.Location;
                //STEP1: Transfer of the version to a dedicated folder
                bool result_Step1 =  transferVersionToDedicatedFolder(versionURL);
                //Missing step releated to WPMDB record   

                return result_Step1;
            }
            else
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


        #endregion
    }
}
