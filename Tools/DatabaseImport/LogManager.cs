using DotNetNuke.Instrumentation;
using log4net.Config;
using System;
using System.IO;

namespace DatabaseImport
{
    public static class LogManager
    {
        #region Properties

        private static string _configFilePath = String.Empty;
        private static string _loggerName = String.Empty;
        private static ILog _dbImportLogger;
        public static ILog DbImportLogger
        {
            get
            {
                if (_dbImportLogger == null)
                    ConfigureLogger();
                return _dbImportLogger;
            }
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureLogger()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(String.IsNullOrEmpty(_configFilePath) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbImport.log4net.config") : _configFilePath));
            _dbImportLogger = LoggerSource.Instance.GetLogger(String.IsNullOrEmpty(_loggerName) ? "DbImportLogger" : _loggerName);
        }

        #endregion

        #region Public Static Methods

        public static void SetConfiguration(string filePath, string loggerName)
        {
            _configFilePath = filePath;
            _loggerName = loggerName;
        }

        #endregion


        public static void LogWarning(string message)
        {
            DbImportLogger.Warn(message);
        }

        public static void LogWarning(string message,string module)
        {
            DbImportLogger.Warn("["+module+"] "+message);
        }
    }
}
