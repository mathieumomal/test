namespace OfflineSetup
{
    partial class Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbOfflineSetup = new System.Windows.Forms.GroupBox();
            this.btnOfflineSetup = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            this.lblDatabaseName = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblServerName = new System.Windows.Forms.Label();
            this.gbOfflineSetup.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbOfflineSetup
            // 
            this.gbOfflineSetup.Controls.Add(this.btnOfflineSetup);
            this.gbOfflineSetup.Controls.Add(this.btnConnect);
            this.gbOfflineSetup.Controls.Add(this.cbIntegratedSecurity);
            this.gbOfflineSetup.Controls.Add(this.txtPassword);
            this.gbOfflineSetup.Controls.Add(this.lblPassword);
            this.gbOfflineSetup.Controls.Add(this.txtUserName);
            this.gbOfflineSetup.Controls.Add(this.lblUserName);
            this.gbOfflineSetup.Controls.Add(this.txtDatabaseName);
            this.gbOfflineSetup.Controls.Add(this.lblDatabaseName);
            this.gbOfflineSetup.Controls.Add(this.txtServerName);
            this.gbOfflineSetup.Controls.Add(this.lblServerName);
            this.gbOfflineSetup.Location = new System.Drawing.Point(13, 12);
            this.gbOfflineSetup.Name = "gbOfflineSetup";
            this.gbOfflineSetup.Size = new System.Drawing.Size(277, 218);
            this.gbOfflineSetup.TabIndex = 0;
            this.gbOfflineSetup.TabStop = false;
            this.gbOfflineSetup.Text = "Offline Setup";
            // 
            // btnOfflineSetup
            // 
            this.btnOfflineSetup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOfflineSetup.ForeColor = System.Drawing.Color.Green;
            this.btnOfflineSetup.Location = new System.Drawing.Point(16, 188);
            this.btnOfflineSetup.Name = "btnOfflineSetup";
            this.btnOfflineSetup.Size = new System.Drawing.Size(248, 23);
            this.btnOfflineSetup.TabIndex = 10;
            this.btnOfflineSetup.Text = "Process Offline Setup";
            this.btnOfflineSetup.UseVisualStyleBackColor = true;
            this.btnOfflineSetup.Click += new System.EventHandler(this.btnOfflineSetup_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(189, 157);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbIntegratedSecurity
            // 
            this.cbIntegratedSecurity.AutoSize = true;
            this.cbIntegratedSecurity.Location = new System.Drawing.Point(103, 132);
            this.cbIntegratedSecurity.Name = "cbIntegratedSecurity";
            this.cbIntegratedSecurity.Size = new System.Drawing.Size(115, 17);
            this.cbIntegratedSecurity.TabIndex = 8;
            this.cbIntegratedSecurity.Text = "Integrated Security";
            this.cbIntegratedSecurity.UseVisualStyleBackColor = true;
            this.cbIntegratedSecurity.CheckedChanged += new System.EventHandler(this.cbIntegratedSecurity_CheckedChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(103, 104);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(161, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(13, 105);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(103, 76);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(161, 20);
            this.txtUserName.TabIndex = 5;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(13, 78);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(60, 13);
            this.lblUserName.TabIndex = 4;
            this.lblUserName.Text = "User Name";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Location = new System.Drawing.Point(103, 48);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(161, 20);
            this.txtDatabaseName.TabIndex = 3;
            // 
            // lblDatabaseName
            // 
            this.lblDatabaseName.AutoSize = true;
            this.lblDatabaseName.Location = new System.Drawing.Point(13, 51);
            this.lblDatabaseName.Name = "lblDatabaseName";
            this.lblDatabaseName.Size = new System.Drawing.Size(84, 13);
            this.lblDatabaseName.TabIndex = 2;
            this.lblDatabaseName.Text = "Database Name";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(103, 20);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(161, 20);
            this.txtServerName.TabIndex = 1;
            // 
            // lblServerName
            // 
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(13, 24);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(69, 13);
            this.lblServerName.TabIndex = 0;
            this.lblServerName.Text = "Server Name";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 244);
            this.Controls.Add(this.gbOfflineSetup);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ultimate Offline Setup";
            this.Load += new System.EventHandler(this.Main_Load);
            this.gbOfflineSetup.ResumeLayout(false);
            this.gbOfflineSetup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOfflineSetup;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.Label lblDatabaseName;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.Button btnOfflineSetup;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox cbIntegratedSecurity;
    }
}

