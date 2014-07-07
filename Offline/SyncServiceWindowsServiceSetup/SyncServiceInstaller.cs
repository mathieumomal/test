using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

namespace SyncService
{
    [RunInstaller(true)]
    public class SyncServiceInstaller: Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public SyncServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "3GU Synchronization Service";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}