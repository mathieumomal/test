using System;
using System.ServiceProcess;
using System.Threading;

namespace SyncClient
{
    static class Program
    {
        #region Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments</param>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (args != null && args.Length > 0)
                {
                    switch (args[0].Trim().ToLower())
                    {
                        case "-debug":
                            debugOfflineSync();
                            break;
                    }
                }
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new OfflineSyncClient() });
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Debug SyncClient Service
        /// </summary>
        private static void debugOfflineSync()
        {
            Console.WriteLine("Offline SyncClient Service running. Press Ctrl+C to exit...");
            OfflineSyncClient service = new OfflineSyncClient();
            service.DebugStart();
            Thread.Sleep(Timeout.Infinite);
            service.DebugEnd();
        }

        #endregion
    }
}
