using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface ISong
    {
        string Url { get; }
        string Name { get; }

        Task<string> Download(IFileWriter fileWriter);
    }

    //public class Songs : List<ISong>
    //{
    //    public async Task<List<string>> GetAllLyrics()
    //    {
    //        var retList = new List<string>();
    //        foreach (var tmpSong in this)
    //        {
    //            retList.Add(await tmpSong.GetLyrics());
    //        }

    //        return retList;
    //    }
    //}

    public class Song : ISong
    {
        public string Url { get; private set; }
        public string Name { get; private set; }

        public Song(string rootUrl, string name)
        {
            Name = name;
            Url = string.Format("{0}/{1}-lyrics", rootUrl.Trim('/'), name);
        }

        /// <summary>
        /// Get data in this.Url and use <paramref name="fileWriter"/> to save to disk
        /// </summary>
        /// <param name="fileWriter"></param>
        /// <returns>Task that completes when file is written to disk</returns>
        public Task<string> Download(IFileWriter fileWriter)
        {
            var tId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                var contentsTask = new HttpClient().GetStringAsync(Url);

                //Block current thread until contentsTask is complete
                contentsTask.Wait();

                //Return task that will write the contents to disk
                return fileWriter.WriteFile(contentsTask.Result, Name);
            }
            catch
            {
                //Return a sucessfully completed Task<string> with the below result
                return Task.FromResult(string.Format("Failed to download: {0}", Url));
            }

        }

    }

    public class AsyncSong : ISong
    {
        public string Url { get; private set; }
        public string Name { get; private set; }

        public AsyncSong(string rootUrl, string name)
        {
            Name = name;
            Url = string.Format("{0}/{1}-lyrics", rootUrl.Trim('/'), name);
        }

        public async Task<string> Download(IFileWriter fileWriter)
        {
            //peep thread Id
            var tId = Thread.CurrentThread.ManagedThreadId;
           
            //suspend execution until this task is complete
            var contents = await new HttpClient().GetStringAsync(Url);

            //suspend execution until this task is complete
            var writeFileTask = fileWriter.WriteFile(contents, Name);

            var returnVal = await writeFileTask;

            //return the value returned from WriteFile
            return returnVal;
        }

    }

}
