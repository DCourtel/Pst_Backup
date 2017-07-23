using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartSingularity.FakeClients
{
    public partial class FrmFakeClientLauncher : Form
    {
        private List<FakeClient> _clients = new List<FakeClient>();
        private List<System.Threading.Thread> _threadList = new List<System.Threading.Thread>();

        public FrmFakeClientLauncher()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (!String.IsNullOrWhiteSpace(txtBxPstFolder.Text) && System.IO.Directory.Exists(txtBxPstFolder.Text))
            {
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowser.SelectedPath = txtBxPstFolder.Text;
            }
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtBxPstFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void btnCreateClients_Click(object sender, EventArgs e)
        {
            dgvClients.Rows.Clear();
            btnCreateClients.Enabled = false;
            _clients.Clear();

            for (int i = 0; i < nupClientsCount.Value; i++)
            {
                int index = dgvClients.Rows.Add();
                FakeClient client = new FakeClient(txtBxPstFolder.Text, chkBxCreatePstFiles.Checked, index);
                _clients.Add(client);
                dgvClients.Rows[index].Cells["ComputerName"].Value = client.ComputerName;
                dgvClients.Rows[index].Cells["UserName"].Value = client.UserName;
                dgvClients.Rows[index].Cells["ClientVersion"].Value = client.ClientVersion.ToString();
                dgvClients.Rows[index].Cells["PstCount"].Value = client.PstFiles.Count;
                dgvClients.Rows[index].Cells["State"].Value = client.State;
                dgvClients.Rows[index].Cells["ClientID"].Value = client.ClientId;

                dgvClients.Rows[index].Cells["ClientObj"].Value = client;
                client.OnStateChanged += Client_OnStateChanged;
                dgvClients.Refresh();
            }
            btnStartClients.Enabled = true;
        }

        private void btnStartClients_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                try
                {
                    FakeClient client = (FakeClient)row.Cells["ClientObj"].Value;
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(client.Start));
                    t.IsBackground = true;
                    _threadList.Add(t);
                    t.Start();
                }
                catch (Exception) { }
            }
            btnStopClients.Enabled = true;
        }

        private void btnStopClients_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvClients.Rows)
            {
                try
                {
                    FakeClient client = (FakeClient)row.Cells["ClientObj"].Value;
                    client.Stop();
                }
                catch (Exception) { }
            }
        }

        private void Client_OnStateChanged(object sender, EventArgs e)
        {
            FakeClient client = (FakeClient)sender;

            Action updateClientState = () =>
            {
                DataGridViewCell cell = dgvClients.Rows[client.RowIndex].Cells["State"];

                cell.Value = client.State.ToString();
                //dgvClients.Refresh();
                dgvClients.InvalidateCell(cell);
            };
            this.Invoke(updateClientState);
        }

        private void FrmFakeClientLauncher_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (FakeClient client in _clients)
            {
                client.Dispose();
            }
        }
    }
}
