using DotNetNuke.Instrumentation;
using log4net.Config;
using System;

namespace Etsi.UserRights.Service
{
    public class LogManager
    {
        #region Properties

        private static string CONFIG_FILE_PATH = String.Empty;
        private static string LOGGER_NAME = String.Empty;
        private static ILog _userRightsLogger;
        public static ILog UserRightsLogger
        {
            get
            {
                if (_userRightsLogger == null)
                    ConfigureLogger();
                return _userRightsLogger;
            }
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureLogger()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(String.IsNullOrEmpty(CONFIG_FILE_PATH) ? "UserRights.log4net.config" : CONFIG_FILE_PATH));
            _userRightsLogger = LoggerSource.Instance.GetLogger(String.IsNullOrEmpty(LOGGER_NAME) ? "UserRightsLogger" : LOGGER_NAME);
        }

        #endregion

        #region Public Static Methods

        public static void SetConfiguration(string filePath, string loggerName)
        {
            CONFIG_FILE_PATH = filePath;
            LOGGER_NAME = loggerName;
        }

        #endregion
    }
}
