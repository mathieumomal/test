﻿using DotNetNuke.Instrumentation;
using log4net.Config;
using System;

namespace Etsi.Ultimate.Utils
{
    public class LogManager
    {
        #region Properties

        private static ILog _ultimateLogger;
        public static ILog UltimateLogger
        {
            get
            {
                if (_ultimateLogger == null)
                    SetDefaultLogger();
                return _ultimateLogger;
            }
        }

        #endregion

        #region Private Methods

        private static void SetDefaultLogger()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(ServerTopology.GetServerRootPath() + "./" + "ULTIMATE.log4net.config"));
            _ultimateLogger = LoggerSource.Instance.GetLogger("ULTIMATELogger");
        }

        #endregion

        #region Debug

        public static void Debug(object message, Exception exception)
        {
            UltimateLogger.Debug(message, exception);
        }

        public static void Debug(object message)
        {
            UltimateLogger.Debug(message);
        }

        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.DebugFormat(provider, format, args);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            UltimateLogger.DebugFormat(format, args);
        }

        #endregion

        #region Error

        public static void Error(object message, Exception exception)
        {
            UltimateLogger.Error(message, exception);
        }

        public static void Error(object message)
        {
            UltimateLogger.Error(message);
        }

        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.ErrorFormat(provider, format, args);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            UltimateLogger.ErrorFormat(format, args);
        }

        #endregion

        #region Fatal

        public static void Fatal(object message, Exception exception)
        {
            UltimateLogger.Fatal(message, exception);
        }

        public static void Fatal(object message)
        {
            UltimateLogger.Fatal(message);
        }

        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.FatalFormat(provider, format, args);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            UltimateLogger.FatalFormat(format, args);
        }

        #endregion

        #region Info

        public static void Info(object message, Exception exception)
        {
            UltimateLogger.Info(message, exception);
        }

        public static void Info(object message)
        {
            UltimateLogger.Info(message);
        }

        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.InfoFormat(provider, format, args);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            UltimateLogger.InfoFormat(format, args);
        }

        #endregion

        #region Trace

        public static void Trace(object message, Exception exception)
        {
            UltimateLogger.Trace(message, exception);
        }

        public static void Trace(object message)
        {
            UltimateLogger.Trace(message);
        }

        public static void TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.TraceFormat(provider, format, args);
        }

        public static void TraceFormat(string format, params object[] args)
        {
            UltimateLogger.TraceFormat(format, args);
        }

        #endregion

        #region Warn

        public static void Warn(object message, Exception exception)
        {
            UltimateLogger.Warn(message, exception);
        }

        public static void Warn(object message)
        {
            UltimateLogger.Warn(message);
        }

        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            UltimateLogger.WarnFormat(provider, format, args);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            UltimateLogger.WarnFormat(format, args);
        }

        #endregion
    }
}