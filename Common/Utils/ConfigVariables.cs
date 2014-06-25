﻿using System;
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

        public static string DefaultPublicTmpAddress
        {
            get
            {
                return "/tmp/";
            }
        }
    }
}
