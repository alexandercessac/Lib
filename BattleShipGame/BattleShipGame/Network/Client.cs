using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BattleShipGame.Identity;
using Newtonsoft.Json;

namespace BattleShipGame.Network
{
    public static class Client
    {
        private static readonly HttpClient MyClient = new HttpClient();

        public delegate void MessageHandler(string msg);

        public static event MessageHandler Msg;

        public static async Task<Game> JoinGame(this Player player, string ip = "localhost")
        {
            var uri = $"http://{ip}/BattleShip/";

            try
            {
                var response = await MyClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(player)));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Accepted:
                        Msg?.Invoke($"Connected to game at IP: {ip}");
                        return JsonConvert.DeserializeObject<Game>(await response.Content.ReadAsStringAsync());
                    case HttpStatusCode.BadRequest:
                        Msg?.Invoke(await response.Content.ReadAsStringAsync()); break;
                        case HttpStatusCode.MethodNotAllowed:
                            Msg?.Invoke("Game has already started :("); break;
                }
            }
            catch (Exception e)
            { System.Diagnostics.Debugger.Launch(); }

            return null;
        }

        public static async Task<Game> Fire(this Shot shot, string ip = "localhost")
        {
            var uri = $"http://{ip}/BattleShip/";

            try
            {
                var stringShot = JsonConvert.SerializeObject(shot);
                var response = await MyClient.PostAsync(uri, new StringContent(stringShot));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Accepted:
                        Msg?.Invoke($"Connected to game at IP: {ip}");
                        return JsonConvert.DeserializeObject<Game>(await response.Content.ReadAsStringAsync());
                    case HttpStatusCode.BadRequest:
                        Msg?.Invoke(await response.Content.ReadAsStringAsync()); break;
                }
            }
            catch (Exception e)
            { System.Diagnostics.Debugger.Launch(); }

            return null;
        }

    }
}
