using System.ServiceProcess;

namespace SyncClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new OfflineSyncClient() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
