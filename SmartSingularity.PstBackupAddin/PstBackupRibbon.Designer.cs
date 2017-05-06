namespace SmartSingularity.PstBackupAddin
{
    partial class PstBackupRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public PstBackupRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PstBackupRibbon));
            this.tabPstBackup = this.Factory.CreateRibbonTab();
            this.ribGrpPstBackup = this.Factory.CreateRibbonGroup();
            this.btnSettings = this.Factory.CreateRibbonButton();
            this.tabPstBackup.SuspendLayout();
            this.ribGrpPstBackup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPstBackup
            // 
            this.tabPstBackup.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabPstBackup.Groups.Add(this.ribGrpPstBackup);
            resources.ApplyResources(this.tabPstBackup, "tabPstBackup");
            this.tabPstBackup.Name = "tabPstBackup";
            // 
            // ribGrpPstBackup
            // 
            this.ribGrpPstBackup.Items.Add(this.btnSettings);
            resources.ApplyResources(this.ribGrpPstBackup, "ribGrpPstBackup");
            this.ribGrpPstBackup.Name = "ribGrpPstBackup";
            // 
            // btnSettings
            // 
            this.btnSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSettings.Image = global::SmartSingularity.PstBackupAddin.Properties.Resources.Ps_tBackup_48x48;
            resources.ApplyResources(this.btnSettings, "btnSettings");
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ShowImage = true;
            this.btnSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSettings_Click);
            // 
            // PstBackupRibbon
            // 
            this.Name = "PstBackupRibbon";
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.tabPstBackup);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.PstBackupRibbon_Load);
            this.tabPstBackup.ResumeLayout(false);
            this.tabPstBackup.PerformLayout();
            this.ribGrpPstBackup.ResumeLayout(false);
            this.ribGrpPstBackup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabPstBackup;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ribGrpPstBackup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSettings;
    }

    partial class ThisRibbonCollection
    {
        internal PstBackupRibbon PstBackupRibbon
        {
            get { return this.GetRibbon<PstBackupRibbon>(); }
        }
    }
}
