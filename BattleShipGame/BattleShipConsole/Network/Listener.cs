using System;
using System.IO;
using System.Net;
using BattleShipGame;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using BattleShipGame.Identity;

namespace BattleShipConsole.Network
{
    public static class Listener
    {
        public delegate void PlayerConnectedHandler(Player player);
        public static event PlayerConnectedHandler PlayerConnected;

        public delegate void PlayerShotHandler(Shot player);
        public static event PlayerShotHandler PlayerShot;

        public static bool WaitForPlayers = true;
        public static async Task HostGame(Game state, CancellationToken token)
        {
            using (var listener = new HttpListener())
            {
                try
                {
                    listener.Prefixes.Add($"http://localhost/BattleShip");
                    listener.Start();
                    while (!token.IsCancellationRequested)
                    {
                        var pendingRequest = listener.GetContextAsync();

                        while (!pendingRequest.IsCompleted) await Task.Delay(100, token);

                        var req = await pendingRequest;

                        switch (req.Request.HttpMethod)
                        {
                            case "GET":
                                await req.Respond(200, state, token);
                                break;
                            case "POST":

                                var contentString = req.ReadContent();

                                if (WaitForPlayers)
                                {
                                    var player = JsonConvert.DeserializeObject<Player>(contentString);
                                    if (player == null)
                                        await req.Respond(400, "Invalid player", token);
                                    else
                                    {
                                        PlayerConnected?.Invoke(player);
                                        await req.Respond(201, state, token);
                                        continue;
                                    }
                                }
                                else
                                {
                                    var shot = JsonConvert.DeserializeObject<Shot>(contentString);
                                    PlayerShot?.Invoke(shot);
                                    await req.Respond(201, state, token);
                                }
                                break;
                            default:
                                await req.Respond(405, "method not allowed", token);
                                break;
                        }
                    }

                }
                catch (Exception e) { System.Diagnostics.Debugger.Launch(); }
                finally { listener.Stop(); }
            }

        }

        private static string ReadContent(this HttpListenerContext req)
        {
            using (var s = req.Request.InputStream)
            using (var sr = new StreamReader(s))
                return sr.ReadToEnd();
        }

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
