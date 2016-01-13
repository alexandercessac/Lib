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

        private const bool QuitBeforeWorkIsDone = false;

        public void DoWork()
        {

            //Populate songs to download
            var music = new List<Artist> { GetDrakeSongs(), GetLilWayne(), Get2Chainz() };

            //update UI
            Console.WriteLine("Press Y to use 'aysnc' keywork implementation");
            _useAsyncSongs = Console.ReadKey().KeyChar == 'y';
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
            //Awaiting a task in the main thread of a console app will      \\
            //end the current running thread causing the application to     \\
            //close before the awaited task can complete.                   \\
            //--------------------------------------------------------------\\

            //Loop through each artist
            foreach (var workItem in result)
            {//Write each track to console
                workItem.ToList().ForEach(Console.WriteLine);
            }
        }

        #region Static Methods to Populate Artists with songs

        private static Artist GetDrakeSongs()
        {
            var drake = new Artist(new FileWriter("C:/TmpMusic/Drake"))
            {
                Url = "http://genius.com/Drake",
            };

            drake.Songs.Add(GetISong(drake.Url, "hotline-bling"));
            drake.Songs.Add(GetISong(drake.Url, "all-me"));
            drake.Songs.Add(GetISong(drake.Url, "forever"));

            return drake;
        }

        private static Artist GetLilWayne()
        {
            var lilWayne = new Artist(new FileWriter("C:/TmpMusic/Lil-wayne"))
            {
                Url = "http://genius.com/Lil-wayne",
            };

            lilWayne.Songs.Add(GetISong(lilWayne.Url, "6-foot-7-foot"));
            lilWayne.Songs.Add(GetISong(lilWayne.Url, "love-me"));
            lilWayne.Songs.Add(GetISong(lilWayne.Url, "Lollipop"));

            return lilWayne;
        }

        private static Artist Get2Chainz()
        {
            var twoChainz = new Artist(new FileWriter("C:/TmpMusic/2-chainz"))
            {
                Url = "http://genius.com/2-chainz",
            };

            twoChainz.Songs.Add(GetISong(twoChainz.Url, "im-different"));
            twoChainz.Songs.Add(GetISong(twoChainz.Url, "no-lie"));
            twoChainz.Songs.Add(GetISong(twoChainz.Url, "feds-watching"));

            return twoChainz;
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