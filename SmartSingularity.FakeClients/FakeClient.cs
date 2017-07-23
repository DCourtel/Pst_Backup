using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.FakeClients
{
    public class FakeClient : IDisposable
    {
        public enum ActivityState
        {
            Stopped,
            Started,
            Sleeping,
            Registering,
            Saving,
            Reporting
        }

        private ReportService.ReportServerClient _proxy;
        private Random rnd = new Random(DateTime.Now.Millisecond);
        private List<FakePstFile> _pstFiles = new List<FakePstFile>();
        private ActivityState _state = ActivityState.Stopped;
        private System.Timers.Timer _chrono = new System.Timers.Timer(1000);
        private bool _stopping = false;

        public FakeClient(string pstLocalFolder, bool createPstFiles, int rowIndex)
        {
            ComputerName = GetRandomComputerName();
            UserName = FakeUser.GetRandomUserName();
            ClientVersion = GetRandomClientVersion();
            ClientId = Guid.NewGuid().ToString();
            CreatePstFiles = createPstFiles;
            _pstFiles = GetFakePstFiles($"{pstLocalFolder}\\{ClientId}");
            LocalStorage = $"{pstLocalFolder}\\{ClientId}";
            RowIndex = rowIndex;

            _proxy = new ReportService.ReportServerClient();

            System.Threading.Thread.Sleep(rnd.Next(70, 250));
            _chrono.Elapsed += _chrono_Elapsed;
        }

        #region Properties

        public bool Stopping { get { return _stopping; } set { _stopping = value; } }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public Version ClientVersion { get; set; }
        public List<FakePstFile> PstFiles
        {
            get { return _pstFiles; }
            set { _pstFiles = value; }
        }
        public ActivityState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnStateChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public string ClientId { get; set; }
        public int RowIndex { get; set; }
        private string LocalStorage { get; set; }
        private bool CreatePstFiles { get; set; }

        #endregion Properties

        #region Methods

        public void Start()
        {
            _chrono.Start();
            Register();
            State = ActivityState.Started;
        }

        public void Stop()
        {

        }

        private void _chrono_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void Register()
        {
            State = ActivityState.Registering;
            try
            {
                ReportService.Client client = new ReportService.Client()
                {
                    ComputerName = this.ComputerName,
                    Id = this.ClientId,
                    Username = this.UserName,
                    Version = this.ClientVersion
                };
                _proxy.RegisterClient(client);
            }
            catch (Exception) { }
        }
        private string GetRandomComputerName()
        {
            return $"Comp{rnd.Next(100, 999)}";
        }
        private Version GetRandomClientVersion()
        {
            return new Version(rnd.Next(1, 7), rnd.Next(0, 100), rnd.Next(0, 300), rnd.Next(0, 9999));
        }
        private List<FakePstFile> GetFakePstFiles(string localFolder)
        {
            List<FakePstFile> pstFiles = new List<FakePstFile>();

            for (int i = 0; i < rnd.Next(1, 5); i++)
            {
                FakePstFile pstFile = new FakePstFile(localFolder, CreatePstFiles);
                pstFiles.Add(pstFile);
            }
            return pstFiles;
        }

        public void Dispose()
        {
            _chrono.Stop();
            foreach (FakePstFile pstFile in PstFiles)
            {
                pstFile.Dispose();
            }
            try
            {
                System.IO.Directory.Delete(LocalStorage, true);
            }
            catch (Exception) { }
        }

        #endregion Methods


        #region (Event Delegates)


        public event EventHandler OnStateChanged;

        #endregion (Event Delegates)
    }
}
