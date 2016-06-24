using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace LyricDownloadWebApp
{
    public partial class Test : Page
    {
        //private const string ResourcePath = "https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.118).aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            //var await = 1;
        }

        protected void btnTest_OnClick(object sender, EventArgs e)
        {
            var contentTask = Task.Run(() => GetContent().Result);
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

        private async Task<HttpResponseMessage> Get()
        {
            using (var client = new HttpClient())
            {
                //var requestUri = new Uri("http://genius.com/Drake-hotline-bling-lyrics");
                return await client.GetAsync("http://genius.com/Drake-hotline-bling-lyrics");
            }
        }

        protected Task<string> GetContent2()
        {
            var uri = new Uri("http://genius.com/Drake-hotline-bling-lyrics");
            var tcsRead = new TaskCompletionSource<string>();

            var client = new HttpClient();
            client.GetAsync(uri)
                            .ContinueWith(completeGet =>
                            {
                                completeGet.Result.Content.ReadAsStringAsync()
                                            .ContinueWith(completeRead =>
                                                       {
                                                           tcsRead.TrySetResult(completeRead.Result);
                                                           client.Dispose();    //dispose the client
                                                       }, CancellationToken.None,
                                                       TaskContinuationOptions.ExecuteSynchronously,
                                                       TaskScheduler.FromCurrentSynchronizationContext());

                            }, CancellationToken.None,
                            TaskContinuationOptions.ExecuteSynchronously,
                            TaskScheduler.FromCurrentSynchronizationContext());

            return tcsRead.Task;
        }


        
    }
}