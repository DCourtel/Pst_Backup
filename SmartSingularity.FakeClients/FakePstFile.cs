using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SmartSingularity.FakeClients
{
    public class FakePstFile : IDisposable
    {
        private System.Random rnd = new Random(System.DateTime.Now.Millisecond);

        public FakePstFile(string localPath, bool createPstFile)
        {
            LocalPath = localPath;
            Filename = $"{System.IO.Path.GetRandomFileName()}.pst";
            FileId = Guid.NewGuid();
            Size = GetRandomFileSize();
            if (createPstFile)
                CreatePstFile();
        }

        #region Properties

        public string LocalPath { get; set; }
        public string Filename { get; set; }
        public Guid FileId { get; set; }
        public long Size { get; set; }

        public void Dispose()
        {
            try
            {
                System.IO.File.Delete(System.IO.Path.Combine(LocalPath, Filename));
            }
            catch (Exception) { }
        }

        #endregion Properties

        #region Methods

        public void UpdateSize()
        {
            System.IO.FileInfo info = new System.IO.FileInfo(System.IO.Path.Combine(LocalPath, Filename));
            if(info.Exists)
            {
                Size = info.Length;
            }
        }

        private long GetRandomFileSize()
        {
            return (long)rnd.Next(31457280, 73400320);
        }

        private void CreatePstFile()
        {
            if (!Directory.Exists(LocalPath))
                Directory.CreateDirectory(LocalPath);

            using (Stream pstFile = System.IO.File.Create(Path.Combine(LocalPath, Filename)))
            {
                long currentSize = 0;
                int chunkSize = 10240;
                do
                {
                    pstFile.Write(GetRandomBytes(chunkSize), 0, chunkSize);
                    currentSize += chunkSize;
                    chunkSize = (int)System.Math.Min(10240, Size - currentSize);
                } while (currentSize < Size);
            }
        }

        private byte[] GetRandomBytes(int byteCount)
        {
            byte[] bytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                bytes[i] = (byte)rnd.Next(0, 255);
            }

            return bytes;
        }


        #endregion Methods
    }
}
