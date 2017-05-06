namespace SmartSingularity.PstBackupAgent
{
    partial class FrmAgent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAgent));
            this.label1 = new System.Windows.Forms.Label();
            this.chkLstBxPstFiles = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pctBxLogo = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.prgBrOverAll = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.prgBrCurrentFile = new System.Windows.Forms.ProgressBar();
            this.chkBxShutdownComputer = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBxDestination = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctBxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // chkLstBxPstFiles
            // 
            resources.ApplyResources(this.chkLstBxPstFiles, "chkLstBxPstFiles");
            this.chkLstBxPstFiles.FormattingEnabled = true;
            this.chkLstBxPstFiles.Name = "chkLstBxPstFiles";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pctBxLogo
            // 
            resources.ApplyResources(this.pctBxLogo, "pctBxLogo");
            this.pctBxLogo.Image = global::SmartSingularity.PstBackupAgent.Properties.Resources.Logo;
            this.pctBxLogo.Name = "pctBxLogo";
            this.pctBxLogo.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // prgBrOverAll
            // 
            resources.ApplyResources(this.prgBrOverAll, "prgBrOverAll");
            this.prgBrOverAll.Name = "prgBrOverAll";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // prgBrCurrentFile
            // 
            resources.ApplyResources(this.prgBrCurrentFile, "prgBrCurrentFile");
            this.prgBrCurrentFile.Name = "prgBrCurrentFile";
            // 
            // chkBxShutdownComputer
            // 
            resources.ApplyResources(this.chkBxShutdownComputer, "chkBxShutdownComputer");
            this.chkBxShutdownComputer.Name = "chkBxShutdownComputer";
            this.chkBxShutdownComputer.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtBxDestination
            // 
            resources.ApplyResources(this.txtBxDestination, "txtBxDestination");
            this.txtBxDestination.Name = "txtBxDestination";
            this.txtBxDestination.ReadOnly = true;
            // 
            // FrmAgent
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtBxDestination);
            this.Controls.Add(this.chkBxShutdownComputer);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.prgBrCurrentFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.prgBrOverAll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pctBxLogo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkLstBxPstFiles);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAgent";
            ((System.ComponentModel.ISupportInitialize)(this.pctBxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox chkLstBxPstFiles;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pctBxLogo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar prgBrOverAll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar prgBrCurrentFile;
        private System.Windows.Forms.CheckBox chkBxShutdownComputer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBxDestination;
    }
}

