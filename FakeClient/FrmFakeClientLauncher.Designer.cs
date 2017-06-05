namespace FakeClient
{
    partial class FrmFakeClientLauncher
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
            this.label1 = new System.Windows.Forms.Label();
            this.nupClientsCount = new System.Windows.Forms.NumericUpDown();
            this.chkBxCreatePstFiles = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStartClients = new System.Windows.Forms.Button();
            this.btnStopClients = new System.Windows.Forms.Button();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.ClientObj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComputerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClientVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PstCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Activity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClientID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDeleteClients = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBxPstFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCreateClients = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nupClientsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre de clients à générer : ";
            // 
            // nupClientsCount
            // 
            this.nupClientsCount.Location = new System.Drawing.Point(181, 28);
            this.nupClientsCount.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.nupClientsCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupClientsCount.Name = "nupClientsCount";
            this.nupClientsCount.Size = new System.Drawing.Size(61, 20);
            this.nupClientsCount.TabIndex = 1;
            this.nupClientsCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // chkBxCreatePstFiles
            // 
            this.chkBxCreatePstFiles.AutoSize = true;
            this.chkBxCreatePstFiles.Location = new System.Drawing.Point(248, 29);
            this.chkBxCreatePstFiles.Name = "chkBxCreatePstFiles";
            this.chkBxCreatePstFiles.Size = new System.Drawing.Size(220, 17);
            this.chkBxCreatePstFiles.TabIndex = 2;
            this.chkBxCreatePstFiles.Text = "Générer des fichiers &PST pour les clients.";
            this.chkBxCreatePstFiles.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(757, 343);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Fermer";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnStartClients
            // 
            this.btnStartClients.Enabled = false;
            this.btnStartClients.Location = new System.Drawing.Point(356, 93);
            this.btnStartClients.Name = "btnStartClients";
            this.btnStartClients.Size = new System.Drawing.Size(135, 23);
            this.btnStartClients.TabIndex = 4;
            this.btnStartClients.Text = "&Démarrer les clients";
            this.btnStartClients.UseVisualStyleBackColor = true;
            // 
            // btnStopClients
            // 
            this.btnStopClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopClients.Enabled = false;
            this.btnStopClients.Location = new System.Drawing.Point(714, 93);
            this.btnStopClients.Name = "btnStopClients";
            this.btnStopClients.Size = new System.Drawing.Size(118, 23);
            this.btnStopClients.TabIndex = 5;
            this.btnStopClients.Text = "&Arrêter les clients";
            this.btnStopClients.UseVisualStyleBackColor = true;
            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToAddRows = false;
            this.dgvClients.AllowUserToDeleteRows = false;
            this.dgvClients.AllowUserToOrderColumns = true;
            this.dgvClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClientObj,
            this.ComputerName,
            this.UserName,
            this.ClientVersion,
            this.PstCount,
            this.Activity,
            this.ClientID});
            this.dgvClients.Location = new System.Drawing.Point(12, 122);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.ReadOnly = true;
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.Size = new System.Drawing.Size(820, 215);
            this.dgvClients.TabIndex = 6;
            // 
            // ClientObj
            // 
            this.ClientObj.HeaderText = "ClientObj";
            this.ClientObj.Name = "ClientObj";
            this.ClientObj.ReadOnly = true;
            this.ClientObj.Visible = false;
            // 
            // ComputerName
            // 
            this.ComputerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ComputerName.FillWeight = 15F;
            this.ComputerName.HeaderText = "Computer Name";
            this.ComputerName.Name = "ComputerName";
            this.ComputerName.ReadOnly = true;
            // 
            // UserName
            // 
            this.UserName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UserName.FillWeight = 15F;
            this.UserName.HeaderText = "User Name";
            this.UserName.Name = "UserName";
            this.UserName.ReadOnly = true;
            // 
            // ClientVersion
            // 
            this.ClientVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClientVersion.FillWeight = 20F;
            this.ClientVersion.HeaderText = "Client Version";
            this.ClientVersion.Name = "ClientVersion";
            this.ClientVersion.ReadOnly = true;
            // 
            // PstCount
            // 
            this.PstCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PstCount.FillWeight = 10F;
            this.PstCount.HeaderText = "Pst Count";
            this.PstCount.Name = "PstCount";
            this.PstCount.ReadOnly = true;
            // 
            // Activity
            // 
            this.Activity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Activity.FillWeight = 10F;
            this.Activity.HeaderText = "Activity";
            this.Activity.Name = "Activity";
            this.Activity.ReadOnly = true;
            // 
            // ClientID
            // 
            this.ClientID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ClientID.FillWeight = 30F;
            this.ClientID.HeaderText = "Client ID";
            this.ClientID.Name = "ClientID";
            this.ClientID.ReadOnly = true;
            // 
            // btnDeleteClients
            // 
            this.btnDeleteClients.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteClients.Enabled = false;
            this.btnDeleteClients.Location = new System.Drawing.Point(12, 343);
            this.btnDeleteClients.Name = "btnDeleteClients";
            this.btnDeleteClients.Size = new System.Drawing.Size(135, 23);
            this.btnDeleteClients.TabIndex = 7;
            this.btnDeleteClients.Text = "Supprimer les clients";
            this.btnDeleteClients.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Emplacement des fichiers PST : ";
            // 
            // txtBxPstFolder
            // 
            this.txtBxPstFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBxPstFolder.Location = new System.Drawing.Point(181, 55);
            this.txtBxPstFolder.Name = "txtBxPstFolder";
            this.txtBxPstFolder.Size = new System.Drawing.Size(570, 20);
            this.txtBxPstFolder.TabIndex = 9;
            this.txtBxPstFolder.Text = "E:\\Pst Backup\\Pst Files\\FakeClients";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(757, 53);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "&Parcourir…";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCreateClients
            // 
            this.btnCreateClients.Location = new System.Drawing.Point(12, 93);
            this.btnCreateClients.Name = "btnCreateClients";
            this.btnCreateClients.Size = new System.Drawing.Size(135, 23);
            this.btnCreateClients.TabIndex = 11;
            this.btnCreateClients.Text = "Générer les clients";
            this.btnCreateClients.UseVisualStyleBackColor = true;
            this.btnCreateClients.Click += new System.EventHandler(this.btnCreateClients_Click);
            // 
            // FrmFakeClientLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 378);
            this.Controls.Add(this.btnCreateClients);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtBxPstFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDeleteClients);
            this.Controls.Add(this.dgvClients);
            this.Controls.Add(this.btnStopClients);
            this.Controls.Add(this.btnStartClients);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkBxCreatePstFiles);
            this.Controls.Add(this.nupClientsCount);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(860, 415);
            this.Name = "FrmFakeClientLauncher";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fake Client Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.nupClientsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nupClientsCount;
        private System.Windows.Forms.CheckBox chkBxCreatePstFiles;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStartClients;
        private System.Windows.Forms.Button btnStopClients;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientObj;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComputerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn PstCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Activity;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClientID;
        private System.Windows.Forms.Button btnDeleteClients;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBxPstFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCreateClients;
    }
}

