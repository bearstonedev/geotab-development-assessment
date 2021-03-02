using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JokeGenerator
{
    public class JokeGeneratorApi
    {
        private readonly HttpClient client;

        public JokeGeneratorApi(string endpoint) : this(new HttpClientHandler(), endpoint)
        {
        }

        // Creates the API instance with a provided HTTP handler. Intended for use in unit tests to mock out network calls.
        public JokeGeneratorApi(HttpMessageHandler messageHandler, string baseAddress)
        {
            this.client = new HttpClient(messageHandler)
            {
                BaseAddress = new Uri(baseAddress)
            };
            this.client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<string[]> GetRandomJokes(int numberOfJokes = 1)
        {
            var jokeTasks = new List<Task<string>>(Enumerable.Range(0, numberOfJokes).Select(_ => GetRandomJoke()));
            var aggregateTask = Task.WhenAll(jokeTasks.ToArray());
            return await aggregateTask;
        }

        private async Task<string> GetRandomJoke()
        {
            var requestTask = Get<dynamic>("jokes/random");
            await requestTask;
            var joke = ParseJokeResponse(requestTask.Result);
            return joke;
        }

        private string ParseJokeResponse(dynamic jokeResponse)
        {
            return jokeResponse.value;
        }

        public string[] GetRandomJokes(string firstname, string lastname, string category)
        {
            string url = "jokes/random";
            if (category != null)
            {
                if (url.Contains('?'))
                    url += "&";
                else url += "?";
                url += "category=";
                url += category;
            }

            string joke = Task.FromResult(client.GetStringAsync(url).Result).Result;

            if (firstname != null && lastname != null)
            {
                int index = joke.IndexOf("Chuck Norris");
                string firstPart = joke.Substring(0, index);
                string secondPart = joke.Substring(0 + index + "Chuck Norris".Length, joke.Length - (index + "Chuck Norris".Length));
                joke = firstPart + " " + firstname + " " + lastname + secondPart;
            }

            return new string[] { JsonConvert.DeserializeObject<dynamic>(joke).value };
        }

        /// <summary>
        /// returns an object that contains name and surname
        /// </summary>
        /// <param name="client2"></param>
        /// <returns></returns>
		public dynamic GetNames()
        {
            var result = client.GetStringAsync("").Result;
            return JsonConvert.DeserializeObject<dynamic>(result);
        }

        public string[] GetCategories()
        {
            return Get<string[]>("/jokes/categories").Result;
        }

        private async Task<T> Get<T>(string path)
        {
            var requestTask = client.GetStringAsync(path);
            var deserializedResponse = Deserialize<T>(await requestTask);
            return deserializedResponse;
        }

        private T Deserialize<T>(string jsonResponse)
        {
            var deserializedResult = JsonConvert.DeserializeObject<T>(jsonResponse);
            return deserializedResult;
        }
    }
}
