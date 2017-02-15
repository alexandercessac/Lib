using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DancingCube.RequestStuff
{
    public static class RequestClient
    {
        private static readonly HttpClient Client = new HttpClient {BaseAddress = new Uri("http://localhost/CubeDance/")};


        public static async Task<Dancer[]> GetGame(string gameId)
        {
            try
            {
                var httpResponseMessage = await Client.GetAsync(gameId);

                return JsonConvert.DeserializeObject<Dancer[]>(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return null;
                //throw;
            }
        }

        public static async Task<Dancer> JoinGame(this Dancer user, string gameId)
        {
            try
            {
                var tmp = new StringContent(JsonConvert.SerializeObject(user));
                //tmp.Headers.Add("Content-Type", "application/json");

                var httpResponseMessage = await Client.PostAsync(gameId, tmp);

                return JsonConvert.DeserializeObject<Dancer>(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return null;
                //throw;
            }
        }

        public static async Task<Dancer[]> SubmitMove(this MoveRequest move, string gameId)
        {
            try
            {
                var tmp = new StringContent(JsonConvert.SerializeObject(move));
                //tmp.Headers.Add("Content-Type", "application/json");

                var httpResponseMessage = await Client.PostAsync($"{gameId}/moves", tmp);

                return JsonConvert.DeserializeObject<Dancer[]>(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return null;
                //throw;
            }
        }

    }
}
