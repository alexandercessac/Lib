using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using LyricDownload;
using WebGrease.Css.Extensions;

namespace LyricDownloadWebApp
{
    public partial class _Default : Page
    {
        private List<CheckBoxList> _artists = new List<CheckBoxList>();

        private const string RapGeniusUrl = "http://genius.com/";

        private IMusic _music;

        private void Page_Load(object sender, EventArgs e)
        {
            _artists = new List<CheckBoxList>();

            _artists.Add(cbxDrakeSongs);
            _artists.Add(cbxLilWayneSongs);
            _artists.Add(cbxTwoChainzSongs);

            if (!Page.IsPostBack)
                DeselectAllSongs();

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            _music = new Music(new FileWriter("C:/tmpMusic/"));

            litOutput.Text = "";

            AddSongsToMusic(cbxDrakeSongs.Items);
            AddSongsToMusic(cbxLilWayneSongs.Items);
            AddSongsToMusic(cbxTwoChainzSongs.Items);



            if (cbxAsync.Checked)
            {
                //var taskThatAwaitsAnotherTask = _music.AwaitAllSongLyrics();

                //AwaitGetAllSongLyrics(taskThatAwaitsAnotherTask);

                AwaitGetAllSongLyrics(_music.ContinueWithAllSongLyrics());
            }
            else
            {
                UpdateUi(_music.GetAllSongLyrics());    
            }
            

            //Task.WhenAll(work);

            //foreach (var workItem in work)
            //{
            //    workItem.Result.ForEach(x => litOutput.Text = string.Format("{0}<br/>{1}", litOutput.Text, x));
            //}

            DeselectAllSongs();
        }

        private void AwaitGetAllSongLyrics(Task<string[]> work )
        {
            var result = work.Result;
            
            //DEADLOCK!

            UpdateUi(result);
        }

        private void UpdateUi(IEnumerable<string> results)
        {
            results.ForEach(x => litOutput.Text = string.Format("{0}<br/>{1}", litOutput.Text, x));
        }

        private void AddSongsToMusic(ListItemCollection songsToAdd)
        {
            foreach (var selectedSong in songsToAdd.Cast<ListItem>().Where(song => song.Selected))
            {
                _music.Songs.Add(new Song(RapGeniusUrl, selectedSong.Value));
            }
        }

        private void DeselectAllSongs()
        {
            foreach (var currentSong in _artists.SelectMany(artist => artist.Items.Cast<ListItem>()))
            {
                currentSong.Selected = false;
            }
        }
    }
}