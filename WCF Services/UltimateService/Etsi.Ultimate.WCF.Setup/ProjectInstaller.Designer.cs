namespace Etsi.Ultimate.WCF.Setup
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
            this.ultimateServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ultimateServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ultimateServiceProcessInstaller
            // 
            this.ultimateServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ultimateServiceProcessInstaller.Password = null;
            this.ultimateServiceProcessInstaller.Username = null;
            // 
            // ultimateServiceInstaller
            // 
            this.ultimateServiceInstaller.Description = "Provide the information which is related to ultimate database";
            this.ultimateServiceInstaller.DisplayName = "3GU Ultimate Service";
            this.ultimateServiceInstaller.ServiceName = "3GU Ultimate Service";
            this.ultimateServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ultimateServiceProcessInstaller,
            this.ultimateServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ultimateServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ultimateServiceInstaller;
    }
}