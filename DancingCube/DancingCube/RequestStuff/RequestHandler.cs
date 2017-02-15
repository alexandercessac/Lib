using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DancingCube.RequestStuff
{
    public static class RequestHandler
    {

        public static async Task HostGameState(string gameId, CancellationToken token)
        {

            using (var listener = new HttpListener())
            {
                try
                {
                    listener.Prefixes.Add($"http://localhost/CubeDance/{Program.GameId}/");
                    listener.Start();
                    while (!token.IsCancellationRequested)
                    {
                        var pendingRequest = listener.GetContextAsync();
                        while (!pendingRequest.IsCompleted) await Task.Delay(500, token);
                        var req = await pendingRequest;

                        switch (req.Request.HttpMethod)
                        {
                            case "GET":
                                await req.Respond(200, Program.Dancers, token);
                                break;
                            case "POST":
                                //await req.Respond(405, "No longer accepting new dancers", token);
                                break;
                        }
                    }

                }
                catch (TaskCanceledException tce)
                {

                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        public static async Task AwaitUserMovements(CancellationToken token)
        {

            using (var listener = new HttpListener())
            {
                try
                {
                    listener.Prefixes.Add($"http://localhost/CubeDance/{Program.GameId}/Moves/");
                    listener.Start();

                    while (!token.IsCancellationRequested)
                    {
                        var pendingRequest = listener.GetContextAsync();
                        while (!pendingRequest.IsCompleted) await Task.Delay(500, token);
                        var req = await pendingRequest;



                        if (req.Request.HttpMethod == "POST")
                        {
                            string x;
                            using (var s = req.Request.InputStream)
                            using (var sr = new StreamReader(s))
                                x = sr.ReadToEnd();

                            var moveRequest = JsonConvert.DeserializeObject(x, typeof(MoveRequest)) as MoveRequest;

                            if (moveRequest == null)
                                await req.Respond(400, "Invalid DancerRequest", token);
                            else if (!Program.Users.ContainsKey(moveRequest.Identity ?? ""))
                                await req.Respond(400, "Unregistered DancerId", token);
                            else
                            {
                                var responseCode = Program.TryMoveUser(moveRequest)? 200 : 400; 
                                await req.Respond(responseCode, Program.Dancers, token);
                            }
                        }
                    }

                }
                catch (TaskCanceledException tce)
                {

                    //
                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        public static async Task AwaitAdditionalDancers(this Dictionary<string, Dancer> users, int maxSize, CancellationToken token)
        {
            var dancersAdded = -1;

            using (var listener = new HttpListener())
            {
                try
                {
                    listener.Prefixes.Add($"http://localhost/CubeDance/{Program.GameId}/");
                    listener.Start();
                    while (dancersAdded < maxSize - 1)
                    {

                        var pendingRequest = listener.GetContextAsync();

                        if (token.IsCancellationRequested) return;

                        while (!pendingRequest.IsCompleted) await Task.Delay(500, token);

                        var req = await pendingRequest;



                        if (req.Request.HttpMethod == "POST")
                        {
                            string x;
                            using (var s = req.Request.InputStream)
                            using (var sr = new StreamReader(s))
                                x = sr.ReadToEnd();

                            var dancer = JsonConvert.DeserializeObject(x, typeof(Dancer)) as Dancer;

                            if (dancer == null)
                                await req.Respond(400, "Invalid Dancer", token);
                            else
                            {
                                //todo: validate
                                Task respond;
                                lock (Program.USERS_CHANGED_LOCK)
                                {
                                    if (
                                        users.Values.Any(
                                            d => d.Name.Equals(dancer.Name, StringComparison.OrdinalIgnoreCase)))
                                        respond = req.Respond(400, dancer.Name.IsDuplicateName(), token);
                                    else
                                    {
                                        dancer.Position = new Point(++dancersAdded, 0);
                                        dancer.Identity = Guid.NewGuid().ToString();
                                        dancer.Color = Program.DarkColors[dancersAdded];

                                        users.Add(dancer.Identity, dancer);
                                        respond = req.Respond(201, dancer, token);
                                    }
                                }
                                await respond;
                            }

                        }
                        else if (req.Request.HttpMethod == "GET")
                            await req.Respond(200, users.Values, token);
                    }


                }
                catch (TaskCanceledException tce)
                {

                    //
                }
                finally
                {
                    listener.Stop();
                }


            }
        }

        private static string IsDuplicateName(this string name) => $"There is already a dancer named {name}. Please use a unique name";
        private static string IsDuplicateColor(this int color) => $"There is already a dancer using color: {color}. Please use a unique color";

        public static async Task WriteResponseContentAsync(this HttpListenerContext ctx, string content, CancellationToken token)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            await ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, token);
        }
        public static async Task WriteResponseContentAsync(this HttpListenerContext ctx, string content)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            await ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task Respond(this HttpListenerContext ctx, int statusCode, object content, CancellationToken token)
        {

            ctx.Response.StatusCode = statusCode;
            ctx.Response.AddHeader("Content-Type", "application/json");
            await ctx.WriteResponseContentAsync(JsonConvert.SerializeObject(content), token);
            ctx.Response.OutputStream.Close();
        }
    }
}
