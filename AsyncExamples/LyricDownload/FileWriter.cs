using System.Threading.Tasks;

namespace LyricDownload
{
    public interface IFileWriter
    {
        string RootDir { get; }

        Task<string> WriteFile(string fileContents, string fileName);
        Task<string> WriteFile(ISong song);
    }

    public class FileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        public FileWriter(string rootDir) { RootDir = rootDir.Replace('\\', '/').TrimEnd('/'); }

        public Task<string> WriteFile(string fileContents, string fileName)
        {
            //Return task that will write file contents to RootDir/filename
            return HelperTasks.Write(fileContents, RootDir, fileName);
        }

        public Task<string> WriteFile(ISong song)
        {
            Task.Delay(25000).Wait();

            return new TaskFactory<string>().StartNew(() =>
            {
                //Get task that will write the result of GetLyrics to a file with the name name as song.Name
                var writeFileTask = WriteFile(song.GetLyrics().Result, song.Name);

                //??
                ////Wait for writeFileTask to complete
                //writeFileTask.Wait();

                //Return the result of writeFileTask
                return writeFileTask.Result;
            });
        }
    }

    public class ContinuationFileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        public ContinuationFileWriter(string rootDir) { RootDir = rootDir.Replace('\\', '/').TrimEnd('/'); }

        public Task<string> WriteFile(string fileContents, string fileName)
        {
            //Return task that will write file contents to RootDir/filename
            return HelperTasks.Write(fileContents, RootDir, fileName);
        }

        public Task<string> WriteFile(ISong song)
        {
            return new TaskFactory<string>().StartNew(() =>
            {
                //Get task that will write the result of GetLyrics to a file with the name name as song.Name

                song.GetLyrics().ContinueWith(myTask =>
                {
                    //Will execute when song.GetLyrics is complete
                    WriteFile(myTask.Result, song.Name);
                }, TaskContinuationOptions.ExecuteSynchronously);



                //Return immediately
                return RootDir + "/" + song.Name + ".html";
            });
        }
    }

    public class AsyncFileWriter : IFileWriter
    {
        public string RootDir { get; private set; }

        public AsyncFileWriter(string rootDir) { RootDir = rootDir.Replace('\\', '/').TrimEnd('/'); }

        public async Task<string> WriteFile(string fileContents, string fileName)
        {
            //await the completion of AsyncWrite task and return its result
            return await HelperTasks.AsyncWrite(fileContents, RootDir, fileName);
        }

        public async Task<string> WriteFile(ISong song)
        {
            var contents = await song.GetLyrics();

            return await WriteFile(contents, song.Name);
        }
    }
}
