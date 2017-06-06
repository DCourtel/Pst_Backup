using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSingularity.FakeClients
{
    public class FakePstFile : IDisposable
    {
        public FakePstFile(string localPath)
        {
            LocalPath = localPath;
            Filename = $"{System.IO.Path.GetRandomFileName()}.pst";
            FileId = Guid.NewGuid();
            Size = GetRandomFileSize();
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

        private long GetRandomFileSize()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            return (long)rnd.Next(31457280, 73400320);
        }


        #endregion Methods
    }
}
