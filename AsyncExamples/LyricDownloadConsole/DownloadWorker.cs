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

        private static bool _blockOnResult;
        private static bool _awaitCompletion;
        private static bool _useContinuation;

        private DateTime startTime;

        public void DoWork()
        {
            const string rootDir = "C:/TmpMusic/";



            var music = new Music(new ContinuationFileWriter(rootDir));

            var allSongs = Music.DefaultPlayList;

            //Populate songs to download
            GetSongs(allSongs).ForEach(x => music.Songs.Add(x));

            //Songs populated and ready to download. 
            //Get user input to determine download method
            GetUserInput();

            //Set start time to track processing duration
            startTime = DateTime.Now;

            //Collect all tasks to be done



            //Show why it is bad to use await on the
            //main thread of a console application
            if (_blockOnResult)
            {
                var downloadTask = _awaitCompletion ? music.AwaitAllSongLyricsAsync() : music.GetAllSongLyricsTask();

                var result = downloadTask.Result;

                UpdatUi(result);

            } 
            else
            {

                var downloadTask = music.GetAllSongLyricsTask();

                if (_useContinuation)
                {
                    downloadTask.ContinueWith(t => 
                        UpdatUi(t.Result));
                }
                else
                {
                    var result = downloadTask.Result;

                    UpdatUi(result);

                }



            }
           
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
            Console.WriteLine("Press Y to use 'aysnc' keywork implementation");
            var input = Console.ReadKey().KeyChar;
            _useAsyncSongs = input == 'y';


            Console.WriteLine("\nPlease select method of task execution:");
            Console.WriteLine("r: block calling thread until the task result is available");
            Console.WriteLine("a: await the task");
            Console.WriteLine("c: schedule continuation to handle results of task");
            input = Console.ReadKey().KeyChar;
            _blockOnResult = input == 'r';
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