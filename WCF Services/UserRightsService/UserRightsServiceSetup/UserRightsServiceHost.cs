using Etsi.UserRights.Service;
using System;
using System.ServiceModel;
using System.ServiceProcess;

namespace Etsi.UserRights.UserRightsServiceSetup
{
    public partial class UserRightsServiceHost : ServiceBase
    {
        #region Variables

        ServiceHost host; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize required components to use user rights service
        /// </summary>
        public UserRightsServiceHost()
        {
            InitializeComponent();
        } 

        #endregion

        #region Overridable Methods

        /// <summary>
        /// Open host to receive requests for user rights
        /// </summary>
        /// <param name="args">Event arguments</param>
        protected override void OnStart(string[] args)
        {
            Type serviceType = typeof(UserRightsService);
            host = new ServiceHost(serviceType);
            host.Open();
        }

        /// <summary>
        /// Close host to stop receiving requests for user rights
        /// </summary>
        protected override void OnStop()
        {
            if (host != null)
                host.Close();
        } 

        #endregion
    }
}
