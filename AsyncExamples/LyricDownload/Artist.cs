using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LyricDownload
{
    public interface IArtist
    {
        IFileWriter Writer { get; }
        List<ISong> Songs { get; set; }
        string Url { get; }
        Task<string[]> DownloadLyrics();
    }

    public class Artist : IArtist
    {
        public List<ISong> Songs { get; set; }
        public string Url { get; set; }
        public IFileWriter Writer { get; private set; }

        public Artist(IFileWriter writer)
        {
            Writer = writer;
            Songs = new Songs();
        }

        public Task<string[]> DownloadLyrics()
        {
            return new TaskFactory<string[]>().StartNew(() =>
            {

                var work = new Task<string>[Songs.Count];
                var results = new string[Songs.Count];

                for (var i = 0; i < Songs.Count; i++)
                {
                    work[i] = Save(Songs[i]);
                }

                Task.WhenAll(work);

                for (var i = 0; i < Songs.Count; i++)
                {
                    results[i] = work[i].Result;
                }

                return results;
            });
        }

        private Task<string> Save(ISong currentSong)
        {
            var lyricsTask = currentSong.GetLyrics();

            lyricsTask.Wait();

            return Writer.WriteFile(lyricsTask.Result, currentSong.Name);
        }
    }

    public class AsyncArtist : IArtist
    {
        public List<ISong> Songs { get; set; }
        public string Url { get; set; }
        public IFileWriter Writer { get; private set; }

        public AsyncArtist(IFileWriter writer)
        {
            Writer = writer;
            Songs = new Songs();
        }

        public async Task<string[]> DownloadLyrics()
        {
            var work = new Task<string>[Songs.Count];

            for (var i = 0; i < Songs.Count; i++)
            { work[i] = Save(Songs[i]); }

            return await Task.WhenAll(work);
        }

        private async Task<string> Save(ISong currentSong)
        {
            return await Writer.WriteFile(await currentSong.GetLyrics(), currentSong.Name);
        }
    }
}
