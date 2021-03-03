using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JokeGenerator
{
    public class ApiService
    {
        private readonly HttpClient client;

        public ApiService(string baseAddress) : this(new HttpClientHandler(), baseAddress)
        {
        }

        public ApiService(HttpMessageHandler messageHandler, string baseAddress)
        {
            this.client = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(baseAddress)
            };
            this.client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<T> Get<T>(string path)
        {
            var requestTask = client.GetStringAsync(path);
            return Deserialize<T>(await requestTask);
        }

        private T Deserialize<T>(string jsonResponse)
        {
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
    }
}