using System.ServiceProcess;

namespace Etsi.Ultimate.WCF.Setup
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
                new UltimateServiceHost() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
