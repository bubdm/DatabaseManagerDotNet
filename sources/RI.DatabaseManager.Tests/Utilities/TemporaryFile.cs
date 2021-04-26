using System;
using System.IO;




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
            if (this.File.Exists)
            {
                this.File.Delete();
            }
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
