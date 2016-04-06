using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;

namespace LyricDownloadWebApp
{
    public partial class Test : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTest_OnClick(object sender, EventArgs e)
        {
            //var tmp = Task.Run(()=> GetContent("https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx"));

            var tmp = GetContent("https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx");

            //tmp.Wait();

            lblResult.Text = tmp.Result;
        }


        protected async Task<string> GetContent(string Uri)
        {
            using (var client = new HttpClient())
            {
                var getResult = client.GetAsync(Uri).Result;

                var content = getResult.Content.ReadAsStringAsync().Result;

                return content;
            }
        }
    }
}