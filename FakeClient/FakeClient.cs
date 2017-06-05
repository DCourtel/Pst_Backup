using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeClient
{
    public class FakeClient : IDisposable
    {
        public enum ActivityState
        {
            Stopped,
            Starting,
            Sleeping,
            Registering,
            Saving,
            Reporting
        }

        private Random rnd = new Random(DateTime.Now.Millisecond);
        private List<FakePstFile> _pstFiles = new List<FakePstFile>();
        private ActivityState _activity = ActivityState.Stopped;

        public FakeClient(string pstLocalFolder, bool createPstFiles)
        {
            ComputerName = GetRandomComputerName();
            UserName = FakeUser.GetRandomUserName();
            ClientVersion = GetRandomClientVersion();
            ClientId = Guid.NewGuid();
            _pstFiles = GetFakePstFiles($"{pstLocalFolder}\\{ClientId}");
            LocalStorage = $"{pstLocalFolder}\\{ClientId}";
            CreatePstFiles = createPstFiles;

            System.Threading.Thread.Sleep(rnd.Next(70, 250));
        }

        #region Properties

        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public Version ClientVersion { get; set; }
        public List<FakePstFile> PstFiles
        {
            get { return _pstFiles; }
            set { _pstFiles = value; }
        }
        public ActivityState Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }
        public Guid ClientId { get; set; }
        private string LocalStorage { get; set; }
        private bool CreatePstFiles { get; set; }

        #endregion Properties

        #region Methods

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
                FakePstFile pstFile = new FakePstFile(localFolder);
                pstFiles.Add(pstFile);
            }
            return pstFiles;
        }

        public void Dispose()
        {
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


    }
}
