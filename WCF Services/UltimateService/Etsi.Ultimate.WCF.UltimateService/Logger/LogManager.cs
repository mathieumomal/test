using DotNetNuke.Instrumentation;
using log4net.Config;
using System;
using System.IO;

namespace Etsi.UserRights.Service
{
    /// <summary>
    /// Log Manager class to log errors / warnings
    /// </summary>
    public class LogManager
    {
        #region Properties

        private static string CONFIG_FILE_PATH = String.Empty;
        private static string LOGGER_NAME = String.Empty;
        private static ILog _ultimateServiceLogger;

        /// <summary>
        /// Ultimate Service Logger
        /// </summary>
        public static ILog UltimateServiceLogger
        {
            get
            {
                if (_ultimateServiceLogger == null)
                    ConfigureLogger();
                return _ultimateServiceLogger;
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Configures the logger.
        /// </summary>
        private static void ConfigureLogger()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(String.IsNullOrEmpty(CONFIG_FILE_PATH) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logger\\UltimateService.log4net.config") : CONFIG_FILE_PATH));
            _ultimateServiceLogger = LoggerSource.Instance.GetLogger(String.IsNullOrEmpty(LOGGER_NAME) ? "UltimateServiceLogger" : LOGGER_NAME);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="loggerName">Name of the logger.</param>
        public static void SetConfiguration(string filePath, string loggerName)
        {
            CONFIG_FILE_PATH = filePath;
            LOGGER_NAME = loggerName;
        }

        #endregion
    }
}
