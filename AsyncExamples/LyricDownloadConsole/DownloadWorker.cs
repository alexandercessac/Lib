using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LyricDownload;

namespace LyricDownloadConsole
{
    class DownloadWorker
    {
        private static bool _useAsyncSongs;

        private static bool _useAwaitInMainThread;

        private static bool _taskWait;
        private static bool _awaitCompletion;
        private static bool _useContinuation;

        public void DoWork()
        {
            const string rootDir = "C:/TmpMusic/";

            var music = new Music(new FileWriter(rootDir));

            var allSongs = new List<string>
            {
                "Drake-hotline-bling", "Drake-all-me", "Drake-forever",
                "Lil-wayne-6-foot-7-foot", "Lil-wayne-love-me","Lil-wayne-Lollipop",
                "2-chainz-im-different", "2-chainz-no-lie", "2-chainz-feds-watching"
            };

            //Populate songs to download
            GetSongs(allSongs).ForEach(x => music.Songs.Add(x));

            //Songs populated and ready to download. 
            //Get user input to determine download method
            GetUserInput();

            //Set start time to track processing duration
            var startTime = DateTime.Now;

            //Collect all tasks to be done


            //TODO: handle method of Music used based on user input

            //TODO: test this jank
            //var tmp = new Task<string>(()=> "just like await?").ContinueWith(Console.WriteLine,TaskScheduler.FromCurrentSynchronizationContext());

            //Show why it is bad to use await on the
            //main thread of a console application
            if (_useAwaitInMainThread)
            { AwaitWriteResults(music.AwaitAllSongLyrics()); } //Application will end without writting results to console when using this implementation
            else
            { WaitWriteResults(music.ContinueWithAllSongLyrics().Result); }

            Console.WriteLine("#####Download complete in {0}ms!#####\n", DateTime.Now.Subtract(startTime).TotalMilliseconds);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void GetUserInput()
        {
            Console.WriteLine("Press Y to use 'aysnc' keywork implementation");
            var input = Console.ReadKey().KeyChar;
            _useAsyncSongs = input == 'y';
            _useAwaitInMainThread = input == 'q';


            Console.WriteLine("\nPlease select method of task execution:");
            Console.WriteLine("w: block calling thread until tasks finish");
            Console.WriteLine("a: await completion of tasks");
            Console.WriteLine("c: schedule continuation to handle results of tasks");
            input = Console.ReadKey().KeyChar;
            _taskWait = input == 'w';
            _awaitCompletion = input == 'a';
            _useContinuation = input == 'c';

            Console.WriteLine("\nDownloading...");
        }

        private static void WaitWriteResults(IEnumerable<string> work)
        {

            //Create a task that will complete when all subtasks have finished

            //Wait for task to complete indicating all subtasks are finished

            //write each track to the console
            foreach (var workItem in work)
                Console.WriteLine(workItem);
        }

        private static async void AwaitWriteResults(Task<string[]> work)
        {
            //returns a list of list of string
            var result = await work;

            //-----------NOTE: This code will not execute-------------------\\
            // this is not true
            //--------------------------------------------------------------\\

            //Write each track to console
            result.ToList().ForEach(Console.WriteLine);

        }

        #region Static Methods to Populate Artists with songs

        private static List<ISong> GetSongs(List<string> artistAndSong)
        {

            var songsToDownload = new List<ISong>();

            artistAndSong.ForEach(x => songsToDownload.Add(GetISong("http://genius.com/", x)));


            return songsToDownload;
        }

        #endregion

        #region Determine whether to use async implementation of song or not based on _useAsyncSongs

        private static ISong GetISong(string parentUrl, string songName)
        {
            return _useAsyncSongs ? GetAsyncSong(parentUrl, songName) : GetSong(parentUrl, songName);
        }

        private static ISong GetSong(string rootUrl, string songName)
        { return new Song(rootUrl, songName); }

        private static ISong GetAsyncSong(string rootUrl, string songName)
        { return new AsyncSong(rootUrl, songName); }

        #endregion

    }
}