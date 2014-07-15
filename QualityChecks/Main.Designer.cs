namespace QualityChecks
{
    partial class frmMain
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
            this.gbConfiguration = new System.Windows.Forms.GroupBox();
            this.tlpQualityChecks = new System.Windows.Forms.TableLayoutPanel();
            this.tlpConfiguration = new System.Windows.Forms.TableLayoutPanel();
            this.lblQCFolder = new System.Windows.Forms.Label();
            this.txtQCFolder = new System.Windows.Forms.TextBox();
            this.btnQCFolder = new System.Windows.Forms.Button();
            this.btnProcessQualityChecks = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.rtbQCResults = new System.Windows.Forms.RichTextBox();
            this.gbConfiguration.SuspendLayout();
            this.tlpQualityChecks.SuspendLayout();
            this.tlpConfiguration.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbConfiguration
            // 
            this.gbConfiguration.Controls.Add(this.tlpQualityChecks);
            this.gbConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbConfiguration.Location = new System.Drawing.Point(0, 0);
            this.gbConfiguration.Name = "gbConfiguration";
            this.gbConfiguration.Size = new System.Drawing.Size(876, 490);
            this.gbConfiguration.TabIndex = 0;
            this.gbConfiguration.TabStop = false;
            this.gbConfiguration.Text = "Configuration";
            // 
            // tlpQualityChecks
            // 
            this.tlpQualityChecks.ColumnCount = 1;
            this.tlpQualityChecks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQualityChecks.Controls.Add(this.tlpConfiguration, 0, 0);
            this.tlpQualityChecks.Controls.Add(this.rtbQCResults, 0, 1);
            this.tlpQualityChecks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpQualityChecks.Location = new System.Drawing.Point(3, 16);
            this.tlpQualityChecks.Name = "tlpQualityChecks";
            this.tlpQualityChecks.RowCount = 2;
            this.tlpQualityChecks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpQualityChecks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQualityChecks.Size = new System.Drawing.Size(870, 471);
            this.tlpQualityChecks.TabIndex = 0;
            // 
            // tlpConfiguration
            // 
            this.tlpConfiguration.ColumnCount = 3;
            this.tlpConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConfiguration.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpConfiguration.Controls.Add(this.lblQCFolder, 0, 0);
            this.tlpConfiguration.Controls.Add(this.txtQCFolder, 1, 0);
            this.tlpConfiguration.Controls.Add(this.btnQCFolder, 2, 0);
            this.tlpConfiguration.Controls.Add(this.btnProcessQualityChecks, 1, 1);
            this.tlpConfiguration.Controls.Add(this.btnExport, 2, 1);
            this.tlpConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpConfiguration.Location = new System.Drawing.Point(3, 3);
            this.tlpConfiguration.Name = "tlpConfiguration";
            this.tlpConfiguration.RowCount = 2;
            this.tlpConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpConfiguration.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpConfiguration.Size = new System.Drawing.Size(864, 74);
            this.tlpConfiguration.TabIndex = 0;
            // 
            // lblQCFolder
            // 
            this.lblQCFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQCFolder.AutoSize = true;
            this.lblQCFolder.Location = new System.Drawing.Point(3, 11);
            this.lblQCFolder.Name = "lblQCFolder";
            this.lblQCFolder.Size = new System.Drawing.Size(94, 13);
            this.lblQCFolder.TabIndex = 0;
            this.lblQCFolder.Text = "QC Folder";
            // 
            // txtQCFolder
            // 
            this.txtQCFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQCFolder.Location = new System.Drawing.Point(103, 7);
            this.txtQCFolder.Name = "txtQCFolder";
            this.txtQCFolder.Size = new System.Drawing.Size(658, 20);
            this.txtQCFolder.TabIndex = 1;
            // 
            // btnQCFolder
            // 
            this.btnQCFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnQCFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQCFolder.Location = new System.Drawing.Point(767, 6);
            this.btnQCFolder.Name = "btnQCFolder";
            this.btnQCFolder.Size = new System.Drawing.Size(39, 23);
            this.btnQCFolder.TabIndex = 2;
            this.btnQCFolder.Text = "...";
            this.btnQCFolder.UseVisualStyleBackColor = true;
            this.btnQCFolder.Click += new System.EventHandler(this.btnQCFolder_Click);
            // 
            // btnProcessQualityChecks
            // 
            this.btnProcessQualityChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnProcessQualityChecks.Location = new System.Drawing.Point(353, 38);
            this.btnProcessQualityChecks.Name = "btnProcessQualityChecks";
            this.btnProcessQualityChecks.Size = new System.Drawing.Size(158, 33);
            this.btnProcessQualityChecks.TabIndex = 3;
            this.btnProcessQualityChecks.Text = "Process Quality Checks";
            this.btnProcessQualityChecks.UseVisualStyleBackColor = true;
            this.btnProcessQualityChecks.Click += new System.EventHandler(this.btnProcessQualityChecks_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnExport.BackgroundImage = global::QualityChecks.Properties.Resources.save_24x24;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.Location = new System.Drawing.Point(797, 38);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(34, 33);
            this.btnExport.TabIndex = 4;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // rtbQCResults
            // 
            this.rtbQCResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbQCResults.Location = new System.Drawing.Point(3, 83);
            this.rtbQCResults.Name = "rtbQCResults";
            this.rtbQCResults.Size = new System.Drawing.Size(864, 385);
            this.rtbQCResults.TabIndex = 1;
            this.rtbQCResults.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 490);
            this.Controls.Add(this.gbConfiguration);
            this.Name = "frmMain";
            this.Text = "Quality Checks";
            this.gbConfiguration.ResumeLayout(false);
            this.tlpQualityChecks.ResumeLayout(false);
            this.tlpConfiguration.ResumeLayout(false);
            this.tlpConfiguration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConfiguration;
        private System.Windows.Forms.TableLayoutPanel tlpQualityChecks;
        private System.Windows.Forms.TableLayoutPanel tlpConfiguration;
        private System.Windows.Forms.Label lblQCFolder;
        private System.Windows.Forms.TextBox txtQCFolder;
        private System.Windows.Forms.Button btnQCFolder;
        private System.Windows.Forms.Button btnProcessQualityChecks;
        private System.Windows.Forms.RichTextBox rtbQCResults;
        private System.Windows.Forms.Button btnExport;
    }
}

