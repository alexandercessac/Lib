using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        private Task<string[]> GetWorkTask()
        {
            var work = new Task<string>[Songs.Count];

            //Setting download task for each song to download
            for (var i = 0; i < Songs.Count; i++)
            {
                //start downloading/writting current song and add task to work[]
                work[i] = Writer.WriteFile(Songs[i]);
            }

            return Task.WhenAll(work).ContinueWith(x =>
            {
                Task.Delay(5000).Wait();
                return x.Result;
            }, TaskContinuationOptions.ExecuteSynchronously);
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

        public Task<string[]> GetAllSongLyricsTask()
        {
            var resultOfGetWork = new TaskFactory<string[]>().StartNew(() =>
            {
                var tmp = Task.WhenAll(GetWork());

                tmp.Wait();

                return tmp.Result;
            });

            return resultOfGetWork;
        }

        public async Task<string[]> AwaitAllSongLyrics()
        {
            //Create list of tasks representing lyric downloads
            var work = GetWork();

            //All songs have started downloading; return task that will complete will all songs are donwloaded/written
            return await Task.WhenAll(work);
        }

        public Task<string[]> ContinueWithAllSongLyrics()
        {
            //Create list of tasks representing lyric downloads


            return GetWorkTask().ContinueWith(x =>
            {
                Task.Delay(5000).Wait();
                return x.Result;
            },TaskContinuationOptions.ExecuteSynchronously);


            //All songs have started downloading

            //create task that will execute the continuation when complete
            //return Task.WhenAll(work);
            //Continuation that will return the result of all tasks
            //.ContinueWith(myTask => myTask.Result, TaskScheduler.FromCurrentSynchronizationContext());
            //.ContinueWith(myTask => myTask.Result, TaskContinuationOptions.ExecuteSynchronously);




        }

    }
}
