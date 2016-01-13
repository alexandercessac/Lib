using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LyricDownload;

namespace LyricDownloadConsole
{
    class DownloadWorker
    {
        private static bool _useAsyncSongs;

        private static bool QuitBeforeWorkIsDone;

        public void DoWork()
        {
            const string rootDir = "C:/TmpMusic/";

            //Populate songs to download
            var music = new List<IArtist>
            {
                GetArtist(rootDir, "Drake",new List<string>{"hotline-bling","all-me","forever"} ),
                GetArtist(rootDir, "Lil-wayne",new List<string>{"6-foot-7-foot","love-me","Lollipop"} ),
                GetArtist(rootDir, "2-chainz",new List<string>{"im-different","no-lie","feds-watching"} ),
                
            };

            //update UI TODO: clean up
            Console.WriteLine("Press Y to use 'aysnc' keywork implementation");
            var input = Console.ReadKey().KeyChar;
            _useAsyncSongs =  input == 'y';
            QuitBeforeWorkIsDone = input == 'q';
            Console.WriteLine("\nDownloading...");

            //Set start time to track processing duration
            var startTime = DateTime.Now;

            //Collect all tasks to be done
            var work = new Task<string[]>[music.Count];
            for (var i = 0; i < music.Count; i++)
            { work[i] = music[i].DownloadLyrics(); }

            //Show why it is bad to use await on the
            //main thread of a console application
            if (QuitBeforeWorkIsDone)
            { AwaitWriteResults(work); } //Application will end without writting results to console when using this implementation
            else
            { WaitWriteResults(work); }

            Console.WriteLine("#####Download complete in {0}ms!#####\n", DateTime.Now.Subtract(startTime).TotalMilliseconds);
        }

        private void WaitWriteResults(Task<string[]>[] work)
        {

            //Create a task that will complete when all subtasks have finished
            var waitingTask = Task.WhenAll(work);

            //Wait for task to complete indicating all subtasks are finished
            waitingTask.Wait();

            //loop through the result of each task
            foreach (var workItem in work)
            {//write each track to the console
                workItem.Result.ToList().ForEach(Console.WriteLine);
            }
        }

        private async void AwaitWriteResults(Task<string[]>[] work)
        {
            //returns a list of list of string
            var result = await Task.WhenAll(work);

            //-----------NOTE: This code will not execute-------------------\\
            // this is not true
            //--------------------------------------------------------------\\

            //Loop through each artist
            foreach (var workItem in result)
            {//Write each track to console
                workItem.ToList().ForEach(Console.WriteLine);
            }
        }

        #region Static Methods to Populate Artists with songs

        private static IArtist GetArtist(string rootDir, string artistName, List<string> songTitles)
        {
            var myArtist = new Artist(new FileWriter(rootDir + artistName))
            {
                Url = "http://genius.com/" + artistName
            };
            songTitles.ForEach(x=>myArtist.Songs.Add(GetISong(myArtist.Url, x)));

            return myArtist;
        }

        #endregion

        #region Determine whether to use async implementation of song or not based on _useAsyncSongs

        private static ISong GetISong(string parentUrl, string songName)
        {
            return _useAsyncSongs ? GetAsyncSong(parentUrl, songName) : GetSong(parentUrl, songName);
        }

        private static ISong GetSong(string parentUrl, string songName)
        {
            return new Song(parentUrl, songName);
        }

        private static ISong GetAsyncSong(string parentUrl, string songName)
        {
            return new AsyncSong(parentUrl, songName);
        }

        #endregion

    }
}