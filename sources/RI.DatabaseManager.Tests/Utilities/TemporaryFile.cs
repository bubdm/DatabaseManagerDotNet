using System;
using System.IO;
using System.Threading;




namespace RI.DatabaseManager.Tests.Utilities
{
    public sealed class TemporaryFile : IDisposable
    {
        public TemporaryFile ()
        {
            this.File = new FileInfo(Path.GetTempFileName());
            this.Create();
        }

        public TemporaryFile (FileInfo file)
        {
            this.File = file;
            this.Create();
        }

        public TemporaryFile (string file)
        {
            this.File = new FileInfo(file);
            this.Create();
        }

        public FileInfo File { get; }

        public void Delete ()
        {
            ThreadPool.QueueUserWorkItem((_) =>
            {
                while (this.File.Exists)
                {
                    try
                    {
                        Thread.Sleep(100);
                        this.File.Delete();
                        return;
                    }
                    catch (IOException)
                    {

                    }
                }
            });
        }

        private void Create ()
        {
            if (!this.File.Exists)
            {
                this.File.Create().Close();
            }
        }

        public void Dispose ()
        {
            this.Delete();
            GC.SuppressFinalize(this);
        }

        ~TemporaryFile ()
        {
            this.Delete();
        }

        public string FullPath => this.File.FullName;
    }
}
