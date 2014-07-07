using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace Etsi.Ultimate.Utils
{

    /// <summary>
    /// This class is an entry point for the different variables that the environment might require.
    /// </summary>
    public static class ConfigVariables
    {
        /// <summary>
        /// Gets the Meeting Details URL
        /// </summary>
        public static string MeetingDetailsAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["MeetingDetailsAddress"] != null)
                    return ConfigurationManager.AppSettings["MeetingDetailsAddress"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Gets the Rapporteur base address
        /// </summary>
        public static string RapporteurDetailsAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["RapporteurDetailsAddress"] != null)
                    return ConfigurationManager.AppSettings["RapporteurDetailsAddress"].ToString();
                return "";
            }
        }

        public static string FtpBasePath
        {
            get
            {
                if (ConfigurationManager.AppSettings["FtpVersionsPhysicalPath"] != null)
                    return ConfigurationManager.AppSettings["FtpVersionsPhysicalPath"].ToString();
                return "";
            }

        }

        /// <summary>
        /// FTP Folder name to store latest versions
        /// </summary>
        public static string VersionsLatestFTPFolder
        {
            get
            {
                if (ConfigurationManager.AppSettings["VersionsLatestFTPFolder"] != null)
                    return ConfigurationManager.AppSettings["VersionsLatestFTPFolder"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Gets FTP path for downloading export zips
        /// </summary>
        public static string FtpExportAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["FtpExportAddress"] != null)
                    return ConfigurationManager.AppSettings["FtpExportAddress"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Default "From" address for the email
        /// </summary>
        public static string EmailDefaultFrom
        {
            get
            {
                if (ConfigurationManager.AppSettings["EmailDefaultFrom"] != null)
                    return ConfigurationManager.AppSettings["EmailDefaultFrom"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Default Bcc address for the email
        /// </summary>
        public static string EmailDefaultBcc
        {
            get
            {
                if (ConfigurationManager.AppSettings["EmailDefaultBcc"] != null)
                    return ConfigurationManager.AppSettings["EmailDefaultBcc"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Temporary folder where files can be stored
        /// </summary>
        public static string DefaultPublicTmpPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["DefaultTmpPath"] != null)
                    return ConfigurationManager.AppSettings["DefaultTmpPath"].ToString();
                
                return HttpContext.Current.Request.MapPath("~/tmp/");
            }
        }

        /// <summary>
        /// TranspositionFolder for ETSI Secretariat
        /// </summary>
        public static string TranspositionFolderPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["TranspositionFolderPath"] != null)
                    return ConfigurationManager.AppSettings["TranspositionFolderPath"].ToString();
                return "";
            }
        }

        /// <summary>
        /// WorkItem Report path
        /// </summary>
        public static string WIReportPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["WIReportPath"] != null)
                    return ConfigurationManager.AppSettings["WIReportPath"].ToString();
                return "";
            }
        }

        /// <summary>
        /// Global 3GPP project id
        /// </summary>
        public static int Global3GPPProjetId
        {
            get
            {
                if (ConfigurationManager.AppSettings["Global3GPPProjetId"] != null)
                {
                    int returnValue = 0;
                    bool success = int.TryParse(ConfigurationManager.AppSettings["Global3GPPProjetId"].ToString(), out returnValue);
                    if (success)
                        return returnValue;
                    else
                        return 0;
                }
                return 0;
            }
        }

        public static string DefaultPublicTmpAddress
        {
            get
            {
                return "/tmp/";
            }
        }
    }
}
