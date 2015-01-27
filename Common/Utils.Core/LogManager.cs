using System;
using System.IO;
using DotNetNuke.Instrumentation;
using log4net.Config;

namespace Etsi.Ultimate.Utils.Core
{
    /// <summary>
    /// Log Manager class to handle logging information
    /// </summary>
    public static class LogManager
    {
        #region Properties

        private static string CONFIG_FILE_PATH = String.Empty;
        private static string LOGGER_NAME = String.Empty;
        private static ILog _logger;

        /// <summary>
        /// ILogger
        /// </summary>
        public static ILog Logger
        {
            get
            {
                if (_logger == null)
                    ConfigureLogger();
                return _logger;
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Configure logger
        /// </summary>
        private static void ConfigureLogger()
        {
            var rootPath = "";
            if (String.IsNullOrEmpty(CONFIG_FILE_PATH))
            {
                if (String.IsNullOrEmpty(ServerTopology.GetServerRootPath()))
                {
                    rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "Ultimate.log4net.config");
                }
                else
                {
                    rootPath = ServerTopology.GetServerRootPath() + "./" + "Ultimate.log4net.config";
                }
            }
            else
            {
                rootPath = CONFIG_FILE_PATH;
            }

            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo( rootPath ));
            _logger = LoggerSource.Instance.GetLogger(String.IsNullOrEmpty(LOGGER_NAME) ? "UltimateLogger" : LOGGER_NAME);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Apply configuration for logger
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <param name="loggerName">Logger name</param>
        public static void SetConfiguration(string filePath, string loggerName)
        {
            CONFIG_FILE_PATH = filePath;
            LOGGER_NAME = loggerName;
        }

        #endregion

        #region Debug

        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Debug(object message, Exception exception)
        {
            Logger.Debug(message, exception);
        }

        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Debug(object message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// Log formatted debug message
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.DebugFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted debug message
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void DebugFormat(string format, params object[] args)
        {
            Logger.DebugFormat(format, args);
        }

        #endregion

        #region Error

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Error(object message, Exception exception)
        {
            Logger.Error(message, exception);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Error(object message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// Log formatted error message
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.ErrorFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted error message
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void ErrorFormat(string format, params object[] args)
        {
            Logger.ErrorFormat(format, args);
        }

        #endregion

        #region Fatal

        /// <summary>
        /// Log fatal error message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Fatal(object message, Exception exception)
        {
            Logger.Fatal(message, exception);
        }

        /// <summary>
        /// Log fatal error message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Fatal(object message)
        {
            Logger.Fatal(message);
        }

        /// <summary>
        /// Log formatted fatal error message
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.FatalFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted fatal error message
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void FatalFormat(string format, params object[] args)
        {
            Logger.FatalFormat(format, args);
        }

        #endregion

        #region Info

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Info(object message, Exception exception)
        {
            Logger.Info(message, exception);
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Info(object message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Log formatted information message
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.InfoFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted information message
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void InfoFormat(string format, params object[] args)
        {
            Logger.InfoFormat(format, args);
        }

        #endregion

        #region Trace

        /// <summary>
        /// Log trace message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Trace(object message, Exception exception)
        {
            Logger.Trace(message, exception);
        }

        /// <summary>
        /// Log trace message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Trace(object message)
        {
            Logger.Trace(message);
        }

        /// <summary>
        /// Log formatted trace message
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.TraceFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted trace message
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void TraceFormat(string format, params object[] args)
        {
            Logger.TraceFormat(format, args);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public static void Warn(object message, Exception exception)
        {
            Logger.Warn(message, exception);
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="message">Message</param>
        public static void Warn(object message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        /// Log formatted warning
        /// </summary>
        /// <param name="provider">Format provider</param>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Logger.WarnFormat(provider, format, args);
        }

        /// <summary>
        /// Log formatted warning
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Arguments</param>
        public static void WarnFormat(string format, params object[] args)
        {
            Logger.WarnFormat(format, args);
        }

        #endregion
    }
}
