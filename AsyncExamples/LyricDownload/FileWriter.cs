using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface IFileWriter
    {
        string RootDir { get; }

        Task<string> WriteFile(string fileContents, string fileName);
    }

    public class FileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        public FileWriter(string rootDir) { RootDir = rootDir.Replace('\\', '/').TrimEnd('/'); }

        public Task<string> WriteFile(string fileContents, string fileName)
        {
            var tId = Thread.CurrentThread.ManagedThreadId;

            return Task.Run(() =>
            {
                Directory.CreateDirectory(RootDir);

                var fullPath = string.Format("{0}/{1}.html", RootDir, fileName);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                using (var sr = new StreamWriter(fs))
                {
                    //must wait on async task to prevent closing
                    //stream before sr is finished writting
                    sr.WriteAsync(fileContents).Wait();
                }
                return fullPath;
            });
        }

    }

    public class AsyncFileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        public AsyncFileWriter(string rootDir) { RootDir = rootDir.Replace('\\', '/').TrimEnd('/'); }

        public async Task<string> WriteFile(string fileContents, string fileName)
        {
            var tId = Thread.CurrentThread.ManagedThreadId;

            Directory.CreateDirectory(RootDir);

            var fullPath = string.Format("{0}/{1}.html", RootDir, fileName);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            using (var sr = new StreamWriter(fs))
                await sr.WriteAsync(fileContents);
            return fullPath;

        }

    }
}
