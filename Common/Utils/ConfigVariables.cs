﻿using System.Web;
using System.Configuration;
using System.IO;

namespace Etsi.Ultimate.Utils
{

    /// <summary>
    /// This class is an entry point for the different variables that the environment might require.
    /// </summary>
    public static class ConfigVariables
    {
        /// <summary>
        /// Relative ftp path for versions to be transposed
        /// </summary>
        public static string PortalUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["PortalUrl"] != null)
                    return ConfigurationManager.AppSettings["PortalUrl"].ToString();
                return "";
            }
        }

        public static int LimitOfTdocsToSearchPerRequest
        {
            get
            {
                var defaultValue = 100;
                if (ConfigurationManager.AppSettings["LimitOfTdocsToSearchPerRequest"] == null)
                    return defaultValue;
                int returnValue;
                var success = int.TryParse(ConfigurationManager.AppSettings["LimitOfTdocsToSearchPerRequest"], out returnValue);
                return success ? returnValue : defaultValue;
            }
        }

        /// <summary>
        /// Relative ftp path for versions to be transposed
        /// </summary>
        public static string RelativeFtpPathForVersionsToBeTransposed
        {
            get
            {
                if (ConfigurationManager.AppSettings["RelativeFtpPathForVersionsToBeTransposed"] != null)
                    return ConfigurationManager.AppSettings["RelativeFtpPathForVersionsToBeTransposed"].ToString();
                return "";
            }
        }

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


        public static string FtpBaseAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["FtpBaseAddress"] != null)
                    return ConfigurationManager.AppSettings["FtpBaseAddress"].ToString();
                return "";
            }

        }
        public static string FtpBasePhysicalPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["FtpBasePhysicalPath"] != null)
                    return ConfigurationManager.AppSettings["FtpBasePhysicalPath"].ToString();
                return "";
            }

        }

        /// <summary>
        /// Gets the FTP base physical path for backup.
        /// </summary>
        public static string FtpBaseBackupPhysicalPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["FtpBaseBackupPhysicalPath"] != null)
                    return ConfigurationManager.AppSettings["FtpBaseBackupPhysicalPath"].ToString();
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

        public static string UploadVersionTemporaryPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["UploadVersionTemporaryPath"] != null)
                    return ConfigurationManager.AppSettings["UploadVersionTemporaryPath"].ToString();
                return "";
            }
        }

        public static string UploadVersionArchiveFolder
        {
            get
            {
                if (ConfigurationManager.AppSettings["UploadVersionArchiveFolder"] != null)
                    return ConfigurationManager.AppSettings["UploadVersionArchiveFolder"].ToString();
                return "";
            }
        }

        public static bool UploadVersionSystemShouldKeepUnZipFolder
        {
            get
            {
                bool val = true;
                bool.TryParse(ConfigurationManager.AppSettings["UploadVersionSystemShouldKeepUnZipFolder"], out val);
                return val;
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

        /// <summary>
        /// Global 3GPP security group TBId (SA3, 386)
        /// </summary>
        public static int Global3GPPSecurityGroupTbId
        {
            get
            {
                if (ConfigurationManager.AppSettings["Global3GPPSecurityGroupTbId"] != null)
                {
                    int returnValue = 0;
                    bool success = int.TryParse(ConfigurationManager.AppSettings["Global3GPPSecurityGroupTbId"], out returnValue);
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

        /// <summary>
        /// Gets the name of the portal (ETSI / Ultimate).
        /// </summary>
        public static string PortalName
        {
            get
            {
                if (ConfigurationManager.AppSettings["PortalName"] != null)
                    return ConfigurationManager.AppSettings["PortalName"].ToString();
                return "";
            }
        }

        public static string EtsiWorkitemsDeliveryFolder
        {
            get
            {
                if (ConfigurationManager.AppSettings["EtsiWorkitemsDeliveryFolder"] != null)
                    return ConfigurationManager.AppSettings["EtsiWorkitemsDeliveryFolder"];
                return "";
            }
        }

        public static string SpecificationDetailsUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["SpecificationDetailsUrl"] != null)
                    return ConfigurationManager.AppSettings["SpecificationDetailsUrl"];
                return "";
            }
        }

        public static string ReleaseDetailsUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["ReleaseDetailsUrl"] != null)
                    return ConfigurationManager.AppSettings["ReleaseDetailsUrl"];
                return "";
            }
        }

        public static string TdocDetailsUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["TdocDetailsUrl"] != null)
                    return ConfigurationManager.AppSettings["TdocDetailsUrl"];
                return "";
            }
        }

        public static int CRsListRecordsMaxSize
        {
            get
            {
                if (ConfigurationManager.AppSettings["CRsListRecordsMaxSize"] == null)
                    return 0;
                var returnValue = 0;
                var success = int.TryParse(ConfigurationManager.AppSettings["CRsListRecordsMaxSize"], out returnValue);
                return success ? returnValue : 0;
            }
        }

        public static string RelativeUrlWiRelatedSpecs
        {
            get
            {
                if (ConfigurationManager.AppSettings["RelativeUrlWiRelatedSpecs"] != null)
                    return ConfigurationManager.AppSettings["RelativeUrlWiRelatedSpecs"];
                return "";
            }
        }

        public static string RelativeUrlWiRelatedCrs
        {
            get
            {
                if (ConfigurationManager.AppSettings["RelativeUrlWiRelatedCrs"] != null)
                    return ConfigurationManager.AppSettings["RelativeUrlWiRelatedCrs"];
                return "";
            }
        }

        public static string RelativeUrlSpecRelatedCrs
        {
            get
            {
                if (ConfigurationManager.AppSettings["RelativeUrlSpecRelatedCrs"] != null)
                    return ConfigurationManager.AppSettings["RelativeUrlSpecRelatedCrs"];
                return "";
            }
        }

        /// <summary>
        /// Gets the relative URL version related CRS.
        /// </summary>
        public static string RelativeUrlVersionRelatedCrs
        {
            get
            {
                if (ConfigurationManager.AppSettings["RelativeUrlVersionRelatedCrs"] != null)
                    return ConfigurationManager.AppSettings["RelativeUrlVersionRelatedCrs"];
                return "";
            }
        }

        /// <summary>
        /// Gets the relative URL for Spec WithdrawMeetingSelectPopUp.
        /// </summary>
        public static string SpecificationWithdrawMeetingSelectPopUpRelativeLink
        {
            get
            {
                if (ConfigurationManager.AppSettings["Specification_WithdrawMeetingSelectPopUp_RelativeLink"] != null)
                    return ConfigurationManager.AppSettings["Specification_WithdrawMeetingSelectPopUp_RelativeLink"];
                return "";
            }
        }

        /// <summary>
        /// Gets the relative URL for Spec UploadVersion popup.
        /// </summary>
        public static string SpecificationUploadVersionRelativeLink
        {
            get
            {
                if (ConfigurationManager.AppSettings["Specification_UploadVersion_RelativeLink"] != null)
                    return ConfigurationManager.AppSettings["Specification_UploadVersion_RelativeLink"];
                return "";
            }
        }

        /// <summary>
        /// Gets the relative URL for Spec RemoveSpecReleasePopUp.
        /// </summary>
        public static string SpecificationRemoveSpecReleasePopUpRelativeLink
        {
            get
            {
                if (ConfigurationManager.AppSettings["Specification_RemoveSpecReleasePopUp_RelativeLink"] != null)
                    return ConfigurationManager.AppSettings["Specification_RemoveSpecReleasePopUp_RelativeLink"];
                return "";
            }
        }

        /// <summary>
        /// System should export Workplan after import as Excel and Docx (or not)
        /// </summary>
        public static bool ActivateWorkPlanExportAfterImport
        {
            get
            {
                bool activateWorkPlanExportAfterImport;
                bool.TryParse(ConfigurationManager.AppSettings["ActivateWorkPlanExportAfterImport"],
                    out activateWorkPlanExportAfterImport);
                return activateWorkPlanExportAfterImport;
            }
        }

        #region Cookies to exclude from security checks
        /// <summary>
        /// Tdocs list cookie name
        /// </summary>
        public static string CookieNameCrsList
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CookieName_CrsList"]))
                    return ConfigurationManager.AppSettings["CookieName_CrsList"];
                return "";
            }
        }

        /// <summary>
        /// Tdocs list cookie name
        /// </summary>
        public static string CookieNameWisList
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CookieName_WisList"]))
                    return ConfigurationManager.AppSettings["CookieName_WisList"];
                return "";
            }
        }

        /// <summary>
        /// Tdocs list cookie name
        /// </summary>
        public static string CookieNameSpecsList
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CookieName_SpecsList"]))
                    return ConfigurationManager.AppSettings["CookieName_SpecsList"];
                return "";
            }
        }
        #endregion
    }
}
