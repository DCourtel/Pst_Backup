namespace SmartSingularity.PstBackupSettings
{
    partial class FrmSettings
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFilesAndFolders = new System.Windows.Forms.TabPage();
            this.chkBxCompressFile = new System.Windows.Forms.CheckBox();
            this.chkLstBxPstFiles = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpBxBackupDestination = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.nupBackupServerPort = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rdBtnBackupServer = new System.Windows.Forms.RadioButton();
            this.rdBtnFileSystem = new System.Windows.Forms.RadioButton();
            this.txtBxBackupServerName = new System.Windows.Forms.TextBox();
            this.txtBxDestination = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grpBxSchedule = new System.Windows.Forms.GroupBox();
            this.cmbBxWeekly = new System.Windows.Forms.ComboBox();
            this.cmbBxEvery = new System.Windows.Forms.ComboBox();
            this.nupMonthly = new System.Windows.Forms.NumericUpDown();
            this.nupEvery = new System.Windows.Forms.NumericUpDown();
            this.rdBtnMonthly = new System.Windows.Forms.RadioButton();
            this.rdBtnWeekly = new System.Windows.Forms.RadioButton();
            this.rdBtnEvery = new System.Windows.Forms.RadioButton();
            this.tabEventLog = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBxEventLogSeverity = new System.Windows.Forms.ComboBox();
            this.chkBxEventLog = new System.Windows.Forms.CheckBox();
            this.tabReporting = new System.Windows.Forms.TabPage();
            this.nupReportingServerPort = new System.Windows.Forms.NumericUpDown();
            this.txtBxReportingServerName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkBxReporting = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabBackupAgent = new System.Windows.Forms.TabPage();
            this.grpBxBackupMethod = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rdBtnMethodDifferential = new System.Windows.Forms.RadioButton();
            this.rdBtnMethodFull = new System.Windows.Forms.RadioButton();
            this.txtBxAdditionalSubnets = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkBxDontBackupOverWan = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabFilesAndFolders.SuspendLayout();
            this.grpBxBackupDestination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupBackupServerPort)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.grpBxSchedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupMonthly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupEvery)).BeginInit();
            this.tabEventLog.SuspendLayout();
            this.tabReporting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupReportingServerPort)).BeginInit();
            this.tabBackupAgent.SuspendLayout();
            this.grpBxBackupMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabFilesAndFolders);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabEventLog);
            this.tabControl1.Controls.Add(this.tabReporting);
            this.tabControl1.Controls.Add(this.tabBackupAgent);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabFilesAndFolders
            // 
            this.tabFilesAndFolders.Controls.Add(this.chkBxCompressFile);
            this.tabFilesAndFolders.Controls.Add(this.chkLstBxPstFiles);
            this.tabFilesAndFolders.Controls.Add(this.label1);
            this.tabFilesAndFolders.Controls.Add(this.grpBxBackupDestination);
            resources.ApplyResources(this.tabFilesAndFolders, "tabFilesAndFolders");
            this.tabFilesAndFolders.Name = "tabFilesAndFolders";
            this.tabFilesAndFolders.UseVisualStyleBackColor = true;
            // 
            // chkBxCompressFile
            // 
            resources.ApplyResources(this.chkBxCompressFile, "chkBxCompressFile");
            this.chkBxCompressFile.Name = "chkBxCompressFile";
            this.chkBxCompressFile.UseVisualStyleBackColor = true;
            // 
            // chkLstBxPstFiles
            // 
            resources.ApplyResources(this.chkLstBxPstFiles, "chkLstBxPstFiles");
            this.chkLstBxPstFiles.CheckOnClick = true;
            this.chkLstBxPstFiles.FormattingEnabled = true;
            this.chkLstBxPstFiles.Name = "chkLstBxPstFiles";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // grpBxBackupDestination
            // 
            resources.ApplyResources(this.grpBxBackupDestination, "grpBxBackupDestination");
            this.grpBxBackupDestination.Controls.Add(this.label9);
            this.grpBxBackupDestination.Controls.Add(this.nupBackupServerPort);
            this.grpBxBackupDestination.Controls.Add(this.label8);
            this.grpBxBackupDestination.Controls.Add(this.label2);
            this.grpBxBackupDestination.Controls.Add(this.rdBtnBackupServer);
            this.grpBxBackupDestination.Controls.Add(this.rdBtnFileSystem);
            this.grpBxBackupDestination.Controls.Add(this.txtBxBackupServerName);
            this.grpBxBackupDestination.Controls.Add(this.txtBxDestination);
            this.grpBxBackupDestination.Controls.Add(this.btnBrowse);
            this.grpBxBackupDestination.Name = "grpBxBackupDestination";
            this.grpBxBackupDestination.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // nupBackupServerPort
            // 
            resources.ApplyResources(this.nupBackupServerPort, "nupBackupServerPort");
            this.nupBackupServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nupBackupServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupBackupServerPort.Name = "nupBackupServerPort";
            this.nupBackupServerPort.Value = new decimal(new int[] {
            443,
            0,
            0,
            0});
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // rdBtnBackupServer
            // 
            resources.ApplyResources(this.rdBtnBackupServer, "rdBtnBackupServer");
            this.rdBtnBackupServer.Name = "rdBtnBackupServer";
            this.rdBtnBackupServer.UseVisualStyleBackColor = true;
            this.rdBtnBackupServer.CheckedChanged += new System.EventHandler(this.rdBtnDestination_CheckedChanged);
            // 
            // rdBtnFileSystem
            // 
            resources.ApplyResources(this.rdBtnFileSystem, "rdBtnFileSystem");
            this.rdBtnFileSystem.Checked = true;
            this.rdBtnFileSystem.Name = "rdBtnFileSystem";
            this.rdBtnFileSystem.TabStop = true;
            this.rdBtnFileSystem.UseVisualStyleBackColor = true;
            this.rdBtnFileSystem.CheckedChanged += new System.EventHandler(this.rdBtnDestination_CheckedChanged);
            // 
            // txtBxBackupServerName
            // 
            resources.ApplyResources(this.txtBxBackupServerName, "txtBxBackupServerName");
            this.txtBxBackupServerName.Name = "txtBxBackupServerName";
            this.txtBxBackupServerName.TextChanged += new System.EventHandler(this.txtBxBackupServerName_TextChanged);
            // 
            // txtBxDestination
            // 
            resources.ApplyResources(this.txtBxDestination, "txtBxDestination");
            this.txtBxDestination.Name = "txtBxDestination";
            this.txtBxDestination.TextChanged += new System.EventHandler(this.txtBxDestination_TextChanged);
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.grpBxSchedule);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grpBxSchedule
            // 
            resources.ApplyResources(this.grpBxSchedule, "grpBxSchedule");
            this.grpBxSchedule.Controls.Add(this.cmbBxWeekly);
            this.grpBxSchedule.Controls.Add(this.cmbBxEvery);
            this.grpBxSchedule.Controls.Add(this.nupMonthly);
            this.grpBxSchedule.Controls.Add(this.nupEvery);
            this.grpBxSchedule.Controls.Add(this.rdBtnMonthly);
            this.grpBxSchedule.Controls.Add(this.rdBtnWeekly);
            this.grpBxSchedule.Controls.Add(this.rdBtnEvery);
            this.grpBxSchedule.Name = "grpBxSchedule";
            this.grpBxSchedule.TabStop = false;
            // 
            // cmbBxWeekly
            // 
            this.cmbBxWeekly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbBxWeekly, "cmbBxWeekly");
            this.cmbBxWeekly.FormattingEnabled = true;
            this.cmbBxWeekly.Name = "cmbBxWeekly";
            // 
            // cmbBxEvery
            // 
            this.cmbBxEvery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxEvery.FormattingEnabled = true;
            this.cmbBxEvery.Items.AddRange(new object[] {
            resources.GetString("cmbBxEvery.Items"),
            resources.GetString("cmbBxEvery.Items1"),
            resources.GetString("cmbBxEvery.Items2")});
            resources.ApplyResources(this.cmbBxEvery, "cmbBxEvery");
            this.cmbBxEvery.Name = "cmbBxEvery";
            this.cmbBxEvery.SelectedIndexChanged += new System.EventHandler(this.cmbBxEvery_SelectedIndexChanged);
            // 
            // nupMonthly
            // 
            resources.ApplyResources(this.nupMonthly, "nupMonthly");
            this.nupMonthly.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nupMonthly.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupMonthly.Name = "nupMonthly";
            this.nupMonthly.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nupEvery
            // 
            resources.ApplyResources(this.nupEvery, "nupEvery");
            this.nupEvery.Maximum = new decimal(new int[] {
            365,
            0,
            0,
            0});
            this.nupEvery.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupEvery.Name = "nupEvery";
            this.nupEvery.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // rdBtnMonthly
            // 
            resources.ApplyResources(this.rdBtnMonthly, "rdBtnMonthly");
            this.rdBtnMonthly.Name = "rdBtnMonthly";
            this.rdBtnMonthly.UseVisualStyleBackColor = true;
            this.rdBtnMonthly.CheckedChanged += new System.EventHandler(this.rdBtnSchedule_CheckedChanged);
            // 
            // rdBtnWeekly
            // 
            resources.ApplyResources(this.rdBtnWeekly, "rdBtnWeekly");
            this.rdBtnWeekly.Name = "rdBtnWeekly";
            this.rdBtnWeekly.UseVisualStyleBackColor = true;
            this.rdBtnWeekly.CheckedChanged += new System.EventHandler(this.rdBtnSchedule_CheckedChanged);
            // 
            // rdBtnEvery
            // 
            resources.ApplyResources(this.rdBtnEvery, "rdBtnEvery");
            this.rdBtnEvery.Checked = true;
            this.rdBtnEvery.Name = "rdBtnEvery";
            this.rdBtnEvery.TabStop = true;
            this.rdBtnEvery.UseVisualStyleBackColor = true;
            this.rdBtnEvery.CheckedChanged += new System.EventHandler(this.rdBtnSchedule_CheckedChanged);
            // 
            // tabEventLog
            // 
            this.tabEventLog.Controls.Add(this.label3);
            this.tabEventLog.Controls.Add(this.cmbBxEventLogSeverity);
            this.tabEventLog.Controls.Add(this.chkBxEventLog);
            resources.ApplyResources(this.tabEventLog, "tabEventLog");
            this.tabEventLog.Name = "tabEventLog";
            this.tabEventLog.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmbBxEventLogSeverity
            // 
            resources.ApplyResources(this.cmbBxEventLogSeverity, "cmbBxEventLogSeverity");
            this.cmbBxEventLogSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBxEventLogSeverity.FormattingEnabled = true;
            this.cmbBxEventLogSeverity.Items.AddRange(new object[] {
            resources.GetString("cmbBxEventLogSeverity.Items"),
            resources.GetString("cmbBxEventLogSeverity.Items1"),
            resources.GetString("cmbBxEventLogSeverity.Items2"),
            resources.GetString("cmbBxEventLogSeverity.Items3")});
            this.cmbBxEventLogSeverity.Name = "cmbBxEventLogSeverity";
            // 
            // chkBxEventLog
            // 
            resources.ApplyResources(this.chkBxEventLog, "chkBxEventLog");
            this.chkBxEventLog.Checked = true;
            this.chkBxEventLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxEventLog.Name = "chkBxEventLog";
            this.chkBxEventLog.UseVisualStyleBackColor = true;
            this.chkBxEventLog.CheckedChanged += new System.EventHandler(this.chkBxEventLog_CheckedChanged);
            // 
            // tabReporting
            // 
            this.tabReporting.Controls.Add(this.nupReportingServerPort);
            this.tabReporting.Controls.Add(this.txtBxReportingServerName);
            this.tabReporting.Controls.Add(this.label6);
            this.tabReporting.Controls.Add(this.label5);
            this.tabReporting.Controls.Add(this.chkBxReporting);
            this.tabReporting.Controls.Add(this.label4);
            resources.ApplyResources(this.tabReporting, "tabReporting");
            this.tabReporting.Name = "tabReporting";
            this.tabReporting.UseVisualStyleBackColor = true;
            // 
            // nupReportingServerPort
            // 
            resources.ApplyResources(this.nupReportingServerPort, "nupReportingServerPort");
            this.nupReportingServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nupReportingServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupReportingServerPort.Name = "nupReportingServerPort";
            this.nupReportingServerPort.Value = new decimal(new int[] {
            443,
            0,
            0,
            0});
            // 
            // txtBxReportingServerName
            // 
            resources.ApplyResources(this.txtBxReportingServerName, "txtBxReportingServerName");
            this.txtBxReportingServerName.Name = "txtBxReportingServerName";
            this.txtBxReportingServerName.TextChanged += new System.EventHandler(this.txtBxReportingServerName_TextChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // chkBxReporting
            // 
            resources.ApplyResources(this.chkBxReporting, "chkBxReporting");
            this.chkBxReporting.Name = "chkBxReporting";
            this.chkBxReporting.UseVisualStyleBackColor = true;
            this.chkBxReporting.CheckedChanged += new System.EventHandler(this.chkBxReporting_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tabBackupAgent
            // 
            this.tabBackupAgent.Controls.Add(this.grpBxBackupMethod);
            this.tabBackupAgent.Controls.Add(this.txtBxAdditionalSubnets);
            this.tabBackupAgent.Controls.Add(this.label7);
            this.tabBackupAgent.Controls.Add(this.chkBxDontBackupOverWan);
            resources.ApplyResources(this.tabBackupAgent, "tabBackupAgent");
            this.tabBackupAgent.Name = "tabBackupAgent";
            this.tabBackupAgent.UseVisualStyleBackColor = true;
            // 
            // grpBxBackupMethod
            // 
            resources.ApplyResources(this.grpBxBackupMethod, "grpBxBackupMethod");
            this.grpBxBackupMethod.Controls.Add(this.label11);
            this.grpBxBackupMethod.Controls.Add(this.label10);
            this.grpBxBackupMethod.Controls.Add(this.rdBtnMethodDifferential);
            this.grpBxBackupMethod.Controls.Add(this.rdBtnMethodFull);
            this.grpBxBackupMethod.Name = "grpBxBackupMethod";
            this.grpBxBackupMethod.TabStop = false;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // rdBtnMethodDifferential
            // 
            resources.ApplyResources(this.rdBtnMethodDifferential, "rdBtnMethodDifferential");
            this.rdBtnMethodDifferential.Name = "rdBtnMethodDifferential";
            this.rdBtnMethodDifferential.UseVisualStyleBackColor = true;
            this.rdBtnMethodDifferential.CheckedChanged += new System.EventHandler(this.rdBtnMethodFull_CheckedChanged);
            // 
            // rdBtnMethodFull
            // 
            resources.ApplyResources(this.rdBtnMethodFull, "rdBtnMethodFull");
            this.rdBtnMethodFull.Checked = true;
            this.rdBtnMethodFull.Name = "rdBtnMethodFull";
            this.rdBtnMethodFull.TabStop = true;
            this.rdBtnMethodFull.UseVisualStyleBackColor = true;
            this.rdBtnMethodFull.CheckedChanged += new System.EventHandler(this.rdBtnMethodFull_CheckedChanged);
            // 
            // txtBxAdditionalSubnets
            // 
            resources.ApplyResources(this.txtBxAdditionalSubnets, "txtBxAdditionalSubnets");
            this.txtBxAdditionalSubnets.BackColor = System.Drawing.SystemColors.Window;
            this.txtBxAdditionalSubnets.Name = "txtBxAdditionalSubnets";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // chkBxDontBackupOverWan
            // 
            resources.ApplyResources(this.chkBxDontBackupOverWan, "chkBxDontBackupOverWan");
            this.chkBxDontBackupOverWan.Checked = true;
            this.chkBxDontBackupOverWan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxDontBackupOverWan.Name = "chkBxDontBackupOverWan";
            this.chkBxDontBackupOverWan.UseVisualStyleBackColor = true;
            this.chkBxDontBackupOverWan.CheckedChanged += new System.EventHandler(this.chkBxDontBackupOverWan_CheckedChanged);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // FrmSettings
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabFilesAndFolders.ResumeLayout(false);
            this.tabFilesAndFolders.PerformLayout();
            this.grpBxBackupDestination.ResumeLayout(false);
            this.grpBxBackupDestination.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupBackupServerPort)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.grpBxSchedule.ResumeLayout(false);
            this.grpBxSchedule.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupMonthly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupEvery)).EndInit();
            this.tabEventLog.ResumeLayout(false);
            this.tabEventLog.PerformLayout();
            this.tabReporting.ResumeLayout(false);
            this.tabReporting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupReportingServerPort)).EndInit();
            this.tabBackupAgent.ResumeLayout(false);
            this.tabBackupAgent.PerformLayout();
            this.grpBxBackupMethod.ResumeLayout(false);
            this.grpBxBackupMethod.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFilesAndFolders;
        private System.Windows.Forms.CheckBox chkBxCompressFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtBxDestination;
        private System.Windows.Forms.CheckedListBox chkLstBxPstFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox grpBxSchedule;
        private System.Windows.Forms.ComboBox cmbBxWeekly;
        private System.Windows.Forms.ComboBox cmbBxEvery;
        private System.Windows.Forms.NumericUpDown nupMonthly;
        private System.Windows.Forms.NumericUpDown nupEvery;
        private System.Windows.Forms.RadioButton rdBtnMonthly;
        private System.Windows.Forms.RadioButton rdBtnWeekly;
        private System.Windows.Forms.RadioButton rdBtnEvery;
        private System.Windows.Forms.TabPage tabEventLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBxEventLogSeverity;
        private System.Windows.Forms.CheckBox chkBxEventLog;
        private System.Windows.Forms.TabPage tabReporting;
        private System.Windows.Forms.NumericUpDown nupReportingServerPort;
        private System.Windows.Forms.TextBox txtBxReportingServerName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkBxReporting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabBackupAgent;
        private System.Windows.Forms.TextBox txtBxAdditionalSubnets;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkBxDontBackupOverWan;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox grpBxBackupDestination;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nupBackupServerPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdBtnBackupServer;
        private System.Windows.Forms.RadioButton rdBtnFileSystem;
        private System.Windows.Forms.TextBox txtBxBackupServerName;
        private System.Windows.Forms.GroupBox grpBxBackupMethod;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rdBtnMethodDifferential;
        private System.Windows.Forms.RadioButton rdBtnMethodFull;
    }
}

