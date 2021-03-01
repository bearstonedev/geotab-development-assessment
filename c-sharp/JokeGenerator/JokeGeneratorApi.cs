using System;
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
            return new string[] { Task.FromResult(client.GetStringAsync("/jokes/categories").Result).Result };
        }
    }
}
