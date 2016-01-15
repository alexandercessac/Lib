using System.Collections.Generic;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface IMusic
    {
        List<ISong> Songs { get; set; }
        IFileWriter Writer { get; }
        string[] GetAllSongLyrics();
        Task<string[]> AwaitAllSongLyrics();
        Task<string[]> ContinueWithAllSongLyrics();
    }

    public class Music : IMusic
    {
        public List<ISong> Songs { get; set; }

        public IFileWriter Writer { get; private set; }

        public Music(IFileWriter writer)
        {
            Writer = writer;
            Songs = new List<ISong>();
        }

        private Task<string>[] GetWork()
        {
            var work = new Task<string>[Songs.Count];

            //Setting download task for each song to download
            for (var i = 0; i < Songs.Count; i++)
            {
                //start downloading/writting current song and add task to work[]
                work[i] = Writer.WriteFile(Songs[i]);
            }

            return work;
        }

        public string[] GetAllSongLyrics()
        {
            //Create list of tasks representing lyric downloads
            var work = GetWork();

            //All songs have started downloading; synchronously wait for all task to complete
            return Task.WhenAll(work).Result;
        }

        public async Task<string[]> AwaitAllSongLyrics()
        {
            //Create list of tasks representing lyric downloads
            var work = GetWork();

            //All songs have started downloading; return task that will complete will all songs are donwloaded/written
            return await Task.WhenAll(work);
        }


        public async Task<string[]> ContinueWithAllSongLyrics()
        {
            var results = new string[Songs.Count];

            //Create list of tasks representing lyric downloads
            var work = GetWork();

            //All songs have started downloading
            //create task that will execute the continuation when complete
            Task.WhenAll(work).ContinueWith(myTask =>
            {
                results = myTask.Result;
            }, TaskContinuationOptions.ExecuteSynchronously).Wait();
            
            
            
            
            //wait on WhenAll task to complete so that results will be set before function returns
            
            return results;
        }

    }
}
