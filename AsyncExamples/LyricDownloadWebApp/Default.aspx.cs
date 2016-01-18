using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using LyricDownload;
using WebGrease.Css.Extensions;

namespace LyricDownloadWebApp
{
    public partial class Default : Page
    {

        private const string RapGeniusUrl = "http://genius.com/";
        private const string DownloadDir = "C:/tmpMusic/";

        private IMusic _music;

        private IFileWriter myWriter
        {
            get { return getFileWriter(DownloadDir); }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            //Populate UI on initial load
            if (!Page.IsPostBack)
                PopulateCheckBoxes();
        }

        /// <summary>
        /// Set input with default values
        /// </summary>
        private void PopulateCheckBoxes()
        {
            cbxSongs.Items.Clear();
            Music.DefaultPlayList.ForEach(x => cbxSongs.Items.Add(new ListItem()
            {
                Text = x,
                Selected = false
            }));
        }

        /// <summary>
        /// Get songs selected by the user for download
        /// </summary>
        /// <returns>List of strings representing the name of a song for lyric download</returns>
        private List<string> GetSelectedSongs()
        {
            return
                cbxSongs.Items.Cast<ListItem>()
                    .Where(song => song.Selected)
                    .Select(selectedSong => selectedSong.Text)
                    .ToList();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //Instantiate new Music object to handle downloads
            _music = new Music(myWriter);

            //Clear output field
            litOutput.Text = "";

            //Add each song selected by the user to _music.Songs
            GetSelectedSongs()
                .ForEach(selectedSong =>
                    _music.Songs.Add(new Song(RapGeniusUrl, selectedSong)));

            //array to contain results of the download
            var downloadResult = new string[_music.Songs.Count];

            //Determine method to use for downloading lyrics
            if (cbxAsync.Checked)
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                var asyncTask = _music.AwaitAllSongLyricsAsync();

                var runningOnTheSameThread = (Thread.CurrentThread.ManagedThreadId == threadId);

                //Block current thread until result of task 
                //returned by AwaitAllSongLyricsAsync is available
                downloadResult = asyncTask.Result;
            }
            else
            {
                //Synchronously obtain result of GetAllSongLyrics method
                downloadResult = _music.GetAllSongLyrics();
            }

            //Show the path(s) of files created by download to the user
            UpdateUi(downloadResult);
        }

        //private async Task<string[]> awaitAsyncOperation()
        //{
            
        //}

    private void UpdateUi(IEnumerable<string> results)
        {
            results.ForEach(x => litOutput.Text = string.Format("{0}<br/>{1}", litOutput.Text, x));
        }

        private IFileWriter getFileWriter(string args)
        {
            return new FileWriter(args);
        }



    }
}