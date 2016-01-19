using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LyricDownload;

namespace LyricDownloadConsole
{
    class DownloadWorker
    {
        private static bool _useAsyncSongs;

        private static bool _awaitCompletion;
        private static bool _waitOnTask;


        private DateTime startTime;

        private const string RapGeniusUrl = "http://genius.com/";
        private const string DownloadDir = "C:/tmpMusic/";

        public async void DoWork()
        {


            var music = new Music(new FileWriter(DownloadDir));

            var allSongs = Music.DefaultPlayList.Take(2).ToList();

            //Get user input to determine download method
            GetUserInput();

            //determine song download method based on user input
            var songType = _useAsyncSongs ? Music.EmplementaionType.Async : Music.EmplementaionType.Sync;


            //Populate songs to download
            allSongs.ForEach(x => music.Songs.Add(Music.GetSong(songType, RapGeniusUrl, x)));

            //Set start time to track processing duration
            startTime = DateTime.Now;

            //var to see on which thread we are running
            var tId = Thread.CurrentThread.ManagedThreadId;

            //result of work
            var result = new string[music.Songs.Count];

            if (_awaitCompletion)
            {
                var tmp = await music.AwaitAllSongLyricsAsync();

                result = tmp;
            }
            else if (_waitOnTask)
            {
                //create task that returns the result of GetAllSongLyricsTask executed in a separate thread
                var work = music.GetAllSongLyricsTask();

                //get result or block current thread until result is available
                result = work.Result;
            }
            else
            {//Run Sync implementation
                result = music.GetAllSongLyrics();
            }

            //Loop through results and write to console
            UpdatUi(result);


            Console.WriteLine("--------------------------------------------------------------------------------DoWork Ends\n");
            //Console.ReadKey();
        }


        private void UpdatUi(IEnumerable<string> output)
        {
            foreach (var outputItem in output)
                Console.WriteLine(outputItem);

            Console.WriteLine("#####Download complete in {0}ms!#####\n", DateTime.Now.Subtract(startTime).TotalMilliseconds);
        }

        private static void GetUserInput()
        {
            Console.WriteLine("Press Y to use 'async' method for individual song download");
            var input = Console.ReadKey().KeyChar;
            _useAsyncSongs = input == 'y';


            Console.WriteLine("\nPlease select method of task execution:");
            Console.WriteLine("t: use task implementation");
            Console.WriteLine("a: use 'async' task implementation");
            input = Console.ReadKey().KeyChar;
            _awaitCompletion = input == 'a';
            _waitOnTask = input == 't';

            Console.WriteLine("\nDownloading...");
        }


    }
}