namespace Etsi.UserRights.UserRightsServiceSetup
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.userRightsServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.userRightsServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // userRightsServiceProcessInstaller
            // 
            this.userRightsServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.userRightsServiceProcessInstaller.Password = null;
            this.userRightsServiceProcessInstaller.Username = null;
            // 
            // userRightsServiceInstaller
            // 
            this.userRightsServiceInstaller.Description = "Provide user rights for all ETSI portals";
            this.userRightsServiceInstaller.DisplayName = "ETSI User Rights Service";
            this.userRightsServiceInstaller.ServiceName = "ETSI User Rights Service";
            this.userRightsServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.userRightsServiceProcessInstaller,
            this.userRightsServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller userRightsServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller userRightsServiceInstaller;
    }
}