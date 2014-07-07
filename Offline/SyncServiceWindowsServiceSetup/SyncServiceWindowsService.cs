using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Web;

namespace SyncService
{
    public class SyncServiceWindowsService: ServiceBase
    {
        public ServiceHost serviceHost = null;
        public SyncServiceWindowsService()
        {
            // Name the Windows Service
            ServiceName = "3GU Synchronization Service";
        }

        public static void Main()
        {
            ServiceBase.Run(new SyncServiceWindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(SyncService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }

    }
}