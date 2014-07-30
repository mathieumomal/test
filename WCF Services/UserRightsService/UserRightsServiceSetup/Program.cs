using System.ServiceProcess;

namespace Etsi.UserRights.UserRightsServiceSetup
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
                new UserRightsServiceHost() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
