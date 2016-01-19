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

        private Music.EmplementaionType myWriterType
        {
            get { return cbxUseAsyncFileWriter.Checked ? Music.EmplementaionType.Async : Music.EmplementaionType.Sync; }
        }

        private Music.EmplementaionType mySongType
        {
            get { return cbxUseAsyncSongs.Checked ? Music.EmplementaionType.Async : Music.EmplementaionType.Sync; }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            //Populate UI on initial load
            if (!Page.IsPostBack)
                PopulateCheckBoxes();

            //Clear output field
            litOutput.Text = "";
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

        private void PrepareDownload()
        {
            //Instantiate IFileWritter to handle writting downloaded data to file system
            var myWriter = Music.GetFileWriter(myWriterType, DownloadDir);

            //Instantiate new Music object to handle downloads
            _music = new Music(myWriter);

            //Add each song selected by the user to _music.Songs
            GetSelectedSongs()
                .ForEach(selectedSong =>
                    _music.Songs.Add(Music.GetSong(mySongType, RapGeniusUrl, selectedSong)));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            PrepareDownload();

            //array to contain results of the download
            var downloadResult = new string[_music.Songs.Count];

            //var to see on which thread we are running
            var tId = Thread.CurrentThread.ManagedThreadId;

            //Determine method to use for downloading lyrics
            if (cbxUseAsyncMethod.Checked)
            {
                //Return task that yields control to the calling method
                //when awaiting subsequent async methods
                var asyncTask = _music.AwaitAllSongLyricsAsync();

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


        private void UpdateUi(IEnumerable<string> results)
        {
            results.ForEach(x => litOutput.Text = string.Format("{0}<br/>{1}", litOutput.Text, x));
        }





    }
}