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

        public FileWriter(string rootDir)
        {
            RootDir = rootDir.Replace('\\', '/').TrimEnd('/');
        }

        /// <summary>
        /// Using TaskFactory.StartNew()
        /// </summary>
        /// <param name="fileContents"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<string> WriteFile(string fileContents, string fileName)
        {
            return HelperTasks.Write(fileContents, RootDir, fileName);
        }
    }

    class AsyncFileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        AsyncFileWriter(string rootDir)
        {
            RootDir = rootDir.Replace('\\', '/').TrimEnd('/');
        }

        /// <summary>
        /// Using async Keyword
        /// </summary>
        /// <param name="fileContents"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> WriteFile(string fileContents, string fileName)
        {
            return await HelperTasks.AsyncWrite(fileContents, RootDir, fileName);
        }
    }
}
