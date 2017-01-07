using System;
using System.IO;

namespace UnitTests
{
    internal class TemproaryDirectory : IDisposable
    {
        string _path = null;

        private string GetRandomName()
        {
            return string.Format("{0}_{1}_TEMPDIR", DateTime.Now.Ticks, new Random().Next());
        }

        public string AppendPath(string path)
        {
            return Path.Combine(_path, path);
        }

        public TemproaryDirectory()
        {
            var tempPath = System.IO.Path.GetTempPath();
            _path = Directory.CreateDirectory(Path.Combine(tempPath, GetRandomName())).FullName;
        }

        public void Dispose()
        {
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path, true);
            }
        }
    }
}
