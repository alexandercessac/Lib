using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace msg
{
    public static class Listener
    {
        private static readonly HttpListener Server = new HttpListener();
        public static readonly string MyUri = $"http://{Dns.GetHostName()}/msg/";

        public static void Start()
        {
            Server.Prefixes.Add(MyUri);
            Server.Start();

            Server.GetContextAsync().ContinueWith(HandleContext);
        }

        public static void Stop() => Server.Stop();

        private static void HandleContext(Task<HttpListenerContext> ctxTask)
        {
            var ctx = ctxTask.Result;
            try
            {
                switch (ctx.Request.HttpMethod.ToUpper())
                {
                     case "POST":
                         ctx.HandlePost();
                         break;
                    // case "GET":
                    //     ctx.HandleGet();
                    //     break;
                    default:
                        ctx.Response.StatusCode = 404;
                        break;
                }
            }
            catch (Exception e)
            {
                ctx.Response.StatusCode = 500;
                ctx.Response.WriteResponseStream(e.Message);
                Log.Error(e.Message);
            }
            finally
            {
                ctx.Response.OutputStream.Flush();
                ctx.Response.OutputStream.Close();
                Server.GetContextAsync().ContinueWith(HandleContext);
            }
        }

        private static void HandlePost(this HttpListenerContext ctx)
        {
            var msg = new Message
            {
                From = ctx.Request.Headers.Get("from") ?? "unknown",
                Title = ctx.Request.Headers.Get("title") ?? "unknown"
            };

            using (var sr = new StreamReader(ctx.Request.InputStream))
                msg.Body = sr.ReadToEnd();

            ctx.Response.StatusCode = 201;

            msg.Save();
        }

        private static void WriteResponseStream(this HttpListenerResponse res, string content)
        {
           using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                ms.CopyTo(res.OutputStream);
        }

    }
}