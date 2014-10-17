using DotNetNuke.Instrumentation;
using log4net.Config;
using System;
using System.IO;

namespace Etsi.UserRights.Service
{
    public static class LogManager
    {
        #region Properties

        private static string _configFilePath = String.Empty;
        private static string _loggerName = String.Empty;
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
            XmlConfigurator.ConfigureAndWatch(new FileInfo(String.IsNullOrEmpty(_configFilePath) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserRights.log4net.config") : _configFilePath));
            _userRightsLogger = LoggerSource.Instance.GetLogger(String.IsNullOrEmpty(_loggerName) ? "UserRightsLogger" : _loggerName);
        }

        #endregion

        #region Public Static Methods

        public static void SetConfiguration(string filePath, string loggerName)
        {
            _configFilePath = filePath;
            _loggerName = loggerName;
        }

        #endregion
    }
}
