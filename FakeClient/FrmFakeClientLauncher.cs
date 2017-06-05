using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FakeClient
{
    public partial class FrmFakeClientLauncher : Form
    {
        private List<FakeClient> _clients = new List<FakeClient>();

        public FrmFakeClientLauncher()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if(!String.IsNullOrWhiteSpace(txtBxPstFolder.Text) && System.IO.Directory.Exists(txtBxPstFolder.Text))
            {
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowser.SelectedPath = txtBxPstFolder.Text;
            }
            if(folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtBxPstFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void btnCreateClients_Click(object sender, EventArgs e)
        {
            dgvClients.Rows.Clear();
            _clients.Clear();

            for (int i = 0; i < nupClientsCount.Value; i++)
            {
                FakeClient client = new FakeClient(txtBxPstFolder.Text, chkBxCreatePstFiles.Checked);
                _clients.Add(client);
                int index = dgvClients.Rows.Add();
                dgvClients.Rows[index].Cells["ComputerName"].Value = client.ComputerName;
                dgvClients.Rows[index].Cells["UserName"].Value = client.UserName;
                dgvClients.Rows[index].Cells["ClientVersion"].Value = client.ClientVersion.ToString();
                dgvClients.Rows[index].Cells["PstCount"].Value = client.PstFiles.Count;
                dgvClients.Rows[index].Cells["Activity"].Value = client.Activity;
                dgvClients.Rows[index].Cells["ClientID"].Value = client.ClientId;

                dgvClients.Rows[index].Cells["ClientObj"].Value = client;
            }
            btnStartClients.Enabled = true;
        }
    }
}
