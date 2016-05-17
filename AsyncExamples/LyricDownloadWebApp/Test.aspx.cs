using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;

namespace LyricDownloadWebApp
{
    public partial class Test : Page
    {
        //private const string ResourcePath = "https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTest_OnClick(object sender, EventArgs e)
        {
            var contentTask = GetContent();
            lblResult.Text = contentTask.Result;
        }
        protected async Task<string> GetContent()
        {
            var uri = new Uri("http://genius.com/Drake-hotline-bling-lyrics");

            using (var client = new HttpClient())
            {
                var getResult = await client.GetAsync(uri);
                var content = await getResult.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}