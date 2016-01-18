using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface IMusic
    {
        List<ISong> Songs { get; set; }
        IFileWriter Writer { get; }
        string[] GetAllSongLyrics();
        Task<string[]> AwaitAllSongLyricsAsync();
    }

    public class Music : IMusic
    {
        public List<ISong> Songs { get; set; }

        public IFileWriter Writer { get; private set; }

        public enum DownloadMethod
        {
            Sync,
            WhenAllTask,
            AsyncTask

        }

        public static readonly List<string> DefaultPlayList = new List<string>
        {
            "Drake-hotline-bling", "Drake-all-me", "Drake-forever",
            "Lil-wayne-6-foot-7-foot", "Lil-wayne-love-me","Lil-wayne-Lollipop",
            "2-chainz-im-different", "2-chainz-no-lie", "2-chainz-feds-watching"
        };

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
                work[i] = Songs[i].Download(Writer);
            }

            return work;
        }

        public string[] GetAllSongLyrics()
        {
            //Create array of tasks representing lyric downloads
            var work = GetWork();

            //Task that will complete when all tasks in work complete
            var allWork = Task.WhenAll(work);

            //Block the current thread until the Result of allWork is available
            return allWork.Result;
        }

        public Task<string[]> GetAllSongLyricsTask()
        {
            //Create array of tasks representing lyric downloads
            var work = GetWork();

            //Return task<string[]> that will complete when all tasks in work complete
            return Task.WhenAll(work);
        }

        public async Task<string[]> AwaitAllSongLyricsAsync()
        {
            //Get task[] that will process the tasks to be done
            var work = GetWork();

            var completeAllTasks = Task.WhenAll(work);

            //Await completion of all tasks
            var results = await completeAllTasks.ConfigureAwait(false);

            //All songs have started downloading; return task that will complete will all songs are donwloaded/written
            return results;
        }

    }
}
