namespace SyncClient
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
            this.syncClientProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.syncClientInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // syncClientProcessInstaller
            // 
            this.syncClientProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.syncClientProcessInstaller.Password = null;
            this.syncClientProcessInstaller.Username = null;
            // 
            // syncClientInstaller
            // 
            this.syncClientInstaller.Description = "3GPP Ultimate - Synchronization client to send offline changes to online server";
            this.syncClientInstaller.DisplayName = "SyncClient";
            this.syncClientInstaller.ServiceName = "SyncClient";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.syncClientProcessInstaller,
            this.syncClientInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller syncClientProcessInstaller;
        private System.ServiceProcess.ServiceInstaller syncClientInstaller;
    }
}