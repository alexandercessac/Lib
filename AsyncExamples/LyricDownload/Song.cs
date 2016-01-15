using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface ISong
    {
        string Url { get; }
        string Name { get; }

        Task<string> GetLyrics();
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

        public Task<string> GetLyrics()
        {
            return new TaskFactory<string>().StartNew(GetContents);
        }

        private string GetContents()
        {
            return new HttpClient().GetStringAsync(Url).Result;
            
            //var clientTask = new HttpClient().GetStringAsync(Url);

            //clientTask.Wait();

            //return clientTask.Result;
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

        public async Task<string> GetLyrics()
        {
            return await new HttpClient().GetStringAsync(Url);
        }
    }
}
