using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
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

        public Task<string> Download(IFileWriter fileWriter)
        {
            var contentsTask = new HttpClient().GetStringAsync(Url);

            contentsTask.Wait();

            return fileWriter.WriteFile(contentsTask.Result, Name);
        } 

        public Task<string> GetLyricsAsync()
        {
            return new HttpClient().GetStringAsync(Url);
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
            var contents = await new HttpClient().GetStringAsync(Url);
            return await fileWriter.WriteFile(contents, Name);
        }

        public async Task<string> GetLyricsAsync()
        {
            return await new HttpClient().GetStringAsync(Url);
        }
    }

}
