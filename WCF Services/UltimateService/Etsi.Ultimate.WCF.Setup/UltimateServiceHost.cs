using Etsi.Ultimate.WCF.Service;
using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace Etsi.Ultimate.WCF.Setup
{
    /// <summary>
    /// Windows service to host ultimate wcf service
    /// </summary>
    public partial class UltimateServiceHost : ServiceBase
    {
        #region Variables

        ServiceHost host;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UltimateServiceHost class.
        /// </summary>
        public UltimateServiceHost()
        {
            InitializeComponent();
        } 

        #endregion

        #region Overridable Methods

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            Type serviceType = typeof(UltimateService);
            host = new ServiceHost(serviceType);
            host.Open();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            if (host != null)
                host.Close();
        } 

        #endregion
    }
}
