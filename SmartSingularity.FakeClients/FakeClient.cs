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
            Reporting,
            Comparing,
            Altering,
            Expanding
        }

        private ReportService.ReportServerClient _proxy;
        private Random rnd = new Random(DateTime.Now.Millisecond);
        private List<FakePstFile> _pstFiles = new List<FakePstFile>();
        private ActivityState _state = ActivityState.Stopped;
        private System.Timers.Timer _chrono = new System.Timers.Timer(1000);
        private bool _stopping = false;
        private PstBackupEngine.CoreBackupEngine _backupEngine;
        private PstBackupSettings.ApplicationSettings _appSettings = new PstBackupSettings.ApplicationSettings(PstBackupSettings.ApplicationSettings.SourceSettings.Local);

        public FakeClient(string pstLocalFolder, bool createPstFiles, bool compressPstFile, int rowIndex)
        {
            ComputerName = GetRandomComputerName();
            UserName = FakeUser.GetRandomUserName();
            ClientVersion = GetRandomClientVersion();
            ClientId = Guid.NewGuid().ToString();
            CreatePstFiles = createPstFiles;
            _pstFiles = GetFakePstFiles($"{pstLocalFolder}\\{ClientId}");
            LocalStorage = $"{pstLocalFolder}\\{ClientId}";
            RowIndex = rowIndex;
            _appSettings.BackupAgentBackupMethod = PstBackupSettings.ApplicationSettings.BackupMethod.Full;
            _appSettings.FilesAndFoldersCompressFiles = compressPstFile;
            _appSettings.FilesAndFoldersDestinationPath = @"\\192.168.0.250\Share\Transit\PstFiles\" + ClientId;
            _backupEngine = PstBackupEngine.CoreBackupEngine.GetBackupEngine(_appSettings);
            _backupEngine.OnBackupFinished += _backupEngine_OnBackupFinished;

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
            Register();
            _chrono.Interval = rnd.Next(1 * 10 * 1000, 3 * 10 * 1000);
            _chrono.Start();
            State = ActivityState.Started;
        }

        public void Stop()
        {
            _chrono.Stop();
        }

        private void _chrono_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _chrono.Stop();
            if (!Stopping)
            {
                foreach (FakePstFile pstFile in _pstFiles)
                {
                    State = ActivityState.Saving;
                    pstFile.UpdateSize();
                    ReportService.PstFile pstFileToSave = new ReportService.PstFile()
                    {
                        Id = pstFile.FileId,
                        IsSetToBackup = true,
                        LocalPath = System.IO.Path.Combine(pstFile.LocalPath, pstFile.Filename),
                        Size = pstFile.Size
                    };
                    _proxy.RegisterPstFile(ClientId, pstFileToSave);
                    PstBackupSettings.PSTRegistryEntry regEntry = new PstBackupSettings.PSTRegistryEntry(pstFileToSave.LocalPath);
                    if (!Stopping)
                    {
                        _backupEngine.Backup((object)regEntry);
                    }
                }
                State = ActivityState.Sleeping;
                _chrono.Interval = rnd.Next(3 * 60 * 1000, 10 * 60 * 1000);
                _chrono.Start();
            }
        }

        private void _backupEngine_OnBackupFinished(object sender, PstBackupEngine.BackupFinishedEventArgs e)
        {
            ReportService.BackupSession bckSession = new ReportService.BackupSession()
            {
                BackupMethod = _appSettings.BackupAgentBackupMethod,
                ChunkCount = e.Result.ChunkCount,
                CompressedSize = e.Result.CompressedSize,
                EndTime = e.Result.EndTime,
                ErrorCode = e.Result.ErrorCode,
                ErrorMessage = e.Result.ErrorMessage,
                IsCompressed = e.Result.IsCompressed,
                LocalPath = e.Result.LocalPath,
                RemotePath = e.Result.RemotePath,
                StartTime = e.Result.StartTime
            };

            State = ActivityState.Comparing;
            if (!CompareLocalAndRemotePstFiles(e.Result.LocalPath, e.Result.RemotePath))
            {
                bckSession.ErrorCode = PstBackupEngine.BackupResultInfo.BackupResult.Failed;
                bckSession.ErrorMessage = "Remote PST file and local PST file are not identical.";
            }

            State = ActivityState.Reporting;
            _proxy.RegisterBackupResult(ClientId, bckSession);

            State = ActivityState.Altering;
            AlterLocalPstFile(e.Result.LocalPath);

            State = ActivityState.Expanding;
            ExpandLocalPstFile(e.Result.LocalPath);
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
        private bool CompareLocalAndRemotePstFiles(string localPath, string remotePath)
        {
            bool result = true;
            System.IO.FileInfo localPST = new System.IO.FileInfo(localPath);
            System.IO.FileInfo remotePST = new System.IO.FileInfo(remotePath);
            if (_appSettings.FilesAndFoldersCompressFiles)
            {
                System.IO.FileInfo uncompressedPST = new System.IO.FileInfo(System.IO.Path.Combine(remotePST.DirectoryName, "uncompressed_PST.pst"));

                using (System.IO.FileStream compressedFile = remotePST.OpenRead())
                using (System.IO.Stream sourceFile = new System.IO.Compression.GZipStream(compressedFile, System.IO.Compression.CompressionMode.Decompress))
                using (System.IO.Stream outputFile = System.IO.File.Create(uncompressedPST.FullName))
                {
                    int bufferLength = 1024 * 1024;
                    byte[] buffer = new byte[bufferLength];
                    int readBytes = 0;
                    while ((readBytes = sourceFile.Read(buffer, 0, bufferLength)) > 0)
                    {
                        outputFile.Write(buffer, 0, readBytes);
                    }
                }
                remotePST = uncompressedPST;
            }

            if (localPST.Exists && remotePST.Exists && localPST.Length == remotePST.Length)
            {
                using (System.IO.FileStream localStream = localPST.OpenRead())
                {
                    using (System.IO.FileStream remoteStream = remotePST.OpenRead())
                    {
                        int currentPosition = 0;
                        int bytesRead = 0;
                        byte[] localBytes = new byte[10240];
                        byte[] remoteBytes = new byte[10240];

                        do
                        {
                            bytesRead = localStream.Read(localBytes, 0, 10240);
                            remoteStream.Read(remoteBytes, 0, 10240);
                            if (!CompareArrays(localBytes, remoteBytes))
                                result = false;
                            currentPosition += bytesRead;
                        } while (currentPosition < localStream.Length);
                    }
                }
            }
            else
                result = false;
            if (_appSettings.FilesAndFoldersCompressFiles)
            {
                remotePST.Delete();
            }
            return result;
        }
        private bool CompareArrays(byte[] array1, byte[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
        private void AlterLocalPstFile(string pstFilePath)
        {
            System.IO.FileInfo pstFile = new System.IO.FileInfo(pstFilePath);
            int changeCount = rnd.Next(3, 8);

            try
            {
                using (System.IO.FileStream pstStream = pstFile.OpenWrite())
                {
                    do
                    {
                        int origin = (int)(pstFile.Length / changeCount - rnd.Next(100_000, 200_000));
                        pstStream.Position = origin;
                        pstStream.Write(GetRandomBytes(), 0, 2048);
                        changeCount--;
                    } while (changeCount > 0);
                }
            }
            catch (Exception) { }
        }
        private byte[] GetRandomBytes()
        {
            byte[] randomBytes = new byte[2048];

            for (int i = 0; i < randomBytes.Length; i++)
            {
                randomBytes[i] = (byte)rnd.Next(0, 255);
            }

            return randomBytes;
        }
        private void ExpandLocalPstFile(string pstFilePath)
        {
            try
            {
                using (System.IO.FileStream pstFile = new System.IO.FileInfo(pstFilePath).Open(System.IO.FileMode.Append, System.IO.FileAccess.Write))
                {
                    pstFile.Write(GetRandomBytes(), 0, 2048);
                }
            }
            catch (Exception) { }
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
                System.IO.Directory.Delete(_appSettings.FilesAndFoldersDestinationPath, true);
            }
            catch (Exception) { }
        }

        #endregion Methods


        #region (Event Delegates)


        public event EventHandler OnStateChanged;

        #endregion (Event Delegates)
    }
}
